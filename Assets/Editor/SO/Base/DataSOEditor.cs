using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class DataSOEditor<T> : Editor where T : ScriptableObject
{
    protected T dataSO;

    private TextAsset csv;

    protected void OnEnable()
    {
        dataSO = (T)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        csv = EditorGUILayout.ObjectField("CSV", csv, typeof(TextAsset), true) as TextAsset;

        if (GUILayout.Button("Load CSV"))
        {
            LoadCSV(csv);
        }
    }

    protected abstract void LoadCSV(TextAsset csv);
}
