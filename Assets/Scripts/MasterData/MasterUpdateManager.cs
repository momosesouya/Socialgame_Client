using System.Collections;
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

    public void MasterUpdate()
    {
        StartCoroutine(VersionCheckProcess());
    }

    // バージョンチェック
    IEnumerator VersionCheckProcess()
    {
        string masterVersion = SaveManager.Instance.GetMasterDataVersion().ToString();
        List<IMultipartFormSection> masterForm = new List<IMultipartFormSection>
        {
            new MultipartFormDataSection("mv", masterVersion)
        };
        Debug.Log("MasterForm"+ masterForm);
        yield return StartCoroutine(CommunicationManager.ConnectServer(GameUtil.Uri.Master_Get_URL, masterForm, () =>
        {
            Debug.Log("更新完了パネル表示");
            // 更新完了パネル表示
            StartCoroutine(titleManager.SuccessMasterPannel());
        }));
    }
}