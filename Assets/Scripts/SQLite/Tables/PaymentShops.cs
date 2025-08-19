using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MasterDataResponse
{
    public string master_data_version;
    public List<PaymentShopModel> payment_shops;
}

[Serializable]
public class PaymentShopModel
{
    public int product_id;      // 商品ID
    public string product_name; // 商品名
    public int paid_currency;   // 有償通貨数
    public int bonus_currency;   // おまけ無償通貨
    public int price;           // 販売価格
}

/// <summary>
/// payment_shopsテーブル
/// 通貨ショップ
/// </summary>
public class PaymentShops
{
    /// <summary>
    /// テーブル生成
    /// </summary>
    public static void CreateShopTable()
    {
        string createQuery = "create table if not exists payment_shops (" +
            "product_id int, " +
            "product_name text, " +
            "paid_currency int, " +
            "bonus_currency int, " +
            "price int, " +
            "primary key(product_id))";
        SqliteDatabase sqlDB = new SqliteDatabase(GameUtil.Common.DBFileName);
        sqlDB.ExecuteQuery(createQuery);
    }

    /// <summary>
    /// 新規登録
    /// </summary>
    /// <param name="payment_model_list"></param>
    public static void RegistShopInfo(PaymentShopModel[] payment_model_list)
    {
        foreach (PaymentShopModel paymentShopModel in payment_model_list)
        {
            string query = "insert or replace into payment_shops(product_id, product_name, paid_currency, bonus_currency, price) " +
                            "values (@product_id, @product_name, @paid_currency, @bonus_currency, @price)";
            Dictionary<string, object> param = new Dictionary<string, object>()
            {
                {"@product_id", paymentShopModel.product_id },
                {"@product_name", paymentShopModel.product_name },
                {"@paid_currency", paymentShopModel.paid_currency },
                {"@bonus_currency", paymentShopModel.bonus_currency },
                {"@price", paymentShopModel.price },
            };
            SqliteDatabase sqlDB = new SqliteDatabase(GameUtil.Common.DBFileName);
            sqlDB.ExecuteQuery(query, param);
        }
    }

    /// <summary>
    /// 全ての商品を取得
    /// </summary>
    /// <returns></returns>
    public static PaymentShopModel[] GetShopDataAll()
    {
        List<PaymentShopModel> list = new();
        string query = "select * from payment_shops";
        SqliteDatabase sqlDB = new SqliteDatabase(GameUtil.Common.DBFileName);
        DataTable dataTable = sqlDB.ExecuteQuery(query);
        foreach (DataRow dr in dataTable.Rows)
        {
            PaymentShopModel paymentShopsModel = new();
            paymentShopsModel.product_id = int.Parse(dr["product_id"].ToString());
            paymentShopsModel.product_name = dr["product_name"].ToString();
            paymentShopsModel.paid_currency = int.Parse(dr["paid_currency"].ToString());
            paymentShopsModel.bonus_currency = int.Parse(dr["bonus_currency"].ToString());
            paymentShopsModel.price = int.Parse(dr["price"].ToString());
            list.Add(paymentShopsModel);
        }
        return list.ToArray(); // Listに変換して返す
    }

    /// <summary>
    /// 指定された商品IDの商品を取得
    /// </summary>
    /// <param name="product_id"></param>
    /// <returns></returns>
    public static PaymentShopModel GetShopData(int product_id)
    {
        PaymentShopModel paymentShopsModel = new();
        string query = string.Format("select * from payment_shops where product_id = {0}", product_id);

        SqliteDatabase sqlDB = new SqliteDatabase(GameUtil.Common.DBFileName);
        DataTable dataTable = sqlDB.ExecuteQuery(query);
        foreach (DataRow dr in dataTable.Rows)
        {
            paymentShopsModel.product_id = int.Parse(dr["product_id"].ToString());
            paymentShopsModel.product_name = dr["product_name"].ToString();
            paymentShopsModel.paid_currency = int.Parse(dr["paid_currency"].ToString());
            paymentShopsModel.bonus_currency = int.Parse(dr["bonus_currency"].ToString());
            paymentShopsModel.price = int.Parse(dr["price"].ToString());
        }
        return paymentShopsModel;
    }
}
