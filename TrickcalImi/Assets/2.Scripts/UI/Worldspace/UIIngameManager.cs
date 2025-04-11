using UnityEngine;

public class UIIngameManager : MonoSingleton<UIIngameManager>
{
    private Canvas canvas;

    public static UIDepolySlotManager DepolySlotManager;

    protected override void Awake()
    {
        base.Awake();
        canvas = GetComponent<Canvas>();
        canvas.sortingOrder = Define.OrderLayer_ingameUI;

        //cast
        DepolySlotManager = GetComponentInChildren<UIDepolySlotManager>();

        DepolySlotManager.InitDepoly();

        //TODO : ������ �׳� �ִµ�, �����δ� é�� ���� ��, �������� ���� �� �� ���� �����ϴ� �۾� �ʿ�
        StageManager.Instance.OnSetupAction += OnSetupAction;
        StageManager.Instance.OnCombatAction += OnCombatAction;
    }

    public void OnSetupAction()
    {
        DepolySlotManager.SetActivePanel(true);
    }

    public void OnCombatAction()
    {
        DepolySlotManager.SetActivePanel(false);
    }

}
