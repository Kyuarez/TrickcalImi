using UnityEngine;

public class AttackManager
{
    protected float normalDamage;
    protected float normalAttackDelay;

    public AttackManager()
    {
        this.normalDamage = Define.Default_Enemy_NormalDamage;
    }

    public AttackManager(float normalDamage, float normalAttackDelay)
    {
        this.normalDamage = normalDamage;
        this.normalAttackDelay = 
        this.normalAttackDelay = normalAttackDelay;
    }

    //@tk : 教臂 鸥百俊 措茄 傍拜
    public virtual void OnNormalAttack(IngameObject target)
    {
        if(target == null)
        {
            return;
        }

        //@TK FX 贸府
        //@TK UI 贸府
        target.Damage(normalDamage);
    }

    
}
