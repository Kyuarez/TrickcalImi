using UnityEngine;
using UnityEngine.UI;

public class UIHealth : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Slider mpSlider;
    [SerializeField] private Image hpFillImage;
    [SerializeField] private Image mpFillImage;

    public void SetUIHealth(IngameObject owner, bool isHero = true)
    {
        owner.HealthManager.RegisterOnUpdateHealth(OnUpdateHealthUI);

        hpFillImage.color = (isHero == true) ? Define.Color_UI_HP_Hero : Define.Color_UI_HP_Monster;
        mpFillImage.color = Define.Color_UI_MP;
    }

    public void ResetUIHealth()
    {
        hpSlider.value = 1f;
        mpSlider.value = 1f;
    }
    

    public void OnUpdateHealthUI(HealthType type, float currentHealth, float maxHealth)
    {
        switch (type)
        {
            case HealthType.HP:
                hpSlider.value = (currentHealth / maxHealth);
                break;
            case HealthType.MP:
                mpSlider.value = (currentHealth / maxHealth);
                break;
        }
    }
}
