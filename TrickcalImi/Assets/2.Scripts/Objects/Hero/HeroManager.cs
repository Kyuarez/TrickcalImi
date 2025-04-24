using UnityEngine;
using System.Collections;
using FSM;

/* [25.04.10]
 영웅은 타겟팅을 후보군 중 우선 순위 점수 높은 애를 타겟팅하기
-> 고려 요소 : 거리, 어그로(위험도) 등 종합 점수로 타겟팅
 */
public class HeroManager : IngameObject
{
    [SerializeField] private SpriteRenderer shadowSpr;

    private float moveSpeed = 2.0f;
    private float chaseSpeed = 5.0f;

    private HeroState currentState;
    private StateManager<HeroManager> stateManager;
    private State<HeroManager>[] states;

    private Vector3 destination;
    private EnemyManager currentTarget;
    private float distanceToDestination;
    private float distanceToTarget;
    private float trackingRange = 10.0f;
    private float attackRange = 2.5f;
    private float attackDelay = 0.75f;

    public Vector3 Destination => destination;
    public bool IsPossibleChase
    {
        get { return distanceToTarget <= trackingRange; }
    }
    public bool IsPossibleAttack
    {
        get
        {
            if (currentTarget != null)
            {
                return distanceToTarget <= attackRange;
            }
            return false;
        }
    }
    public bool IsReachedDestination
    {
        get
        {
            if (destination != null)
            {
                return distanceToDestination <= 0.1f;
            }
            return true;
        }
    }
    public EnemyManager CurrentTarget => currentTarget;
    public float AttackDelay => this.attackDelay;

    protected override void Awake()
    {
        base.Awake();

        //@TODO : tk 이거 이제 열마다 따로 세팅
        spr.sortingOrder = Define.OrderLayer_HeroSecond;
        shadowSpr.sortingOrder = Define.OrderLayer_HeroShadow;

        states = new State<HeroManager>[]
        {
            new HeroIdleState(),
            new HeroWalkState(moveSpeed),
            new HeroChaseState(chaseSpeed),
            new HeroAttackState(),
            new HeroHitState(),
            new HeroDeadState(),
        };
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        //@tk : 임시 데이터, 나중에 json으로 처리
        healthManager = new HealthManager(DefaultHP, DefaultMP);
        stateManager = new StateManager<HeroManager>();
        attackManager = new AttackManager(attackType, NormalDamage, NormalAttackDelay);

        currentState = HeroState.Idle;
        stateManager.Setup(this, states[(int)HeroState.Idle]);

        if (StageManager.Instance != null)
        {
            StageManager.Instance.OnSetupAction += OnSetupAction;
            StageManager.Instance.OnCombatAction += OnCombatAction;
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        if (StageManager.Instance != null)
        {
            StageManager.Instance.OnSetupAction -= OnSetupAction;
            StageManager.Instance.OnCombatAction -= OnCombatAction;
        }

        healthManager.ResetHealthManager();
        healthManager = null;

        attackManager = null;

        ResetHeroState();
    }

    protected override void Start()
    {
        base.Start();
    }
    private void Update()
    {
        //Combat Mode시 적 체크
        if (StageManager.Instance.IsPossibleGetTarget() == true)
        {
            if (currentTarget == null || currentTarget.IsDead == true)
            {
                currentTarget = StageManager.Instance.GetNearestEnemy(transform.position);
            }

            if (currentTarget != null)
            {
                distanceToTarget = Vector2.Distance(transform.position, currentTarget.transform.position);
            }
        }
        else
        {
            if (destination != null)
            {
                distanceToDestination = Vector2.Distance(transform.position, destination);
            }
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

    public void OnSetupAction()
    {
        //TODO : 이거 원 위치 복구해야 함.
        SetHeroState(HeroState.Idle);
    }

    public void OnCombatAction()
    {
        SetHeroState(HeroState.Idle);
    }

    public void SetHeroState(HeroState heroState)
    {
        if (currentState == heroState)
        {
            return;
        }

        currentState = heroState;
        stateManager.ChangeState(states[(int)currentState]);
    }
    public void SetDestination(Vector3 pos)
    {
        destination = pos;
    }
    public void ResetTurn()
    {
        spr.flipX = true;
    }
    public void SetTurn(bool isTurn)
    {
        spr.flipX = isTurn;
    }

    public void ResetHeroState()
    {
        currentState = HeroState.None;
        stateManager = null;
    }


    public void PlayAnim(HeroState state)
    {
        anim.Play(state.ToString());
    }

    public void OnDeadAction()
    {
        PoolManager.Instance.DespawnObject("TestHero", gameObject);
    }
}
