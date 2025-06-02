using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WalletsModel
{
    public int free_amount;
    public int paid_amount;
}

public class Wallets
{
    /// <summary>
    /// テーブル生成
    /// </summary>
    public static void CreateTable()
    {

        string createQuery = "create table if not exists wallets (" +
            "user_id text, " +
            "free_amount midiumint, " +
            "paid_amount midiumint, " +
            "primary key(user_id))";
        SqliteDatabase sqlDB = new SqliteDatabase(GameUtil.Common.DBFileName);
        sqlDB.ExecuteQuery(createQuery);
    }

    /// <summary>
    /// 登録
    /// </summary>
    public static void RegistWalletinfo(WalletsModel walletsModel, string user_id)
    {
        string query = "insert or replace into user_wallets(user_id,free_amount,paid_amount,max_amount) values(\"" + user_id + "\"," + walletsModel.free_amount + "," + walletsModel.paid_amount + ")";
        SqliteDatabase sqlDB = new SqliteDatabase(GameUtil.Common.DBFileName);
        sqlDB.ExecuteQuery(query);
    }

    /// <summary>
    /// テーブル取得
    /// </summary>
    public static WalletsModel Get()
    {
        string getQuery = "select * from user_wallets";
        SqliteDatabase sqlDB = new SqliteDatabase(GameUtil.Common.DBFileName);
        DataTable dataTable = sqlDB.ExecuteQuery(getQuery);
        WalletsModel walletsModel = new WalletsModel();
        foreach (DataRow dr in dataTable.Rows)
        {
            walletsModel.free_amount = int.Parse(dr["free_amount"].ToString());
            walletsModel.paid_amount = int.Parse(dr["paid_amount"].ToString());
        }
        return walletsModel;
    }

}
