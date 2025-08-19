using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
    

public class LevelUpManager : WeaponBase
{
    [SerializeField] GameObject reinforcePanel;
    [SerializeField] GameObject weaponImage;
    [SerializeField] TextMeshProUGUI changeLevelText, hasItemText;
    [SerializeField] Button levelUpButton; // レベルアップボタン

    private int stagedLevelUp = 1; // ＋や－で変動させる準備レベル
    private int stagedItem = 0; // ＋や－で変動させるアイテム
    private int hasItemCount = 0; // 所持アイテム数
    private const int itemID = 1002;
    private const int resetItemNum = 0;

    string LevelStr = "Lv.";
    string slashStr = " / ";
    int weaponId, rarityId, currentLevel;

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
        // アイテム所持数取得
        hasItemCount = Items.GetItemData(itemID).has_enhancement_item;
        levelUpButton.interactable = false;
    }

    void Update()
    {
        //weaponId = Weapons.GetWeaponData(choiceWeaponManager.WeaponId).weapon_id;
        currentLevel = Weapons.GetWeaponData(choiceWeaponManager.WeaponId).level;
        //rarityId = Weapons.GetWeaponData(choiceWeaponManager.WeaponId).rarity_id;

        // UI更新
        StandbyUpdateUI();

        IsPushLevelUpBUtton();  
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

    public void OpenReinforcePanel()
    {
        stagedLevelUp = Weapons.GetWeaponData(choiceWeaponManager.WeaponId).level;
        weaponId = Weapons.GetWeaponData(choiceWeaponManager.WeaponId).weapon_id;
        //currentLevel = Weapons.GetWeaponData(choiceWeaponManager.WeaponId).level;
        rarityId = Weapons.GetWeaponData(choiceWeaponManager.WeaponId).rarity_id;
        Debug.Log(GetRequiredItemCount(rarityId));

        // 武器画像変更
        choiceWeaponManager.SetDetailData(weaponId);

        reinforcePanel.SetActive(true);
    }

    public void CloseReinforcePanel()
    {
        reinforcePanel.SetActive(false);
    }

    // +ボタンの処理
    public void OnPlusButton()
    {
        // 現在のレベルが上限ではないとき
        if (currentLevel + stagedLevelUp < Weapons.GetWeaponData(weaponId).level_max)
        {
            int cost = GetRequiredItemCount(rarityId);

            if (hasItemCount >= cost)
            {
                stagedLevelUp++;
                Debug.Log(stagedLevelUp);
                stagedItem += cost;
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
        if (stagedLevelUp > 0)
        {
            stagedLevelUp--;
            stagedItem -= GetRequiredItemCount(rarityId);
            StandbyUpdateUI();
        }

        if (stagedLevelUp <= 1)
        {
            currentState = UnPushReason.NOTSELECT;
        }
    }

    // レベルスキップボタンの処理
    public void OnMaxButton()
    {
        int maxUp = Mathf.Min(
            (Weapons.GetWeaponData(weaponId).level_max - currentLevel),
            hasItemCount / GetRequiredItemCount(rarityId)
        );
        stagedLevelUp = maxUp;
        stagedItem = maxUp;
        StandbyUpdateUI();
        currentState = UnPushReason.NONE;
    }

    // レベルリセットボタンの処理
    public void OnMinButton()
    {
        stagedLevelUp = 1;
        stagedItem = 0;
        StandbyUpdateUI();
    }

    // 強化前のスタンバイ状態でのUI変更
    void StandbyUpdateUI()
    {
        changeLevelText.text = string.Format("{0}{1}{2}{3}", LevelStr, stagedLevelUp, slashStr, Weapons.GetWeaponData(weaponId).level_max);
        // 所持アイテム数のUI更新
        hasItemText.text = string.Format("{0}{1}{2}", stagedItem, slashStr, hasItemCount);
    }

    // 確定したレベルやアイテム数などのUI変更
    private void UpdateUI()
    {
        // 武器レベルのUI更新
        changeLevelText.text = string.Format("{0}{1}{2}{3}", LevelStr, currentLevel, slashStr, Weapons.GetWeaponData(weaponId).level_max);
        // 所持アイテム数のUI更新
        hasItemText.text = string.Format("{0}{1}{2}", resetItemNum, slashStr, hasItemCount);

    }

    public void PushLevelUpButton()
    {
        StartCoroutine(LevelUp());
    }

    // ボタンの状態
    void IsPushLevelUpBUtton()
    {
        switch (currentState)
        {
            case UnPushReason.NONE:
                isPush = true;
                levelUpButton.interactable = true;
                break;
            case UnPushReason.SHORTAGE:
                //StartCoroutine(ResultPanelController.DisplayResultPanel("強化ポイントが足りません"));
                isPush = false;
                levelUpButton.interactable = false;
                break;
            case UnPushReason.NOTSELECT:
                isPush = false;
                levelUpButton.interactable = false;
                break;
            case UnPushReason.MAX:
                //StartCoroutine(ResultPanelController.DisplayResultPanel("レベル上限です"));
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
        weaponId = Weapons.GetWeaponData(choiceWeaponManager.WeaponId).weapon_id;
        // 使用するアイテム数
        //int totalUseItemCount = stagedLevelUp + GetRequiredItemCount(rarityId);

        List<IMultipartFormSection> levelUpForm = new()
        {
            new MultipartFormDataSection("uid", Users.Get().user_id),
            new MultipartFormDataSection("wid", weaponId.ToString()),
            new MultipartFormDataSection("count", (GetRequiredItemCount(rarityId) * stagedLevelUp).ToString())
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
        WeaponsModel responseLevel = JsonUtility.FromJson<WeaponsModel>(json);
        ItemsModel responseItem = JsonUtility.FromJson<ItemsModel>(json);

        if (responseLevel != null && responseItem != null)
        {
            // サーバーのレスポンスから更新
            currentLevel = responseLevel.level;
            hasItemCount = responseItem.has_enhancement_item;

            // UI更新
            stagedLevelUp = responseLevel.level;
            //stagedLevelUp = 0;
            stagedItem = 0;

            UpdateUI();
            Debug.Log("レベルアップ通信成功");
            Debug.Log(responseLevel.level);
            Debug.Log(responseItem.has_enhancement_item);
        }
    }
}
