using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GachaWeaponModel
{
    public int gacha_id;  // ガチャID
    public int weapon_id; // 武器ID
    public int weight;    // 重み
}

/// <summary>
/// gacha_weaponsテーブル
/// 武器ガチャ
/// </summary>
public class GachaWeapons
{
    /// <summary>
    /// テーブル生成
    /// </summary>
    public static void CreateGachaTable()
    {
        string createQuery = "create table if not exists gacha_weapons (" +
            "gacha_id int, " +
            "weapon_id int, " +
            "weight int, " +
            "primary key(gacha_id, weapon_id))";
        SqliteDatabase sqlDB = new SqliteDatabase(GameUtil.Common.DBFileName);
        sqlDB.ExecuteQuery(createQuery);
    }

    /// <summary>
    /// 新規登録
    /// </summary>
    /// <param name="gacha_weapon_list"></param>
    public static void RegistGachaInfo(GachaWeaponModel[] gacha_weapon_list)
    {
        foreach (GachaWeaponModel gachaWeaponModel in gacha_weapon_list)
        {
            string query = "insert or replace into gacha_weapons(gacha_id, weapon_id, weight) " +
                            "values (@gacha_id, @weapon_id, @weight)";
            Dictionary<string, object> param = new Dictionary<string, object>()
            {
                {"@gacha_id", gachaWeaponModel.gacha_id },
                {"@weapon_id", gachaWeaponModel.weapon_id },
                {"@weight", gachaWeaponModel.weight },
            };
            SqliteDatabase sqlDB = new SqliteDatabase(GameUtil.Common.DBFileName);
            sqlDB.ExecuteQuery(query, param);
        }
    }

    /// <summary>
    /// 確率表記用にソートしてデータを取得
    /// </summary>
    /// <returns></returns>
    public static GachaWeaponModel[] GetSortDataAll()
    {
        List<GachaWeaponModel> gachaWeaponList = new();
        string query = string.Format("select * from gacha_weapons order by weapon_id desc");
        SqliteDatabase sqlDB = new SqliteDatabase(GameUtil.Common.DBFileName);
        DataTable dataTable = sqlDB.ExecuteQuery(query);
        foreach (DataRow dr in dataTable.Rows)
        {
            GachaWeaponModel gachaWeaponModel = new();
            gachaWeaponModel.gacha_id = int.Parse(dr["gacha_id"].ToString());
            gachaWeaponModel.weapon_id = int.Parse(dr["weapon_id"].ToString());
            gachaWeaponModel.weight = int.Parse(dr["weight"].ToString());
            gachaWeaponList.Add(gachaWeaponModel);
        }
        return gachaWeaponList.ToArray();
    }

    /// <summary>
    /// 指定された武器IDの武器を取得
    /// </summary>
    /// <param name="gacha_id"></param>
    /// <returns></returns>
    public static GachaWeaponModel[] GetGachaWeaponData(int gacha_id)
    {
        List<GachaWeaponModel> list = new();
        string query = "select * from gacha_weapons where gacha_id = @gacha_id";
        Dictionary<string, object> param = new()
        {
            { "@gacha_id", gacha_id }
        };

        SqliteDatabase sqlDB = new SqliteDatabase(GameUtil.Common.DBFileName);
        DataTable dataTable = sqlDB.ExecuteQuery(query, param);
        foreach (DataRow dr in dataTable.Rows)
        {
            GachaWeaponModel model = new()
            {
                gacha_id = int.Parse(dr["gacha_id"].ToString()),
                weapon_id = int.Parse(dr["weapon_id"].ToString()),
                weight = int.Parse(dr["weight"].ToString())
            };
            list.Add(model);
        }
        return list.ToArray();
    }
}