using UnityEngine;

/*
 Ingame �󿡼� worldspace ĵ���� UI ����
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
        canvas.sortingOrder = Define.OrderLayer_ingameUI;

        //cast
        DepolySlotManager = GetComponentInChildren<UIDepolySlotManager>();
        BillboardManager = GetComponentInChildren<UIBillboardManager>();

        damagePopupParent = transform.FindRecursiveChild("@DamagePopupParent");
        
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
