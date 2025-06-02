using System;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

[Serializable]
public class UsersModel
{
    public string user_id;
    public string user_name;
    public int max_stamina;
    public int last_stamina;
    public string stamina_updated;
    public string last_login;
}

/// <summary>
/// usersテーブル
/// </summary>
public static class Users
{
    /// <summary>
    /// テーブル生成
    /// </summary>
    public static void CreateTable()
    {

        string createQuery = "create table if not exists users (" +
            "user_id text, " +
            "user_name text, " +
            "max_stamina int, " +
            "last_stamina int, " +
            "stamina_updated text, " +
            "last_login text, " +
            "primary key(user_id))";
        SqliteDatabase sqlDB = new SqliteDatabase(GameUtil.Common.DBFileName);
        sqlDB.ExecuteQuery(createQuery);
    }


    /// <summary>
    /// 新規登録
    /// </summary>
    public static void RegistUserinfo(UsersModel model)
    {
        // 日本語対応
        string query = "insert or replace into users (user_id, user_name, max_stamina, last_stamina, stamina_updated, last_login) " +
                   "values (@user_id, @user_name, @max_stamina, @last_stamina, datetime(@stamina_updated), datetime(@last_login))";
        Dictionary<string, object> param = new Dictionary<string, object>()
        {
            {"@user_id", model.user_id},
            {"@user_name", model.user_name},
            {"@max_stamina", model.max_stamina},
            {"@last_stamina", model.last_stamina},
            {"@stamina_updated", model.stamina_updated},
            {"@last_login", model.last_login},
        };
        
        SqliteDatabase sqlDB = new SqliteDatabase(GameUtil.Common.DBFileName);
        sqlDB.ExecuteQuery(query, param);
    }

    /// <summary>
    /// テーブル取得
    /// </summary>
    public static UsersModel Get()
    {
        string getQuery = "select * from users limit 1";
        SqliteDatabase sqlDB = new SqliteDatabase(GameUtil.Common.DBFileName);
        DataTable dataTable = sqlDB.ExecuteQuery(getQuery);
        UsersModel usersModel = new UsersModel();
        foreach (DataRow dr in dataTable.Rows)
        {
            usersModel.user_id = dr["user_id"].ToString();
            usersModel.user_name = dr["user_name"].ToString();
            usersModel.max_stamina = int.Parse(dr["max_stamina"].ToString());
            usersModel.last_stamina = int.Parse(dr["last_stamina"].ToString());
            usersModel.stamina_updated = dr["stamina_updated"].ToString();
            usersModel.last_login = dr["last_login"].ToString();
        }
        return usersModel;
    }

    /// <summary>
    /// 最終ログイン更新
    /// </summary>
    /// <param name="userId">更新対象のユーザーID</param>
    public static void SetLastLogin(string userId)
    {
        DateTime dt = DateTime.Now;
        string nowTimeStamp = dt.ToString("yyyy-MM-dd HH:mm:ss");
        string query = "update users set last_login = '" + nowTimeStamp + "' where user_id = '" + userId + "'";
        SqliteDatabase sqlDB = new SqliteDatabase(GameUtil.Common.DBFileName);
        sqlDB.ExecuteNonQuery(query);
    }
}