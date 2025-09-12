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
    [SerializeField] GameObject completeMasterPanel;
    [SerializeField] InputField inputName;
    [SerializeField] TextMeshProUGUI errMsg;
    [SerializeField] TextMeshProUGUI failedMsg;
    [SerializeField] TextMeshProUGUI registerName;

    UsersModel usersModel;
    WalletsModel walletsModel;
    WeaponsModel weaponsModel;
    ItemsModel[] itemsModel;

    MasterVersionCheckManager masterCheckManager;

    bool isExistAccount = false; // アカウントデータが存在するか

    void Awake()
    {
        // SQLiteのファイルチェック
        string DBPath = Application.dataPath + "/" + GameUtil.Common.DBFileName;
        if (!File.Exists(DBPath))
        {
            using (File.Create(DBPath)) { }
        }
        // SaveDataのファイルチェック
        if (!SaveManager.Instance.SaveDataCheck())
        {
            SaveManager.Instance.CreateSaveData();
        }
        

        // SQLiteテーブル生成
        // インスタンステーブル
        Users.CreateTable();
        Wallets.CreateTable();
        Weapons.CreateWeaponModel();
        Items.CreateItemModel();
        // マスタテーブル
        PaymentShops.CreateShopTable();               // ショップテーブル
        GachaPeriods.CreateGachaPeriodTable();        // ガチャ期間テーブル
        GachaWeapons.CreateGachaTable();              // 武器ガチャテーブル
        MasterWeapons.CreateWeaponMasterTable();      // 武器マスタテーブル
        WeaponCategories.CreateWeaponCategoryTable(); // 武器カテゴリーテーブル
        WeaponRarities.CreateRarityTable();           // 武器レアリティテーブル
        MasterItems.CreateItemMasterTable();          // アイテムマスタテーブル
        ItemCategories.CreateItemCategoryTable();     // アイテムカテゴリーテーブル

        masterCheckManager = FindObjectOfType<MasterVersionCheckManager>();
    }

    void Start()
    {
        registerPanel.SetActive(false);
        confirmPanel.SetActive(false);
        registerFaildPanel.SetActive(false);
        registerCompletePanel.SetActive(false);

        // ユーザーデータ取得
        usersModel = Users.Get();
        walletsModel = Wallets.Get();
        weaponsModel = Weapons.Get(usersModel.user_id);
        itemsModel = Items.GetItemDataAll();
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

        // マスタデータチェック(データが無いか古い場合は更新)
        masterCheckManager.MasterCheck();
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
            walletsModel = Wallets.Get();

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
    /// 開いているパネルを閉じる
    /// </summary>
    public void CloseAllPanel()
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

    public IEnumerator SuccessMasterPannel()
    {
        completeMasterPanel.SetActive(true);

        yield return new WaitForSeconds(5f);

        completeMasterPanel.SetActive(false);
    }

    /// 登録処理
    /// </summary>
    IEnumerator RegisterProcess()
    {
        List<IMultipartFormSection> postData = new List<IMultipartFormSection>
        {
            new MultipartFormDataSection("un", registerName.text)
        };

        // SQLiteに登録
        yield return StartCoroutine(CommunicationManager.ConnectServer(GameUtil.Uri.Register, postData, () => 
        {
            // 登録完了
            isExistAccount = true;
            confirmPanel.SetActive(false);
            registerCompletePanel.SetActive(true);
        }));

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
            yield break;
        }
        postData.Add(new MultipartFormDataSection("uid", usersModel.user_id));
        UnityWebRequest request = UnityWebRequest.Post(GameUtil.Uri.Login, postData);
        yield return request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            OpenRegisterFailed(GameUtil.Common.ErrMsg_RequestFailed);
            confirmPanel.SetActive(false);
            startButton.interactable = true;
            yield break;
        }

        // ログイン成功、ホーム画面へ
        Users.SetLastLogin(usersModel.user_id);
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