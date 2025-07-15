using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class HeaderHUD : MonoBehaviour
{
    [SerializeField] private GameObject panel;

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
    public void OnClickBackward()
    {
        SoundManager.Instance.PlaySFX(10002);
        GameSceneManager.Instance.OnLobby(LobbyType.LobbyMain);
    }
}
