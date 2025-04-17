using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIStageInfo : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    [SerializeField] private TextMeshProUGUI stageHeaderText;
    [SerializeField] private TextMeshProUGUI stageDescText;
    [SerializeField] private TextMeshProUGUI rewardHeaderText;

    [SerializeField] private Button btn_Exit;
    [SerializeField] private Button btn_Absolution;
    [SerializeField] private Button btn_SelectDeque;

    private void Awake()
    {
        btn_Exit.onClick.AddListener(OnClickExit);
        btn_Absolution.onClick.AddListener(OnClickAbsolution);
        btn_SelectDeque.onClick.AddListener(OnClickSelectDeque);
    }

    public void SetActivePanel(bool active)
    {
        if(panel.activeSelf == !active)
        {
            panel.SetActive(active);
        }
    }

    public void SetUIStageInfo(/*Stage Data*/)
    {
        //stageHeaderText
        //stageDescText
    }
    public void ResetUIStageInfo()
    {

    }

    #region OnClick
    public void OnClickExit()
    {
        SetActivePanel(false);
        ResetUIStageInfo();
    }

    public void OnClickAbsolution()
    {
        //TODO ���߿� ���� (����)
    }

    //@tk �ϴ� Deque�� ���߿� �����ϰ�, �ٷ� �ΰ������� �Ѿ��
    public void OnClickSelectDeque()
    {
        GameSceneManager.Instance.OnIngame();
        SetActivePanel(false);
        ResetUIStageInfo();
    }

    #endregion
}
