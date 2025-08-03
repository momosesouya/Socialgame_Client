using TMPro;
using UnityEngine;

public class BagItemManager : WeaponBase
{
    [SerializeField] int weapon_level, weapon_exp, item_category, weapon_id, rarity_id;
    [SerializeField] string item_name = "no name";
    string default_name = "no name";

    [SerializeField] TextMeshProUGUI detailNameText, reinforceNameText;
    [SerializeField] GameObject detailWeaponBack, reinforceWeaponBack;
    ChoiceWeaponDataManager choiceManager;

    void OnEnable()
    {
        choiceManager = FindObjectOfType<ChoiceWeaponDataManager>();
    }

    /// <summary>
    /// 各自パラメータを設定する
    /// </summary>
    public void SetParameters(int weaponId, int currentLevel, int currentExp, int weaponCategory, string weaponName)
    {
        weapon_id = weaponId;
        weapon_level = currentLevel;
        weapon_exp = currentExp;
        item_category = weaponCategory;
        item_name = weaponName;
    }

    // クリックされた時に指定されたIDの武器の情報を表示する
    public void DisplayWeaponData()
    {
        if (item_name == default_name) { return; }
        choiceManager.SetChoiceWeaponData(weapon_id, weapon_level);
        choiceManager.SetDetailData(weapon_id);
    }

    public void SetEmpty()
    {
        detailWeaponBack.SetActive(false);
        detailNameText.text = default_name.ToString();
    }
}
