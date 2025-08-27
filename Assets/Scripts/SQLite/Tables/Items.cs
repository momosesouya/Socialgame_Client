using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsModel
{
    public int item_id;                    // アイテムID
    public int has_enhancement_item;       // 強化アイテム所持数
    public int has_stamina_item;           // スタミナアイテム所持数
    public int has_exchange_item;          // 交換アイテム所持数
}

/// <summary>
/// itemsテーブル
/// </summary>
public class Items
{
    /// <summary>
    /// テーブル生成
    /// </summary>
    public static void CreateItemModel()
    {
        string createQuery = "create table if not exists items (" +
            "user_id int, " +
            "item_id int, " +
            "has_enhancement_item int, " +
            "has_stamina_item int, " +
            "has_exchange_item int, " +
            "primary key(item_id))";
        SqliteDatabase sqlDB = new SqliteDatabase(GameUtil.Common.DBFileName);
        sqlDB.ExecuteQuery(createQuery);
    }

    /// <summary>
    /// 新規登録
    /// </summary>
    /// <param name="items_model_list"></param>
    /// <param name="item_id"></param>
    public static void RegistItemInfo(ItemsModel[] items_model_list, string user_id)
    {
        foreach (ItemsModel itemModel in items_model_list)
        {
            string query = "insert or replace into items(user_id, item_id, has_enhancement_item, has_stamina_item, has_exchange_item) " +
                            "values (@user_id, @item_id, @has_enhancement_item, @has_stamina_item, @has_exchange_item)";
            Dictionary<string, object> param = new Dictionary<string, object>()
            {
                {"@user_id", user_id },
                {"@item_id", itemModel.item_id },
                {"@has_enhancement_item", itemModel.has_enhancement_item },
                {"@has_stamina_item", itemModel.has_stamina_item },
                {"@has_exchange_item", itemModel.has_exchange_item },
            };
            SqliteDatabase sqlDB = new SqliteDatabase(GameUtil.Common.DBFileName);
            sqlDB.ExecuteQuery(query, param);
        }
    }

    /// <summary>
    /// 指定されたデータを取得
    /// </summary>
    public static ItemsModel GetItemData(int item_id)
    {
        string getQuery = "select * from items where item_id = @item_id";
        Dictionary<string, object> param = new() 
        {
            {"@item_id", item_id }
        };

        SqliteDatabase sqlDB = new SqliteDatabase(GameUtil.Common.DBFileName);
        DataTable dataTable = sqlDB.ExecuteQuery(getQuery, param);

        ItemsModel itemsModel = new ItemsModel();
        if (dataTable.Rows.Count > 0)
        {
            DataRow dr = dataTable.Rows[0];
            itemsModel.item_id = int.Parse(dr["item_id"].ToString());
            itemsModel.has_enhancement_item = int.Parse(dr["has_enhancement_item"].ToString());
            itemsModel.has_stamina_item = int.Parse(dr["has_stamina_item"].ToString());
            itemsModel.has_exchange_item = int.Parse(dr["has_exchange_item"].ToString());
        }
        return itemsModel;
    }

    // アイテム更新
    public static void UpdateItemData(int item_id, int has_enhancement_item, int has_stamina_item = 0, int has_exchange_item = 0)
    {
        string updateQuery = @"update items set 
            has_enhancement_item = @has_enhancement_item,
            has_stamina_item = @has_stamina_item,
            has_exchange_item = @has_exchange_item
            WHERE item_id = @item_id";

        Dictionary<string, object> param = new()
        {
            { "@has_enhancement_item", has_enhancement_item },
            { "@has_stamina_item", has_stamina_item },
            { "@has_exchange_item", has_exchange_item },
            { "@item_id", item_id }
        };

        SqliteDatabase sqlDB = new SqliteDatabase(GameUtil.Common.DBFileName);
        sqlDB.ExecuteQuery(updateQuery, param);
    }

    /// <summary>
    /// 全データ取得
    /// </summary>
    public static ItemsModel[] GetItemDataAll()
    {
        List<ItemsModel> list = new();
        string getQuery = "select * from items";
        SqliteDatabase sqlDB = new SqliteDatabase(GameUtil.Common.DBFileName);
        DataTable dataTable = sqlDB.ExecuteQuery(getQuery);
        foreach (DataRow dr in dataTable.Rows)
        {
            ItemsModel itemsModel = new();
            itemsModel.item_id = int.Parse(dr["item_id"].ToString());
            itemsModel.has_enhancement_item = int.Parse(dr["has_enhancement_item"].ToString());
            itemsModel.has_stamina_item = int.Parse(dr["has_stamina_item"].ToString());
            itemsModel.has_exchange_item = int.Parse(dr["has_exchange_item"].ToString());
            list.Add(itemsModel);
        }
        return list.ToArray(); // Listを配列に変換して返す
    }


}
