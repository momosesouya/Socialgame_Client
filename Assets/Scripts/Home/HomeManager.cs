using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class HomeResponse
{
    public int currentStamina;
}

public class HomeManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI userNameText;
    [SerializeField] TextMeshProUGUI totalStaminaText;
    [SerializeField] Slider staminaBar;
    [SerializeField] TextMeshProUGUI currentStamina;
    [SerializeField] TextMeshProUGUI maxStamina;
    [SerializeField] TextMeshProUGUI freeAmount;
    [SerializeField] TextMeshProUGUI paidAmount;
    [SerializeField] GameObject errorPanel;

    UsersModel usersModel;
    WalletsModel walletsModel;

    void Awake()
    {
        usersModel = Users.Get();
    }

    void Start()
    {
        walletsModel = Wallets.Get();
        StartCoroutine(HomeProcess());
        GetHomeData();
    }

    void FixedUpdate() => RefreshText();

    public void GetHomeData()
    {
        List<IMultipartFormSection> homeForm = new();
        string user_id = Users.Get().user_id;
        homeForm.Add(new MultipartFormDataSection("uid", user_id));
        StartCoroutine(CommunicationManager.ConnectServer(GameUtil.Uri.Home, homeForm, null));
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
            var requestResult = JsonUtility.FromJson<HomeResponse>(request.downloadHandler.text);
            // ユーザー情報取得
            userNameText.text = usersModel.user_name;
            maxStamina.text = usersModel.max_stamina.ToString();
            staminaBar.maxValue = usersModel.max_stamina;
            currentStamina.text = requestResult.currentStamina.ToString();
            staminaBar.value = requestResult.currentStamina;
            freeAmount.text = walletsModel.free_amount.ToString();
            paidAmount.text = walletsModel.paid_amount.ToString();
        }
    }

    public void RefreshText()
    {
        walletsModel = Wallets.Get();
        // 通貨更新
        freeAmount.text = walletsModel.free_amount.ToString();
        paidAmount.text = walletsModel.paid_amount.ToString();
    }

    /// <summary>
    /// 認証エラーパネルを生成
    /// </summary>
    void OpenAuthErrorPanel() => Instantiate(errorPanel);

}
