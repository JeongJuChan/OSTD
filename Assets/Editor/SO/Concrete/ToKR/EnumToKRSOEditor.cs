using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnumToKRSO))]
public class EnumToKRSOEditor : ListDataSOEditor<EnumToKRSO>
{
    public static void LoadCSVToSO(EnumToKRSO enumToKRSO, TextAsset csv)
    {
        enumToKRSO.ClearDatas();

        string[] rows = csv.text.Split('\n');

        Debug.Log(rows[0]);

        List<EnumToKRData> enumToKRDatas = new List<EnumToKRData>();

        Assembly assembly = null;

        for (int i = 1; i < rows.Length; i++)
        {
            string[] element = rows[i].Split(',');

            Type type = null;
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            if (assembly != null)
            {
                type = assembly.GetType(element[0]);
            }
            else
            {
                foreach (Assembly tempAssembly in assemblies)
                {
                    Type tempType = tempAssembly.GetType(element[0]);
                    if (tempType != null)
                    {
                        type = tempType;
                        assembly = tempAssembly;
                        break;
                    }
                }
            }

            int index = int.Parse(element[1]);
            Enum enumType = (Enum)Enum.Parse(type, element[2]);

            enumToKRDatas.Add(new EnumToKRData(type, index, enumType, element[3]));
        }

        enumToKRSO.AddDatas(enumToKRDatas);
        enumToKRSO.InitDict();
        EditorUtility.SetDirty(enumToKRSO);
    }

    protected override void ClearDatas()
    {
        dataSO.ClearDatas();
    }

    protected override void LoadCSV(TextAsset csv)
    {
        string[] rows = csv.text.Split('\n');

        Debug.Log(rows[0]);

        List<EnumToKRData> enumToKRDatas = new List<EnumToKRData>();

        Assembly assembly = null;

        for (int i = 1; i < rows.Length; i++)
        {
            string[] element = rows[i].Split(',');

            Type type = null;

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            //type = assembly.GetType(element[0]);

            if (assembly != null)
            {
                type = assembly.GetType(element[0]);
            }
            else
            {
                foreach (Assembly tempAssembly in assemblies)
                {
                    Type tempType = tempAssembly.GetType(element[0]);
                    if (tempType != null)
                    {
                        type = tempType;
                        assembly = tempAssembly;
                        break;
                    }
                }
            }

            int index = int.Parse(element[1]);
            Enum enumType = (Enum)Enum.Parse(type, element[2]);

            enumToKRDatas.Add(new EnumToKRData(type, index, enumType, element[3]));
        }

        dataSO.AddDatas(enumToKRDatas);
        dataSO.InitDict();
        EditorUtility.SetDirty(dataSO);
    }
}
