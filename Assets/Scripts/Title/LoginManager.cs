using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    [SerializeField] Button startButton;
    [SerializeField] GameObject confirmPanel;
    [SerializeField] GameObject communicationFaildPanel;
    [SerializeField] TextMeshProUGUI failedMsg;

    UsersModel usersModel;

    /// <summary>
    /// 通信失敗パネルを開く
    /// </summary>
    public void OpenRegisterFailed(string msg)
    {
        failedMsg.text = msg;
        communicationFaildPanel.SetActive(true);
    }

    /// <summary>
    /// ログイン処理
    /// </summary>
    /// <returns></returns>
    public IEnumerator LoginProcess()
    {
        List<IMultipartFormSection> postData = new List<IMultipartFormSection>();
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
}
