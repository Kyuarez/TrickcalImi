using UnityEngine;

//@tk (25.04.17) �ϴ� �ΰ��� ��ȯ�� �߿��ϴϱ�, Adventure�� ��� ���� : �ٷ� �������� ����Ʈ��
//@tk Adventure�� ���� ����
public class UILobby : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    private UILobbyBackground lobbyBackground;
    private UILobbyMain lobbyMain;
    private UILobbyAdventure lobbyAdventure;
    private UILobbySelectStage lobbySelectStage;
    private UILobbyHUD lobbyHUD;

    private void Awake()
    {
        lobbyBackground = GetComponentInChildren<UILobbyBackground>();
        lobbyMain = GetComponentInChildren<UILobbyMain>();
        lobbyAdventure = GetComponentInChildren<UILobbyAdventure>();
        lobbySelectStage = GetComponentInChildren<UILobbySelectStage>();
        lobbyHUD = GetComponentInChildren<UILobbyHUD>();
    }

    public void OnUILobby(LobbyType lobbyType) 
    {
        if(panel.activeSelf == false)
        {
            panel.SetActive(true);
        }

        //UISetting
        lobbyHUD.SetLobbyHUD(lobbyType);

        switch (lobbyType)
        {
            case LobbyType.LobbyMain:
                lobbyMain.SetActivePanel(true);
                lobbyAdventure.SetActivePanel(false);
                lobbySelectStage.SetActivePanel(false);
                break;
            case LobbyType.LobbyAdventure:
                lobbyMain.SetActivePanel(false);
                lobbyAdventure.SetActivePanel(true);
                lobbySelectStage.SetActivePanel(false);
                break;
            case LobbyType.LobbySelectStage:
                lobbyMain.SetActivePanel(false);
                lobbyAdventure.SetActivePanel(false);
                lobbySelectStage.SetActivePanel(true);
                break;
            default:
                break;
        }

    }

    public void OffUILobby()
    {
        if (panel.activeSelf == true)
        {
            panel.SetActive(false);
        }
    }
}
