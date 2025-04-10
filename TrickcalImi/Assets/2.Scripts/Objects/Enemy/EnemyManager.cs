using System.Collections;
using UnityEngine;

/* [25.04.10]
 ���� Ÿ������ ������ �Ÿ� ����� ������ ���
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

    private Transform currentTarget;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        spr = GetComponent<SpriteRenderer>();

        spr.sortingOrder = Define.OrderLayer_HeroFirst;
        shadowSpr.sortingOrder = Define.OrderLayer_HeroShadow;
    }

    private void OnEnable()
    {
        SetEnemyState(EnemyState.Walk);
    }

    private void OnDisable()
    {
        SetEnemyState(EnemyState.None);
        stateCoroutine = null;
    }

    private void Update()
    {
        
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
        yield return null;
    }
    private IEnumerator Walk()
    {
        anim.Play("Walk");
        
        while (true) 
        {
            transform.position += Vector3.left * moveSpeed * Time.deltaTime;
            yield return null;
        }
    }
    private IEnumerator Chase()
    {
        yield return null;
    }
    private IEnumerator Attack()
    {
        yield return null;
    }

    private IEnumerator Hit()
    {
        yield return null;
    }
    private IEnumerator Die()
    {
        yield return null;
    }

}
