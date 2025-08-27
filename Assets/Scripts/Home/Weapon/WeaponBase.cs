using UnityEngine;
using UnityEngine.UI;

public class WeaponBase : MonoBehaviour
{

    protected void WeaponSetting(GameObject weapon, int weaponId)
    {
        Image weaponImage = weapon.transform.GetChild(0).GetComponent<Image>();
        Image weaponBack = weapon.GetComponent<Image>();
        weaponImage.sprite = Resources.Load<Sprite>(string.Format("WeaponImage/w{0}", weaponId.ToString())); // Resourcesフォルダの中の特定の画像を取得して入れる
        Outline outline = weapon.GetComponent<Outline>();
        int rarity = MasterWeapons.GetWeaponMasterData(weaponId).rarity_id;
        switch (rarity)
        {
            case 10000: // Normal
                outline.effectColor = Color.green;
                weaponBack.color = Color.white;
                break;
            case 15000: // Rare 
                outline.effectColor = Color.yellow;
                weaponBack.color = Color.white;
                break;
            case 20000: // SRare
                outline.effectColor = Color.cyan;
                weaponBack.color = Color.white;
                break;
            case 25000: // SSRare
                outline.effectColor = Color.magenta;
                break;
            case 30000: // UltraRare
                outline.effectColor = Color.red;
                break;
            default:
                break;
        }
    }

    // 指定した数字の指定の桁の数値を返す 参考サイト https://santerabyte.com/c-sharp-get-nth-digit-num/
    protected int GetNthDigitNum(int num, int digit)
    {
        int currentDigitNum = 1;
        num = System.Math.Abs(num);
        while (num > 0)
        {
            if (currentDigitNum == digit) return num % 10;
            num /= 10;
            currentDigitNum++;
        }
        return 0;
    }
}
