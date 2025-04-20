using UnityEngine;

/*
 Ingame �󿡼� worldspace ĵ���� UI ����
 */
public class UIIngameManager : MonoSingleton<UIIngameManager>
{
    private Canvas canvas;

    public static UIDepolySlotManager DepolySlotManager;
    public static UIBillboardManager BillboardManager;

    protected override void Awake()
    {
        base.Awake();
        canvas = GetComponent<Canvas>();
        canvas.sortingOrder = Define.OrderLayer_ingameUI;

        //cast
        DepolySlotManager = GetComponentInChildren<UIDepolySlotManager>();
        BillboardManager = GetComponentInChildren<UIBillboardManager>();

        DepolySlotManager.InitDepoly(); 

        //TODO : ������ �׳� �ִµ�, �����δ� é�� ���� ��, �������� ���� �� �� ���� �����ϴ� �۾� �ʿ�
        StageManager.Instance.OnSetupAction += OnSetupAction;
        StageManager.Instance.OnCombatAction += OnCombatAction;
        StageManager.Instance.OnResetAction += OnResetAction;
    }

    public void OnSetupAction()
    {
        DepolySlotManager.SetActivePanel(true);
    }

    public void OnCombatAction()
    {
        DepolySlotManager.SetActivePanel(false);
    }

    public void OnResetAction()
    {
        DepolySlotManager.ResetStage();
        BillboardManager.ResetStage();
    }
}
