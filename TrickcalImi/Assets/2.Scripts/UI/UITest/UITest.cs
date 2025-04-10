using UnityEngine;
using UnityEngine.UI;

public class UITest : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Button btn_Depoly;
    [SerializeField] private Button btn_CombatMode;

    private void Awake()
    {
        btn_Depoly.onClick.AddListener(OnClickDeployHero);
        btn_CombatMode.onClick.AddListener(OnClickOnCombatMode);

        StageManager.Instance.OnSetupAction += OnSetupAction;
        StageManager.Instance.OnCombatAction += OnCombatAction;
    }


    public void OnSetupAction()
    {
        if(btn_Depoly.interactable == false)
        {
            btn_Depoly.interactable = true;
        }
    }

    public void OnCombatAction()
    {
        if (btn_Depoly.interactable == true)
        {
            btn_Depoly.interactable = false;
        }
    }

    #region OnClick
    public void OnClickDeployHero()
    {
        GameObject obj = Resources.Load<GameObject>("Prefabs/Objects/Hero/TestHero");
        UIIngameManager.DepolySlotManager.OnDepolyHero(obj);
    }

    public void OnClickOnCombatMode() 
    {
        StageManager.Instance.OnCombatMode();
    }
    #endregion
}
