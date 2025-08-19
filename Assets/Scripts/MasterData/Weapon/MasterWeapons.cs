using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class WeaponMasterModel
{
    public int weapon_id;       // 武器ID
    public string weapon_name;  // 武器名
    public int rarity_id;       // レアリティID
    public int weapon_category; // 武器カテゴリー
}

/// <summary>
/// weapon_mastersテーブル
/// 武器マスタ
/// </summary>
public class MasterWeapons
{
    /// <summary>
    /// テーブル生成
    /// </summary>
    public static void CreateWeaponMasterTable()
    {
        string createQuery = "create table if not exists weapon_masters (" +
            "weapon_id int, " +
            "weapon_name text, " +
            "rarity_id int, " +
            "weapon_category int, " +
            "primary key(weapon_id))";
        SqliteDatabase sqlDB = new SqliteDatabase(GameUtil.Common.DBFileName);
        sqlDB.ExecuteQuery(createQuery);
    }

    /// <summary>
    /// 新規登録
    /// </summary>
    /// <param name="weapon_master_list"></param>
    public static void RegistWeaponMasterInfo(WeaponMasterModel[] weapon_master_list)
    {
        foreach (WeaponMasterModel weaponsMasterModel in weapon_master_list)
        {
            string query = "insert or replace into weapon_masters(weapon_id, weapon_name, rarity_id, weapon_category) " +
                            "values (@weapon_id, @weapon_name, @rarity_id, @weapon_category)";
            Dictionary<string, object> param = new Dictionary<string, object>()
            {
                {"@weapon_id", weaponsMasterModel.weapon_id },
                {"@weapon_name", weaponsMasterModel.weapon_name },
                {"@rarity_id", weaponsMasterModel.rarity_id },
                {"@weapon_category", weaponsMasterModel.weapon_category },
            };
            SqliteDatabase sqlDB = new SqliteDatabase(GameUtil.Common.DBFileName);
            sqlDB.ExecuteQuery(query, param);
        }
    }

    /// <summary>
    /// 全ての武器データを取得
    /// </summary>
    /// <returns></returns>
    public static WeaponMasterModel[] GetWeaponMasterDataAll()
    {
        List<WeaponMasterModel> list = new();
        string query = "select * from weapons";
        SqliteDatabase sqlDB = new SqliteDatabase(GameUtil.Common.DBFileName);
        DataTable dataTable = sqlDB.ExecuteQuery(query);
        foreach (DataRow dr in dataTable.Rows)
        {
            WeaponMasterModel weaponsMasterModel = new();
            weaponsMasterModel.weapon_id = int.Parse(dr["weapon_id"].ToString());
            weaponsMasterModel.weapon_name = dr["weapon_name"].ToString();
            weaponsMasterModel.rarity_id = int.Parse(dr["rarity_id"].ToString());
            weaponsMasterModel.weapon_category = int.Parse(dr["weapon_category"].ToString());
            list.Add(weaponsMasterModel);
        }
        return list.ToArray(); // Listに変換して返す
    }

    /// <summary>
    /// 指定された武器IDのマスターデータのみを取得
    /// </summary>
    /// <param name="weapon_id"></param>
    /// <returns></returns>
    public static WeaponMasterModel GetWeaponMasterData(int weapon_id)
    {
        WeaponMasterModel weaponMasterModel = new();
        string query = "select * from weapon_masters where weapon_id=" + weapon_id;
        SqliteDatabase sqlDB = new SqliteDatabase(GameUtil.Common.DBFileName);
        DataTable dataTable = sqlDB.ExecuteQuery(query);
        foreach (DataRow dr in dataTable.Rows)
        {
            weaponMasterModel.weapon_id = int.Parse(dr["weapon_id"].ToString());
            weaponMasterModel.weapon_name = dr["weapon_name"].ToString();
            weaponMasterModel.rarity_id = int.Parse(dr["rarity_id"].ToString());
            weaponMasterModel.weapon_category = int.Parse(dr["weapon_category"].ToString());
        }
        return weaponMasterModel;
    }
}
