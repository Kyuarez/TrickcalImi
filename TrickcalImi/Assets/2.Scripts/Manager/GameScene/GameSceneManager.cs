using UnityEngine;

public class GameSceneManager : MonoSingleton<GameSceneManager>
{
    private GameSceneType gameSceneType;

    [RuntimeInitializeOnLoadMethod]
    public static void OnLoadGameAction()
    {
        TableManager table = new TableManager();
        table.OnLoadGameAction();

        //Test
        Debug.Log((TableManager.Instance == null));
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

    public void OnIngame(int stageID)
    {
        if(stageID == -1)
        {
            Debug.Assert(false, "Current Stage is null");
            return;
        }

        JsonStage stageData = TableManager.Instance.FindTableData<JsonStage>(stageID);
        if(stageData != null)
        {
            gameSceneType = GameSceneType.Ingame;
            StageManager.Instance.OnStage(stageData);
        }
    }

    
}
