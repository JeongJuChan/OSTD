using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Pooler<T> : UI_Base where T : MonoBehaviour, IPoolable<T>
{
    [SerializeField] protected T prefab;


    [SerializeField] private int initCount = 10;

    private Queue<T> inactivePool = new Queue<T>();
    private Queue<T> activePool = new Queue<T>();

    public override void Init()
    {
        for (int i = 0; i < initCount; i++)
        {
            CreateNewUI();
        }
    }

    public T GetUI()
    {
        if (inactivePool.Count == 0)
        {
            CreateNewUI();
        }

        T t = inactivePool.Dequeue();
        t.gameObject.SetActive(true);
        activePool.Enqueue(t);
        return t;
    }

    public void ReturnUI(T t)
    {
        t.gameObject.SetActive(false);
        inactivePool.Enqueue(t);
    }

    public void ReturnAllUI()
    {
        while (activePool.Count > 0)
        {
            activePool.Dequeue().ReturnToPool();
        }
    }

    protected virtual void CreateNewUI()
    {
        T t = Instantiate(prefab, transform);
        t.Initialize(ReturnUI);
        ReturnUI(t);
    }
}
