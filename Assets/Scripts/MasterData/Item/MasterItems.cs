using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class ItemMasterModel
{
    public int item_id;       // アイテムID
    public string item_name;  // アイテム名
    public int item_category; // アイテムカテゴリー
}

/// <summary>
/// item_mastersテーブル
/// アイテムマスタ
/// </summary>
public class MasterItems
{
    /// <summary>
    /// テーブル生成
    /// </summary>
    public static void CreateItemMasterTable()
    {
        string createQuery = "create table if not exists item_masters (" +
            "item_id int, " +
            "item_name text, " +
            "item_category int, " +
            "primary key(item_id))";
        SqliteDatabase sqlDB = new SqliteDatabase(GameUtil.Common.DBFileName);
        sqlDB.ExecuteQuery(createQuery);
    }

    /// <summary>
    /// 新規登録
    /// </summary>
    /// <param name="item_master_list"></param>
    public static void RegistItemMasterInfo(ItemMasterModel[] item_master_list)
    {
        foreach (ItemMasterModel itemsMasterModel in item_master_list)
        {
            string query = "insert or replace into item_masters(item_id, item_name, item_category) " +
                            "values (@item_id, @item_name, @item_category)";
            Dictionary<string, object> param = new Dictionary<string, object>()
            {
                {"@item_id", itemsMasterModel.item_id },
                {"@item_name", itemsMasterModel.item_name },
                {"@item_category", itemsMasterModel.item_category },
            };
            SqliteDatabase sqlDB = new SqliteDatabase(GameUtil.Common.DBFileName);
            sqlDB.ExecuteQuery(query, param);
        }
    }

    /// <summary>
    /// 全ての武器データを取得
    /// </summary>
    /// <returns></returns>
    public static ItemMasterModel[] GetItemMasterDataAll()
    {
        List<ItemMasterModel> list = new();
        string query = "select * from items";
        SqliteDatabase sqlDB = new SqliteDatabase(GameUtil.Common.DBFileName);
        DataTable dataTable = sqlDB.ExecuteQuery(query);
        foreach (DataRow dr in dataTable.Rows)
        {
            ItemMasterModel itemsMasterModel = new();
            itemsMasterModel.item_id = int.Parse(dr["item_id"].ToString());
            itemsMasterModel.item_name = dr["item_name"].ToString();
            itemsMasterModel.item_category = int.Parse(dr["item_category"].ToString());
            list.Add(itemsMasterModel);
        }
        return list.ToArray(); // Listに変換して返す
    }

    /// <summary>
    /// 指定された武器IDのマスターデータのみを取得
    /// </summary>
    /// <param name="item_id"></param>
    /// <returns></returns>
    public static ItemMasterModel GetItemMasterData(int item_id)
    {
        ItemMasterModel itemsMasterModel = new();
        string query = "select * from item_masters where item_id=" + item_id;
        SqliteDatabase sqlDB = new SqliteDatabase(GameUtil.Common.DBFileName);
        DataTable dataTable = sqlDB.ExecuteQuery(query);
        foreach (DataRow dr in dataTable.Rows)
        {
            itemsMasterModel.item_id = int.Parse(dr["item_id"].ToString());
            itemsMasterModel.item_name = dr["item_name"].ToString();
            itemsMasterModel.item_category = int.Parse(dr["item_category"].ToString());
        }
        return itemsMasterModel;
    }
}
