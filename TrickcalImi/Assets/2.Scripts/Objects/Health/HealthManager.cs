using System;
using UnityEngine;

public class HealthManager
{
    private float currentHP;
    private float maxHP;
    private float currentMP;
    private float maxMP;

    public float CurrentHP => currentHP;
    public float CurrentMP => currentMP;

    private Action<HealthType> OnDecreased;
    private Action<HealthType> OnIncreased;

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
        OnIncreased = null;
        OnDecreased = null;
    }

    public void RegisterOnDecreased(Action<HealthType> action)
    {
        OnDecreased += OnDecreased;
    }
    public void RegisterOnIncreased(Action<HealthType> action)
    {
        OnIncreased += action;
    }

    public void OnDecreasedHealth(HealthType type, float amount)
    {
        switch (type)
        {
            case HealthType.HP:
                currentHP = Mathf.Min(0f, currentHP - amount);
                break;
            case HealthType.MP:
                currentMP = Mathf.Min(0f, currentMP - amount);
                break;
            default:
                break;
        }

        OnDecreased?.Invoke(type);
    }
    public void OnIncreasedHealth(HealthType type, float amount)
    {
        switch (type)
        {
            case HealthType.HP:
                currentHP = Mathf.Max(currentHP + amount, maxHP);
                break;
            case HealthType.MP:
                currentMP = Mathf.Max(currentMP + amount, maxMP);
                break;
            default:
                break;
        }

        OnIncreased?.Invoke(type);
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
