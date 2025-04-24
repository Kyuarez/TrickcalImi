using UnityEngine;

public class AttackManager
{
    protected AttackType attackType;
    protected float normalDamage;
    protected float normalAttackDelay;

    public AttackManager()
    {
        this.normalDamage = Define.Default_Enemy_NormalDamage;
    }

    public AttackManager(AttackType attackType ,float normalDamage, float normalAttackDelay)
    {
        this.attackType = attackType;
        this.normalDamage = normalDamage;
        this.normalAttackDelay = normalAttackDelay;
    }

    //@tk : ½Ì±Û Å¸°Ù¿¡ ´ëÇÑ °ø°Ý
    public virtual void OnNormalAttack(IngameObject target)
    {
        if(target == null)
        {
            return;
        }

        FXManager.Instance.OnEffect(FXType.Hit_Red, target.transform.position);
        UIIngameManager.Instance.OnDamagePopup(normalDamage, target.Mark_Star);
        target.Damage(normalDamage);
    }

    
}
