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
        //@tk �ϴ� é�� ���� â ��� ���� é�ͷ� �ϱ�� ��.
        int currentChapter = LocalDataManager.Instance.LocalUserData.CurrentChapter;
        JsonChapter json = TableManager.Instance.FindTableData<JsonChapter>(currentChapter);
        uiStageManager.SetUIStageManager(json);
        SoundManager.Instance.PlaySFX(10001);
    }

    public void ResetStageSelect()
    {
        uiStageManager.ResetUIStageManager();
        uiStageInfo.SetActivePanel(false);
    }
}
