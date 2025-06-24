using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MasterUpdateManager : MonoBehaviour
{
    public void PushMasterUpdateButton()
    {
        List<IMultipartFormSection> masterForm = new List<IMultipartFormSection>();
        string masterVersion = SaveManager.Instance.GetMasterDataVersion().ToString();
        masterForm.Add(new MultipartFormDataSection("mv", masterVersion));

        StartCoroutine(CommunicationManager.ConnectServer(GameUtil.Uri.Master_Check_URL, masterForm, null));
        StartCoroutine(CommunicationManager.ConnectServer(GameUtil.Uri.Master_Get_URL, null, null));
    }
}