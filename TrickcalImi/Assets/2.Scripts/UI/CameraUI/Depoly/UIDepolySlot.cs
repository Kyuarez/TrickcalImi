using UnityEngine;

public class UIDepolySlot : MonoBehaviour
{
    [SerializeField] private HeroManager currentDepolyHero;
    
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

    public void OnDepoly(HeroManager hero)
    {
        if(IsDepoly == true)
        {
            return;
        }

        currentDepolyHero = hero;
        
    }

    
}
