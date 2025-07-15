using TMPro;
using UnityEngine;

public class UICostCountHUD : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI costCountText;

    public void OnSetupAction(int costAmount)
    {
        costCountText.text = costAmount.ToString();
    }

    public void OnUpdateCostCount(int costCount)
    {
        costCountText.text = costCount.ToString();
    }
}
