using UnityEngine;

public class UIIngameHUD : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    private UIIngameTimer ingameTimer;
    private UIIngameHUD ingameHUD;
    private UIWaveHUD waveHUD;

    private void Awake()
    {
        ingameTimer = GetComponentInChildren<UIIngameTimer>();
        ingameHUD = GetComponent<UIIngameHUD>();
        waveHUD = GetComponentInChildren<UIWaveHUD>();

        StageManager.Instance.OnSetupAction += OnSetupAction;
        StageManager.Instance.OnCombatAction += OnCombatAction;
    }

    public void OnSetupAction()
    {
        ingameTimer.OnSetupUIIngameTimer();
        waveHUD.UpdateWaveHUD();
    }

    public void OnCombatAction()
    {
        ingameTimer.OnCombatUIIngameTimer();
    }

}
