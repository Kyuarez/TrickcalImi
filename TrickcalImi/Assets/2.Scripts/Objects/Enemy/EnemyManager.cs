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

    private Animator anim;
    private SpriteRenderer spr;
    

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
    public float AttackDelay => this.attackDelay;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        spr = GetComponent<SpriteRenderer>();

        spr.sortingOrder = Define.OrderLayer_HeroFirst;
        shadowSpr.sortingOrder = Define.OrderLayer_HeroShadow;

        stateManager = new StateManager<EnemyManager>();
        states = new State<EnemyManager>[]
        {
            new EnemyIdleState(),
            new EnemyWalkState(),
            new EnemyChaseState(),
            new EnemyAttackState(),
            new EnemyHitState(),
        };
    }

    private void OnEnable()
    {
        if(isFirstCall == true)
        {
            isFirstCall = false;
        }
        else
        {
            currentTarget = StageManager.Instance.GetRandomHeroInStage();
            currentState = EnemyState.Idle;
            stateManager.Setup(this, states[(int)EnemyState.Idle]);
        }

    }

    private void OnDisable()
    {
        SetEnemyState(EnemyState.Idle);
        currentTarget = null;
        distanceToTarget = -1f;
    }

    private void Update()
    {
        if(currentTarget != null)
        {
            distanceToTarget = Vector2.Distance(transform.position, currentTarget.transform.position);
        }             
    }

    public void Updated()
    {
        if (stateManager != null)
        {
            stateManager.Excute();
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

    public void TestMove()
    {
        transform.position += Vector3.left * moveSpeed * Time.deltaTime;
    }

    public void TestChase()
    {
        Vector3 currentPosition = transform.position;
        Vector3 targetPosition = new Vector3(currentTarget.transform.position.x, currentTarget.transform.position.y, currentPosition.z);
        transform.position = Vector3.Lerp(currentPosition, targetPosition, moveSpeed * Time.deltaTime);
    }

    public void PlayAnim(EnemyState state)
    {
        anim.Play(state.ToString());
    }
}
