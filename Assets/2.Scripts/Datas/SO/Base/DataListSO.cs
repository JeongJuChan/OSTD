using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DataListSO<T> : ScriptableObject
{
    [SerializeField] protected List<T> datas;

    protected abstract void InitData(int index);
}
