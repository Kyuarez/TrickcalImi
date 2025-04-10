using UnityEngine;
using System.Collections;

/* [25.04.10]
 ������ Ÿ������ �ĺ��� �� �켱 ���� ���� ���� �ָ� Ÿ�����ϱ�
-> ��� ��� : �Ÿ�, ��׷�(���赵) �� ���� ������ Ÿ����
 */
public class HeroManager : MonoBehaviour
{
    [SerializeField] private SpriteRenderer shadowSpr;

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

        //@TODO : tk �̰� ���� ������ ���� ����
        spr.sortingOrder = Define.OrderLayer_HeroSecond;
        shadowSpr.sortingOrder = Define.OrderLayer_HeroShadow;
    }

    private void Start()
    {
        StageManager.Instance.OnSetupAction += OnSetupAction;
        StageManager.Instance.OnCombatAction += OnCombatAction;
    }

    public void OnSetupAction()
    {
        //TODO : �̰� �� ��ġ �����ؾ� ��.
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
