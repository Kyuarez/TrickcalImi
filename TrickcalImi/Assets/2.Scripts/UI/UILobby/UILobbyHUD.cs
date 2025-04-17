using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILobbyHUD : MonoBehaviour
{
    [Header("panel")]
    [SerializeField] private GameObject panel;



    [Header("HeaderHUD")]
    [SerializeField] private Button btn_back;
    [SerializeField] private TextMeshProUGUI headerText;

    private void Awake()
    {
        btn_back.onClick.AddListener(OnClickBackward);
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
        SetLobbyHeader(type);
    }

    public void SetLobbyHeader(LobbyType type)
    {
        switch (type)
        {
            case LobbyType.LobbyMain:
                SetActivePanel(false);
                break;
            case LobbyType.LobbyAdventure:
                SetActivePanel(true);
                headerText.text = "모험하기";
                break;
            case LobbyType.LobbySelectStage:
                SetActivePanel(true);
                headerText.text = "스테이지 리스트";
                break;
            default:
                break;
        }
    }

    #region OnClick
    public void OnClickBackward()
    {
        
        GameSceneManager.Instance.OnLobby(LobbyType.LobbyMain);
    }
    #endregion
}
