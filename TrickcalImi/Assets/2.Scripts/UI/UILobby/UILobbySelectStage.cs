using System;
using System.Collections.Generic;
using UnityEngine;

public class UILobbySelectStage : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    private UIStageManager uiStageManager;
    private UIStageInfo uiStageInfo;

    public Action<UIStageSlot> OnSelectSlot;

    private void Awake()
    {
        uiStageManager = GetComponentInChildren<UIStageManager>();
        uiStageInfo = GetComponentInChildren<UIStageInfo>();

    }

    public void SetActivePanel(bool active)
    {
        if (panel.activeSelf == !active)
        {
            panel.SetActive(active);
        }
    }

    public void OnStageSelect()
    {
        //@tk 일단 챕터 저장 없어서 임시로 챕터 1로 함.
        JsonChapter json = TableManager.Instance.FindTableData<JsonChapter>(1);
        uiStageManager.SetUIStageManager(json);
    }

    public void ResetStageSelect()
    {
        uiStageManager.ResetUIStageManager();
        uiStageInfo.SetActivePanel(false);
    }
}
