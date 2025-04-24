using UnityEngine;
using FSM;
using System;

/// <summary>
/// Stage 인게임에 존재하는 오브젝트(StageManager 에서 관리)
/// </summary>
public class IngameObject : MonoBehaviour
{
    [Header("IngameObject ID")]
    [SerializeField] protected int objectID;

    protected string poolPath;
    protected int DefaultHP;
    protected int DefaultMP;
    protected float MoveSpeed;
    protected float ChaseSpeed;
    protected float TrackingRange;
    protected int NormalDamage;
    protected float NormalAttackDelay;
    protected float NormalAttackRange;

    protected int soundWeaponID; //@TK 이거 나중에 옮겨야함.

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
    public int SoundWeaponID => soundWeaponID;
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
    public string PoolPath => poolPath;

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
        poolPath = data.PoolPath;
        DefaultHP = data.HP;
        DefaultMP = data.MP;
        MoveSpeed = data.MoveSpeed;
        ChaseSpeed = data.ChaseSpeed;
        TrackingRange = data.TrackingRange;
        NormalDamage = data.NormalDamage;
        NormalAttackDelay = data.NormalAttackDelay;
        NormalAttackRange = data.NormalAttackRange;
        attackType = data.AttackType;
        soundWeaponID = data.WeaponSoundID;
    }


    public virtual void Damage(float amount)
    {
        healthManager.OnDecreasedHealth(HealthType.HP, amount);
    }
    public virtual void Heal(float amount)
    {
        healthManager.OnIncreasedHealth(HealthType.HP, amount);
        SoundManager.Instance.PlaySFX(30000);
        FXManager.Instance.OnEffect(FXType.Heal_HP, transform.position);
    }
    public virtual void HealRatio(float ratio)
    {
        healthManager.OnIncreasedHealthRatio(HealthType.HP, ratio);
        SoundManager.Instance.PlaySFX(30000);
        FXManager.Instance.OnEffect(FXType.Heal_HP, transform.position);
    }
    public virtual void HealMP(float amount)
    {
        healthManager.OnIncreasedHealth(HealthType.MP, amount);
        SoundManager.Instance.PlaySFX(30000);
        FXManager.Instance.OnEffect(FXType.Heal_MP, transform.position);
    }
    public virtual void HealMPRatio(float ratio)
    {
        healthManager.OnIncreasedHealthRatio(HealthType.MP, ratio);
        SoundManager.Instance.PlaySFX(30000);
        FXManager.Instance.OnEffect(FXType.Heal_MP, transform.position);
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
