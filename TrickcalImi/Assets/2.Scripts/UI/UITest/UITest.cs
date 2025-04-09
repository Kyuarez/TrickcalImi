using UnityEngine;

public class UITest : MonoBehaviour
{
    #region OnClick
    public void OnClickDeployHero()
    {
        GameObject obj = Resources.Load<GameObject>("Prefabs/Objects/Hero/TestHero");
        UIIngameManager.DepolySlotManager.OnDepolyHero(obj);
    }

    public void OnClickOnCombatMode() 
    {
        StageManager.Instance.OnCombatMode();
    }
    #endregion
}
