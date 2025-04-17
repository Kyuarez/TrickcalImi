using System;
using System.Collections.Generic;
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
    private float setupLimitTime = 10.0f;
    private float combatLimitTime = 10.0f;

    private StageSpawnArea spawnArea;
    private LocalTimer timer;

    public Action OnSetupAction; //card setup
    public Action OnCombatAction; //wave
    public Action OnSuccessAction;
    public Action OnFailureAction;
    public Action<float, float> OnTickAction; //local Timer

    private Dictionary<int, HeroManager> currentHeros = new Dictionary<int, HeroManager>();
    private List<EnemyManager> currentEnemyList = new List<EnemyManager>();

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

    public void OnStage() //stage 진입
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

        //알아서 몬스터 생성?
        for (int i = 0; i < testEnemyCount; i++)
        {
            Vector3 worldPos = spawnArea.GetRandomPosByArea();
            EnemyManager enemy = PoolManager.Instance.SpawnObject("TestEnemy", worldPos).GetComponent<EnemyManager>();
            if(enemy != null)
            {
                currentEnemyList.Add(enemy);
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

        ResetCurrentHeros();
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

        ResetCurrentHeros();
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

    private void ResetCurrentHeros()
    {
        currentHeros.Clear();
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
            UIIngameManager.DepolySlotManager.SetSlotDeployState(slotIndex);
        }
        else
        {
            //TODO : UI상으로 경고하기
        }
    }

    //@tk : 일단 적이 타겟을 세팅.
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



}
