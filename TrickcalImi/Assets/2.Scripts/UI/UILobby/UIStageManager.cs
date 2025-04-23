using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIStageManager : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform contentRect;

    private Dictionary<int, UIStageSlot> stageSlotDict;
    private UIStageSlot currentSlot;

    private int slotCount = 10;

    private Coroutine moveCoroutine;
    private float focusSpeed = 2.0f;
    private float focusTime = 1.0f;

    private void Start()
    {
        UIManager.LobbySelectStage.OnSelectSlot += OnSelectSlot;
    }

    public void InitUIStageManager(JsonChapter chapter)
    {
        stageSlotDict = new Dictionary<int, UIStageSlot>();
        currentSlot = null;

        SpawnSlots(chapter);
    }

    //�ϴ� ��ŸƮ���� �����ϵ�, Json �����Ǹ� ����Ʈ �������� �̵� ������ �����ؼ� ������ �ִ� ���·� �����ؾ� ��.
    public void SpawnSlots(JsonChapter chapter) 
    {
        List<int> stageIDlList = chapter.StageIDList;
        for (int i = 0; i < stageIDlList.Count; i++)
        {
            GameObject slotObj = PoolManager.Instance.SpawnObject("UIStageSlot");
            if(slotObj != null)
            {
                UIStageSlot slot = slotObj.GetComponent<UIStageSlot>();
                if(slot != null)
                {
                    JsonStage json = TableManager.Instance.FindTableData<JsonStage>(stageIDlList[i]);
                    if(json != null)
                    {
                        slot.InitStageSlot(json);
                        slot.transform.SetParent(contentRect, false); //�̰� ��ġ �����ؾ���..
                        slot.StageNumber = stageIDlList[i];
                        stageSlotDict.Add(slot.StageNumber, slot);
                    }
                }
            }
        }

        //Sort Zigzag ����
        SortStageSlots();
    }

    private void SortStageSlots()
    {
        if(stageSlotDict == null | stageSlotDict.Count == 0)
        {
            return;
        }

        UIStageSlot model = stageSlotDict.First().Value;
        float intervalX = model.Width;
        float intervalY = model.Height / 2f;

        float x = -960.0f;
        float y = 0.0f;
        foreach (KeyValuePair<int, UIStageSlot> kv in stageSlotDict)
        {
            x = (kv.Key == 1) ? x : x + intervalX; 
            y = (kv.Key % 2 == 0) ? 0f : intervalY;
            kv.Value.SetRectPosition(x, y);
        }
    }

    public void OnSelectSlot(UIStageSlot slot)
    {
        currentSlot = slot;
        //TODO ��ũ�� ��ġ ����
        if(moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }

        moveCoroutine = StartCoroutine(FocusSelectSlotCo(currentSlot.PosX));
        //contentRect.localPosition = new Vector2()
    }

    private IEnumerator FocusSelectSlotCo(float destinationX)
    {
        float elapsedTime = 0.0f;
        while(elapsedTime >= focusTime)
        {
            //contentRect.localPosition = new Vector2()
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

}
