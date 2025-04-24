using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILobbyHUD : MonoBehaviour
{
    [Header("panel")]
    [SerializeField] private GameObject panel;
    [SerializeField] private Button btn_Home;

    private CurrencyHUD currency;
    private UserInfoHUD userInfo;
    private HeaderHUD header;


    private void Awake()
    {
        currency = GetComponentInChildren<CurrencyHUD>();
        userInfo = GetComponentInChildren<UserInfoHUD>();
        header = GetComponentInChildren<HeaderHUD>();
        btn_Home.onClick.AddListener(OnClickHome);
    }

    public void SetActivePanel(bool active)
    {
        if (panel.activeSelf == !active)
        {
            panel.SetActive(active);
        }
    }
    public void SetLobbyHUD(LobbyType type)
    {
        userInfo.SetLobbyUserInfo(type);
        header.SetLobbyHeader(type);
        currency.SetLobbyCurrency(type);
    }

    public void OnClickHome()
    {
        //TODO @TK : 원래는 세팅창. 차후 구현 예정 (현재는 나가기 버튼)
        LocalDataManager.Instance.SaveLocalData();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Appllication.Quit();
#endif
    }
}
