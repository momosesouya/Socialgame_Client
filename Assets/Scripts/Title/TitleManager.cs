using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


public class TitleManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI UserId;
    [SerializeField] Button startButton;
    [SerializeField] GameObject registerPanel;
    [SerializeField] GameObject registerFaildPanel;
    [SerializeField] GameObject registerCompletePanel;
    [SerializeField] GameObject confirmPanel;
    [SerializeField] InputField inputName;
    [SerializeField] TextMeshProUGUI errMsg;
    [SerializeField] TextMeshProUGUI failedMsg;
    [SerializeField] TextMeshProUGUI registerName;

    UsersModel usersModel;
    WalletsModel walletsModel;
    PaymentShopModel paymentShopModel;


    bool isExistAccount = false; // アカウントデータが存在するか

    void Awake()
    {
        // SQLiteのファイルチェック
        string DBPath = Application.dataPath + "/" + GameUtil.Common.DBFileName;
        if (!File.Exists(DBPath))
        {
            using (File.Create(DBPath)) { }
        }
        // SQLiteテーブル生成
        Users.CreateTable();
        Wallets.CreateTable();
        PaymentShops.CreateShopTable();
    }

    void Start()
    {
        registerPanel.SetActive(false);
        confirmPanel.SetActive(false);
        registerFaildPanel.SetActive(false);
        registerCompletePanel.SetActive(false);

        // ユーザーデータ取得
        usersModel = Users.Get();
        walletsModel = Wallets.Get(usersModel.user_id);
        if (usersModel.user_id == null)
        {
            // アカウントなし
            Debug.Log("ユーザーデータが見つかりません");
            isExistAccount = false;
            UserId.text = "0";
        }
        else
        {
            Debug.Log("ユーザーデータ取得完了");
            isExistAccount = true;
            UserId.text = usersModel.user_id.ToString();
        }
    }

    private void Update()
    {
    }

    /// <summary>
    /// スタートボタン押下時
    /// </summary>
    public void StartButton()
    {
        // アカウントがある場合は通常ログイン
        if (isExistAccount)
        {
            // ユーザーデータ取得(保持)
            usersModel = Users.Get();
            walletsModel = Wallets.Get(usersModel.user_id);

            registerPanel.SetActive(false);
            confirmPanel.SetActive(false);
            registerFaildPanel.SetActive(false);
            registerCompletePanel.SetActive(false);
            startButton.interactable = false;
            StartCoroutine(LoginProcess());
        }
        // アカウントがない場合は登録処理
        else
        {
            registerPanel.SetActive(true);
        }
    }

    /// <summary>
    /// 確認ボタン押下時
    /// </summary>
    public void ConfirmButton()
    {
        // 入力文字数チェック
        if (inputName.text.Length <= 0 || inputName.text.Length >= 13)
        {
            errMsg.text = GameUtil.Common.ErrMsg_NameInput;
            Debug.Log("入力エラーです。");
            return;
        }
        StartCoroutine(StopLoding());

        registerName.text = inputName.text;
        CloseRegister();
        confirmPanel.SetActive(true);
    }

    /// <summary>
    /// 登録ボタン押下時
    /// </summary>
    public void RegisterButton()
    {
        // 登録処理へ
        StartCoroutine(RegisterProcess());
    }


    /// <summary>
    /// 名前入力ウィンドウを閉じる
    /// </summary>
    public void CloseRegister()
    {
        inputName.text = "";
        errMsg.text = "";
        registerPanel.SetActive(false);
    }

    /// <summary>
    /// 登録失敗パネルを開く
    /// </summary>
    public void OpenRegisterFailed(string msg)
    {
        failedMsg.text = msg;
        registerFaildPanel.SetActive(true);
    }

    /// <summary>
    /// 登録失敗パネルを閉じる
    /// </summary>
    public void CloseRegisterFailed()
    {
        registerFaildPanel.SetActive(false);
    }

    /// <summary>
    /// 登録をやめる
    /// </summary>
    public void CanselRegister()
    {
        confirmPanel.SetActive(false);
        registerName.text = "";
    }

    IEnumerator StopLoding()
    {
        yield return null;
    }


    /// <summary>
    /// 起動プロセス
    /// </summary>
    IEnumerator StartProcess()
    {
        yield return null;
    }

    /// 登録処理
    /// </summary>
    IEnumerator RegisterProcess()
    {
        List<IMultipartFormSection> postData = new List<IMultipartFormSection>();
        postData.Add(new MultipartFormDataSection("un", registerName.text));
        
        // SQLiteに登録
        yield return StartCoroutine(CommunicationManager.ConnectServer(GameUtil.Uri.Register, postData, () => {}));

        // 登録完了
        Debug.Log("登録完了");
        isExistAccount = true;
        confirmPanel.SetActive(false);
        registerCompletePanel.SetActive(true);
    }


    /// <summary>
    /// ログイン処理
    /// </summary>
    IEnumerator LoginProcess()
    {
        List<IMultipartFormSection> postData = new List<IMultipartFormSection>();
        // user_idが存在しなかった場合
        if (string.IsNullOrEmpty(usersModel.user_id))
        {
            Debug.LogError("user_id が null または 空です。LoginProcess を中断します。");
            yield break;
        }
        postData.Add(new MultipartFormDataSection("uid", usersModel.user_id));
        UnityWebRequest request = UnityWebRequest.Post(GameUtil.Uri.Login, postData);
        yield return request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("送信先URL: " + GameUtil.Uri.Login);
            Debug.Log("送信するUID: " + usersModel.user_id);
            Debug.Log("通信結果: " + request.result);
            Debug.Log("レスポンスコード: " + request.responseCode);
            Debug.Log("レスポンス内容: " + request.downloadHandler.text);
            OpenRegisterFailed(GameUtil.Common.ErrMsg_RequestFailed);
            confirmPanel.SetActive(false);
            startButton.interactable = true;
            yield break;
        }
        Debug.Log("ログイン成功。ホーム画面に移行する。");
        // ログイン成功、ホーム画面へ
        Users.SetLastLogin(usersModel.user_id);
        //StartCoroutine(CommunicationManager.ConnectServer(GameUtil.Uri.Login, postData, null));
        // マスタデータを取得
        StartCoroutine(CommunicationManager.ConnectServer(GameUtil.Uri.Master_Get_URL, postData, null));
        List<IMultipartFormSection> buyform = new();
        buyform.Add(new MultipartFormDataSection("uid", usersModel.user_id));
        //buyform.Add(new MultipartFormDataSection("pid", paymentShop.product_id));
        StartCoroutine(CommunicationManager.ConnectServer(GameUtil.Uri.Buy_Currency, buyform, null));
        Debug.Log("Walletsデータ登録をしました。" + GameUtil.Uri.Buy_Currency);
        GameUtil.FadeManager.Instance.LoadScene("HomeScene");
    }

    /// <summary>
    /// 登録完了時レスポンス用
    /// </summary>
    public class RegistResult : UsersModel
    {
        public int manage_id;
        public int result;
        public string created_at;
        public string updated_at;
    }

    public void FinishGameButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // ゲームプレイ終了
#else
    Application.Quit(); // ゲームプレイ終了
#endif
    }
}