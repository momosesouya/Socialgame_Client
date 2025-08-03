using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GachaPeriodModel
{
    public int gacha_id;          // ガチャID
    public string gacha_name;     // ガチャ名
    public int single_cost;       // 単発ガチャ価格
    public int multi_cost;        // 連ガチャ価格
    public string period_start;   // 開始日時
    public string period_end;     // 終了日時
}

/// <summary>
/// gacha_periodsテーブル
/// ガチャ期間
/// </summary>
public class GachaPeriods
{
    /// <summary>
    /// テーブル生成
    /// </summary>
    public static void CreateGachaPeriodTable()
    {
        string createQuery = "create table if not exists gacha_periods (" +
            "gacha_id int, " +
            "gacha_name text, " +
            "single_cost int, " +
            "multi_cost int, " +
            "period_start text, " +
            "period_end text, " +
            "primary key(gacha_id))";
        SqliteDatabase sqlDB = new SqliteDatabase(GameUtil.Common.DBFileName);
        sqlDB.ExecuteQuery(createQuery);
    }

    /// <summary>
    /// 新規登録
    /// </summary>
    /// <param name="gacha_period_list"></param>
    public static void RegistGachaPeriodInfo(GachaPeriodModel[] gacha_period_list)
    {
        foreach (GachaPeriodModel gachaPeriodModel in gacha_period_list)
        {
            string query = "insert or replace into gacha_periods(gacha_id, gacha_name, single_cost, multi_cost, period_start, period_end) " +
                            "values (@gacha_id, @gacha_name, @single_cost, @multi_cost, @period_start, @period_end)";
            Dictionary<string, object> param = new Dictionary<string, object>()
            {
                {"@gacha_id", gachaPeriodModel.gacha_id },
                {"@gacha_name", gachaPeriodModel.gacha_name },
                {"@single_cost", gachaPeriodModel.single_cost },
                {"@multi_cost", gachaPeriodModel.multi_cost },
                {"@period_start", gachaPeriodModel.period_start },
                {"@period_end", gachaPeriodModel.period_end },
            };
            SqliteDatabase sqlDB = new SqliteDatabase(GameUtil.Common.DBFileName);
            sqlDB.ExecuteQuery(query, param);
        }
    }

    /// <summary>
    /// 指定されたガチャIDでガチャの期間を取得
    /// </summary>
    /// <param name="gacha_id"></param>
    /// <returns></returns>
    public static GachaPeriodModel GetGachaPeriodData(int gacha_id)
    {
        GachaPeriodModel gachaPeriodModel = new();
        string query = string.Format("select * from gacha_periods where gacha_id = " + gacha_id);

        SqliteDatabase sqlDB = new SqliteDatabase(GameUtil.Common.DBFileName);
        DataTable dataTable = sqlDB.ExecuteQuery(query);
        foreach (DataRow dr in dataTable.Rows)
        {
            gachaPeriodModel.gacha_id = int.Parse(dr["gacha_id"].ToString());
            gachaPeriodModel.gacha_name = dr["weapon_id"].ToString();
            gachaPeriodModel.single_cost = int.Parse(dr["single_cost"].ToString());
            gachaPeriodModel.multi_cost = int.Parse(dr["multi_cost"].ToString());
            gachaPeriodModel.period_start = dr["single_cost"].ToString();
            gachaPeriodModel.period_end = dr["weapon_id"].ToString();
        }
        return gachaPeriodModel;
    }
}