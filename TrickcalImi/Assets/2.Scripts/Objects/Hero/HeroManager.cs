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

    private bool isAutoBattle;

    private Animator anim;
    private SpriteRenderer spr;

    private HeroState currentState;
    private Coroutine stateCoroutine;
    private StateManager<HeroManager> stateManager;
    private State<HeroManager>[] states;

    private Transform currentTarget;
    private float distanceToEnemy;


    private void Awake()
    {
        anim = GetComponent<Animator>();
        spr = GetComponent<SpriteRenderer>();

        //@TODO : tk �̰� ���� ������ ���� ����
        spr.sortingOrder = Define.OrderLayer_HeroSecond;
        shadowSpr.sortingOrder = Define.OrderLayer_HeroShadow;

        stateManager = new StateManager<HeroManager>();
        states = new State<HeroManager>[]
        {
            new HeroIdleState(),
            new HeroWalkState(),
            new HeroChaseState(),
            new HeroAttackState(),
            new HeroHitState(),
        };

        currentState = HeroState.Idle;
        stateManager.Setup(this, states[(int)EnemyState.Idle]);
    }

    private void OnEnable()
    {
        SetHeroState(HeroState.Idle);
    }

    private void Start()
    {
        StageManager.Instance.OnSetupAction += OnSetupAction;
        StageManager.Instance.OnCombatAction += OnCombatAction;
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
        //TODO : �̰� �� ��ġ �����ؾ� ��.
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


    public void PlayAnim(HeroState state)
    {
        anim.Play(state.ToString());
    }

    public void TestMove()
    {
        transform.position += Vector3.right * moveSpeed * Time.deltaTime;
    }
}
