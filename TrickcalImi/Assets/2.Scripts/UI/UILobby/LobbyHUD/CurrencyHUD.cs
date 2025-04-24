using TMPro;
using UnityEngine;

public class CurrencyHUD : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI ticketText;
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private TextMeshProUGUI cashText;

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
                SetUICurrencyData();
                break;
            default:
                SetActivePanel(false);
                break;
        }
    }

    public void SetUICurrencyData()
    {
        JsonLocalUserData localData = LocalDataManager.Instance.LocalUserData;
        if(localData != null)
        {
            ticketText.text = localData.Ticket.ToString();
            coinText.text = localData.Coin.ToString();
            cashText.text = localData.Cash.ToString();
        }
    }
}
