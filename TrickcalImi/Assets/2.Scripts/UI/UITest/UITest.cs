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
    }

    private void Start()
    {
        StageManager.Instance.OnSetupAction += OnSetupAction;
        StageManager.Instance.OnCombatAction += OnCombatAction;
    }

    public void SetActivePanel(bool isActive)
    {
        if(panel.activeSelf == !isActive)
        {
            panel.SetActive(isActive);
        }
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
        StageManager.Instance.SpawnHeroInStage();
    }

    public void OnClickOnCombatMode() 
    {
        StageManager.Instance.OnCombatMode();
    }
    #endregion
}
