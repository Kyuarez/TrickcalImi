using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIStageSlot : MonoBehaviour
{
    private int stageNumber;
    private Button btn_onlick;
    private Image lockImage;
    private TextMeshProUGUI stageHeaderText;
    private RectTransform rectTransform;

    public float Width => rectTransform.rect.width;
    public float Height => rectTransform.rect.height;  
    public float PosX => rectTransform.localPosition.x;

    public int StageNumber
    {
        get { return stageNumber; }
        set { stageNumber = value; }
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        btn_onlick = GetComponent<Button>();
        btn_onlick.onClick.AddListener(OnClickSlot);
    }

    public void InitStageSlot(/*StageData*/)
    {
        
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
