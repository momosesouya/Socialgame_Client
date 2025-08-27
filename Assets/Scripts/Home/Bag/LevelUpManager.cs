using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[Serializable]
public class LevelUpResponse
{
    public int level;
    public int has_enhancement_item;
}

public class LevelUpManager : WeaponBase
{
    [SerializeField] GameObject reinforcePanel;
    [SerializeField] GameObject weaponImage;
    [SerializeField] TextMeshProUGUI changeLevelText, hasItemText;
    [SerializeField] Button levelUpButton; // レベルアップボタン

    private int stagedLevelUp = 0; // 現在のレベルからの増加数
    private int stagedItem = 0; // 仮のアイテム数
    private int hasItemCount = 0; // 所持アイテム数
    private int currentLevel = 0; // 現在のレベル
    private const int itemID = 1002;
    private const int resetItemNum = 0;

    string LevelStr = "Lv.";
    string slashStr = " / ";                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                            
    int weaponId, rarityId;

    bool isPush = false; // ボタンを押せるか

    ChoiceWeaponDataManager choiceWeaponManager;

    enum UnPushReason
    {
        NONE = 0,  // 押せる
        SHORTAGE,  // 所持数不足
        NOTSELECT, // 選択されていない
        MAX        // 上限に達している
    }
    UnPushReason currentState = UnPushReason.NOTSELECT;

    void Start()
    {
        reinforcePanel.SetActive(false);
        choiceWeaponManager = FindObjectOfType<ChoiceWeaponDataManager>();

        levelUpButton.interactable = false;
    }

    void Update()
    {
        IsPushLevelUpButton();  
    }

    // 武器のレアリティごとのアイテム必要数
    int GetRequiredItemCount(int rarityId)
    {
        switch (rarityId)
        {
            case 10000: return 1;
            case 15000: return 5;
            case 20000: return 10;
            case 25000: return 15;
            case 30000: return 20;
            default: return 0;
        }
    }

    // 強化画面表示
    public void OpenReinforcePanel()
    {
        weaponId = Weapons.GetWeaponData(choiceWeaponManager.WeaponId).weapon_id;
        rarityId = Weapons.GetWeaponData(choiceWeaponManager.WeaponId).rarity_id;

        // 武器画像変更
        choiceWeaponManager.SetDetailData(weaponId);
        Debug.Log("リセットしました");
        currentLevel = Weapons.GetWeaponData(weaponId).level;
        hasItemCount = Items.GetItemData(itemID).has_enhancement_item;
        stagedLevelUp = 0;
        stagedItem = 0;
        StandbyUpdateUI();
        reinforcePanel.SetActive(true);
    }

    public void CloseReinforcePanel()
    {
        reinforcePanel.SetActive(false);
    }

    // +ボタンの処理
    public void OnPlusButton()
    {
        Debug.Log("現在のレベル:" + currentLevel);

        // 現在のレベルが上限ではないとき
        if (currentLevel < Weapons.GetWeaponData(weaponId).level_max)
        {
            int cost = GetRequiredItemCount(rarityId);

            if (hasItemCount >= cost)
            {
                currentLevel += 1; // 仮のレベルアップ処理
                stagedLevelUp += 1;
                Debug.Log("変更時のレベル:" + currentLevel);
                Debug.Log("stagedLevelUp:" + stagedLevelUp);
                stagedItem += cost;
                hasItemCount -= cost;
                StandbyUpdateUI();
                currentState = UnPushReason.NONE;
            }
            else
            {
                currentState = UnPushReason.SHORTAGE;
                Debug.Log("アイテムが不足しています");
            }
        }
        else
        {
            currentState = UnPushReason.MAX;
            Debug.Log("レベルが上限です");
        }
    }

    // -ボタンの処理
    public void OnMinusButton()
    {
        if (stagedLevelUp > 0 && currentLevel > 0)
        {
            currentLevel -= 1;
            stagedLevelUp -= 1;
            stagedItem -= GetRequiredItemCount(rarityId);
            StandbyUpdateUI();
        }
        else if (stagedLevelUp == 0)
        {
            currentState = UnPushReason.NOTSELECT;
        }
    }

    // レベルスキップボタンの処理
    //public void OnMaxButton()
    //{
    //    int maxUp = Mathf.Min(
    //        (Weapons.GetWeaponData(weaponId).level_max - currentLevel),
    //        hasItemCount / GetRequiredItemCount(rarityId)
    //    );
    //    currentLevel = maxUp;
    //    stagedLevelUp = maxUp;
    //    stagedItem = maxUp;
    //    StandbyUpdateUI();
    //    currentState = UnPushReason.NONE;
    //}

