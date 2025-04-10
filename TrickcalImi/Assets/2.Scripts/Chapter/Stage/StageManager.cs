using System;
using UnityEngine;

public class StageManager : MonoSingleton<StageManager>
{

    //@tk (25.04.09) 지금은 프로토타입 제작 때문에, 데이터를 강제로 주입했지만 이후로는 챕터로 부터 받아오기
    #region Test
    [Header("Test")]
    public int testEnemyCount = 5;
    #endregion

    [Header("Elements")]
    [SerializeField] private SpriteRenderer backgroundSpr;


    private IngameModeType currentMode;
    //@tk 이거 나중에 챕터로부터 받기
    private int currentWaveCount = 0; 
    private int totalWaveCount = 5;
    private float setupLimitTime = 60.0f;
    private float combatLimitTime = 120.0f;

    private StageSpawnArea spawnArea;
    private LocalTimer timer;

    public Action OnSetupAction; //card setup
    public Action OnCombatAction; //wave
    public Action<float, float> OnTickAction; //local Timer


    public int CurrentWaveCount => currentWaveCount;
    public int TotalWaveCount => totalWaveCount;
    public float SetupLimitTime => setupLimitTime;
    public float CombatLimitTime => combatLimitTime;


    protected override void Awake()
    {
        base.Awake();
        backgroundSpr.sortingOrder = Define.OrderLayer_background;

        spawnArea = GetComponentInChildren<StageSpawnArea>();
        
        timer = new LocalTimer();
    }

    private void Start()
    {
        OnSetupMode();
    }

    private void Update()
    {
        if(currentMode == IngameModeType.Setup || currentMode == IngameModeType.Combat)
        {
            timer?.UpdateTimer(Time.deltaTime);
            
        }   
    }

    public void OnSetupMode()
    {
        if (currentMode == IngameModeType.Setup)
        {
            return;
        }

        currentMode = IngameModeType.Setup;
        currentWaveCount = Mathf.Min(totalWaveCount, currentWaveCount++);

        timer.OnTick = null;
        timer.OnTick += UpdateOnTick;

        timer.OnTimeOver = null;
        timer.OnTimeOver += OnCombatMode;
        timer.OnTimer(setupLimitTime);

        OnSetupAction?.Invoke();
    }

    public void OnCombatMode()
    {
        if(currentMode == IngameModeType.Combat)
        {
            return;
        }

        currentMode = IngameModeType.Combat;
        
        timer.OnTick = null;
        timer.OnTick += UpdateOnTick;
        timer.OnTimeOver = null;
        timer.OnTimeOver += OnFailureMode;
        timer.OnTimer(combatLimitTime);
        
        OnCombatAction?.Invoke();

        //알아서 몬스터 생성?
        for (int i = 0; i < testEnemyCount; i++)
        {
            Vector3 worldPos = spawnArea.GetRandomPosByArea();
            PoolManager.Instance.SpawnObject("TestEnemy", worldPos);
        }
    }

    public void OnSuccessMode()
    {
        if(currentMode == IngameModeType.Success)
        {
            return;
        }

        currentMode = IngameModeType.Success;
    }

    public void OnFailureMode()
    {
        if (currentMode == IngameModeType.Failure)
        {
            return;
        }

        currentMode = IngameModeType.Failure;
    }

    public void UpdateOnTick()
    {
        if(currentMode == IngameModeType.Setup)
        {
            OnTickAction?.Invoke(timer.GetRemainingTime(), SetupLimitTime);
        }
        else if(currentMode == IngameModeType.Combat)
        {
            OnTickAction?.Invoke(timer.GetRemainingTime(), combatLimitTime);
        }

        return;
    }
}
