using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStageManager : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform contentRect;

    private UIStageSlot stageSlotObj;
    private Dictionary<int, UIStageSlot> stageSlotDict;
    private UIStageSlot currentSlot;

    private int slotCount = 10;
    private float slotDistance = 20;

    public void InitUIStageManager()
    {
        stageSlotObj = Resources.Load<UIStageSlot>(Define.Res_UI_LobbyStageSlot).GetComponent<UIStageSlot>();
        stageSlotDict = new Dictionary<int, UIStageSlot>();
        currentSlot = null;

        SpawnSlots();
    }

    public void SpawnSlots()
    {
        for (int i = 1; i <= slotCount; i++)
        {
            UIStageSlot slot = Instantiate(stageSlotObj, contentRect);
            slot.InitStageSlot();
            slot.StageNumber = i;
            stageSlotDict.Add(i, slot);
        }
    }

    public void OnSelectSlot(UIStageSlot slot)
    {
        currentSlot = slot;
    }

}
