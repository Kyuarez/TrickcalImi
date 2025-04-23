using System.Security.Cryptography.X509Certificates;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIStageSlot : MonoBehaviour
{
    private int stageNumber;
    private JsonStage stageData;
    
    [SerializeField] private TextMeshProUGUI stageHeaderText;
    [SerializeField] private Image lockImage;
    
    private RectTransform rectTransform;
    private Button btn_onlick;

    public float Width => rectTransform.rect.width;
    public float Height => rectTransform.rect.height;  
    public float PosX => rectTransform.localPosition.x;

    public int StageNumber
    {
        get { return stageNumber; }
        set { stageNumber = value; }
    }
    public JsonStage StageData
    {
        get { return stageData; }
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        btn_onlick = GetComponent<Button>();
        btn_onlick.onClick.AddListener(OnClickSlot);

    }

    public void InitStageSlot(JsonStage stage)
    {
        stageData = stage;
        stageNumber = stage.StageID;
        stageHeaderText.text = $"{stage.ChapterNumber} - {stage.StageID % (10 * stage.ChapterNumber)}";
    }

    public void SetRectPosition(float x, float y)
    {
        rectTransform.anchoredPosition = new Vector2(x, y);
    }

    public void OnClickSlot()
    {
        UIManager.LobbySelectStage.OnSelectSlot?.Invoke(this);
    }
}
