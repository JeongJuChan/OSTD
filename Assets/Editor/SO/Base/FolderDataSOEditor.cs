using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public abstract class FolderDataSOEditor<SO, Data> : Editor where SO : FolderDataSO<Data>
{
    protected SO dataSO;

    private UnityEngine.Object folder;

    private void OnEnable()
    {
        dataSO = (SO)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        folder = EditorGUILayout.ObjectField("CSV 드래그", folder, typeof(UnityEngine.Object), true);

        if (GUILayout.Button("Load All CSV"))
        {
            LoadAllCSVFromFolder();
        }
    }

    protected virtual void LoadAllCSVFromFolder()
    {
        string path = AssetDatabase.GetAssetPath(folder);
        string[] files = Directory.GetFiles(path, "*.csv", SearchOption.AllDirectories);

        if (dataSO != null)
        {
            dataSO.ClearDatas();
        }

        foreach (var file in files)
        {
            TextAsset csv = AssetDatabase.LoadAssetAtPath(file, typeof(TextAsset)) as TextAsset;
            LoadCSV(csv);
        }
    }

    protected abstract void LoadCSV(TextAsset csv);
}
