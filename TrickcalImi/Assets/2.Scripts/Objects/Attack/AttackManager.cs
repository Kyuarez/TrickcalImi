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

    //@tk : �̱� Ÿ�ٿ� ���� ����
    public virtual void OnNormalAttack(IngameObject target)
    {
        if(target == null)
        {
            return;
        }

        //@TK FX ó��
        //@TK UI ó��
        target.Damage(normalDamage);
    }

    
}
