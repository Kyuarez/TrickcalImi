using FSM;
using System.Collections;
using UnityEngine;

/* [25.04.10]
 적의 타겟팅은 무조건 거리 가까운 순으로 잡기
 */
public class EnemyManager : IngameObject
{
    [SerializeField] private SpriteRenderer shadowSpr;

    private float moveSpeed = 2.0f;
    private float chaseSpeed = 5.0f;

    private EnemyState currentState;
    private StateManager<EnemyManager> stateManager;
    private State<EnemyManager>[] states;

    private HeroManager currentTarget;
    private float distanceToTarget = -1f;
    private float trackingRange = 10.0f;
    private float attackRange = 2.5f;
    private float attackDelay = 0.75f;
    

    private bool isFirstCall = true; //pool에서 OnEnable 막을려고 만든 코드

    public bool IsPossibleChase
    {
        get { return distanceToTarget <= trackingRange; }
    }
    public bool IsPossibleAttack
    {
        get 
        { 
            if( currentTarget != null)
            {
                return distanceToTarget <= attackRange;
            }
            return false;
        } 
    }
    public HeroManager CurrentTarget => currentTarget;
    public float AttackDelay => this.attackDelay;

    protected override void Awake()
    {
        base.Awake();

        spr.sortingOrder = Define.OrderLayer_HeroFirst;
        shadowSpr.sortingOrder = Define.OrderLayer_HeroShadow;

        states = new State<EnemyManager>[]
        {
            new EnemyIdleState(),
            new EnemyWalkState(moveSpeed),
            new EnemyChaseState(chaseSpeed),
            new EnemyAttackState(),
            new EnemyHitState(),
            new EnemyDeadState(),
        };

    }

    protected override void OnEnable()
    {
        base.OnEnable();

        stateManager = new StateManager<EnemyManager>();
        healthManager = new HealthManager(100f, 100f);
        attackManager = new AttackManager();

        OnDead += OnDeadAction;

        if (isFirstCall == true)
        {
            isFirstCall = false;
        }
        else
        {
            currentState = EnemyState.Idle;
            stateManager.Setup(this, states[(int)EnemyState.Idle]);
        }

    }

    protected override void Start()
    {
        base.Start();

    }

    protected override void OnDisable()
    {
        base.OnDisable();

        healthManager.ResetHealthManager();
        healthManager = null;

        attackManager = null;

        OnDead -= OnDeadAction;

        ResetEnemyState();

        currentTarget = null;
        distanceToTarget = -1f;
    }

    private void Update()
    {
        if(StageManager.Instance.IsPossibleGetTarget() == false)
        {
            return;
        }

        if(currentTarget == null)
        {
            currentTarget = StageManager.Instance.GetNearestHero(transform.position);
        }

        if(currentTarget != null)
        {
            distanceToTarget = Vector2.Distance(transform.position, currentTarget.transform.position);
        }             
    }

    public void Updated()
    {
        if (stateManager != null)
        {
            if (IsDead != true)
            {
                stateManager.Excute();
            }
        }
    }

    public void SetEnemyState(EnemyState enemyState)
    {
        if(currentState == enemyState)
        {
            return;
        }

        currentState = enemyState;
        stateManager.ChangeState(states[(int)currentState]);
    }
    public void ResetEnemyState()
    {
        currentState = EnemyState.None;
        stateManager = null;
    }

    public void PlayAnim(EnemyState state)
    {
        anim.Play(state.ToString());
    }

    public void OnDeadAction()
    {
        PoolManager.Instance.DespawnObject("TestEnemy", gameObject);
    }
}
