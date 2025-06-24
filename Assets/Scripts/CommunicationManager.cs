using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class ResponseObjects
{
    // マスタデータ
    [JsonProperty("master_data_version")]
    public int Master_data_version { get; set; }
    public UsersModel users;
    public WalletsModel wallets;

    // マスタデータ
    public PaymentShopModel[] payment_shop;

    public string errcode;
}

public class CommunicationManager : MonoBehaviour
{
    public static CommunicationManager Instance { get; private set; }

    private void Awake()
    {
        // シングルトンにする
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    /// <summary>
    /// ユーザー情報更新
    /// </summary>
    /// <param name="responseObjects"></param>
    static void UpdateUserData(ResponseObjects responseObjects)
    {
        if (responseObjects.users != null && !string.IsNullOrEmpty(responseObjects.users.user_id))
        {
            Users.RegistUserinfo(responseObjects.users);
        }
    }

    /// <summary>
    /// ウォレット情報更新
    /// </summary>
    /// <param name="responseObjects"></param>
    static void UpdateWalletData(ResponseObjects responseObjects)
    {
        UsersModel usersModel = Users.Get();
        if (responseObjects.wallets != null)
        {
            Debug.Log("ウォレット更新: " + responseObjects.wallets.paid_amount);
            Wallets.RegistWalletinfo(responseObjects.wallets, usersModel.user_id);
        }
        else
        {
            Debug.LogWarning("responseObjects.wallets is null");
        }
    }

    /// <summary>
    /// マスターデータの更新
    /// </summary>
    static void UpdateMasterData(ResponseObjects responseObjects)
    {
        // バージョン保存
        if (responseObjects.Master_data_version != 0)
        {
            SaveManager.Instance.SetMasterDataVersion(responseObjects.Master_data_version);
        }
        if (responseObjects.payment_shop != null)
        {
            PaymentShops.RegistShopInfo(responseObjects.payment_shop);
            Debug.Log("ショップのデータを登録しました。");
        }
        Debug.Log("UpdateMasterData 終了");
    }

    /// <summary>
    /// リンクごとに情報の更新、保存を行う
    /// </summary>
    static void ConnectMove(string connectURL, ResponseObjects responseObjects)
    {
        switch (connectURL)
        {
            case GameUtil.Uri.Login:
                Debug.Log("ConnectMove: payment_shop によるマスタデータ更新を実行");
                UpdateMasterData(responseObjects);
                break;
            case GameUtil.Uri.Register:
            case GameUtil.Uri.Home:
            case GameUtil.Uri.Stamina_Recovery:
                UpdateUserData(responseObjects);
                UpdateWalletData(responseObjects);
                break;
            case GameUtil.Uri.Buy_Currency:
                UpdateWalletData(responseObjects);
                break;
            case GameUtil.Uri.Master_Get_URL:
                UpdateMasterData(responseObjects);
                break;
            default:
                break;
        }
    }

    public static IEnumerator ConnectServer(string connectURL, List<IMultipartFormSection> parameter, Action action = null)
    {
        // **リクエストの送付** //
        using (UnityWebRequest webRequest = UnityWebRequest.Post(connectURL, parameter))
        {
            webRequest.timeout = 10;
            yield return webRequest.SendWebRequest();
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                // エラーの場合
                if (!string.IsNullOrEmpty(webRequest.error))
                {
                    // パラメータの内容をログに出力
                    foreach (var param in parameter)
                    {
                        var sectionName = param.sectionName;
                        var sectionData = param.sectionData;
                        Debug.Log($"Parameter Name: {sectionName}, Parameter Value: {System.Text.Encoding.UTF8.GetString(sectionData)}");
                    }
                    // フォールバックする処理を作成したらここに追加する
                    Debug.LogError("URL:" + connectURL + " のURLに接続した際にエラーが発生 " + webRequest.error);
                    yield break;
                }
            }
            else
            {
                // レスポンスの取得
                string text = webRequest.downloadHandler.text;
                Debug.Log("※URL:" + connectURL + "レスポンス : " + text);
                Debug.Log("=== [Raw Response Text] ===");
                Debug.Log(text);
                if (!text.Contains("\"payment_shop\""))
                {
                    Debug.LogWarning("[Check] 'payment_shop' は JSON に含まれていません。");
                }
                else
                {
                    Debug.Log("[Check] 'payment_shop' が JSON に含まれています。");
                }

                // エラーの場合
                if (text.All(char.IsNumber))
                {
                    switch (text)
                    {
                        case GameUtil.Common.ERROR_MASTER_DATA_UPDATE:
                            Debug.LogError("マスタの状態が古いようです。[マスタバージョン不整合]");
                            // ここにアップデートができたことを表示するイメージを表示する処理を追記
                            break;
                        case GameUtil.Common.ERROR_DB_UPDATE:
                            Debug.LogError("サーバーでエラーが発生しました。[データベース更新エラー]");
                            break;
                        default:
                            string debugTex = string.Format("テキストの内容:{0}", text);
                            Debug.LogError(debugTex);
                            break;
                    }
                    yield break;
                }

                // SQLiteへの保存処理
                //ResponseObjects responseObjects = JsonUtility.FromJson<ResponseObjects>(text);
                //ConnectMove(connectURL, responseObjects);
                //if (responseObjects.errcode != null)
                //{
                //    // フォールバックする処理を作成したらここに追加する
                //}
                MasterDataResponse masterData = JsonConvert.DeserializeObject<MasterDataResponse>(text);
                ResponseObjects responseObjects = JsonConvert.DeserializeObject<ResponseObjects>(text);


                try
                {
                    responseObjects = JsonConvert.DeserializeObject<ResponseObjects>(text);
                    Debug.Log("[Check] JsonConvert によるパースに成功しました。");
                }
                catch (Exception e)
                {
                    Debug.LogError("[Error] JSONのパースに失敗: " + e.Message);
                    yield break;
                }

                if (responseObjects.payment_shop == null)
                {
                    Debug.LogWarning("[Check] responseObjects.payment_shop は null です。");
                }
                else
                {
                    Debug.Log("[Check] payment_shop の件数: " + responseObjects.payment_shop.Length);
                    foreach (var shop in responseObjects.payment_shop)
                    {
                        Debug.Log($"[Shop] ID: {shop.product_id}, Name: {shop.product_name}"); // モデルのフィールドに応じて修正
                    }
                }

                ConnectMove(connectURL, responseObjects);

                // 正常終了アクション実行
                if (action != null)
                {
                    action();
                    action = null;
                }

            }
        }
    }
}
