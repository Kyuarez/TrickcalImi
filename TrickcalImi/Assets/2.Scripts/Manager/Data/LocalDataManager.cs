using UnityEngine;
using System.IO;
using System.Collections.Generic;
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

    public void UpdateLocalChapterData(int currentChapter, int clearStage)
    {
        if (currentChapter > LocalUserData.ChapterOpenData.Count)
        {
            return;
        }
        List<bool> stageOpenList = LocalUserData.ChapterOpenData[currentChapter];
        if (stageOpenList == null || stageOpenList.Count == 0 || clearStage > stageOpenList.Count)
        {
            return;
        }
        List<bool> stageClearList = LocalUserData.ChapterClearData[currentChapter];
        if (stageClearList == null || stageClearList.Count == 0 || clearStage > stageClearList.Count)
        {
            return;
        }

        if (clearStage == 10)
        {
            if (stageClearList[clearStage - 1] == false)
            {
                stageClearList[clearStage - 1] = true;
            }
            //다음 챕터 활성화
            if(currentChapter == LocalUserData.CurrentChapter)
            {
                if (currentChapter == LocalUserData.ChapterOpenData.Count) //현재 데이터 상 챕터 다 클리어
                {
                    //TODO : 축하
                    return;
                }

                LocalUserData.CurrentChapter++;
            }
        }
        else
        {
            if (stageOpenList[clearStage] == false)
            {
                stageOpenList[clearStage] = true;
            }
            if (stageClearList[clearStage - 1] == false)
            {
                stageClearList[clearStage - 1] = true;
            }
        }
    }
}
