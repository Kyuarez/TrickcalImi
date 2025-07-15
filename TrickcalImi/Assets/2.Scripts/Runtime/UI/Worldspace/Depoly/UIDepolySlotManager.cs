using System.Collections.Generic;
using UnityEngine;

public class UIDepolySlotManager : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    private Dictionary<int, UIDepolySlot> slotDicts = new Dictionary<int, UIDepolySlot>();

    public bool IsPossibleDepoly
    {
        get
        {
            foreach (UIDepolySlot slot in slotDicts.Values)
            {
                if(slot.IsDepoly == false)
                {
                    return true;
                }
            }
            return false;
        }
    }

    public void SetActivePanel(bool active)
    {
        if(active == true)
        {
            if(panel.activeSelf == false)
            {
                panel.SetActive(true);
            }
        }
        else
        {
            if (panel.activeSelf == true)
            {
                panel.SetActive(false);
            }
        }
    }

    public void InitDepoly()
    {
        slotDicts.Clear();

        UIDepolySlot[] arr = GetComponentsInChildren<UIDepolySlot>();
        for (int i = 1; i <= arr.Length; i++)
        {
            arr[i - 1].SlotIndex = i;
            slotDicts.Add(i, arr[i - 1]);
        }
    }

    public void ResetStage()
    {
        foreach (UIDepolySlot slot in slotDicts.Values)
        {
            slot.IsDepoly = false;
        }
    }

    public Vector3 GetSpawnPosBySlotIndex(int slotIndex)
    {
        if(slotDicts.ContainsKey(slotIndex) == false)
        {
            Debug.Assert(false, "SlotIndex is wrong");
            return Vector3.zero;
        }

        return slotDicts[slotIndex].GetDepolyPosition();
    }

    #region Test
    public int GetHeroDepolySlotData(ref Vector3 spawnPosition)
    {
        //Todo : 지금은 순서대로 넣지만, 나중엔 로컬 저장 위치 슬롯 대로 위치에 넣도록 수정
        foreach (UIDepolySlot slot in slotDicts.Values)
        {
            if(slot.IsDepoly == false)
            {
                spawnPosition = slot.GetDepolyPosition();
                return slot.SlotIndex;
            }
        }

        spawnPosition = Vector3.zero;
        return -1;
    }
    #endregion

    public void SetSlotDeployState(int slotIndex)
    {
        if(slotDicts.ContainsKey(slotIndex) == true)
        {
            slotDicts[slotIndex].IsDepoly = true;
        }
    }
}
