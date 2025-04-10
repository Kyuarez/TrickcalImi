using UnityEngine;
using System.Collections;

public class HeroManager : MonoBehaviour
{
    private float moveSpeed = 2.0f;
    private float chaseSpeed = 5.0f;

    private bool isAutoBattle;

    private Animator anim;
    private SpriteRenderer spr;

    private HeroState currentState;
    private Coroutine stateCoroutine;

    private Transform currentTarget;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        spr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        StageManager.Instance.OnSetupAction += OnSetupAction;
        StageManager.Instance.OnCombatAction += OnCombatAction;
    }

    public void OnSetupAction()
    {
        //TODO : 이거 원 위치 복구해야 함.
        SetEnemyState(HeroState.Idle);
    }

    public void OnCombatAction()
    {
        SetEnemyState(HeroState.Walk);
    }

    public void SetEnemyState(HeroState heroState)
    {
        if (currentState == heroState)
        {
            return;
        }

        currentState = heroState;



        if (stateCoroutine != null)
        {
            StopCoroutine(stateCoroutine);
            stateCoroutine = null;
        }

        if (currentState == HeroState.None)
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
            transform.position += Vector3.right * moveSpeed * Time.deltaTime;
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
