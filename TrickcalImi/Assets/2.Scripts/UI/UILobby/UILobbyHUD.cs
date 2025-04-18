using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILobbyHUD : MonoBehaviour
{
    [Header("panel")]
    [SerializeField] private GameObject panel;

    private CurrencyHUD currency;
    private UserInfoHUD userInfo;
    private HeaderHUD header;

    private void Awake()
    {
        currency = GetComponentInChildren<CurrencyHUD>();
        userInfo = GetComponentInChildren<UserInfoHUD>();
        header = GetComponentInChildren<HeaderHUD>();
    }

    public void SetActivePanel(bool active)
    {
        if (panel.activeSelf == !active)
        {
            panel.SetActive(active);
        }
    }
    public void SetLobbyHUD(LobbyType type)
    {
        userInfo.SetLobbyUserInfo(type);
        header.SetLobbyHeader(type);
        currency.SetLobbyCurrency(type);
    }
}