    //// レベルリセットボタンの処理
    //public void OnMinButton()
    //{
    //    currentLevel = 1;
    //    stagedLevelUp = 0;
    //    stagedItem = 0;
    //    StandbyUpdateUI();
    //}

    // 強化前のスタンバイ状態でのUI変更
    void StandbyUpdateUI()
    {
        changeLevelText.text = string.Format("{0} {1}{2}{3}", LevelStr, currentLevel, slashStr, Weapons.GetWeaponData(weaponId).level_max);
        // アイテム数のUI更新
        hasItemText.text = string.Format("{0}{1}{2}", stagedItem, slashStr, hasItemCount);
    }

    // 確定したレベルやアイテム数などのUI変更
    private void ResetUI()
    {
        stagedLevelUp = 0;
        stagedItem = 0;
        currentLevel = Weapons.GetWeaponData(choiceWeaponManager.WeaponId).level;
        hasItemCount = Items.GetItemData(itemID).has_enhancement_item;

        // 武器レベルのUI更新
        changeLevelText.text = string.Format("{0} {1}{2}{3}", LevelStr, currentLevel, slashStr, Weapons.GetWeaponData(weaponId).level_max);
        // 所持アイテム数のUI更新
        hasItemText.text = string.Format("{0}{1}{2}", resetItemNum, slashStr, hasItemCount);
    }

    public void PushLevelUpButton()
    {
        StartCoroutine(LevelUp());
    }

    // ボタンの状態
    void IsPushLevelUpButton()
    {
        switch (currentState)
        {
            case UnPushReason.NONE:
                isPush = true;
                levelUpButton.interactable = true;
                break;
            case UnPushReason.SHORTAGE:
                isPush = false;
                levelUpButton.interactable = false;
                break;
            case UnPushReason.NOTSELECT:
                isPush = false;
                levelUpButton.interactable = false;
                break;
            case UnPushReason.MAX:
                isPush = false;
                levelUpButton.interactable = false;
                break;
        }
    }

    // 武器のレベルアップを行う
    IEnumerator LevelUp()
    {
        if (!isPush) { yield break; }
        // 武器データ
        //weaponId = Weapons.GetWeaponData(choiceWeaponManager.WeaponId).weapon_id;
        // 使用するアイテム数
        //int totalUseItemCount = stagedLevelUp + GetRequiredItemCount(rarityId);
        List<IMultipartFormSection> levelUpForm = new()
        {
            new MultipartFormDataSection("uid", Users.Get().user_id),
            new MultipartFormDataSection("wid", weaponId.ToString()),
            new MultipartFormDataSection("add_level", stagedLevelUp.ToString()),
        };

        UnityWebRequest request = UnityWebRequest.Post(GameUtil.Uri.LevelUp, levelUpForm);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("通信エラー: " + request.error);
            Debug.Log(request.downloadHandler.text);
            yield break;
        }

        string json = request.downloadHandler.text;
        LevelUpResponse response = JsonUtility.FromJson<LevelUpResponse>(json);

        if (response != null)
        {
            // サーバーのレスポンスから更新
            currentLevel = response.level;
            hasItemCount = response.has_enhancement_item;
            Debug.Log("返ってきたアイテムの数" + response.has_enhancement_item);

            // UI更新
            stagedLevelUp = 0;
            //stagedLevelUp = 0;
            stagedItem = 0;

            // 武器レベル保存
            WeaponsModel weapon = new WeaponsModel()
            {
                weapon_id = weaponId,
                rarity_id = rarityId,
                level = currentLevel,
                level_max = Weapons.GetWeaponData(weaponId).level_max
            };
            // 配列にして渡す
            WeaponsModel[] weapons = new WeaponsModel[] { weapon };
            Weapons.UpdateWeaponData(weapons , Users.Get().user_id);

            // アイテムの保存
            ItemsModel item = Items.GetItemData(itemID);
            int updatedItemNum = response.has_enhancement_item;
            Items.UpdateItemData(itemID, updatedItemNum);

            ResetUI();
            Debug.Log("レベルアップ通信成功");
            Debug.Log("新しいレベル:" + response.level);
            Debug.Log("所持アイテム数:" + updatedItemNum);

            // 武器レベル、所持アイテム数再取得
            currentLevel = Weapons.GetWeaponData(choiceWeaponManager.WeaponId).level;
            hasItemCount = Items.GetItemData(itemID).has_enhancement_item;

            currentState = UnPushReason.NOTSELECT;
        }
        else
        {
            Debug.LogError("レスポンスの内容が想定外: " + json);
        }
    }
}
