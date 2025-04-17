using UnityEngine;
using FSM;

/// <summary>
/// Stage 인게임에 존재하는 오브젝트(StageManager 에서 관리)
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
