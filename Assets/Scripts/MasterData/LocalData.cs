using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LocalData
{
    public static void SetMasterDataVersion(int version)
    {
        PlayerPrefs.SetInt(GameUtil.Common.SAVE_KEY_MASTER_VERSION, version);
    }

    public static int GetMasterDataVersion()
    {
        return PlayerPrefs.GetInt(GameUtil.Common.SAVE_KEY_MASTER_VERSION, 0);
    }
}
