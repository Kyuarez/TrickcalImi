using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

//@tk : 시작할 때 로드할 데이터 관리 매니저
public partial class TableManager 
{
    #region Singletone
    private static TableManager instance;
    public static TableManager Instance
    {
        get { return instance; }
    }
    
    
    #endregion

    //@tk 게임 시작하고, 처음 호출(일단 게임씬 매니저에서 호출하되 나중에 타이틀로 옮기기) : 25.04.23
    public void OnLoadGameAction()
    {
        instance = this;

        LoadAllTables();
    }


    //@tk 일단 타이틀에서 작업하되 
    private void LoadAllTables()
    {
        LoadTable<JsonIngameObject>("JsonIngameObject", out ingameObjectDict, x => x.ObjectID);
        LoadTable<JsonChapter>("JsonChapter", out chapterDict, x => x.ChapterID);
        LoadTable<JsonStage>("JsonStage", out stageDict, x => x.StageID);
    }

    private void LoadTable<T>(string tableName, out Dictionary<int, T> outDict, System.Func<T, int> keySelector)
    {
        string path = Application.streamingAssetsPath + $"/{tableName}.json";
        if (!File.Exists(path))
        {
            Debug.LogError($"File not found: {path}");
            outDict = new Dictionary<int, T>();
            return;
        }

#if UNITY_EDITOR || UNITY_STANDALONE
        string jsonText = File.ReadAllText(path);
#else
        // 안드로이드에서는 StreamingAssets가 WWW/UnityWebRequest 필요
        UnityWebRequest www = UnityWebRequest.Get(path);
        www.SendWebRequest();
        while (!www.isDone) { }
        jsonText = www.downloadHandler.text;
#endif

        Dictionary<string, List<T>> parsed = JsonConvert.DeserializeObject<Dictionary<string, List<T>>>(jsonText);
        List<T> items = parsed[tableName];

        outDict = new Dictionary<int, T>();
        foreach (var item in items)
        {
            outDict[keySelector(item)] = item;
        }

        Debug.Log($"{tableName} loaded: {outDict.Count} entries");

        RegisterTable<T>(outDict);
    }

    private void RegisterTable<T>(Dictionary<int, T> table)
    {
        tableMap[typeof(T)] = table;
    }

    public T FindTableData<T>(int id) where T : class
    {
        if (tableMap.TryGetValue(typeof(T), out var obj))
        {
            var typedDict = obj as Dictionary<int, T>;
            if (typedDict != null && typedDict.TryGetValue(id, out var result))
                return result;
        }

        Debug.LogError($"[TableManager] No data found for type {typeof(T).Name} and ID {id}");
        return null;
    }

    #region DataTable
    private Dictionary<Type, object> tableMap = new Dictionary<Type, object>();

    private Dictionary<int, JsonIngameObject> ingameObjectDict = new Dictionary<int, JsonIngameObject>();
    private Dictionary<int, JsonChapter> chapterDict = new Dictionary<int, JsonChapter>();
    private Dictionary<int, JsonStage> stageDict = new Dictionary<int, JsonStage>();

    #endregion
}
