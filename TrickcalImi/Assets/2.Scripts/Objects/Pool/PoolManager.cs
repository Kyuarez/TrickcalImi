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
    [SerializeField] private int enemyCount;
    [SerializeField] private int fxCount;

    private Transform parent_Enemy;
    private Transform parent_FX;
    private Transform parent_UI;

    protected override void Awake()
    {
        base.Awake();
        //Casting
        parent_Enemy = transform.FindRecursiveChild(Define.Name_Pool_Enemy);
        parent_FX = transform.FindRecursiveChild(Define.Name_Pool_FX);
        parent_UI = transform.FindRecursiveChild(Define.Name_Pool_UI);
        //Init
        InitPoolManager();
    }

    private void InitPoolManager()
    {
        //Enemy
        GameObject[] enemies = Resources.LoadAll<GameObject>("Prefabs/Objects/Enemy")
            .Where(obj => obj.GetComponent<EnemyManager>() != null)
            .ToArray();

        foreach (var enemy in enemies)
        {
            GameObject poolObj = new GameObject($"Pool:{enemy.name}");
            poolObj.transform.SetParent(parent_Enemy);

            Pool pool = new Pool();
            pool.LoadObject(poolObj.transform, enemy, enemyCount);
            AddPool(enemy.name, pool);
        }

        //FX

        //UI
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
