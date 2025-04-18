using TMPro;
using UnityEngine;

public class UIWaveHUD : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI waveText;

    public void UpdateWaveHUD()
    {
        waveText.text = $"{StageManager.Instance.CurrentWaveCount}/{StageManager.Instance.TotalWaveCount}";
    }
    public void ResetWaveHUD()
    {
        waveText.text = string.Empty;
    }
}
