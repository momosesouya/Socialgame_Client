using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class GachaMove : WeaponBase
{
    [SerializeField] GameObject gachaSingleResult, gachaMultiResult;
    [SerializeField] GameObject gachaNormalRatePanel, gachaRareRatePanel;
    [SerializeField] GameObject resultWeapon;
    [SerializeField] TextMeshProUGUI[] fragmentText;
    [SerializeField] Sprite[] weaponsSprites;
    [SerializeField] int gachaId;
    [SerializeField] Button resultSingleButton;
    [SerializeField] Button resultMultiButton;
    [SerializeField] Button backMultiButton;
    [SerializeField] BagSortManager bagSortManager;

    bool isGachaRunning = false;
    GameObject[] weaponClone;

    [SerializeField] Vector3[] MultiPos;

    GachaManager gachaManager;

    int fragmentNum = 0;
    int[] newWeapons, resultWeapons = { };

    void Start()
    {
        gachaSingleResult.SetActive(false);
        gachaMultiResult.SetActive(false);
        weaponClone = new GameObject[10];
        resultWeapons = new int[10];
        newWeapons = new int[10];

        gachaManager = FindObjectOfType<GachaManager>();
    }

    public void ActiveButton()
    {
        if (!resultMultiButton.interactable && !backMultiButton.interactable)
        {
            resultMultiButton.interactable = true;
            backMultiButton.interactable = true;
        }
        else
        {
            resultMultiButton.interactable = false;
            backMultiButton.interactable = false;
        }
    }

    // 単発ガチャの時
    public void SingleMove()
    {
        if (isGachaRunning) return;
        isGachaRunning = true;
        resultMultiButton.interactable = false;

        if (Wallets.Get().free_amount + Wallets.Get().paid_amount >= 160)
        {
            List<IMultipartFormSection> gachaForm = new();
            gachaForm.Add(new MultipartFormDataSection("uid", Users.Get().user_id));
            gachaForm.Add(new MultipartFormDataSection("gCount", "1"));
            gachaForm.Add(new MultipartFormDataSection("gacha_id", gachaId.ToString()));
            Action afterAction = new(() =>
            {
                SuccessGachaSingle();
                isGachaRunning = false;
            });
            StartCoroutine(ConnectServer(GameUtil.Uri.Gacha_Execute, gachaForm, afterAction));
        }
        else
        {
            gachaManager.OpenCurrencyPanel();
            ActiveButton();
            gachaSingleResult.SetActive(false);
            isGachaRunning = false;
        }
        // バッグ情報更新
        bagSortManager.UpdateBag();
    }

    // 十連ガチャの時
    public void MultiMove()
    {
        if (isGachaRunning) return;
        isGachaRunning = true;
        resultMultiButton.interactable = false;
        backMultiButton.interactable = false;

        if (Wallets.Get().free_amount + Wallets.Get().paid_amount >= 1600)
        {
            List<IMultipartFormSection> gachaForm = new();
            gachaForm.Add(new MultipartFormDataSection("uid", Users.Get().user_id));
            gachaForm.Add(new MultipartFormDataSection("gCount", "10"));
            gachaForm.Add(new MultipartFormDataSection("gacha_id", gachaId.ToString()));
            Action afterAction = new(() =>
            {
                SuccessGachaMulti();
                isGachaRunning = false;
            });
            StartCoroutine(ConnectServer(GameUtil.Uri.Gacha_Execute, gachaForm, afterAction));

        }
        // 通貨が足りない場合
        else
        {
            for (int i = 0; i < weaponClone.Length; i++)
            {
                if (weaponClone[i] != null)
                {
                    Destroy(weaponClone[i]);
                    weaponClone[i] = null;
                }
            }

            gachaManager.OpenCurrencyPanel();
            gachaMultiResult.SetActive(false);
            isGachaRunning = false;
        }
        // バッグ情報更新
        bagSortManager.UpdateBag();
    }

    void SuccessGachaSingle()
    {
        SingleWait();
    }

    void SuccessGachaMulti()
    {
        MultiWait();
    }

    void SingleWait()
    {
        gachaSingleResult.SetActive(false);
        if (weaponClone[0] != null) { Destroy(weaponClone[0]); }
        gachaSingleResult.SetActive(true);
        weaponClone[0] = Instantiate(resultWeapon, new Vector3(0, 0, 0), Quaternion.identity);
        weaponClone[0].transform.SetParent(gachaSingleResult.transform, false);
        int weaponId = resultWeapons[0];
        WeaponSetting(weaponClone[0], weaponId);
        fragmentText[0].text = string.Format("取得かけら数:{0}個", fragmentNum);
    }

    void MultiWait()
    {
        StartCoroutine(MultiGachaResult());
        fragmentText[1].text = string.Format("取得かけら数:{0}個", fragmentNum);
    }

    public void PushRatePanel()
    {
        if (gachaId == 100)
        {
            gachaNormalRatePanel.SetActive(true);
        }
        else
        {
            gachaRareRatePanel.SetActive(true);
        }
    }

    public void PushBackButton()
    {
        gachaSingleResult.SetActive(false);
        Destroy(weaponClone[0]);
        gachaMultiResult.SetActive(false);
        gachaNormalRatePanel.SetActive(false);
        gachaRareRatePanel.SetActive(false);
    }

    // サーバーからの情報を保存する
    void GachaSetting(ResponseObjects responseObjects)
    {
        Debug.Log($"[GachaSetting] ガチャID: {gachaId}, 武器数: {responseObjects?.weapons?.Length ?? 0}");

        if (responseObjects.weapons != null && responseObjects.new_weapons != null)
        {
            UsersModel usersModel = Users.Get();
            List<WeaponsModel> newWeaponList = new();

            foreach (var weapon in responseObjects.weapons)
            {
                if (responseObjects.new_weapons.Contains(weapon.weapon_id))
                {
                    newWeaponList.Add(weapon);
                }
            }

            if (newWeaponList.Count > 0)
            {
                Weapons.RegistWeaponInfo(newWeaponList.ToArray(), usersModel.user_id);
            }
        }
        if (responseObjects.wallets != null)
        {
            UsersModel usersModel = Users.Get();
            Wallets.RegistWalletinfo(responseObjects.wallets, usersModel.user_id);
        }
        if (responseObjects.new_weapons != null)
        {
            newWeapons = responseObjects.new_weapons;
        }
        if (responseObjects.fragment_num != null)
        {
            fragmentNum = responseObjects.fragment_num;
        }

        // "/"区切りで10件取得
        if (responseObjects.weapons != null)
        {
            string[] splitResult = responseObjects.gacha_result.Split('/');
            int[] gachaResult = new int[Math.Min(splitResult.Length, 10)];
            for (int i = 0; i < gachaResult.Length; i++)
            {
                gachaResult[i] = int.Parse(splitResult[i]);
            }
            resultWeapons = gachaResult;
        }
    }

    // 10連の結果を出力
    public IEnumerator MultiGachaResult()
    {
        // 前の要素が残っていたら削除
        for (int i = 0; i < weaponClone.Length; i++)
        {
            if (weaponClone[i] != null)
            {
                Destroy(weaponClone[i]);
                weaponClone[i] = null;
            }
        }

        yield return new WaitForSeconds(0.5f);
        gachaMultiResult.SetActive(true);

        for (int i = 0; i < 10; i++)
        {
            weaponClone[i] = Instantiate(resultWeapon, MultiPos[i], Quaternion.identity);
            weaponClone[i].transform.SetParent(gachaMultiResult.transform, false);
            int weaponId = resultWeapons[i];
            WeaponSetting(weaponClone[i], weaponId);
            yield return new WaitForSeconds(0.5f);
        }
        // ボタンをアクティブ
        ActiveButton();
    }

    // サーバーに接続する
    public IEnumerator ConnectServer(string connectURL, List<IMultipartFormSection> parameter, Action action)
    {
        // *** リクエストの送付 ***
        using (UnityWebRequest webRequest = UnityWebRequest.Post(connectURL, parameter))
        {
            webRequest.timeout = 10;
            yield return webRequest.SendWebRequest();
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                // エラーの場合
                if (!string.IsNullOrEmpty(webRequest.error))
                {
                    Debug.LogError(webRequest.error);
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
                            break;
                        case GameUtil.Common.ERROR_DB_UPDATE:
                            Debug.LogError("サーバーでエラーが発生しました。[データベース更新エラー]");
                            break;
                        default:
                            Debug.LogError("サーバーでエラーが発生しました。[システムエラー]");
                            break;
                    }
                    yield break;
                }

                // SQLiteへの保存処理
                ResponseObjects responseObjects = JsonConvert.DeserializeObject<ResponseObjects>(text);
                Debug.Log($"レスポンス: + {text}");
                yield return new WaitForSeconds(0.5f);
                GachaSetting(responseObjects);

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