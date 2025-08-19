using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class WeaponCategoryModel
{
    public int weapon_category;
    public string category_name;
}

/// <summary>
/// weapon_categoriesテーブル
/// 武器カテゴリー
/// </summary>
public class WeaponCategories
{
    /// <summary>
    /// テーブル生成
    /// </summary>
    public static void CreateWeaponCategoryTable()
    {
        string createQuery = "create table if not exists weapon_categories (" +
                "weapon_category int, " +
                "category_name text, " +
                "primary key(weapon_category))";
        SqliteDatabase sqlDB = new SqliteDatabase(GameUtil.Common.DBFileName);
        sqlDB.ExecuteQuery(createQuery);
    }

    /// <summary>
    /// 新規登録
    /// </summary>
    /// <param name="weapon_categories_list"></param>
    public static void RegistWeaponCategoryInfo(WeaponCategoryModel[] weapon_categories_list)
    {
        foreach (WeaponCategoryModel weaponCategoryModel in weapon_categories_list)
        {
            string query = "insert or replace into weapon_categories(weapon_category, category_name) " +
                            "values (@weapon_category, @category_name)";
            Dictionary<string, object> param = new Dictionary<string, object>()
            {
                {"@weapon_category", weaponCategoryModel.weapon_category },
                {"@category_name", weaponCategoryModel.category_name },
            };
            SqliteDatabase sqlDB = new SqliteDatabase(GameUtil.Common.DBFileName);
            sqlDB.ExecuteQuery(query, param);
        }
    }

    /// <summary>
    /// 指定された武器カテゴリーのみ取得
    /// </summary>
    /// <param name="weapon_category"></param>
    /// <returns></returns>
    public static WeaponCategoryModel GetWeaponCategoryData(int weapon_category)
    {
        WeaponCategoryModel weaponCategoryModel = new();
        string query = "select * from weapon_categories where weapon_category=" + weapon_category;
        SqliteDatabase sqlDB = new SqliteDatabase(GameUtil.Common.DBFileName);
        DataTable dataTable = sqlDB.ExecuteQuery(query);
        foreach (DataRow dr in dataTable.Rows)
        {
            weaponCategoryModel.weapon_category = int.Parse(dr["weapon_category"].ToString());
            weaponCategoryModel.category_name = dr["category_name"].ToString();
        }
        return weaponCategoryModel;
    }
}
