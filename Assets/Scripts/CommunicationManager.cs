using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ResponseObjects
{
    public UsersModel users;
    public WalletsModel wallets;
    // マスタデータ
    public int master_data_version;
}

public class CommunicationManager : MonoBehaviour
{
    public static CommunicationManager instance { get; private set; }

    private void Awake()
    {
        // シングルトンにする
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    /// <summary>
    /// ユーザー情報更新
    /// </summary>
    /// <param name="responseObjects"></param>
    static void UpdateUserData(ResponseObjects responseObjects)
    {
        if (responseObjects.users != null && !string.IsNullOrEmpty(responseObjects.users.user_id))
        {
            Users.RegistUserinfo(responseObjects.users);
        }
    }

    /// <summary>
    /// ウォレット情報更新
    /// </summary>
    /// <param name="responseObjects"></param>
    static void UpdateWalletData(ResponseObjects responseObjects)
    {
        if (responseObjects.wallets != null)
        {
            UsersModel usersModel = Users.Get();
            if (responseObjects.wallets.paid_amount + responseObjects.wallets.free_amount > 0)
            {
                Wallets.RegistWalletinfo(responseObjects.wallets, usersModel.user_id);
            }
        }
    }

    /// <summary>
    /// マスターデータの更新
    /// </summary>
    /// <param name="requireComponent"></param>
    static void UpdateMasterData(ResponseObjects responseObjects)
    {
        // バージョン保存
        if (responseObjects.master_data_version != null && responseObjects.master_data_version != 0)
        {
        SaveManager.instance.SetMasterDataVersion(responseObjects.master_data_version);
        }
    }

    /// <summary>
    /// リンクごとに情報の更新、保存を行う
    /// </summary>
    static void ConnectMove(string connectURL, ResponseObjects responseObjects)
    {
        switch (connectURL)
        {
            case GameUtil.Uri.Login:
                UpdateMasterData(responseObjects); 
                break;
            case GameUtil.Uri.Home:
            case GameUtil.Uri.Register:
            case GameUtil.Uri.Stamina_Recovery:
                UpdateUserData(responseObjects);
                UpdateWalletData(responseObjects);
                break;
            case GameUtil.Uri.Buy_Currency:
                UpdateWalletData(responseObjects);
                break;
        }
    }
}
