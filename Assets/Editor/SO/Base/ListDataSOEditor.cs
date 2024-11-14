using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class ListDataSOEditor<T> : Editor where T : ScriptableObject
{
    public T dataSO;

    private TextAsset csv;

    private void OnEnable()
    {
        SetTarget();
    }

    protected void SetTarget()
    {
        dataSO = (T)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        csv = EditorGUILayout.ObjectField("csv", csv, typeof(TextAsset), true) as TextAsset;

        if (GUILayout.Button("Load CSV"))
        {
            LoadCSV(csv);
        }

        if (GUILayout.Button("Clear Datas"))
        {
            ClearDatas();
        }
    }

    protected abstract void ClearDatas();

    protected abstract void LoadCSV(TextAsset csv);
}
public abstract class ListDatasSOEditor<T> : Editor where T : ScriptableObject
{
    public T dataSO;

    private TextAsset csv;

    private void OnEnable()
    {
        SetTarget();
    }

    protected void SetTarget()
    {
        dataSO = (T)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        csv = EditorGUILayout.ObjectField("csv", csv, typeof(TextAsset), true) as TextAsset;

        if (GUILayout.Button("Load CSV"))
        {
            LoadCSV(csv);
        }

        if (GUILayout.Button("Clear Datas_T1"))
        {
            ClearDatas_T1();
        }

        if (GUILayout.Button("Clear Datas_T2"))
        {
            ClearDatas_T2();
        }
    }

    protected abstract void ClearDatas_T1();
    protected abstract void ClearDatas_T2();

    protected abstract void LoadCSV(TextAsset csv);
}

