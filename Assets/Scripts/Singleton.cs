using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class Singleton<T> : SerializedMonoBehaviour where T : Component
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
        if (instance == null)
        {
            instance = this as T;
        }
        else if(instance != this)
        {
          
            Destroy(gameObject);
        }
    }
}