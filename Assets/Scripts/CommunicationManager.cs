using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class ResponseObjects
{
    public UsersModel users;
    public WalletsModel wallets;
    public ItemsModel[] items;
    [JsonProperty("weapons")]
    public WeaponsModel[] weapons { get; set; }

    // マスタデータ
    public PaymentShopModel[] payment_shop;
    public GachaWeaponModel[] gacha_weapon;
    public GachaPeriodModel[] gacha_period;
    public WeaponMasterModel[] weapon_master;
    public WeaponCategoryModel[] weapon_category;
    public WeaponRarityModel[] weapon_rarity;
    public ItemMasterModel[] item_master;
    public ItemCategoryModel[] item_category;

    // ガチャ用
    public int[] new_weapons;
    public string gacha_result;
    public int fragment_num;

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
        if (responseObjects.wallets != null)
        {
            UsersModel usersModel = Users.Get();
            Wallets.RegistWalletinfo(responseObjects.wallets, usersModel.user_id);
        }
    }

    /// <summary>
    /// 武器情報更新
    /// </summary>
    /// <param name="responseObjects"></param>
    static void UpdateWeaponData(ResponseObjects responseObjects)
    {
        if (responseObjects.weapons != null)
        {
            UsersModel usersModel = Users.Get();
            Weapons.RegistWeaponInfo(responseObjects.weapons, usersModel.user_id);
        }
    }

    /// <summary>
    /// アイテム情報更新
    /// </summary>
    /// <param name="responseObjects"></param>
    static void UpdateItemData(ResponseObjects responseObjects)
    {
        if (responseObjects.items != null)
        {
            UsersModel usersModel = Users.Get();
            Items.RegistItemInfo(responseObjects.items, usersModel.user_id);
        }
    }

    /// <summary>
    /// マスターデータの更新
    /// </summary>
    static void UpdateMasterData(ResponseObjects responseObjects)
    {
        if (responseObjects.payment_shop != null)
        {
            PaymentShops.RegistShopInfo(responseObjects.payment_shop);
        }
        if (responseObjects.gacha_period != null)
        {
            GachaPeriods.RegistGachaPeriodInfo(responseObjects.gacha_period);
        }
        if (responseObjects.gacha_weapon != null)
        {
            GachaWeapons.RegistGachaInfo(responseObjects.gacha_weapon);
        }
        if (responseObjects.weapon_master != null)
        {
            MasterWeapons.RegistWeaponMasterInfo(responseObjects.weapon_master);
        }
        if (responseObjects.weapon_category != null)
        {
            WeaponCategories.RegistWeaponCategoryInfo(responseObjects.weapon_category);
        }   
        if (responseObjects.weapon_rarity != null)
        {
            WeaponRarities.RegistWeaponRarityInfo(responseObjects.weapon_rarity);
        }
        if (responseObjects.item_master != null)
        {
            MasterItems.RegistItemMasterInfo(responseObjects.item_master);
        }
        if (responseObjects.item_category != null)
        {
            ItemCategories.RegistItemCategoryInfo(responseObjects.item_category);
        }
    }

    /// <summary>
    /// リンクごとに情報の更新、保存を行う
    /// </summary>
    static void ConnectMove(string connectURL, ResponseObjects responseObjects)
    {
        switch (connectURL)
        {
            case GameUtil.Uri.Login:
                UpdateUserData(responseObjects);
                break;
            case GameUtil.Uri.Home:
            //case GameUtil.Uri.Stamina_Recovery:
                UpdateUserData(responseObjects);
                UpdateWalletData(responseObjects);
                UpdateWeaponData(responseObjects);
                UpdateItemData(responseObjects);
                break;
            case GameUtil.Uri.Register:
                UpdateUserData(responseObjects);
                UpdateWalletData(responseObjects);
                UpdateItemData(responseObjects);
                break;
            case GameUtil.Uri.Stamina_Consumption:
                UpdateUserData(responseObjects);
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
                    }
                    // フォールバックする処理を作成したらここに追加
                    yield break;
                }
            }
            else
            {
                // レスポンスの取得
                string text = webRequest.downloadHandler.text;

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
                            break;
                    }
                    yield break;
                }

                // デシリアライズでSQLiteに保存
                ResponseObjects responseObjects = JsonConvert.DeserializeObject<ResponseObjects>(text);

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
