using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIStageSlot : MonoBehaviour
{
    private Button btn_onlick;
    private Image lockImage;
    private TextMeshProUGUI stageHeaderText;


    public void InitStageSlot(/*StageData*/)
    {
        btn_onlick = GetComponent<Button>();
        btn_onlick.onClick.AddListener(OnClickSlot);
    }

    public void OnClickSlot()
    {

    }
}
