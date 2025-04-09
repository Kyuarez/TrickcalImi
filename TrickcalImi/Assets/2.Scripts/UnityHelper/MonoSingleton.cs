using System;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance
    {
        get 
        {
            if (instance == null)
            {
                Debug.Assert(false, $"{typeof(T).Name} is null");
                throw new NullReferenceException();
            }
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
