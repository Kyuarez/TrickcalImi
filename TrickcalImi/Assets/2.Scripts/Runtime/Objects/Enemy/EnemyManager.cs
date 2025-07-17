using FSM;
using System.Collections;
using TK.BT;
using UnityEngine;

/* [25.04.10]
 적의 타겟팅은 무조건 거리 가까운 순으로 잡기
 */
[RequireComponent(typeof(BT))]
public class EnemyManager : IngameObject
{
    [SerializeField] private SpriteRenderer shadowSpr;

    private BT _bt;

    private EnemyState currentState;
    private HeroManager currentTarget;
    private float distanceToTarget = -1f;

    private GameObject weaponObj;
    private bool isFirstCall = true; //pool에서 OnEnable 막을려고 만든 코드

    public bool IsPossibleChase
    {
        get { return distanceToTarget <= TrackingRange; }
    }
    public bool IsPossibleAttack
    {
        get 
        { 
            if( currentTarget != null)
            {
                return distanceToTarget <= NormalAttackRange;
            }
            return false;
        } 
    }
    public HeroManager CurrentTarget => currentTarget;
    public float AttackDelay => this.NormalAttackDelay;

    protected override void Awake()
    {
        base.Awake();
        weaponObj = transform.FindRecursiveChild(Define.Name_Hero_Weapon).gameObject;

        _bt = GetComponent<BT>();
        Builder builder = new Builder(_bt);
        builder.Root();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        healthManager = new HealthManager(DefaultHP, DefaultMP);
        attackManager = new AttackManager();

        OnDead += OnDeadAction;

        if (isFirstCall == true)
        {
            isFirstCall = false;
        }
        else
        {
            currentState = EnemyState.Idle;
        }

    }

    protected override void Start()
    {
        base.Start();

    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if(weaponObj.activeSelf == true)
        {
            weaponObj.SetActive(false);
        }


        healthManager.ResetHealthManager();
        healthManager = null;

        attackManager = null;

        OnDead -= OnDeadAction;

        //ResetEnemyState();

        currentTarget = null;
        distanceToTarget = -1f;

        
    }

    protected override void InitIngameObjectData(JsonIngameObject data)
    {
        base.InitIngameObjectData(data);
        
    }

    private void Update()
    {
        if(StageManager.Instance.IsPossibleGetTarget() == false)
        {
            return;
        }

        if(currentTarget == null || currentTarget.IsDead == true)
        {
            //currentTarget = StageManager.Instance.GetNearestHero(transform.position);
        }

        if(currentTarget != null)
        {
            distanceToTarget = Vector2.Distance(transform.position, currentTarget.transform.position);
        }             
    }

    public void Updated()
    {
        
    }

    public void SetEnemyState(EnemyState enemyState)
    {
        if(currentState == enemyState)
        {
            return;
        }

        currentState = enemyState;
    }
    public void ResetTurn()
    {
        spr.flipX = false;
    }
    public void SetTurn(bool isTurn)
    {
        spr.flipX = isTurn;
    }
    //public void ResetEnemyState()
    //{
    //    currentState = EnemyState.None;
    //    stateManager = null;
    //}

    public void PlayAnim(EnemyState state)
    {
        anim.Play(state.ToString());
    }

    public void OnDeadAction()
    {
        PoolManager.Instance.DespawnObject(poolPath, gameObject);
    }
}
