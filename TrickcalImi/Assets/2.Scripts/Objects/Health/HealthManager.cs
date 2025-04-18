using System;
using UnityEngine;

public class HealthManager
{
    private float currentHP;
    private float maxHP;
    private float currentMP;
    private float maxMP;

    private Action<HealthType, float, float> OnUpdateHealth; //type, current, max

    /*@tk : 나중에 Health Data로 전달하게 하기*/
    public HealthManager(float maxHp, float maxMp)
    {
        currentHP = maxHp;
        maxHP = maxMp;
        currentMP = maxMp;
        maxMP = maxMp;
    }

    public void ResetHealthManager()
    {
        OnUpdateHealth = null;
    }

    public void RegisterOnUpdateHealth(Action<HealthType, float, float> action)
    {
        OnUpdateHealth += action;
    }

    public void OnDecreasedHealth(HealthType type, float amount)
    {
        switch (type)
        {
            case HealthType.HP:
                currentHP = Mathf.Min(0f, currentHP - amount);
                OnUpdateHealth?.Invoke(type, currentHP, maxHP);
                break;
            case HealthType.MP:
                currentMP = Mathf.Min(0f, currentMP - amount);
                OnUpdateHealth?.Invoke(type, currentMP, maxMP);
                break;
            default:
                break;
        }

    }
    public void OnIncreasedHealth(HealthType type, float amount)
    {
        switch (type)
        {
            case HealthType.HP:
                currentHP = Mathf.Max(currentHP + amount, maxHP);
                OnUpdateHealth?.Invoke(type, currentHP, maxHP);
                break;
            case HealthType.MP:
                currentMP = Mathf.Max(currentMP + amount, maxMP);
                OnUpdateHealth?.Invoke(type, currentMP, maxMP);
                break;
            default:
                break;
        }
    }

    public bool IsDead()
    {
        if(currentHP <= 0)
        {
            return true;
        }

        return false;
    }
}
