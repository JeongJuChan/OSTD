using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class ListDataSO<T> : ScriptableObject
{
    [SerializeField] protected List<T> datas;

    public virtual void AddDatas(List<T> datas)
    {
        this.datas.AddRange(datas);
    }

    public void ClearDatas()
    {
        datas.Clear();
    }


    public abstract void InitDict();
}

[Serializable]
public abstract class ListDatasSO<T1, T2> : ScriptableObject
{
    [SerializeField] protected List<T1> datas_1;
    [SerializeField] protected List<T2> datas_2;

    public virtual void AddDatas(List<T1> datas)
    {
        this.datas_1.AddRange(datas);
    }

    public virtual void AddDatas(List<T2> datas)
    {
        this.datas_2.AddRange(datas);
    }

    public void ClearDatas_T1()
    {
        datas_1.Clear();
    }

    public void ClearDatas_T2()
    {
        datas_2.Clear();
    }

    public abstract void InitDict_T1();
    public abstract void InitDict_T2();
}
