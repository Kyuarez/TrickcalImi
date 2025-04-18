using UnityEngine;

public class CurrencyHUD : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    public void SetActivePanel(bool active)
    {
        if (panel.activeSelf == !active)
        {
            panel.SetActive(active);
        }
    }
    public void SetLobbyCurrency(LobbyType type)
    {
        switch (type)
        {
            case LobbyType.LobbyMain:
            case LobbyType.LobbyAdventure:
            case LobbyType.LobbySelectStage:
                SetActivePanel(true);
                break;
            default:
                SetActivePanel(false);
                break;
        }
    }
}
