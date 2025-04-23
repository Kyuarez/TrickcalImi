using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class LocalDataManager 
{
    #region Singletone
    private static LocalDataManager instance;
    public static LocalDataManager Instance
    {
        get { return instance; }
    }

    #endregion
    
    public JsonLocalUserData LocalUserData { get; private set; }


    public void OnLoadGameAction()
    {
        instance = this;

        LoadLocalData();
    }

    private void LoadLocalData()
    {
        if (!File.Exists(Define.FilePath_LocalData))
        {
            Debug.LogWarning("Save file not found, creating new data.");
            LoadDefaultData();
            SaveLocalData();
        }
        else
        {
            string json = File.ReadAllText(Define.FilePath_LocalData);
            LocalUserData = JsonConvert.DeserializeObject<JsonLocalUserData>(json);
        }

    }
    public void LoadDefaultData()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "JsonLocalUserData.json");

#if UNITY_ANDROID && !UNITY_EDITOR
        UnityWebRequest www = UnityWebRequest.Get(path);
        www.SendWebRequest();
        while (!www.isDone) {}
        string json = www.downloadHandler.text;
#else
        string json = File.ReadAllText(path);
#endif

        LocalUserData = JsonConvert.DeserializeObject<JsonLocalUserData>(json);
    }
    public void SaveLocalData()
    {
        string json = JsonConvert.SerializeObject(LocalUserData, Formatting.Indented);
        File.WriteAllText(Define.FilePath_LocalData, json);
    }   
}
