using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[CreateAssetMenu(fileName = "EnumToKR", menuName = "ToKR/EnumToKR")]
public class EnumToKRSO : ListDataSO<EnumToKRData>
{
    private Dictionary<Type, Dictionary<int, string>> enumToKRDict = new Dictionary<Type, Dictionary<int, string>>();

    public string GetEnumToKRByType(Type type, int index)
    {
        if (enumToKRDict.ContainsKey(type))
        {
            if (enumToKRDict[type].ContainsKey(index))
            {
                return enumToKRDict[type][index];
            }
        }

        return "";
    }

    public override void InitDict()
    {
        ClearDatas();

        string[] rows = Resources.Load<TextAsset>($"CSV/ToKR/EnumToKR CSV").text.Split('\n');

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

            enumToKRDatas.Add(new EnumToKRData(type, index, enumType, element[3].Trim('\r')));
        }

        AddDatas(enumToKRDatas);

        foreach (EnumToKRData data in datas)
        {
            Type type = data.type;

            if (!enumToKRDict.ContainsKey(type))
            {
                enumToKRDict.Add(type, new Dictionary<int, string>());
            }

            if (!enumToKRDict[type].ContainsKey(data.index))
            {
                enumToKRDict[type].Add(data.index, data.enumToKR);
            }
        }
    }
}
