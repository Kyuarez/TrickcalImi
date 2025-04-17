using UnityEngine;

public class UILobbySelectStage : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    private UIStageManager uiStageManager;
    private UIStageInfo uiStageInfo;

    private UIStageSlot currentSlot;

    private void Awake()
    {
        uiStageManager = GetComponentInChildren<UIStageManager>();
        uiStageInfo = GetComponentInChildren<UIStageInfo>();

        currentSlot = null;
    }

    public void SetActivePanel(bool active)
    {
        if (panel.activeSelf == !active)
        {
            panel.SetActive(active);
        }
    }
}
