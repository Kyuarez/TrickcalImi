using System;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance
    {
        get 
        {
            return instance; 
        }
    }

    protected virtual void Awake()
    {
        if(instance == null)
        {
            instance = this as T;
            DontDestroyOnLoad(this);
        }
        else if(instance != this as T)
        {   
            Destroy(gameObject);
        }
    }
}
