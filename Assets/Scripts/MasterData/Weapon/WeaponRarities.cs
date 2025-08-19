using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class WeaponRarityModel
{
    public int rarity_id;       // レアリティID
    public string rarity_name;  // レアリティ名
    public int get_item_amount; // ガチャで取得できる交換アイテム数
}

/// <summary>
/// weapon_mastersテーブル
/// 武器レアリティマスタ
/// </summary>
public class WeaponRarities
{
    /// <summary>
    /// テーブル生成
    /// </summary>
    public static void CreateRarityTable()
    {
        string createQuery = "create table if not exists weapon_rarities (" +
            "rarity_id int, " +
            "rarity_name text, " +
            "get_item_amount int, " +
            "primary key(rarity_id))";
        SqliteDatabase sqlDB = new SqliteDatabase(GameUtil.Common.DBFileName);
        sqlDB.ExecuteQuery(createQuery);
    }

    /// <summary>
    /// 新規登録
    /// </summary>
    /// <param name="weapon_master_list"></param>
    public static void RegistWeaponRarityInfo(WeaponRarityModel[] weapon_rarities_list)
    {
        foreach (WeaponRarityModel weaponRaritiesModel in weapon_rarities_list)
        {
            string query = "insert or replace into weapon_rarities(rarity_id, rarity_name, get_item_amount) " +
                            "values (@rarity_id, @rarity_name, @get_item_amount)";
            Dictionary<string, object> param = new Dictionary<string, object>()
            {
                {"@rarity_id", weaponRaritiesModel.rarity_id },
                {"@rarity_name", weaponRaritiesModel.rarity_name },
                {"@get_item_amount", weaponRaritiesModel.get_item_amount },
            };
            SqliteDatabase sqlDB = new SqliteDatabase(GameUtil.Common.DBFileName);
            sqlDB.ExecuteQuery(query, param);
        }
    }

    /// <summary>
    /// 全ての武器のレアリティデータを取得
    /// </summary>
    /// <returns></returns>
    public static WeaponRarityModel[] GetWeaponMasterDataAll()
    {
        List<WeaponRarityModel> weaponRarityList = new();
        string query = "select * from weapons";
        SqliteDatabase sqlDB = new SqliteDatabase(GameUtil.Common.DBFileName);
        DataTable dataTable = sqlDB.ExecuteQuery(query);
        foreach (DataRow dr in dataTable.Rows)
        {
            WeaponRarityModel weaponRaritiesModel = new();
            weaponRaritiesModel.rarity_id = int.Parse(dr["rarity_id"].ToString());
            weaponRaritiesModel.rarity_name = dr["rarity_name"].ToString();
            weaponRaritiesModel.get_item_amount = int.Parse(dr["get_item_amount"].ToString());
            weaponRarityList.Add(weaponRaritiesModel);
        }
        return weaponRarityList.ToArray(); // Listに変換して返す
    }

    /// <summary>
    /// 指定されたレアリティIDのデータのみを取得
    /// </summary>
    /// <param name="weapon_id"></param>
    /// <returns></returns>
    public static WeaponRarityModel GetWeaponRaritiesData(int rarity_id)
    {
        WeaponRarityModel weaponRaritiesModel = new();
        string query = "select * from weapon_rarities where rarity_id=" + rarity_id;
        SqliteDatabase sqlDB = new SqliteDatabase(GameUtil.Common.DBFileName);
        DataTable dataTable = sqlDB.ExecuteQuery(query);
        foreach (DataRow dr in dataTable.Rows)
        {
            weaponRaritiesModel.rarity_id = int.Parse(dr["rarity_id"].ToString());
            weaponRaritiesModel.rarity_name = dr["rarity_name"].ToString();
            weaponRaritiesModel.get_item_amount = int.Parse(dr["get_item_amount"].ToString());
        }
        return weaponRaritiesModel;
    }
}
