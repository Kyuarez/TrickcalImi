using UnityEngine;
using System.Collections;
using FSM;

/* [25.04.10]
 ������ Ÿ������ �ĺ��� �� �켱 ���� ���� ���� �ָ� Ÿ�����ϱ�
-> ��� ��� : �Ÿ�, ��׷�(���赵) �� ���� ������ Ÿ����
 */
public class HeroManager : IngameObject
{
    [SerializeField] private SpriteRenderer shadowSpr;

    private float moveSpeed = 2.0f;
    private float chaseSpeed = 5.0f;

    private HeroState currentState;
    private StateManager<HeroManager> stateManager;
    private State<HeroManager>[] states;

    private EnemyManager currentTarget;
    private float distanceToTarget;
    private float trackingRange = 10.0f;
    private float attackRange = 2.5f;
    private float attackDelay = 0.75f;

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
    public EnemyManager CurrentTarget => currentTarget;
    public float AttackDelay => this.attackDelay;

    protected override void Awake()
    {
        base.Awake();

        //@TODO : tk �̰� ���� ������ ���� ����
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

        OnDead += OnDeadAction;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        //@tk : �ӽ� ������, ���߿� json���� ó��
        healthManager = new HealthManager(100f, 100f);
        stateManager = new StateManager<HeroManager>();
        attackManager = new AttackManager(20f);

        currentState = HeroState.Idle;
        stateManager.Setup(this, states[(int)HeroState.Idle]);

        StageManager.Instance.OnSetupAction += OnSetupAction;
        StageManager.Instance.OnCombatAction += OnCombatAction;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        StageManager.Instance.OnSetupAction -= OnSetupAction;
        StageManager.Instance.OnCombatAction -= OnCombatAction;

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
        if (StageManager.Instance.IsPossibleGetTarget() == false)
        {
            return;
        }

        if (currentTarget == null)
        {
            currentTarget = StageManager.Instance.GetNearestEnemy(transform.position);
        }

        if (currentTarget != null)
        {
            distanceToTarget = Vector2.Distance(transform.position, currentTarget.transform.position);
        }
    }

    public void Updated()
    {
        if(stateManager != null)
        {
            if (IsDead != true)
            {
                stateManager.Excute();
            }
        }
    }

    public void OnSetupAction()
    {
        //TODO : �̰� �� ��ġ �����ؾ� ��.
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
        Destroy(gameObject, 1f);
        //PoolManager.Instance.DespawnObject("TestEnemy", gameObject);
    }
}
