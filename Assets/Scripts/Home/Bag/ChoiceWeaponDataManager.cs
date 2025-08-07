using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceWeaponDataManager : WeaponBase
{
    int weapon_id;
    public int WeaponId { get { return weapon_id; } }

    int weapon_level;
    public int weaponLevel { get { return weapon_level; } }

    [SerializeField] TextMeshProUGUI weaponNameText;
    [SerializeField] GameObject detailWeaponBack, reinforceWeaponBack;
    [SerializeField] Button reinforceButton; // 強化ボタン

    private void Awake()
    {
        reinforceButton.interactable = false;
    }

    // 武器が選択された時、強化のために武器の基本データを保存する
    public void SetChoiceWeaponData(int id, int level)
    {
        weapon_id = id;
        weapon_level = level;

        reinforceButton.interactable = true;
    }

    // 詳細画面の設定、強化ボタンが押された時に画像等が変わる(武器)
    public void SetDetailData(int id)
    {
        WeaponsModel weaponData = Weapons.GetWeaponData(id);
        string name = MasterWeapons.GetWeaponMasterData(id).weapon_name;
        // 名前
        weaponNameText.text = name;

        // 画像
        WeaponSetting(detailWeaponBack, id);     // バッグ画面の武器詳細画面の武器画像変更
        WeaponSetting(reinforceWeaponBack, id); // 強化画面の武器詳細画面の武器画像変更
    }

}
