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

    private bool isAutoBattle;

    private HeroState currentState;
    private StateManager<HeroManager> stateManager;
    private State<HeroManager>[] states;

    private EnemyManager currentTarget;
    private float distanceToTarget;

    protected override void Awake()
    {
        base.Awake();

        //@TODO : tk 이거 이제 열마다 따로 세팅
        spr.sortingOrder = Define.OrderLayer_HeroSecond;
        shadowSpr.sortingOrder = Define.OrderLayer_HeroShadow;

        states = new State<HeroManager>[]
        {
            new HeroIdleState(),
            new HeroWalkState(),
            new HeroChaseState(),
            new HeroAttackState(),
            new HeroHitState(),
        };

    }

    protected override void OnEnable()
    {
        base.OnEnable();

        //@tk : 임시 데이터, 나중에 json으로 처리
        healthManager = new HealthManager(100f, 100f);
        stateManager = new StateManager<HeroManager>();
        SetHeroState(HeroState.Idle);

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
            stateManager.Excute();
        }
    }

    public void OnSetupAction()
    {
        //TODO : 이거 원 위치 복구해야 함.
        SetHeroState(HeroState.Walk);
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

    public void TestMove()
    {
        transform.position += Vector3.right * moveSpeed * Time.deltaTime;
    }
}
