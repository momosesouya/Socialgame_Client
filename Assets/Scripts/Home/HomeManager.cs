using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;

public class HomeManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI userNameText;
    [SerializeField] TextMeshProUGUI totalStaminaText;
    //[SerializeField] TextMeshProUGUI walletText;
    [SerializeField] Slider staminaBar;
    [SerializeField] TextMeshProUGUI currentStamina;
    [SerializeField] TextMeshProUGUI maxStamina;
    [SerializeField] TextMeshProUGUI freeAmount;
    [SerializeField] TextMeshProUGUI paidAmount;
    [SerializeField] GameObject errorPanel;

    UsersModel usersModel;
    WalletsModel walletsModel;
    bool isReturnTitle = false;

    void Awake()
    {
        usersModel = Users.Get();
    }

    void Start()
    {
        walletsModel = Wallets.Get(usersModel.user_id);
        StartCoroutine(HomeProcess());
    }

    void Update()
    {
    }

    public void GetHomeData()
    {
        List<IMultipartFormSection> homeForm = new();
        string user_id = Users.Get().user_id;
        homeForm.Add(new MultipartFormDataSection("uid", user_id));
        StartCoroutine(CommunicationManager.ConnectServer(GameUtil.Uri.Master_Get_URL, homeForm, null));
    }

    /// <summary>
    /// ログイン処理
    /// </summary>
    IEnumerator HomeProcess()
    {
        List<IMultipartFormSection> postData = new List<IMultipartFormSection>();
        postData.Add(new MultipartFormDataSection("uid", usersModel.user_id));
        UnityWebRequest request = UnityWebRequest.Post(GameUtil.Uri.Home, postData);
        yield return request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            OpenAuthErrorPanel();
            yield break;
        }
        else
        {
            // 通信に成功した後認証失敗した場合はエラー
            var requestResult = JsonUtility.FromJson<GameUtil.RequestResult>(request.downloadHandler.text);
            if (requestResult.result == GameUtil.Common.ErrCode_Auth)
            {
                OpenAuthErrorPanel();
                yield break;
            }

            // ユーザー情報取得
            userNameText.text = usersModel.user_name;
            currentStamina.text = usersModel.last_stamina.ToString();
            maxStamina.text = usersModel.max_stamina.ToString();
            staminaBar.maxValue = usersModel.max_stamina;
            staminaBar.value = usersModel.last_stamina;
            freeAmount.text = walletsModel.free_amount.ToString();
            paidAmount.text = walletsModel.paid_amount.ToString();
        }
    }

    public void RefreshWalletsText()
    {
        walletsModel = Wallets.Get(usersModel.user_id);
        freeAmount.text = walletsModel.free_amount.ToString();
        paidAmount.text = walletsModel.paid_amount.ToString();
    }

    /// <summary>
    /// 認証エラーパネルを生成
    /// </summary>
    void OpenAuthErrorPanel() => Instantiate(errorPanel);

}
