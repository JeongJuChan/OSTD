using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameData))]
public class GameDataEditor : Editor
{
    private Vector2 scrollPosition;

    public override void OnInspectorGUI()
    {
        // 기본 인스펙터 그리기
        DrawDefaultInspector();

        GameData gameData = (GameData)target;

        if (gameData.columnNames != null && gameData.GetDataRows() != null)
        {
            GUILayout.Label("Data Preview", EditorStyles.boldLabel);
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            EditorGUILayout.BeginHorizontal();
            foreach (var colName in gameData.columnNames)
            {
                GUILayout.Label(colName, GUILayout.Width(100));
            }
            EditorGUILayout.EndHorizontal();

            foreach (var row in gameData.GetDataRows())
            {
                EditorGUILayout.BeginHorizontal();
                foreach (var cell in row.rowData)
                {
                    GUILayout.Label(cell, GUILayout.Width(100));
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
        }
        else
        {
            GUILayout.Label("No data available.", EditorStyles.boldLabel);
        }
    }
}