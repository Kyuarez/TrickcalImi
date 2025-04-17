using NUnit.Framework.Internal;
using UnityEngine;

public class UIManager : MonoSingleton<UIManager>
{
    private Canvas canvas;

    //Lobby
    public static UILobby Lobby;

    //Ingame
    public static UITest Test;
    public static UIIngameHUD IngameHUD;
    public static UIIngameResult IngameResult;

    //Transition


    protected override void Awake()
    {
        canvas = GetComponent<Canvas>();

        canvas.sortingOrder = Define.OrderLayer_baseUI;

        //casting
        Lobby = GetComponentInChildren<UILobby>();

        Test = GetComponentInChildren<UITest>();
        IngameHUD = GetComponentInChildren<UIIngameHUD>();
        IngameResult = GetComponentInChildren<UIIngameResult>();
    }

    private void Start()
    {
        IngameResult.InitIngameResult();

        //TODO : 지금은 그냥 넣는데, 앞으로는 챕터 실행 시, 스테이지 들어가면 그 때 연결 해제하는 작업 필요
        StageManager.Instance.OnSuccessAction += OnSuccessAction;
        StageManager.Instance.OnFailureAction += OnFailureAction;
    }

    public void OnLobby(LobbyType type) 
    {
        Test.SetActivePanel(false);
        IngameHUD.SetActivePanel(false);
        IngameResult.SetActivePanel(false);

        Lobby.OnUILobby(type);
    }
    public void OnIngame()
    {
        Lobby.OffUILobby();

        Test.SetActivePanel(true);
        IngameHUD.SetActivePanel(true);
        IngameResult.SetActivePanel(true);
    }

    //Ingame
    public void OnSuccessAction() 
    {
        IngameResult.OnIngameResult(IngameModeType.Success);
    }
    public void OnFailureAction()
    {
        IngameResult.OnIngameResult(IngameModeType.Failure);
    }
}
