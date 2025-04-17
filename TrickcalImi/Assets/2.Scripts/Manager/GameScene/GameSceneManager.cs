using UnityEngine;

public class GameSceneManager : MonoSingleton<GameSceneManager>
{
    private GameSceneType gameSceneType;

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

    public void OnIngame()
    {
        gameSceneType = GameSceneType.Ingame;
        StageManager.Instance.OnStage();
    }
}
