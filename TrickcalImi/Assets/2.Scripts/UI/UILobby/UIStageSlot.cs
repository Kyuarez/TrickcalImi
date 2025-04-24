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
        int uiStageNumber = stage.StageID % 10;
        stageHeaderText.text = $"{stage.ChapterNumber} - {(uiStageNumber == 0 ? 10 : uiStageNumber)}";

        //Lock ¿©ºÎ 
        bool isStageOpen = LocalDataManager.Instance.LocalUserData.ChapterOpenData[stage.ChapterNumber][(uiStageNumber == 0 ? 10 : uiStageNumber) - 1];
        SetLock(!isStageOpen);
    }

    private void SetLock(bool active)
    {
        if(lockImage.gameObject.activeSelf == !active)
        {
            lockImage.gameObject.SetActive(active);
        }

        btn_onlick.interactable = !active;
    }

    public void SetRectPosition(float x, float y)
    {
        rectTransform.anchoredPosition = new Vector2(x, y);
    }

    public void OnClickSlot()
    {
        SoundManager.Instance.PlaySFX(10001);
        UIManager.LobbySelectStage.OnSelectSlot?.Invoke(this);
    }
}
