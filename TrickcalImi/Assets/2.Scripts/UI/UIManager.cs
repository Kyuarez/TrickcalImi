using UnityEngine;

public class UIManager : MonoSingleton<UIManager>
{
    private Canvas canvas;

    //Lobby
    public static UILobby Lobby;
    public static UILobbySelectStage LobbySelectStage;

    //Ingame
    public static UITest Test;
    public static UIIngameHUD IngameHUD;
    public static UIIngameResult IngameResult;
    public static UICardDraw CardDraw;

    //Transition
    public static UITransition Transition;


    protected override void Awake()
    {
        base.Awake();
        canvas = GetComponent<Canvas>();

        canvas.sortingOrder = Define.OrderLayer_baseUI;

        //casting
        Lobby = GetComponentInChildren<UILobby>();
        LobbySelectStage = GetComponentInChildren<UILobbySelectStage>();

        Test = GetComponentInChildren<UITest>();
        IngameHUD = GetComponentInChildren<UIIngameHUD>();
        IngameResult = GetComponentInChildren<UIIngameResult>();
        CardDraw = GetComponentInChildren<UICardDraw>();

        Transition = GetComponentInChildren<UITransition>();
    }

    private void Start()
    {
        IngameResult.InitIngameResult();

        //TODO : ������ �׳� �ִµ�, �����δ� é�� ���� ��, �������� ���� �� �� ���� �����ϴ� �۾� �ʿ�
        StageManager.Instance.OnTickAction += OnTickAction;
        StageManager.Instance.OnSetupAction += OnSetupAction;
        StageManager.Instance.OnCombatAction += OnCombatAction;
        StageManager.Instance.OnSuccessAction += OnSuccessAction;
        StageManager.Instance.OnFailureAction += OnFailureAction;
        StageManager.Instance.OnResetAction += OnResetAction;
    }

    public void OnLobby(LobbyType type) 
    {
        Test?.SetActivePanel(false);
        IngameHUD.SetActivePanel(false);
        IngameResult.SetActivePanel(false);

        Lobby.OnUILobby(type);
    }
    public void OnIngame()
    {
        Lobby.OffUILobby();

        Test?.SetActivePanel(true);
        IngameHUD.SetActivePanel(true);
        IngameResult.SetActivePanel(false);
    }

    //Ingame
    public void OnTickAction(float remainingTime, float limitTime)
    {
        IngameHUD.OnTickAction(remainingTime, limitTime);
    }
    public void OnSetupAction()
    {
        IngameHUD.OnSetupAction();
        CardDraw?.OnSetupAction();
    }
    public void OnCombatAction()
    {
        IngameHUD.OnCombatAction();
        CardDraw?.OnCombatAction();
    }
    public void OnSuccessAction() 
    {
        IngameResult.OnIngameResult(IngameModeType.Success);
    }
    public void OnFailureAction()
    {
        IngameResult.OnIngameResult(IngameModeType.Failure);
    }
    public void OnResetAction()
    {
        IngameHUD.OnResetAction();
        CardDraw?.OnResetAction();
    }
}
