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
        //@tk �ϴ� Ÿ��Ʋ ����� �������� ���� �κ��
        OnLobby(LobbyType.LobbyMain);
    }

    public void OnLobby(LobbyType type)
    {
        gameSceneType = GameSceneType.Lobby;
        UIManager.Instance.OnLobby(type);
    }

    public void OnIngame()
    {
        gameSceneType = GameSceneType.Ingame;
        StageManager.Instance.OnStage();
    }
}
