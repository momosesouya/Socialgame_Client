using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class ItemCategoryModel
{
    public int item_category;
    public string category_name;
}

/// <summary>
/// item_categoriesテーブル
/// アイテムカテゴリー
/// </summary>
public class ItemCategories
{
    /// <summary>
    /// テーブル生成
    /// </summary>
    public static void CreateItemCategoryTable()
    {
        string createQuery = "create table if not exists item_categories (" +
                "item_category int, " +
                "category_name text, " +
                "primary key(item_category))";
        SqliteDatabase sqlDB = new SqliteDatabase(GameUtil.Common.DBFileName);
        sqlDB.ExecuteQuery(createQuery);
    }

    /// <summary>
    /// 新規登録
    /// </summary>
    /// <param name="weapon_categories_list"></param>
    public static void RegistItemCategoryInfo(ItemCategoryModel[] item_categories_list)
    {
        foreach (ItemCategoryModel itemCategoryModel in item_categories_list)
        {
            string query = "insert or replace into item_categories(item_category, category_name) " +
                            "values (@item_category, @category_name)";
            Dictionary<string, object> param = new Dictionary<string, object>()
            {
                {"@item_category", itemCategoryModel.item_category },
                {"@category_name", itemCategoryModel.category_name },
            };
            SqliteDatabase sqlDB = new SqliteDatabase(GameUtil.Common.DBFileName);
            sqlDB.ExecuteQuery(query, param);
        }
    }

    /// <summary>
    /// 指定されたアイテムカテゴリーのみ取得
    /// </summary>
    /// <param name="item_category"></param>
    /// <returns></returns>
    public static ItemCategoryModel GetItemCategoryData(int item_category)
    {
        ItemCategoryModel itemCategoryModel = new();
        string query = "select * from item_categories where item_category=" + item_category;
        SqliteDatabase sqlDB = new SqliteDatabase(GameUtil.Common.DBFileName);
        DataTable dataTable = sqlDB.ExecuteQuery(query);
        foreach (DataRow dr in dataTable.Rows)
        {
            itemCategoryModel.item_category = int.Parse(dr["item_category"].ToString());
            itemCategoryModel.category_name = dr["category_name"].ToString();
        }
        return itemCategoryModel;
    }
}
