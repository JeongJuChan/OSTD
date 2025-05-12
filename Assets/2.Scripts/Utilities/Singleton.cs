using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> where T : class, new()
{
    private static T _instance;
    public static T instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new T();
            }
            return _instance;
        }
    }

    public static void DisPose()
    {
        _instance = null;
    }
}
