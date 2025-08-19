using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MasterUpdateManager : MonoBehaviour
{
    TitleManager titleManager;
    private void Start()
    {
        titleManager = FindObjectOfType<TitleManager>();
    }

    public void PushMasterUpdateButton()
    {
        List<IMultipartFormSection> masterForm = new List<IMultipartFormSection>();
        string masterVersion = SaveManager.Instance.GetMasterDataVersion().ToString();
        masterForm.Add(new MultipartFormDataSection("mv", masterVersion));
        // マスタデータを取得
        StartCoroutine(CommunicationManager.ConnectServer(GameUtil.Uri.Master_Get_URL, masterForm, null));

        StartCoroutine(titleManager.SuccessMasterPannel());
    }
}