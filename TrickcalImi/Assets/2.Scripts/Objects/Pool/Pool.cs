using System;
using System.Collections.Generic;
using UnityEngine;

public class Pool : IPool
{
    public Transform Parent 
    { 
        get; 
        set; //@tk (25.04.09 : 이거 private로 변경하는 방법 찾아야 함.)
    }
    public Queue<GameObject> pool 
    { 
        get; 
        set; 
    }
    public void LoadObject(Transform parent, GameObject prefab, int maxCount)
    {
        //풀 생성
        Parent = parent;
        pool = new Queue<GameObject>();

        for (int i = 0; i < maxCount; i++)
        {
            GameObject obj = GameObject.Instantiate(prefab);
            DespawnObject(obj);
        }
    }

    public GameObject SpawnObject(Vector2 worldPos, Action<GameObject> action = null)
    {
        var obj = pool.Dequeue();
        if (obj != null) 
        {
            obj.transform.SetParent(null);
            obj.transform.localPosition = worldPos;
            obj.gameObject.SetActive(true);
            action?.Invoke(obj);    
            return obj;
        }

        return null;
    }

    public void DespawnObject(GameObject obj, Action<GameObject> action = null)
    {
        pool.Enqueue(obj);
        obj.SetActive(false);
        obj.transform.SetParent(Parent);
        obj.transform.position = Vector2.zero;
        obj.transform.rotation = Quaternion.identity;
        action?.Invoke(obj);
    }


}
