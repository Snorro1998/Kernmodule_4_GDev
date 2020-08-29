﻿using UnityEngine;

// singleton template
public class V2Singleton<T> : MonoBehaviour where T : Component
{
    public static T Instance
    {
        get;
        private set;
    }

    protected virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = this as T;
            //DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}