using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/* [25.04.09]
 구글 드라이브 내부에 있는 구글 스프레드 시트 데이터들 
해당하는 데이터 클래스로 파싱 후에 딕셔너리에 저장.
 */
public class SheetManager : MonoSingleton<SheetManager>
{
    protected override void Awake()
    {
        base.Awake();
        OnLoadSheetData();
    }


    public string baseUrl = "https://script.google.com/macros/s/AKfycbygjkqWV9uY_JwcV9sK_1-t4uK400rYEWcNDoY1OIdqPajOoXaS5G5Zqb2PxfHFqE_I/exec?sheet=";

    private string stageSheetUrl = "https://docs.google.com/spreadsheets/d/1OIlel3EMZB2nK75rLSyCs_BBW1qe_q_713AkxlEbJTQ/edit?usp=sharing";

    public void OnLoadSheetData()
    {
        StartCoroutine(LoadStageData(stageSheetUrl));
    }
    IEnumerator LoadStageData(string url)
    {
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error loading data: " + www.error);
        }
        else
        {
            string json = "{\"items\":" + www.downloadHandler.text + "}";
            DataListWrapper<Sheet_StageData> dataList = JsonUtility.FromJson<DataListWrapper<Sheet_StageData>>(json);

            foreach (var stage in dataList.items)
            {
                Debug.Log($"ID: {stage.StageID}, Name: {stage.StageName}");
            }
        }
    }

    IEnumerator LoadAllData()
    {
        yield return LoadData<Sheet_StageData>("StageData", stageDict);
        yield return LoadData<Sheet_UnitData>("UnitData", unitDict);
    }

    IEnumerator LoadData<T>(string sheetName, Dictionary<int, T> dict) where T : class
    {
        //Load sheet
        string url = baseUrl + sheetName;
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.AssertFormat(false, $"fail to load {sheetName}");
            yield break;
        }

        //sheet -> json
        string json = "{\"items\":" + www.downloadHandler.text + "}";
        Debug.Log(json);
        DataListWrapper<T> wrapper = JsonUtility.FromJson<DataListWrapper<T>>(json);

        //dict
        dict.Clear();
        foreach (T item in wrapper.items)
        {
            
        }

    }

    private Dictionary<int, Sheet_StageData> stageDict = new Dictionary<int, Sheet_StageData>();
    private Dictionary<int, Sheet_UnitData> unitDict = new Dictionary<int, Sheet_UnitData>();
}
