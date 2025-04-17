using UnityEngine;

public class UILobbyHUD : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    public void SetActivePanel(bool active)
    {
        if (panel.activeSelf == !active)
        {
            panel.SetActive(active);
        }
    }

    public void SetLobbyHUD(LobbyType type)
    {

    }
}
