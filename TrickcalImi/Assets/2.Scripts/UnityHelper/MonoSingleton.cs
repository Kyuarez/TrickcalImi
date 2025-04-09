using System;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private T instance;
    public T Instance
    {
        get 
        {
            if (instance == null)
            {
                Debug.Assert(false, $"{this.GetType().Name} is null");
                throw new NullReferenceException();
            }
            return instance; 
        }
    }

    [RuntimeInitializeOnLoadMethod]
    public void InitSingleton()
    {
        if(instance == null)
        {
            instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else if(instance != this as T)
        {
            Destroy(gameObject);
        }
    }
}
