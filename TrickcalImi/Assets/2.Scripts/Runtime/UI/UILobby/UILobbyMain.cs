using UnityEngine;
using UnityEngine.UI;

public class UILobbyMain : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject panel;

    [Header("Buttons")]
    [SerializeField] private Button btn_Recruit;
    [SerializeField] private Button btn_CashStore;
    [SerializeField] private Button btn_Store;
    [SerializeField] private Button btn_Hero;
    [SerializeField] private Button btn_Deque;
    [SerializeField] private Button btn_Card;
    [SerializeField] private Button btn_Theater;
    [SerializeField] private Button btn_House;

    [SerializeField] private Button btn_Adventure;

    private void Awake()
    {
        btn_Recruit.onClick.AddListener(OnClickRecruit);
        btn_CashStore.onClick.AddListener(OnClickCashStore);
        btn_Store.onClick.AddListener(OnClickStore);
        btn_Hero.onClick.AddListener(OnClickHero);
        btn_Deque.onClick.AddListener(OnClickDeque);
        btn_Card.onClick.AddListener(OnClickCard);
        btn_Theater.onClick.AddListener(OnClickTheater);
        btn_House.onClick.AddListener(OnClickHouse);

        btn_Adventure.onClick.AddListener(OnClickAdventure);
    }

    public void SetActivePanel(bool active)
    {
        if(panel.activeSelf == !active)
        {
            panel.SetActive(active);
        }
    }


    #region OnClick
    public void OnClickRecruit()
    {
        SoundManager.Instance.PlaySFX(10000);
    }

    public void OnClickCashStore()
    {
        SoundManager.Instance.PlaySFX(10000);
    }
    public void OnClickStore()
    {
        SoundManager.Instance.PlaySFX(10000);


    }
    public void OnClickHero()
    {
        SoundManager.Instance.PlaySFX(10000);
    }
    public void OnClickDeque()
    {
        SoundManager.Instance.PlaySFX(10000);
    }
    public void OnClickCard()
    {
        SoundManager.Instance.PlaySFX(10000);
    }
    public void OnClickTheater()
    {
        SoundManager.Instance.PlaySFX(10000);
    }
    public void OnClickHouse()
    {
        SoundManager.Instance.PlaySFX(10000);
    }
    public void OnClickAdventure()
    {
        SoundManager.Instance.PlaySFX(10000);
        //TODO : UILobby type : Adventure (25.04.17) 일단 어드벤처는 넘기기. (차후 구현)
        //GameSceneManager.Instance.OnLobby(LobbyType.LobbyAdventure);
        GameSceneManager.Instance.OnLobby(LobbyType.LobbySelectStage);
    }
    #endregion
}
