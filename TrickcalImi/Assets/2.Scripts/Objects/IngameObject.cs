using UnityEngine;
using FSM;
using System;

/// <summary>
/// Stage 인게임에 존재하는 오브젝트(StageManager 에서 관리)
/// </summary>
public class IngameObject : MonoBehaviour
{
    protected Transform mark_Up;
    protected Transform mark_Health;

    protected SpriteRenderer spr;
    protected Animator anim;

    protected HealthManager healthManager;
    protected AttackManager attackManager;

    public HealthManager HealthManager => healthManager;
    public AttackManager AttackManager => attackManager;

    public Transform Mark_Star => mark_Up;
    public Transform Mark_Health => mark_Health;

    protected Action OnDead;

    protected virtual void Awake()
    {
        mark_Up = transform.FindRecursiveChild("@Mark_Up");
        mark_Health = transform.FindRecursiveChild("@Mark_Health");

        spr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }
    protected virtual void OnEnable()
    {

    }
    protected virtual void Start()
    {

    }
    protected virtual void OnDisable()
    {
        UnRegisterDeadAction();
    }


    protected virtual void Damage(float amount)
    {
        healthManager.OnDecreasedHealth(HealthType.HP, amount);
    }
    protected virtual void Heal(float amount)
    {
        healthManager.OnIncreasedHealth(HealthType.HP, amount);
    }
    protected virtual void HealMP(float amount)
    {
        healthManager.OnIncreasedHealth(HealthType.MP, amount);
    }
    protected virtual void UseMP(float amount)
    {
        healthManager.OnIncreasedHealth(HealthType.MP, amount);
    }

    public void OnRegisterDeadAction(Action action)
    {
        OnDead += action;
    }
    public void UnRegisterDeadAction()
    {
        OnDead = null;
    }
}
