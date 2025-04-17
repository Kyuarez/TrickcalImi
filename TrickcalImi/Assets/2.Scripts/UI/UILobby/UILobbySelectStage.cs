using UnityEngine;

public class UILobbySelectStage : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    public void SetActivePanel(bool active)
    {
        if (panel.activeSelf == !active)
        {
            panel.SetActive(active);
        }
    }
}
