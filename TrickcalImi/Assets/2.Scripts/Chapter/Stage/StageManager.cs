using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class StageManager : MonoSingleton<StageManager>
{

    //@tk (25.04.09) ������ ������Ÿ�� ���� ������, �����͸� ������ ���������� ���ķδ� é�ͷ� ���� �޾ƿ���
    #region Test
    [Header("Test")]
    public int testEnemyCount = 5;
    #endregion

    [Header("Elements")]
    [SerializeField] private SpriteRenderer backgroundSpr;


    private IngameModeType currentMode;
    //@tk �̰� ���߿� é�ͷκ��� �ޱ�
    private int currentWaveCount = 0; 
    private int totalWaveCount = 5;
    private float setupLimitTime = 60.0f;
    private float combatLimitTime = 70.0f;

    private StageSpawnArea spawnArea;
    private LocalTimer timer;

    public Action OnSetupAction; //card setup
    public Action OnCombatAction; //wave
    public Action OnSuccessAction;
    public Action OnFailureAction;
    public Action OnResetAction; //stage reset
    public Action<float, float> OnTickAction; //local Timer

    private Dictionary<int, HeroManager> currentHeros = new Dictionary<int, HeroManager>();
    private List<EnemyManager> currentEnemyList = new List<EnemyManager>();

    public IngameModeType CurrentMode => currentMode;
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

    private void Update()
    {
        if(currentMode == IngameModeType.Setup || currentMode == IngameModeType.Combat)
        {
            timer?.UpdateTimer(Time.deltaTime);            
        }   

        if(currentMode == IngameModeType.Combat)
        {
            if(currentHeros.Count == 0) //���� �� ���
            {
                OnFailureMode();
            }
        }

        if(currentHeros.Count > 0)
        {
            foreach (HeroManager hero in currentHeros.Values)
            {
                hero.Updated();
            }
        }

        if(currentEnemyList.Count > 0)
        {
            foreach (EnemyManager enemy in currentEnemyList)
            {
                enemy.Updated();
            }
        }
    }

    public void OnStage() //stage ����
    {
        UIManager.Instance.OnIngame();
        OnSetupMode(); 
    }

    public void ResetStage() //Stage Reset
    {
        //Timer Reset
        timer.ResetTimer();
        timer.OnTick = null;
        timer.OnTick = null;

        //Stage Reset
        currentWaveCount = 0;
        RemoveAllEntityInStage();
        OnResetAction?.Invoke();
    }

    public void OnSetupMode()
    {
        if (currentMode == IngameModeType.Setup)
        {
            return;
        }

        currentMode = IngameModeType.Setup;
        currentWaveCount = Mathf.Min(totalWaveCount, ++currentWaveCount);

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

        //�˾Ƽ� ���� ����?
        for (int i = 0; i < testEnemyCount; i++)
        {
            Vector3 worldPos = spawnArea.GetRandomPosByArea();
            EnemyManager enemy = PoolManager.Instance.SpawnObject("TestEnemy", worldPos).GetComponent<EnemyManager>();
            if(enemy != null)
            {
                currentEnemyList.Add(enemy);
                UIIngameManager.BillboardManager.OnSpawnIngameObject(enemy, false);
            }
        }
    }

    public void OnSuccessMode()
    {
        if(currentMode == IngameModeType.Success)
        {
            return;
        }

        Time.timeScale = 0.0f;
        currentMode = IngameModeType.Success;
        OnSuccessAction?.Invoke();

    }

    public void OnFailureMode()
    {
        if (currentMode == IngameModeType.Failure)
        {
            return;
        }

        Time.timeScale = 0.0f;
        currentMode = IngameModeType.Failure;
        OnFailureAction?.Invoke();
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

    public void AddCurrentHeros(int index, HeroManager hero)
    {
        if(currentHeros.ContainsKey(index) == true)
        {
            if (currentHeros[index] == hero)
            {
                Debug.AssertFormat(false, $"Hero is duplicated : [{hero.name}]");
            }
            else
            {
                Debug.AssertFormat(false, $"Depoly slot is duplicated : [{index}]");
            }

            return;
        }

        currentHeros.Add(index, hero);
    }

    public void AddCurrentEnemy(EnemyManager enemy)
    {
        if(currentEnemyList != null)
        {
            currentEnemyList.Add(enemy);
        }
    }
    public void RemoveCurrentEnemy(EnemyManager enemy)
    {
        if (currentEnemyList != null)
        {
            currentEnemyList.Remove(enemy);
        }
    }


    private void RemoveAllEntityInStage()
    {
        if(currentHeros != null)
        {
            foreach (HeroManager hero in currentHeros.Values)
            {
                Destroy(hero.gameObject);
            }

            currentHeros.Clear();
        }

        if (currentEnemyList != null)
        {
            foreach (EnemyManager enemy in currentEnemyList)
            {
                PoolManager.Instance.DespawnObject("TestEnemy", enemy.gameObject);
            }

            currentEnemyList.Clear();
        }
    }

    

    public void SpawnHeroInStage()
    {
        Vector3 spawnPosition = Vector3.zero;
        int slotIndex = UIIngameManager.DepolySlotManager.GetHeroDepolySlotData(ref spawnPosition);

        if((spawnPosition != null || spawnPosition != Vector3.zero) && slotIndex != -1)
        {
            GameObject obj = Resources.Load<GameObject>("Prefabs/Objects/Hero/TestHero");
            HeroManager hero = Instantiate(obj).GetComponent<HeroManager>();
            hero.transform.position = spawnPosition;
            hero.transform.localRotation = Quaternion.identity;

            AddCurrentHeros(slotIndex, hero);
            UIIngameManager.BillboardManager.OnSpawnIngameObject(hero);
            UIIngameManager.DepolySlotManager.SetSlotDeployState(slotIndex);
        }
        else
        {
            //TODO : UI������ ����ϱ�
        }
    }

    public HeroManager GetRandomHeroInStage()
    {
        if(currentHeros == null || currentHeros.Count == 0)
        {
            return null;
        }

        int randNum = UnityEngine.Random.Range(1, currentHeros.Count + 1);

        if(currentHeros.ContainsKey(randNum) == false)
        {
            Debug.AssertFormat(false, $"currentHeros Dict's key is strange : get key ({randNum})");
            return null;
        }

        return currentHeros[randNum];
    }

    public HeroManager GetNearestHero(Vector3 enemyPosition)
    {
        if (currentHeros == null || currentHeros.Count == 0)
        {
            return null;
        }

        HeroManager nearestHero = null;
        float shortestDistance = float.MaxValue;

        foreach (KeyValuePair<int, HeroManager> kv in currentHeros)
        {
            HeroManager hero = kv.Value;
            if (hero == null) 
            {
                continue; 
            }

            float distance = Vector3.Distance(enemyPosition, hero.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestHero = hero;
            }
        }

        return nearestHero;
    }
    public EnemyManager GetNearestEnemy(Vector3 heroPosition)
    {
        if (currentEnemyList == null || currentEnemyList.Count == 0)
        {
            return null;
        }

        EnemyManager nearestEnemy = null;
        float shortestDistance = float.MaxValue;

        foreach (EnemyManager enemy in currentEnemyList)
        {
            if (enemy == null)
            {
                continue;
            }

            float distance = Vector3.Distance(heroPosition, enemy.transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestEnemy = enemy;
            }
        }

        return nearestEnemy;
    }


    /// <summary>
    /// CombatMode �����ϰ� �� Idle ó�� üũ �޼ҵ�
    /// </summary>
    public bool IsPossibleGetTarget()
    {
        if(currentMode != IngameModeType.Combat)
        {
            return false;
        }

        return true;
    }
}
