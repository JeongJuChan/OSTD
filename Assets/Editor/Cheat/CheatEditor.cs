using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Cheat))]
public class CheatEditor : Editor
{
    private Cheat cheat;

    private GUIStyle guiStyle;

    private Rank rank;

    private string skillIndex = "21000";
    private string skillCount = "1";

    private void OnEnable()
    {
        cheat = (Cheat)target;
        guiStyle = new GUIStyle();
        guiStyle.fontStyle = FontStyle.Bold;
        guiStyle.alignment = TextAnchor.MiddleCenter;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        guiStyle.normal.textColor = Color.green;

        GUILayout.Label("Stage", guiStyle);

        if (GUILayout.Button("Defeat"))
        {
            cheat.KillCastle();
        }

        // if (GUILayout.Button("Skip Current Routine Stage"))
        // {
        //     cheat.SkipCurrentRoutineStage();
        // }

        // if (GUILayout.Button("Skip Current Sub Stage"))
        // {
        //     cheat.SkipCurrentSubStage();
        // }

        // if (GUILayout.Button("Skip Current Main Stage"))
        // {
        //     cheat.SkipCurrentMainStage();
        // }

        // if (GUILayout.Button("Challenge Boss"))
        // {
        //     cheat.ChallengeBoss();
        // }

        guiStyle.normal.textColor = Color.red;
        GUILayout.Label("Forge", guiStyle);

        guiStyle.normal.textColor = Color.blue;
        GUILayout.Label("Skill", guiStyle);


        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();
        GUILayout.Label("Index");
        skillIndex = GUILayout.TextField(skillIndex);
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        GUILayout.Label("Count");
        skillCount = GUILayout.TextField(skillCount);
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
    }
}
