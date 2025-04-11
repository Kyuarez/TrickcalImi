using UnityEngine;

public class UIDepolySlot : MonoBehaviour
{
    [SerializeField] private int slotIndex;
    [SerializeField] private Transform spawnPos_Center; //pivot
    [SerializeField] private Transform spawnPos_Bottom;

    private bool isDepoly = false;
    
    public int SlotIndex
    {
        get { return slotIndex; }
        set { slotIndex = value; }
    }

    public bool IsDepoly 
    {
        get { return isDepoly; }
        set { isDepoly = value; }
    }
    

    public Vector3 GetDepolyPosition(bool isPivotBottom = false)
    {
        return (isPivotBottom == true) ? spawnPos_Bottom.position : spawnPos_Center.position;
    }

    
}
