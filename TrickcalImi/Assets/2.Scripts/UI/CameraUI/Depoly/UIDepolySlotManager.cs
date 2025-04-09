using System.Collections.Generic;
using UnityEngine;

public class UIDepolySlotManager : MonoBehaviour
{
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
        //Todo : ������ ������� ������, ���߿� ���� ���� ��ġ ���� ��� ��ġ�� �ֵ��� ����
        foreach (UIDepolySlot slot in slotLists)
        {
            if(slot.IsDepoly == false)
            {
                HeroManager hero = Instantiate(prefab, slot.transform.position, Quaternion.identity).GetComponent<HeroManager>();
                if(hero != null)
                {
                    slot.OnDepoly(hero);

                    break;
                }
            }
        }
    }
}
