using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServiceLocator : MonoBehaviourSingleton<ServiceLocator>
{
    private Dictionary<string, MonoBehaviour> monoBehaviorDict = new Dictionary<string, MonoBehaviour>();

    public T GetTypeFromScene<T>() where T : MonoBehaviour
    {
        string name = typeof(T).Name;

        if (!monoBehaviorDict.ContainsKey(name))
        {
            monoBehaviorDict.Add(name, FindAnyObjectByType<T>());
        }

        return monoBehaviorDict[name] as T;
    }
}
