using System;
using UnityEngine;

[Serializable]
public class UsersModel
{
    public string user_id;
    public string user_name;
    public int max_stamina;
    public int last_stamina;
    public int stamina_updated;
    public int last_login;
}

/// <summary>
/// usersテーブル
/// </summary>
public static class Users
{
    /// <summary>
    /// サーバーから受け取ったユーザー情報を保存
    /// </summary>
    public static void RegistUserinfo(UsersModel model)
    {
        PlayerPrefs.SetString("user_id", model.user_id);
        PlayerPrefs.SetString("user_name", model.user_name);
        PlayerPrefs.SetInt("max_stamina", model.max_stamina);
        PlayerPrefs.SetInt("last_stamina", model.last_stamina);
        PlayerPrefs.SetInt("stamina_updated", model.stamina_updated);
        PlayerPrefs.SetInt("last_login", model.last_login);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// サーバーから受け取ったユーザー情報を取得
    /// </summary>
    public static UsersModel Get()
    {
        if (!PlayerPrefs.HasKey("user_id")) return new UsersModel(); // ユーザーなし

        UsersModel model = new UsersModel();
        model.user_id = PlayerPrefs.GetString("user_id");
        model.user_name = PlayerPrefs.GetString("user_name");
        model.max_stamina = PlayerPrefs.GetInt("max_stamina");
        model.last_stamina = PlayerPrefs.GetInt("last_stamina");
        model.stamina_updated = PlayerPrefs.GetInt("stamina_updated");
        model.last_login = PlayerPrefs.GetInt("last_login");
        return model;
    }

    /// <summary>
    /// 最終ログイン更新
    /// </summary>
    /// <param name="userId">更新対象のユーザーID</param>
    public static void SetLastLogin(string userId)
    {
        string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        PlayerPrefs.SetString("last_login", now);
        PlayerPrefs.Save();
    }
}