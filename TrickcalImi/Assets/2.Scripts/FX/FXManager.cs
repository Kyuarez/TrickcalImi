using System;
using System.Collections.Generic;
using UnityEngine;

public class FXManager : MonoSingleton<FXManager>
{
    Queue<Tuple<FXType, GameObject>> fxQueue = new Queue<Tuple<FXType, GameObject>>();

    private void Update()
    {
        if (fxQueue.Count > 0)
        {
            Tuple<FXType, GameObject> fx = fxQueue.Dequeue();
            if(fx.Item2.activeSelf == false)
            {
                PoolManager.Instance.DespawnObject($"FX_{fx.Item1.ToString()}", fx.Item2);
            }
            else
            {
                fxQueue.Enqueue(fx);
            }
        }
    }

    public void OnEffect(FXType type, Vector3 worldPos)
    {
        GameObject fxObj = PoolManager.Instance.SpawnObject($"FX_{type.ToString()}", worldPos);
        fxQueue.Enqueue(new Tuple<FXType, GameObject>(type, fxObj));
    }
}
