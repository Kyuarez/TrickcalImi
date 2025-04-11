using System.Collections;
using UnityEditor;
using UnityEngine;

/* [25.04.10]
 적의 타겟팅은 무조건 거리 가까운 순으로 잡기
 */
public class EnemyManager : MonoBehaviour
{
    [SerializeField] private SpriteRenderer shadowSpr;

    private float moveSpeed = 2.0f;
    private float chaseSpeed = 5.0f;

    private Animator anim;
    private SpriteRenderer spr;
    

    private EnemyState currentState;
    private Coroutine stateCoroutine;

    private HeroManager currentTarget;
    private float distanceToTarget = -1f;
    private float trackingRange = 10.0f;
    private float attackRange = 2.5f;
    private float attackDelay = 0.75f;
    

    private bool isFirstCall = true; //pool에서 OnEnable 막을려고 만든 코드

    private void Awake()
    {
        anim = GetComponent<Animator>();
        spr = GetComponent<SpriteRenderer>();

        spr.sortingOrder = Define.OrderLayer_HeroFirst;
        shadowSpr.sortingOrder = Define.OrderLayer_HeroShadow;
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
            SetEnemyState(EnemyState.Idle);
        }

    }

    private void OnDisable()
    {
        SetEnemyState(EnemyState.None);
        stateCoroutine = null;
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

    public void SetEnemyState(EnemyState enemyState)
    {
        if(currentState == enemyState)
        {
            return;
        }

        currentState = enemyState;



        if(stateCoroutine != null)
        {
            StopCoroutine(stateCoroutine);
            stateCoroutine = null;
        }

        if(currentState == EnemyState.None)
        {
            return;
        }

        stateCoroutine = StartCoroutine(currentState.ToString());
    }

    private IEnumerator Idle()
    {
        anim.Play("Idle");
        while (true)
        {
            if (distanceToTarget <= trackingRange)
            {
                if (currentTarget != null && distanceToTarget <= attackRange)
                {
                    SetEnemyState(EnemyState.Attack);
                    yield break;
                }

                SetEnemyState(EnemyState.Chase);
                yield break;
            }
            else
            {
                SetEnemyState(EnemyState.Walk);
                yield break;
            }

            yield return null;
        }
    }
    private IEnumerator Walk()
    {
        anim.Play("Walk");
        
        while (true) 
        {
            if (distanceToTarget <= trackingRange)
            {
                if (currentTarget != null && distanceToTarget <= attackRange)
                {
                    SetEnemyState(EnemyState.Attack);
                    yield break;
                }

                SetEnemyState(EnemyState.Chase);
                yield break;
            }

            transform.position += Vector3.left * moveSpeed * Time.deltaTime;
            yield return null;
        }
    }
    private IEnumerator Chase()
    {
        anim.Play("Run");
        while (true)
        {
            if(currentTarget != null && distanceToTarget <= trackingRange)
            {
                if (currentTarget != null && distanceToTarget <= attackRange)
                {
                    SetEnemyState(EnemyState.Attack);
                    yield break;
                }

                Vector3 currentPosition = transform.position;
                Vector3 targetPosition = new Vector3(currentTarget.transform.position.x, currentTarget.transform.position.y, currentPosition.z);
                transform.position = Vector3.Lerp(currentPosition, targetPosition, moveSpeed * Time.deltaTime);
            }
            else
            {
                SetEnemyState(EnemyState.Walk);
                yield break;
            }

            yield return null;
        }
    }
    private IEnumerator Attack()
    {
        anim.Play("Attack");
        yield return new WaitForSeconds(attackDelay);
        SetEnemyState(EnemyState.Idle);
        yield return null;
    }

    private IEnumerator Hit()
    {
        anim.Play("Hit");
        yield return null;
    }
    private IEnumerator Die()
    {
        yield return null;
    }

}
