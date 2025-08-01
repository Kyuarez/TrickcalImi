using UnityEngine;

/*
 Ingame 상에서 worldspace 캔버스 UI 관리
 */
public class UIIngameManager : MonoSingleton<UIIngameManager>
{
    private Canvas canvas;

    public static UIDepolySlotManager DepolySlotManager;
    public static UIBillboardManager BillboardManager;

    private Transform damagePopupParent;

    protected override void Awake()
    {
        base.Awake();
        canvas = GetComponent<Canvas>();

        //cast
        DepolySlotManager = GetComponentInChildren<UIDepolySlotManager>();
        BillboardManager = GetComponentInChildren<UIBillboardManager>();

        damagePopupParent = transform.FindRecursiveChild("@DamagePopupParent");
        
        DepolySlotManager.InitDepoly(); 

        //TODO : 지금은 그냥 넣는데, 앞으로는 챕터 실행 시, 스테이지 들어가면 그 때 연결 해제하는 작업 필요
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

    public void OnDamagePopup(float damage, Transform mark)
    {
        UIDamagePopup uiDamagePopup = PoolManager.Instance.SpawnObject("UIDamagePopup").GetComponent<UIDamagePopup>();
        if (uiDamagePopup != null)
        {
            uiDamagePopup.transform.SetParent(damagePopupParent, false);
            uiDamagePopup.transform.position = mark.position;
            uiDamagePopup.transform.localScale = Vector3.one;
            uiDamagePopup.OnDamagePopup(damage);
        }
    }
}
