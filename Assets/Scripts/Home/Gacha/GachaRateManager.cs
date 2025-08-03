using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponData
{
    public string rarity;      // レアリティ
    public string weapon_name; // 武器名
    public float rate;         // 確率
}

public class GachaRateManager : MonoBehaviour
{
    [SerializeField] GameObject gRPanel;
    [SerializeField] GameObject reteRowPrefab;
    [SerializeField] Transform scrollContent; // スクロール領域

    GachaWeaponModel[] gachaWeaponModel;
    public List<WeaponData> weaponsList;

    void Start()
    {
        foreach (var weapon in weaponsList)
        {

        }
    }

    void Update()
    {
        
    }

    void GetData()
    {
    }

    public void OpenButton()
    {
        gRPanel.SetActive(true);
    }

    public void BackButton()
    {
        gRPanel.SetActive(false);
    }
}
