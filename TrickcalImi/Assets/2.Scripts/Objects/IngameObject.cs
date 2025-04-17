using UnityEngine;
using FSM;

/// <summary>
/// Stage �ΰ��ӿ� �����ϴ� ������Ʈ(StageManager ���� ����)
/// </summary>
public class IngameObject : MonoBehaviour
{
    protected SpriteRenderer spr;
    protected Animator anim;

    protected HealthManager healthManager;
    
    
    public HealthManager HealthManager => healthManager;


    protected virtual void Awake()
    {
        spr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }
    protected virtual void OnEnable()
    {

    }
    protected virtual void Start()
    {

    }
    protected virtual void OnDisable()
    {

    }

}
