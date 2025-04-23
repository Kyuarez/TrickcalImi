using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIStageInfo : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    [SerializeField] private TextMeshProUGUI stageHeaderText;
    [SerializeField] private TextMeshProUGUI stageDescText;
    [SerializeField] private TextMeshProUGUI rewardHeaderText;

    [SerializeField] private Button exitArea;
    [SerializeField] private Button btn_Exit;
    [SerializeField] private Button btn_Absolution;
    [SerializeField] private Button btn_SelectDeque;

    private int currentStageID;

    private void Awake()
    {
        btn_Exit.onClick.AddListener(OnClickExit);
        exitArea.onClick.AddListener(OnClickExit);
        btn_Absolution.onClick.AddListener(OnClickAbsolution);
        btn_SelectDeque.onClick.AddListener(OnClickSelectDeque);

        btn_Absolution.interactable = false; //일단 막기
        currentStageID = -1;
    }

    private void Start()
    {
        UIManager.LobbySelectStage.OnSelectSlot += SetUIStageInfo;
    }


    public void SetActivePanel(bool active)
    {
        if(panel.activeSelf == !active)
        {
            panel.SetActive(active);
        }
    }

    public void SetUIStageInfo(UIStageSlot slot)
    {
        SetActivePanel(true);
        currentStageID = slot.StageNumber;
        stageHeaderText.text = slot.StageData.StageName;
        stageDescText.text = string.Empty;
    }
    public void ResetUIStageInfo()
    {
        currentStageID = -1;
        stageHeaderText.text = string.Empty;
        stageDescText.text = string.Empty;
    }

    #region OnClick
    public void OnClickExit()
    {
        SetActivePanel(false);
        ResetUIStageInfo();
    }

    public void OnClickAbsolution()
    {
        //TODO 나중에 구현 (면제)
    }

    //@tk 일단 Deque는 나중에 구현하고, 바로 인게임으로 넘어가기
    public void OnClickSelectDeque()
    {
        GameSceneManager.Instance.OnIngame(currentStageID);
        SetActivePanel(false);
        ResetUIStageInfo();
    }

    #endregion
}
