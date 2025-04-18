using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class UserInfoHUD : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    public void SetActivePanel(bool active)
    {
        if (panel.activeSelf == !active)
        {
            panel.SetActive(active);
        }
    }

    public void SetLobbyUserInfo(LobbyType type)
    {
        switch (type)
        {
            case LobbyType.LobbyMain:
                SetActivePanel(true);
                break;
            default:
                SetActivePanel(false);
                break;
        }
    }
}
