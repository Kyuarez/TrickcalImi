using UnityEngine;
using UnityEngine.UI;

public class UIHealth : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Slider mpSlider;
    [SerializeField] private Image hpFillImage;
    [SerializeField] private Image mpFillImage;

    public void InitUIHealth(Transform owner, bool isHero = true)
    {
        hpFillImage.color = (isHero == true) ? Define.Color_UI_HP_Hero : Define.Color_UI_HP_Monster;
        mpFillImage.color = Define.Color_UI_MP;
    }
}
