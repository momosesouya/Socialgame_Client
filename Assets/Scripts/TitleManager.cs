using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;


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
    [SerializeField] Text registerName;

    UsersModel usersModel;
    bool isExistAccount = false; // アカウントデータが存在するか

    void Awake()
    {
        
    }

    void Start()
    {
        registerPanel.SetActive(false);
        confirmPanel.SetActive(false);
        registerFaildPanel.SetActive(false);
        registerCompletePanel.SetActive(false);

        // ユーザーデータ取得
        usersModel = Users.Get();
        if (usersModel.user_id == null)
        {
            // アカウントなし
            isExistAccount = false;
            UserId.text = "0";
        }
        else
        {
            isExistAccount = true;
            UserId.text = usersModel.user_id.ToString();
        }
    }

    void Update()
    {
        
    }

    /// <summary>
    /// スタートボタン押下時
    /// </summary>
    public void StartButton()
    {
        if (isExistAccount)
        {
            // アカウントがある場合は通常ログイン
            registerPanel.SetActive(false);
            confirmPanel.SetActive(false);
            registerFaildPanel.SetActive(false);
            registerCompletePanel.SetActive(false);
            startButton.interactable = false;
            StartCoroutine(LoginProcess());
        }
        else
        {
            // アカウントがない場合は登録処理
            registerPanel.SetActive(true);
        }
    }

    /// <summary>
    /// 確認ボタン押下時
    /// </summary>
    public void ConfirmButton()
    {
        // 入力文字数チェック
        if (inputName.text.Length <= 0 || inputName.text.Length > 12)
        {
            errMsg.text = GameUtil.Common.ErrMsg_NameInput;
            return;
        }

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
        if (string.IsNullOrEmpty(registerName.text))
        {
            // registerNameが空だったらエラー
            OpenRegisterFailed(GameUtil.Common.ErrMsg_NameInput);
            yield break;
        }

        List<IMultipartFormSection> postData = new List<IMultipartFormSection>();
        postData.Add(new MultipartFormDataSection("un", registerName.text));
        UnityWebRequest request = UnityWebRequest.Post(GameUtil.Uri.Register, postData);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("エラーレスポンス: " + request.downloadHandler.text);
            OpenRegisterFailed(GameUtil.Common.ErrMsg_RequestFailed);
            confirmPanel.SetActive(false);
            yield break;
        }

        // レスポンスのJSONを読み込んでモデルに変換
        string json = request.downloadHandler.text;
        RegistResult model = JsonUtility.FromJson<RegistResult>(json);
        usersModel = model;

        // 保存
        Users.RegistUserinfo(usersModel);

        // ユーザーデータ取得(保持)
        usersModel = Users.Get();


        Debug.Log("登録完了");

        // 登録完了
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
        postData.Add(new MultipartFormDataSection("uid", usersModel.user_id));
        UnityWebRequest request = UnityWebRequest.Post(GameUtil.Uri.Login, postData);
        yield return request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            startButton.interactable = true;
            yield break;
        }

        // ログイン成功、ホーム画面へ
        Users.SetLastLogin(usersModel.user_id);
        GameUtil.FadeManager.Instance.LoadScene("Home");
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
}