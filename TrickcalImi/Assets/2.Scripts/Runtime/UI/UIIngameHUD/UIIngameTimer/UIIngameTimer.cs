using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIIngameTimer : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Image timerSliderImage;
    [SerializeField] private TextMeshProUGUI timerText;

    //public void OnSetupUIIngameTimer()
    //{
    //    float sec = StageManager.Instance.SetupLimitTime;
    //    timerText.text = $"{(int)sec / 60}:{(int)sec % 60}";
    //    timerSliderImage.fillAmount = 1.0f;
    //}

    //public void OnCombatUIIngameTimer()
    //{
    //    float sec = StageManager.Instance.CombatLimitTime;
    //    timerText.text = $"{(int)sec / 60}:{(int)sec % 60}";
    //    timerSliderImage.fillAmount = 1.0f;
    //}

    public void UpdateUIIngameTimer(float remainingTime, float limitTime)
    {
        timerText.text = $"{(int)remainingTime / 60:D2}:{(int)remainingTime % 60:D2}";
        timerSliderImage.fillAmount = remainingTime / limitTime;
    }
}
