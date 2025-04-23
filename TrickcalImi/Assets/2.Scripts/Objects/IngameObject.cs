using UnityEngine;
using FSM;
using System;
using static UnityEditorInternal.ReorderableList;

/// <summary>
/// Stage 인게임에 존재하는 오브젝트(StageManager 에서 관리)
/// </summary>
public class IngameObject : MonoBehaviour
{
    [Header("IngameObject ID")]
    [SerializeField] protected int objectID;

    protected int DefaultHP;
    protected int DefaultMP;
    protected int NormalDamage;
    protected float NormalAttackDelay;

    protected Transform mark_Up;
    protected Transform mark_Health;
    protected AttackType attackType;

    protected SpriteRenderer spr;
    protected Animator anim;

    protected HealthManager healthManager;
    protected AttackManager attackManager;

    public HealthManager HealthManager => healthManager;
    public AttackManager AttackManager => attackManager;

    public Transform Mark_Star => mark_Up;
    public Transform Mark_Health => mark_Health;
    public bool IsDead
    {
        get
        {
            if(healthManager == null)
            {
                //@tk : 죽을 때 healthManger null로 하니까 true
                return true;
            }

            return healthManager.IsDead();
        }
    }

    public Action OnDead;

    protected virtual void Awake()
    {
        mark_Up = transform.FindRecursiveChild("@Mark_Up");
        mark_Health = transform.FindRecursiveChild("@Mark_Health");

        spr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        //Data Init
        JsonIngameObject json = TableManager.Instance.FindTableData<JsonIngameObject>(objectID);
        if (json == null)
        {
            Debug.AssertFormat(false, $"Object ID({objectID} is wrong)");
            return;
        }

        InitIngameObjectData(json);
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

    protected virtual void InitIngameObjectData(JsonIngameObject data)
    {
        DefaultHP = data.HP;
        DefaultMP = data.MP;
        NormalDamage = data.NormalDamage;
        NormalAttackDelay = data.NormalAttackDelay;
        attackType = data.AttackType;
    }


    public virtual void Damage(float amount)
    {
        healthManager.OnDecreasedHealth(HealthType.HP, amount);
    }
    public virtual void Heal(float amount)
    {
        healthManager.OnIncreasedHealth(HealthType.HP, amount);
    }
    public virtual void HealMP(float amount)
    {
        healthManager.OnIncreasedHealth(HealthType.MP, amount);
    }
    public virtual void UseMP(float amount)
    {
        healthManager.OnIncreasedHealth(HealthType.MP, amount);
    }



    #region Action
    public void OnRegisterDeadAction(Action action)
    {
        OnDead += action;
    }
    public void UnRegisterDeadAction()
    {
        OnDead = null;
    }
    #endregion
}
