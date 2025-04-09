using UnityEngine;

public class UIIngameManager : MonoSingleton<UIIngameManager>
{
    public static UIDepolySlotManager DepolySlotManager;

    protected override void Awake()
    {
        base.Awake();

        //cast
        DepolySlotManager = GetComponentInChildren<UIDepolySlotManager>();
    }

    private void Start()
    {
        DepolySlotManager.InitDepoly();
    }
}
