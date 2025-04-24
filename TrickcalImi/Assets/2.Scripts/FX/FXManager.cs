using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXManager : MonoSingleton<FXManager>
{
    Dictionary<FXType, Queue<GameObject>> fxQueueDict = new Dictionary<FXType, Queue<GameObject>>();

    protected override void Awake()
    {
        base.Awake();
        InitFXQueueDict();
    }

    private void Update()
    {
        foreach (FXType fxType in System.Enum.GetValues(typeof(FXType)))
        {
            if (fxQueueDict[fxType].Count > 0)
            {
                GameObject fx = fxQueueDict[fxType].Dequeue();
                if (fx.activeSelf == false)
                {
                    PoolManager.Instance.DespawnObject($"FX_{fxType.ToString()}", fx);
                }
                else
                {
                    fxQueueDict[fxType].Enqueue(fx);
                }
            }
        }
    }

    private void InitFXQueueDict()
    {
        //각 Type 별로 따로 체크하기 위해 Dict<Type, Queue>로 관리
        foreach (FXType fxType in System.Enum.GetValues(typeof(FXType)))
        {
            fxQueueDict[fxType] = new Queue<GameObject>();
        }
    }

    public void OnEffect(FXType type, Vector3 worldPos)
    {
        GameObject fxObj = PoolManager.Instance.SpawnObject($"FX_{type.ToString()}", worldPos);
        fxQueueDict[type].Enqueue(fxObj);
    }
}
