using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/* [25.04.09]
 컨셉 : Resources 폴더에서 필요한 객체들 Count 만큼 생성해서, Pool 기능 구현
 나중에 Json으로 pool 할 객체들 리스트 가지고 있어서, 거기서 load 되게 할까?
 */
public class PoolManager : MonoSingleton<PoolManager>
{
    //@tk 임시 변수 : 나중엔 json으로 처린
    [SerializeField] private int heroCount;
    [SerializeField] private int enemyCount;
    [SerializeField] private int fxCount;
    [SerializeField] private int uiCount;

    private Transform parent_Hero;
    private Transform parent_Enemy;
    private Transform parent_FX;
    private Transform parent_UI;

    protected override void Awake()
    {
        base.Awake();
        //Casting
        parent_Hero = transform.FindRecursiveChild(Define.Name_Pool_Hero);
        parent_Enemy = transform.FindRecursiveChild(Define.Name_Pool_Enemy);
        parent_FX = transform.FindRecursiveChild(Define.Name_Pool_FX);
        parent_UI = transform.FindRecursiveChild(Define.Name_Pool_UI);
    }

    private void Start()
    {
        InitPoolManager();
    }

    private void InitPoolManager()
    {
        //Hero
        GameObject[] heros = Resources.LoadAll<GameObject>("Prefabs/Objects/Hero")
            .Where(obj => obj.GetComponent<HeroManager>() != null)
            .ToArray();
        if (heros != null && heros.Length > 0)
        {
            foreach (var hero in heros)
            {
                GameObject poolObj = new GameObject($"Pool:{hero.name}");
                poolObj.transform.SetParent(parent_Hero);

                Pool pool = new Pool();
                pool.LoadObject(poolObj.transform, hero, heroCount);
                AddPool(hero.name, pool);
            }
        }

        //Enemy
        GameObject[] enemies = Resources.LoadAll<GameObject>("Prefabs/Objects/Enemy")
            .Where(obj => obj.GetComponent<EnemyManager>() != null)
            .ToArray();
        if(enemies != null && enemies.Length > 0)
        {
            foreach (var enemy in enemies)
            {
                GameObject poolObj = new GameObject($"Pool:{enemy.name}");
                poolObj.transform.SetParent(parent_Enemy);

                Pool pool = new Pool();
                pool.LoadObject(poolObj.transform, enemy, enemyCount);
                AddPool(enemy.name, pool);
            }
        }

        //FX
        GameObject[] fxArr = Resources.LoadAll<GameObject>("FX");

        if (fxArr != null && fxArr.Length > 0)
        {
            foreach (var fx in fxArr)
            {
                GameObject poolObj = new GameObject($"Pool:{fx.name}");
                poolObj.transform.SetParent(parent_FX);

                Pool pool = new Pool();
                pool.LoadObject(poolObj.transform, fx, fxCount);
                AddPool(fx.name, pool);
            }
        }

        //UI
        GameObject[] uiArr = Resources.LoadAll<GameObject>(Define.Res_UI_Pool);
        if (uiArr != null && uiArr.Length > 0)
        {
            foreach (var ui in uiArr)
            {
                GameObject poolObj = new GameObject($"Pool:{ui.name}");
                poolObj.transform.SetParent(parent_UI);

                Pool pool = new Pool();
                pool.LoadObject(poolObj.transform, ui, uiCount);
                AddPool(ui.name, pool);
            }
        }
    }

    private void AddPool(string path, IPool pool)
    {
        if(poolDict.ContainsKey(path) == true)
        {
            Debug.AssertFormat(false, $"Pool's key is duplicated : {path}");
            return;
        }

        poolDict.Add(path, pool);
    }

    public GameObject SpawnObject(string path, Action<GameObject> action = null)
    {
        if (poolDict.ContainsKey(path) == false)
        {
            return null;
        }

        return poolDict[path]?.SpawnObject(Vector3.zero, action);
    }
    public GameObject SpawnObject(string path ,Vector2 worldPos, Action<GameObject> action = null)
    {
        if (poolDict.ContainsKey(path) == false)
        {
            return null;
        }

        return poolDict[path]?.SpawnObject(worldPos, action);
    }

    public void DespawnObject(string path, GameObject obj)
    {
        if (poolDict.ContainsKey(path) == false)
        {
            Destroy(obj);
            return;
        }

        poolDict[path].DespawnObject(obj);
    }


    private Dictionary<string, IPool> poolDict = new Dictionary<string, IPool>();

}
