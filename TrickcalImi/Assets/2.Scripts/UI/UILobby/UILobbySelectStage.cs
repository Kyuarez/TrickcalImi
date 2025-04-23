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

    private void Start()
    {
        //@tk �ϴ� é�� ���� ��� �ӽ÷� é�� 1�� ��.
        JsonChapter json = TableManager.Instance.FindTableData<JsonChapter>(1);
        uiStageManager.InitUIStageManager(json);
    }

    public void SetActivePanel(bool active)
    {
        if (panel.activeSelf == !active)
        {
            panel.SetActive(active);
        }
    }
}
