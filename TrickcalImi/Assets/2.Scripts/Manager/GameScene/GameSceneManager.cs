using UnityEngine;

//@tk 일단 게임 매니저 역할!
public class GameSceneManager : MonoSingleton<GameSceneManager>
{
    private GameSceneType gameSceneType;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void OnLoadGameAction()
    {
        TableManager table = new TableManager();
        table.OnLoadGameAction();
        LocalDataManager localData = new LocalDataManager();
        localData.OnLoadGameAction();

        //현재 챕터 계산
    }

    private void Start()
    {
        //@tk 일단 타이틀 만들기 전까지는 시작 로비로
        OnLobby(LobbyType.LobbyMain);
    }

    public void OnLobby(LobbyType type)
    {
        gameSceneType = GameSceneType.Lobby;
        UIManager.Instance.OnLobby(type);
    }

    public void OnIngame(int stageID) //ui stageinfo의 정보 받고
    {
        if(stageID == -1)
        {
            Debug.Assert(false, "Current Stage is null");
            return;
        }

        UIManager.Transition.OnTransition(UITransitionType.Loading);
        JsonStage stageData = TableManager.Instance.FindTableData<JsonStage>(stageID);
        if(stageData != null)
        {
            JsonChapter chapter = TableManager.Instance.FindTableData<JsonChapter>(stageData.ChapterNumber);
            if(chapter != null)
            {
                SoundManager.Instance.PlayBGM(chapter.SoundID);
            }

            gameSceneType = GameSceneType.Ingame;
            StageManager.Instance.OnStage(stageData);
        }
    }
    
}
