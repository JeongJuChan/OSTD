using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonoBehaviourSingleton<T> : MonoBehaviour where T : Component
{
    private static T _instance;
    public static T instance
    {
        get
        {
            if (_instance == null)
            {
                T t = FindObjectOfType<T>();
                if (t == null)
                {
                    InitSingleton(); 
                }
                else
                {
                    _instance = t;
                }
            }

            return _instance;
        }
    }

    private void Awake()
    {
        RemoveDuplicates();
    }

    private static void InitSingleton()
    {
        GameObject go = new GameObject();
        go.name = typeof(T).Name;
        _instance = go.AddComponent<T>();
    }

    private void RemoveDuplicates()
    {
        if (FindObjectsByType<T>(FindObjectsSortMode.None).Length == 1)
        {
            _instance = this as T;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
