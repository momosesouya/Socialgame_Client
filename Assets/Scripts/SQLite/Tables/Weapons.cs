using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponsModel
{
    public int weapon_id;           // 武器ID
    public int rarity_id;           // レアリティID
    public int level;               // 武器レベル
    public int level_max;           // 武器レベル上限
    public int current_exp;         // 現在の武器経験値
}

/// <summary>
/// weaponsテーブル
/// </summary>
public class Weapons
{
    /// <summary>
    /// テーブル生成
    /// </summary>
    public static void CreateWeaponModel()
    {
        string createQuery = "create table if not exists weapons (" +
            "user_id int, " +
            "weapon_id int, " +
            "rarity_id int, " +
            "level int, " +
            "level_max int, " +
            "current_exp int, " +
            "primary key(weapon_id))";
        SqliteDatabase sqlDB = new SqliteDatabase(GameUtil.Common.DBFileName);
        sqlDB.ExecuteQuery(createQuery);
    }

    /// <summary>
    /// 新規登録
    /// </summary>
    /// <param name="weapons_model_list"></param>
    /// <param name="user_id"></param>
    public static void RegistWeaponInfo(WeaponsModel[] weapons_model_list, string user_id)
    {
        if (weapons_model_list == null || user_id == null) { return; }
        foreach (WeaponsModel weaponModel in weapons_model_list)
        {
            string query = "insert or replace into weapons(user_id, weapon_id, rarity_id, level, level_max, current_exp) " +
                            "values (@user_id, @weapon_id, @rarity_id, @level, @level_max, @currency_exp)";
            Dictionary<string, object> param = new Dictionary<string, object>()
            {
                {"@user_id", user_id },
                {"@weapon_id", weaponModel.weapon_id },
                {"@rarity_id", weaponModel.rarity_id },
                {"@level", weaponModel.level },
                {"@level_max", weaponModel.level_max },
                {"@currency_exp", weaponModel.current_exp },
            };
            SqliteDatabase sqlDB = new SqliteDatabase(GameUtil.Common.DBFileName);
            sqlDB.ExecuteQuery(query, param);
        }
        UpdateWeaponData(weapons_model_list, user_id);
    }

    /// <summary>
    /// データ取得
    /// </summary>
    /// <returns></returns>
    public static WeaponsModel Get(string user_id)
    {
        string getQuery = "select * from weapons limit 1";
        SqliteDatabase sqlDB = new SqliteDatabase(GameUtil.Common.DBFileName);
        DataTable dataTable = sqlDB.ExecuteQuery(getQuery);

        WeaponsModel weaponsModel = new WeaponsModel();
        foreach (DataRow dr in dataTable.Rows)
        {
            user_id = dr["user_id"].ToString();
            weaponsModel.weapon_id = int.Parse(dr["weapon_id"].ToString());
            weaponsModel.rarity_id = int.Parse(dr["rarity_id"].ToString());
            weaponsModel.level = int.Parse(dr["level"].ToString());
            weaponsModel.level_max = int.Parse(dr["level_max"].ToString());
            weaponsModel.current_exp = int.Parse(dr["current_exp"].ToString());
        }
        return weaponsModel;
    }

    /// <summary>
    /// データ更新
    /// </summary>
    /// <param name="weapons"></param>
    /// <param name="user_id"></param>
    public static void UpdateWeaponData(WeaponsModel[] weapons, string user_id)
    {
        foreach (WeaponsModel weapon in weapons)
        {
            string query = string.Format("UPDATE weapons SET " +
                "user_id = \"{0}\"," +
                "weapon_id = {1}," +
                "rarity_id = {2}," +
                "level = {3}," +
                "level_max = {4}," +
                "current_exp = {5}" +
                " WHERE user_id = \"{0}\" AND weapon_id = {1}",
                user_id,
                weapon.weapon_id,
                weapon.rarity_id,
                weapon.level,
                weapon.level_max,
                weapon.current_exp);
            SqliteDatabase sqlDB = new SqliteDatabase(GameUtil.Common.DBFileName);
            sqlDB.ExecuteQuery(query);
        }
    }

    /// <summary>
    /// 指定されたクエリを実行し、取得した武器データをWeaponModelの配列として返す。
    /// </summary>
    /// <param name="selectQuery"></param>
    /// <returns></returns>
    public static WeaponsModel[] GetWeaponDataDefault(string selectQuery)
    {
        List<WeaponsModel> weaponsList = new();
        SqliteDatabase sqlDB = new SqliteDatabase(GameUtil.Common.DBFileName);
        DataTable dataTable = sqlDB.ExecuteQuery(selectQuery);
        foreach (DataRow dr in dataTable.Rows)
        {
            WeaponsModel weaponModel = new();
            weaponModel.weapon_id = int.Parse(dr["weapon_id"].ToString());
            weaponModel.rarity_id = int.Parse(dr["rarity_id"].ToString());
            weaponModel.level = int.Parse(dr["level"].ToString());
            weaponModel.level_max = int.Parse(dr["level_max"].ToString());
            weaponModel.current_exp = int.Parse(dr["current_exp"].ToString());
            weaponsList.Add(weaponModel);
        }
        return weaponsList.ToArray();
    }

    public static WeaponsModel[] GetWeaponDataAll()
    {
        WeaponsModel[] weaponsList;
        weaponsList = GetWeaponDataDefault("select * from weapons");
        return weaponsList;
    }

    /// <summary>
    /// レアリティ順に並べ替えて取得
    /// isDescがtrueなら昇順、falseなら降順
    /// </summary>
    /// <param name="isDesc"></param>
    /// <returns></returns>
    public static WeaponsModel[] GetRaritySortDesc(bool isDesc)
    {
        WeaponsModel[] weaponsList;
        string query = "select * from weapons";
        if (isDesc)
        {
            weaponsList = GetWeaponDataDefault(string.Format("{0}{1}", query, " order by weapon_id asc"));
        }
        else
        {
            weaponsList = GetWeaponDataDefault(string.Format("{0}{1}", query, " order by weapon_id desc"));
        }
        return weaponsList;
    }

    /// <summary>
    /// 指定された武器IDの武器を取得
    /// </summary>
    /// <param name="weapon_id"></param>
    /// <returns></returns>
    public static WeaponsModel GetWeaponData(int weapon_id)
    {
        WeaponsModel weaponModel = new();
        string query = string.Format("select * from weapons where weapon_id = " + weapon_id);

        SqliteDatabase sqlDB = new SqliteDatabase(GameUtil.Common.DBFileName);
        DataTable dataTable = sqlDB.ExecuteQuery(query);
        foreach (DataRow dr in dataTable.Rows)
        {
            weaponModel.weapon_id = int.Parse(dr["weapon_id"].ToString());
            weaponModel.rarity_id = int.Parse(dr["rarity_id"].ToString());
            weaponModel.level = int.Parse(dr["level"].ToString());
            weaponModel.level_max = int.Parse(dr["level_max"].ToString());
            weaponModel.current_exp = int.Parse(dr["current_exp"].ToString());
        }
        return weaponModel;
    }


}
