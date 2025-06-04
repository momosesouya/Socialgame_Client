using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

// セーブデータクラス
[System.Serializable]
public class SaveData
{
    public int version;
}

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance {  get; private set; }

    // セーブファイルの名前
    const string FileName = "/savedata.dat";
    // セーブデータのデフォルト値
    static readonly int DefaultVersion = 0;

    FileStream file;
    BinaryFormatter bf;
    string filePath;

    private void Awake()
    {
        // シングルトンにする
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    // ファイル更新共通準備
    void InitFileSave()
    {
        bf = new();
        if (filePath == null)
        {
            filePath = Application.persistentDataPath + FileName;
        }
        file = File.Create(filePath);
    }

    // ファイルロード共通準備
    void InitFileLoad()
    {
        bf = new();
        file = File.Open(filePath, FileMode.Open);
    }

    // ファイルクローズ処理
    void Closefile()
    {
        file.Close();
        file = null;
    }

    // ファイル存在チェック
    public bool SaveDataCheck()
    {
        // ファイルがあればtrue
        if(File.Exists(filePath)) { return true; }
        return false;
    }

    // 新規データ生成
    public void CreateSaveData()
    {
        try
        {
            InitFileSave();

            // セーブデータを生成
            SaveData data = new();
            data.version = DefaultVersion;
            bf.Serialize(file, data);
        }
        catch (IOException)
        {
            Debug.LogError("failed to open file");
        }
        finally
        {
            // fileStreamを使用したら必ず最後にcloseする
            if (file != null) { file.Close(); }
        }
    }

    // バージョンセーブ
    public void SetMasterDataVersion(int version)
    {
        try
        {
            InitFileSave();

            SaveData data = new();
            data.version = version;
            bf.Serialize(file, data);
        }
        catch (IOException)
        {
            Debug.LogError("failed to open file");
        }
        finally
        {
            if (file != null) { file.Close(); }
        }
    }

    // バージョンロード
    public int GetMasterDataVersion()
    {
        int version = DefaultVersion;
        try
        {
            InitFileLoad();

            // セーブデータ読み込み
            SaveData data = bf.Deserialize(file) as SaveData;
            version = data.version;
        }
        catch (IOException)
        {
            Debug.LogError("failed to open file");
        }
        finally
        {
            if (file != null) { file.Close(); }
        }
        return version;
    }
}
