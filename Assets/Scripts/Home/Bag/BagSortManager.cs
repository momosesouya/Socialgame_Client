using UnityEngine;
using UnityEngine.UI;

public class BagSortManager : WeaponBase
{
    [SerializeField] GameObject itemPrefab;
    [SerializeField] GameObject items;
    [SerializeField] GameObject lineupButton;
    [SerializeField] GameObject[] itemClone;
    [SerializeField] Sprite[] lineUpImage;
    Vector3 referencePoint = new Vector3(0, 0, 0);

    int generateVerticalNum = 4; // 生成する縦の個数
    int generateBesideNum = 5;   // 生成する横の個数
    int changeNum = 230;         // 縦横の配置で変化する量

    int weaponNum;

    int[] weaponId, weaponLevel, weaponExp, weaponCategory;
    [SerializeField] string[] weaponName;
    string default_name = "no name";

    [SerializeField] WeaponsModel[] weaponModel;

    bool sortMode = true; // 昇順か降順か、trueが昇順

    const string DEFAULTSORT = "DEFAULT";
    const string RARITYSORTASC = "RARITYASC";
    const string RARITYSORTDESC = "RARITYDESC";


    private void Awake()
    {
        itemClone = new GameObject[generateVerticalNum * generateBesideNum];
        SetWeaponParameters(DEFAULTSORT);
    }

    private void OnDisable() => DestroyItemClone();

    // 外部からバッグの情報更新
    public void UpdateBag()
    {
        DestroyItemClone();
        itemClone = new GameObject[generateVerticalNum * generateBesideNum];
        SetWeaponParameters(DEFAULTSORT);
    }

    void SetWeaponParameters(string setMode)
    {
        switch (setMode)
        {
            case DEFAULTSORT:
            case RARITYSORTASC:
                weaponModel = Weapons.GetRaritySortDesc(true); // レアリティ順に昇順で取得
                break;
            case RARITYSORTDESC:
                weaponModel = Weapons.GetRaritySortDesc(false); // レアリティ順に降順で取得
                break;
            default:
                weaponModel = Weapons.GetWeaponDataAll();
                break;
        }
        weaponNum = weaponModel.Length;
        weaponId = new int[weaponNum];
        weaponLevel = new int[weaponNum];
        weaponExp = new int[weaponNum];
        weaponCategory = new int[weaponNum];
        weaponName = new string[weaponNum];

        for (int i = 0; i < weaponNum; i++)
        {
            bool check = CheckSameId(weaponModel[i].weapon_id);
            if (!check)
            {
                weaponId[i] = weaponModel[i].weapon_id;
                weaponLevel[i] = weaponModel[i].level;
                weaponExp[i] = weaponModel[i].current_exp;
                weaponCategory[i] = MasterWeapons.GetWeaponMasterData(weaponId[i]).weapon_category;
                weaponName[i] = MasterWeapons.GetWeaponMasterData(weaponId[i]).weapon_name;
            }
            else
            { continue; }
        }
        SortGenerate();
    }

    // 重複したIDがないか確認
    bool CheckSameId(int checkId)
    {
        foreach (int id in weaponId)
        {
            if (id == 0) { continue; }
            if (checkId == id) { return true; }
        }

        return false;
    }

    // アイテムを並べて生成する
    public void SortGenerate()
    {
        int count = 0;
        for (int i = 0; i < generateVerticalNum; i++)
        {
            for (int j = 0; j < generateBesideNum; j++)
            {
                Vector3 setPos = new Vector3(referencePoint.x + (changeNum * j), referencePoint.y - (changeNum * i), 0);
                if (count < itemClone.Length && itemClone[count] == null)
                {
                    itemClone[count] = Instantiate(itemPrefab, setPos, Quaternion.identity);
                    GameObject currentClone = itemClone[count];

                    if (count < weaponModel.Length)
                    {
                        // 武器のデータがあるとき
                        currentClone.name = weaponName[count].ToString();
                        WeaponSetting(currentClone, weaponId[count]);
                        if (currentClone.GetComponent<BagItemManager>() != null)
                        {
                            BagItemManager bagItemManager = currentClone.GetComponent<BagItemManager>();
                            if (bagItemManager != null)
                            {
                                bagItemManager.SetParameters(weaponId[count], weaponLevel[count], weaponExp[count], weaponCategory[count], weaponName[count]);
                            }
                        }
                    }
                    else
                    {
                        // データがないとき
                        currentClone.name = "";
                        WeaponSetting(currentClone, 0); // ID:0 など空スロット用処理

                        BagItemManager bagItemManager = currentClone.GetComponent<BagItemManager>();
                        if (bagItemManager != null)
                        {
                            bagItemManager.SetParameters(0, 0, 0, 0, ""); // 空データを渡す
                            SetEmpty(); // ← 空用の表示切替
                        }
                    }
                    itemClone[count].transform.SetParent(items.transform, false);
                }
                count++;
            }
        }
    }

    // アイテムを並び替える
    void SortItems()
    {
        // ソート後のデータを反映
        for (int i = 0; i < itemClone.Length; i++)
        {
            if (i >= weaponModel.Length || itemClone[i] == null)
                continue;

            // 対応するデータを取り出す
            int id = weaponModel[i].weapon_id;
            int level = weaponModel[i].level;
            int exp = weaponModel[i].current_exp;
            int category = MasterWeapons.GetWeaponMasterData(id).weapon_category;
            string name = MasterWeapons.GetWeaponMasterData(id).weapon_name;

            // 表示内容だけ変更
            itemClone[i].name = name;
            WeaponSetting(itemClone[i], id);

            BagItemManager bagItemManager = itemClone[i].GetComponent<BagItemManager>();
            if (bagItemManager != null)
            {
                bagItemManager.SetParameters(id, level, exp, category, name);
            }
        }
    }

    void SetEmpty()
    {
    }



    // アイテムを削除する
    void DestroyItemClone()
    {
        for (int i = 0; i < generateVerticalNum; i++)
        {
            for (int j = 0; j < generateBesideNum; j++)
            {
                if (itemClone[j] != null) { Destroy(itemClone[j]); }
            }
        }
    }

    // 並び替えボタンが押されたら
    public void PushSortButton()
    {
    }

    // 絞り込みボタンが押されたら
    public void PushNarrowDownButton()
    {
    }

    // 昇順、降順のボタンが押されたら
    public void PushLineUpButton()
    {
        Image image = lineupButton.transform.GetChild(0).GetComponent<Image>();
        if (!sortMode)
        {
            SetWeaponParameters(RARITYSORTASC);
            sortMode = true;
            image.sprite = lineUpImage[0];
        }
        else
        {
            SetWeaponParameters(RARITYSORTDESC);
            sortMode = false;
            image.sprite = lineUpImage[1];
        }
        SortItems();
    }
}
