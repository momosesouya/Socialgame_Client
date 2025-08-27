using System.Collections.Generic;
using UnityEngine;

public class GachaLogModel
{
    public int gacha_log_id;
    public int gacha_id;
    public int weapon_id;
    public string created;
}

/// <summary>
/// ガチャログ
/// </summary>
public class GachaLogs : MonoBehaviour
{
    /// <summary>
    /// テーブル生成
    /// </summary>
    public static void CreateGachaLogTable()
    {
        string createQuery = "create table if not exists gacha_logs (" +
            "gacha_log_id int, " +
            "gacha_id int, " +
            "weapon_id int, " +
            "created text, " +
            "primary key(gacha_log_id))";
        SqliteDatabase sqlDB = new SqliteDatabase(GameUtil.Common.DBFileName);
        sqlDB.ExecuteQuery(createQuery);
    }

    /// <summary>
    /// 新規登録
    /// </summary>
    /// <param name="gacha_weapon_list"></param>
    public static void RegistGachaLogInfo(GachaLogModel[] gacha_log_list)
    {
        foreach (GachaLogModel gachaLogModel in gacha_log_list)
        {
            string query = "insert or replace into gacha_logs(gacha_log_id, gacha_id, weapon_id, created) " +
                            "values (@gacha_log_id, @gacha_id, @weapon_id, @created)";
            Dictionary<string, object> param = new Dictionary<string, object>()
            {
                {"@gacha_log_id", gachaLogModel.gacha_log_id },
                {"@gacha_id", gachaLogModel.gacha_id },
                {"@weapon_id", gachaLogModel.weapon_id },
                {"@created", gachaLogModel.created },
            };
            SqliteDatabase sqlDB = new SqliteDatabase(GameUtil.Common.DBFileName);
            sqlDB.ExecuteQuery(query, param);
        }
    }
}
