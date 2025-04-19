using UnityEngine;

public class AttackManager
{
    protected float normalDamage;

    public AttackManager()
    {
        this.normalDamage = Define.Default_Enemy_NormalDamage;
    }

    public AttackManager(float normalDamage)
    {
        this.normalDamage = normalDamage;
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
