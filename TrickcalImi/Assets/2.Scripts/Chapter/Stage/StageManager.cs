using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class StageManager : MonoSingleton<StageManager>
{

    private IngameModeType currentMode;
    
    //@tk (25.04.09) 지금은 프로토타입 제작 때문에, 데이터를 강제로 주입했지만 이후로는 챕터로 부터 받아오기
    #region Test
    [Header("Test")]
    public int testEnemyCount = 5;
    #endregion

    //@tk 이거 나중에 챕터로부터 받기
    private JsonStage onStageData;

    private int currentWaveCount = 0; 
    private int totalWaveCount = 5;
    private float setupLimitTime = 60.0f;
    private float combatLimitTime = 70.0f;

    private int setupCost = 0;
    private int chargeCost = 0;
    private int currentCost = 0;

    private StageSpawnArea spawnArea;
    private LocalTimer timer;

    public Action OnSetupAction; //card setup
    public Action OnCombatAction; //wave
    public Action OnSuccessAction;
    public Action OnFailureAction;
    public Action OnResetAction; //stage reset
    public Action<float, float> OnTickAction; //local Timer

    private Dictionary<int, HeroManager> currentHeros = new Dictionary<int, HeroManager>(); //key : slot index, value : hero
    private List<EnemyManager> currentEnemyList = new List<EnemyManager>();

    public IngameModeType CurrentMode => currentMode;
    public int CurrentWaveCount => currentWaveCount;
    public int TotalWaveCount => totalWaveCount;
    public float SetupLimitTime => setupLimitTime;
    public float CombatLimitTime => combatLimitTime;


    public Action<int> OnChangedCost;
    public int CurrentCost 
    {
        get {  return currentCost; }
        set
        {
            currentCost = value;
            OnChangedCost?.Invoke(currentCost);
        }
    }


    protected override void Awake()
    {
        base.Awake();

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
            if(currentHeros.Count == 0) //영웅 다 사망
            {
                OnFailureMode();
            }
            if(currentEnemyList.Count == 0) //모든 적 죽임
            {
                OnSetupMode();
            }
        }

        if(currentHeros.Count > 0)
        {
            List<int> removeKeyList = new List<int>();
            foreach (KeyValuePair<int, HeroManager> kv in currentHeros)
            {
                if (kv.Value.IsDead == true)
                {
                    removeKeyList.Add(kv.Key);
                }
            }

            foreach (int key in removeKeyList)
            {
                if(currentHeros.ContainsKey(key) == true)
                {
                    HeroManager deadHero = currentHeros[key];
                    currentHeros.Remove(key);
                    deadHero.SetHeroState(HeroState.Dead);
                }
            }

            foreach (HeroManager hero in currentHeros.Values)
            {
                hero.Updated();
            }
        }

        if(currentEnemyList.Count > 0)
        {
            //@tk InvaildOperationException 막기위해 역순 처리
            for (int i = currentEnemyList.Count - 1; i >= 0; i--)
            {
                if (currentEnemyList[i].IsDead == true)
                {
                    EnemyManager deadEnemy = currentEnemyList[i];
                    RemoveCurrentEnemy(currentEnemyList[i]);
                    deadEnemy.SetEnemyState(EnemyState.Dead);
                }
                else
                {
                    currentEnemyList[i].Updated();
                }
            }
        }
    }

    public void OnStage(JsonStage jsonData) //stage 진입
    {
        onStageData = jsonData;
        UIManager.Instance.OnIngame();

        setupCost = jsonData.SetupCost;
        chargeCost = jsonData.ChargeCost;
        currentCost = 0;

        OnSetupMode(); 
    }

    public void ResetStage() //Stage Reset
    {
        //cost Reset
        onStageData = null;

        currentCost = 0;
        chargeCost = 0;
        setupCost = 0;

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
        if(currentWaveCount + 1 > totalWaveCount)
        {
            OnSuccessMode();
            return;
        }

        //wave data setting
        currentMode = IngameModeType.Setup;
        currentWaveCount = Mathf.Min(totalWaveCount, ++currentWaveCount);
        if(CurrentWaveCount == 1)
        {
            currentCost += setupCost;
        }
        else
        {
            currentCost += chargeCost;
        }

        //local timer setting
        timer.OnTick = null;
        timer.OnTick += UpdateOnTick;

        timer.OnTimeOver = null;
        timer.OnTimeOver += OnCombatMode;
        timer.OnTimer(setupLimitTime);

        //체력 업데이트
        if (currentHeros.Count > 0) //턴 지날때마다 체력 회복
        {
            foreach (HeroManager hero in currentHeros.Values)
            {
                hero.HealthManager.OnIncreasedHealthRatio(HealthType.HP, 0.5f); // 50% 회복
            }
        }

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
            EnemyManager enemy = PoolManager.Instance.SpawnObject("TestEnemy", worldPos).GetComponent<EnemyManager>();
            if(enemy != null)
            {
                AddCurrentEnemy(enemy);
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
        //로컬 저장
        int stageIndex = (onStageData.StageID % 10 == 0) ? 10 : onStageData.StageID % 10;
        LocalDataManager.Instance.UpdateLocalChapterData(onStageData.ChapterNumber, stageIndex);
        
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
    public void RemoveCurrentHeros(int index)
    {
        if (currentHeros.ContainsKey(index) == false)
        {
            Debug.Assert(false, "This hero is not in current Hero");
            return;
        }

        currentHeros.Remove(index);
    }

    public void AddCurrentEnemy(EnemyManager enemy)
    {
        if(currentEnemyList != null)
        {
            currentEnemyList.Add(enemy);
            enemy.OnDead += () => { RemoveCurrentEnemy(enemy); };
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


    #region Test
    //@tk 이제는 Deque에 있는 정보 그대로 하나씩만 소환할 것임.
    public void SpawnHeroInStage()
    {
        Vector3 spawnPosition = Vector3.zero;
        int slotIndex = UIIngameManager.DepolySlotManager.GetHeroDepolySlotData(ref spawnPosition);

        if((spawnPosition != null || spawnPosition != Vector3.zero) && slotIndex != -1)
        {
            GameObject obj = PoolManager.Instance.SpawnObject("TestHero");
            HeroManager hero = obj.GetComponent<HeroManager>();
            hero.transform.position = spawnPosition;
            hero.transform.localRotation = Quaternion.identity;

            AddCurrentHeros(slotIndex, hero);
            UIIngameManager.BillboardManager.OnSpawnIngameObject(hero);
            UIIngameManager.DepolySlotManager.SetSlotDeployState(slotIndex);
            hero.OnDead += () => { RemoveCurrentHeros(slotIndex); };
        }
        else
        {
            //TODO : UI상으로 경고하기
        }
    }
    #endregion 
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
    /// CombatMode 제외하고 다 Idle 처리 체크 메소드
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
