using System;
using UnityEngine;

public class StageManager : MonoSingleton<StageManager>
{

    //@tk (25.04.09) 지금은 프로토타입 제작 때문에, 데이터를 강제로 주입했지만 이후로는 챕터로 부터 받아오기
    #region Test
    [Header("Test")]
    public int testEnemyCount = 5;
    #endregion


    private IngameModeType currentMode;
    //@tk 이거 나중에 챕터로부터 받기
    private int currentWaveCount = 0; 
    private int totalWaveCount = 5;
    private float setupLimitTime = 60.0f;
    private float combatLimitTime = 120.0f;

    private StageSpawnArea spawnArea; 
    
    public Action OnSetupAction; //card setup
    public Action OnCombatAction; //wave


    public int CurrentWaveCount => currentWaveCount;
    public int TotalWaveCount => totalWaveCount;
    public float SetupLimitTime => setupLimitTime;
    public float CombatLimitTime => combatLimitTime;


    protected override void Awake()
    {
        base.Awake();
        spawnArea = GetComponentInChildren<StageSpawnArea>();
    }

    private void Start()
    {
        OnSetupMode();
    }

    public void OnSetupMode()
    {
        if (currentMode == IngameModeType.Setup)
        {
            return;
        }

        currentMode = IngameModeType.Setup;
        currentWaveCount = Mathf.Min(totalWaveCount, currentWaveCount++);

        OnSetupAction?.Invoke();
    }

    public void OnCombatMode()
    {
        if(currentMode == IngameModeType.Combat)
        {
            return;
        }

        currentMode = IngameModeType.Combat;

        OnCombatAction?.Invoke();

        //알아서 몬스터 생성?
        for (int i = 0; i < testEnemyCount; i++)
        {
            Vector3 worldPos = spawnArea.GetRandomPosByArea();
            PoolManager.Instance.SpawnObject("TestEnemy", worldPos);
        }
    }

    
}
