using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class UIBillboardManager : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    private Transform hpBarParent;

    private void Awake()
    {
        hpBarParent = transform.FindRecursiveChild("@hpBarParent");
    }

    public void SetActivePanel(bool active)
    {
        if (panel.activeSelf == !active)
        {
            panel.SetActive(active);
        }
    }

    public void OnSpawnIngameObject(IngameObject obj, bool isHero = true)
    {
        UIHealth uiHealth = PoolManager.Instance.SpawnObject("UIHealth").GetComponent<UIHealth>();
        if (uiHealth != null)
        {
            uiHealthDict.Add(obj, uiHealth);
            uiHealth.SetUIHealth(obj, isHero);
            uiHealth.transform.SetParent(hpBarParent, false);
            obj.HealthManager.RegisterOnUpdateHealth(uiHealth.OnUpdateHealthUI);
            obj.OnDead += () =>
            {
                OnDespawnIngameObject(obj);
            };
        }
    }
    public void OnDespawnIngameObject(IngameObject obj)
    {
        UIHealth uiHealth;
        if(uiHealthDict.TryGetValue(obj, out uiHealth) == true)
        {
            uiHealth.transform.localScale = Vector3.one;
            PoolManager.Instance.DespawnObject("UIHealth", uiHealth.gameObject);
            uiHealthDict.Remove(obj);
        }
    }

    public void ResetStage()
    {
        foreach (var uiHealth in uiHealthDict.Values)
        {
            uiHealth.ResetUIHealth();
            PoolManager.Instance.DespawnObject("UIHealth", uiHealth.gameObject);
        }

        uiHealthDict.Clear();
    }

    private void Update()
    {
        foreach(KeyValuePair<IngameObject, UIHealth> kv in uiHealthDict)
        {
            Vector3 pos = kv.Key.Mark_Health.position;
            kv.Value.transform.position = pos;
        }
    }

    private Dictionary<IngameObject, UIHealth> uiHealthDict = new Dictionary<IngameObject, UIHealth>();
}
