using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DataSO<T> : ScriptableObject
{
    [SerializeField] protected T data;

#if UNITY_EDITOR
    public void SetData(T newData)
    {
        data = newData;
    }
#endif

    public virtual void InitData()
    {

    }
}
