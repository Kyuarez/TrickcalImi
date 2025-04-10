using System;
using UnityEngine;

public class StageManager : MonoSingleton<StageManager>
{

    //@tk (25.04.09) ������ ������Ÿ�� ���� ������, �����͸� ������ ���������� ���ķδ� é�ͷ� ���� �޾ƿ���
    #region Test
    [Header("Test")]
    public int testEnemyCount = 5;
    #endregion


    private IngameModeType currentMode;
    //@tk �̰� ���߿� é�ͷκ��� �ޱ�
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

        //�˾Ƽ� ���� ����?
        for (int i = 0; i < testEnemyCount; i++)
        {
            Vector3 worldPos = spawnArea.GetRandomPosByArea();
            PoolManager.Instance.SpawnObject("TestEnemy", worldPos);
        }
    }

    
}
