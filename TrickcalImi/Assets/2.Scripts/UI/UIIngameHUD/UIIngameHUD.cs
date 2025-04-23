using UnityEngine;

public class UIIngameHUD : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    private UIIngameTimer ingameTimer;
    private UIWaveHUD waveHUD;
    private UICostCountHUD costCountHUD;

    public void SetActivePanel(bool isActive)
    {
        if (panel.activeSelf == !isActive)
        {
            panel.SetActive(isActive);
        }
    }

    private void Awake()
    {
        ingameTimer = GetComponentInChildren<UIIngameTimer>();
        waveHUD = GetComponentInChildren<UIWaveHUD>();
        costCountHUD = GetComponentInChildren<UICostCountHUD>();
    }

    private void Start()
    {
        StageManager.Instance.OnChangedCost += OnChangedCost;  
    }

    //Stage
    public void OnTickAction(float remainingTime, float limitTime)
    {
        ingameTimer.UpdateUIIngameTimer(remainingTime, limitTime);
    }

    public void OnSetupAction()
    {
        waveHUD.UpdateWaveHUD();
    }

    public void OnCombatAction()
    {
        //TODO
    }
    public void OnResetAction()
    {
        waveHUD.ResetWaveHUD();
    }

    public void OnChangedCost(int cost)
    {
        costCountHUD.OnUpdateCostCount(cost);
    }

}
