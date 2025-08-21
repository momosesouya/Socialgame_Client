using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

[SerializeField]
public class MasterVersionResponse
{
    public string message;
    public int serverVersion;
    public int success;
}

public class MasterVersionCheckManager : MonoBehaviour
{
    [SerializeField] MasterUpdateManager masterUpdateManager;

    [SerializeField] GameObject updateCheckPanel;
    [SerializeField] TextMeshProUGUI messageText;
    int serverMasterVersion = 0;
    string responseMessage;
    int successResult = 1;

    private void Start()
    {
        updateCheckPanel.SetActive(false);
    }

    public void MasterCheck()
    {
        StartCoroutine(VersionCheckProcess());
    }

    IEnumerator MasterUpdateButton()
    {
        yield return new WaitForSeconds(2);

        updateCheckPanel.SetActive(false);
    }

    // バージョンチェック
    IEnumerator VersionCheckProcess()
    {
        // クライアントのマスターバージョン取得
        string masterVersion = SaveManager.Instance.GetMasterDataVersion().ToString();
        List<IMultipartFormSection> masterForm = new List<IMultipartFormSection>
        {
            new MultipartFormDataSection("mv", masterVersion)
        };

        UnityWebRequest request = UnityWebRequest.Post(GameUtil.Uri.Master_Check_URL, masterForm);
        yield return request.SendWebRequest();

        string json = request.downloadHandler.text;

        // JSON をオブジェクトに変換
        MasterVersionResponse response = JsonUtility.FromJson<MasterVersionResponse>(json);
        if (response != null)
        {
            serverMasterVersion = response.serverVersion;
            responseMessage = response.message.ToString();
            messageText.text = responseMessage;
            Debug.Log(response.success);
            // レスポンスのsuccessが0なら更新が必要
            if (response.success != successResult)
            {
                updateCheckPanel.SetActive(true);
                masterUpdateManager.MasterUpdate();
                StartCoroutine(MasterUpdateButton());

                SaveManager.Instance.SetMasterDataVersion(serverMasterVersion);
            }
        }
        else
        {
            Debug.LogWarning("レスポンスがnullです");
        }
    }
}
