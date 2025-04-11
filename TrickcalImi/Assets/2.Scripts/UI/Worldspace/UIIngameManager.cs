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

        //TODO : 지금은 그냥 넣는데, 앞으로는 챕터 실행 시, 스테이지 들어가면 그 때 연결 해제하는 작업 필요
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
