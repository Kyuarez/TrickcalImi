using UnityEngine;

//@tk (25.04.17) 일단 인게임 전환이 중요하니까, Adventure는 잠시 제외 : 바로 스테이지 셀렉트로
//@tk Adventure는 차후 구현
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
        lobbyBackground.SetBackgroundLobbyMain();

        switch (lobbyType)
        {
            case LobbyType.LobbyMain:
                SoundManager.Instance.PlayBGM(2);
                lobbyMain.SetActivePanel(true);
                lobbyAdventure.SetActivePanel(false);
                lobbySelectStage.ResetStageSelect();
                lobbySelectStage.SetActivePanel(false);
                break;
            case LobbyType.LobbyAdventure:
                SoundManager.Instance.PlayBGM(2);
                lobbyMain.SetActivePanel(false);
                lobbyAdventure.SetActivePanel(true);
                lobbySelectStage.ResetStageSelect();
                lobbySelectStage.SetActivePanel(false);
                break;
            case LobbyType.LobbySelectStage:
                SoundManager.Instance.PlayBGM(3);
                lobbyMain.SetActivePanel(false);
                lobbyAdventure.SetActivePanel(false);
                lobbySelectStage.OnStageSelect();
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
        lobbyMain.SetActivePanel(false);
        lobbyAdventure.SetActivePanel(false); 
        lobbySelectStage.ResetStageSelect();
        lobbySelectStage.SetActivePanel(false);
    }
}
