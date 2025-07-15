using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIIngameResult : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    [Header("MessagePanel")]
    [SerializeField] private TextMeshProUGUI messageText; //Fail, Success

    [Header("InfoPanel")]
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI expText;
    [SerializeField] private Slider expSlider;
    [SerializeField] private Button btn_Exit;
    [SerializeField] private Button btn_Restart;

    public void InitIngameResult()
    {
        btn_Exit.onClick.AddListener(OnClickExit);
        btn_Restart.onClick.AddListener(OnClickRestart);
    }

    public void SetActivePanel(bool isActive)
    {
        if (panel.activeSelf == !isActive)
        {
            panel.SetActive(isActive);
        }
    }

    public void OnIngameResult(IngameModeType type)
    {
        if(type == IngameModeType.Success || type == IngameModeType.Failure)
        {
            SetIngameResultMessage(type);
            panel.SetActive(true);
            return;
        }

        Debug.AssertFormat(false, $"ingame mode type was wrong! : {type.ToString()}");
    }

    public void SetIngameResultMessage(IngameModeType type)
    {
        switch (type)
        {
            case IngameModeType.Success:
                messageText.text = "�������� Ŭ����!";
                break;
            case IngameModeType.Failure:
                messageText.text = "�������� �й�";
                break;
        }
    }

    #region OnClick
    public void OnClickExit()
    {
        //TODO : é�� �κ�� �̵�
        panel.SetActive(false);
        SoundManager.Instance.PlaySFX(10001);
        StageManager.Instance.ResetStage();
        GameSceneManager.Instance.OnLobby(LobbyType.LobbySelectStage);
        Time.timeScale = 1.0f; //@tk �̰� ���� �ڵ� �ƴ�. stage�Ŵ����� gamesc

    }

    public void OnClickRestart() 
    {
        //TODO : �ٽ� �������� ����
        panel.SetActive(false);
        SoundManager.Instance.PlaySFX(10000);
        StageManager.Instance.ResetStage();
        //GameSceneManager.Instance.OnIngame();
        Time.timeScale = 1.0f;
    }

    #endregion
}
