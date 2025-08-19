using System.IO;
using UnityEngine;

public static class DatabaseInitializer
{
    public static string InitDatabase(string dbFileName)
    {
        string sourcePath = Path.Combine(Application.streamingAssetsPath, dbFileName);
        string destPath = Path.Combine(Application.persistentDataPath, dbFileName);

        if (!File.Exists(destPath))
        {
            File.Copy(sourcePath, destPath);
            Debug.Log("DB copied to persistentDataPath: " + destPath);
        }

        return destPath;
    }
}
