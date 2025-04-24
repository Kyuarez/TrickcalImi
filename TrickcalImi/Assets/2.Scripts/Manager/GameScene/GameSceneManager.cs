using UnityEngine;

//@tk �ϴ� ���� �Ŵ��� ����!
public class GameSceneManager : MonoSingleton<GameSceneManager>
{
    private GameSceneType gameSceneType;

    [RuntimeInitializeOnLoadMethod]
    public static void OnLoadGameAction()
    {
        TableManager table = new TableManager();
        table.OnLoadGameAction();
        LocalDataManager localData = new LocalDataManager();
        localData.OnLoadGameAction();

        //���� é�� ���
    }

    private void Start()
    {
        //@tk �ϴ� Ÿ��Ʋ ����� �������� ���� �κ��
        OnLobby(LobbyType.LobbyMain);
    }

    public void OnLobby(LobbyType type)
    {
        gameSceneType = GameSceneType.Lobby;
        UIManager.Instance.OnLobby(type);
    }

    public void OnIngame(int stageID) //ui stageinfo�� ���� �ް�
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
