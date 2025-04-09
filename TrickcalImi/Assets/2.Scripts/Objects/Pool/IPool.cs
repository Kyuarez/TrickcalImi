using System;
using System.Collections.Generic;
using UnityEngine;

public interface IPool 
{
    public Transform Parent { get; set; }

    Queue<GameObject> pool { get; set; }

    public void LoadObject(Transform parent, GameObject prefab, int maxCount);
    public GameObject SpawnObject(Vector2 worldPos, Action<GameObject> action = null);
    public void DespawnObject(GameObject obj, Action<GameObject> action = null);
}
    