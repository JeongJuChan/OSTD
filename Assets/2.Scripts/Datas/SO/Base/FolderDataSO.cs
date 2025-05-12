using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class FolderDataSO<T> : ScriptableObject
{
    [SerializeField] protected List<T> datas;

    public void AddDatas(List<T> datas)
    {
        this.datas.AddRange(datas);
    }

    public void ClearDatas()
    {
        if (datas != null)
        {
            datas.Clear();
        }
    }
}