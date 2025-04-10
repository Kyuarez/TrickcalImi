using System.Collections.Generic;
using UnityEngine;

public class UIDepolySlotManager : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    private List<UIDepolySlot> slotLists = new List<UIDepolySlot>();

    public bool IsPossibleDepoly
    {
        get
        {
            foreach (UIDepolySlot slot in slotLists)
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
        slotLists.Clear();

        UIDepolySlot[] arr = GetComponentsInChildren<UIDepolySlot>();
        for (int i = 0; i < arr.Length; i++)
        {
            slotLists.Add(arr[i]);
        }
    }

    public void OnDepolyHero(GameObject prefab)
    {
        //Todo : 지금은 순서대로 넣지만, 나중엔 로컬 저장 위치 슬롯 대로 위치에 넣도록 수정
        foreach (UIDepolySlot slot in slotLists)
        {
            if(slot.IsDepoly == false)
            {
                HeroManager hero = Instantiate(prefab).GetComponent<HeroManager>();
                if(hero != null)
                {
                    slot.OnDepoly(hero);
                    break;
                }
                else
                {
                    Destroy(hero);
                }
            }
        }
    }
}
