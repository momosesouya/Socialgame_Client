using Newtonsoft.Json;
using System;
using UnityEngine;

public class JsonDebugTest : MonoBehaviour
{
    [TextArea(10, 30)]
    public string jsonInput;

    void Start()
    {
        try
        {
            // パース可能かをまず確認
            var parsedRoot = JsonConvert.DeserializeObject<ResponseObjects>(jsonInput);

            if (parsedRoot == null)
            {
                Debug.LogError("ResponseObjects のデシリアライズに失敗しました（null）");
                return;
            }

            // weaponsの存在確認
            if (parsedRoot.weapons == null)
            {
                Debug.LogWarning("weapons が null です。JSONの構造とWeaponsModelクラスの整合性を確認してください。");
            }
            else
            {
                Debug.Log($"weapons 配列の数: {parsedRoot.weapons.Length}");
                for (int i = 0; i < parsedRoot.weapons.Length; i++)
                {
                    var weapon = parsedRoot.weapons[i];
                    Debug.Log($"[{i}] weapon_id: {weapon.weapon_id}, rarity: {weapon.rarity_id}, level: {weapon.level}");
                }
            }

            // new_weapons も確認
            if (parsedRoot.new_weapons != null)
            {
                Debug.Log($"new_weapons 配列の数: {parsedRoot.new_weapons.Length}");
            }
            else
            {
                Debug.LogWarning("new_weapons は null");
            }

        }
        catch (Exception e)
        {
            Debug.LogError("JSONのパース中に例外が発生: " + e.Message);
            Debug.LogError("スタックトレース: " + e.StackTrace);
        }
    }
}
