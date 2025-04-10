using UnityEngine;

public class UIDepolySlot : MonoBehaviour
{
    [SerializeField] private Transform spawnPos_Center; //pivot
    [SerializeField] private Transform spawnPos_Bottom;

    private HeroManager currentDepolyHero;
    
    public bool IsDepoly
    {
        get
        {
            if (currentDepolyHero == null)
                return false;
            else
                return true;
        }
    }

    public void OnDepoly(HeroManager hero, bool isPivotBottom = false)
    {
        if(IsDepoly == true)
        {
            return;
        }

        currentDepolyHero = hero;
        hero.transform.position = (isPivotBottom == true) ? spawnPos_Bottom.position : spawnPos_Center.position;
        hero.transform.rotation = Quaternion.identity;
    }

    
}
