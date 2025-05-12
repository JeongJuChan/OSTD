using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnumToKRManager : MonoBehaviourSingleton<EnumToKRManager>
{
    private EnumToKRSO enumToKRSO;

    public void Init()
    {
        this.enumToKRSO = ResourceManager.instance.enumToKRSO;
    }

    public string GetStatTypeText(StatType? statType, float effectValue)
    {
        string statTypeKR = GetEnumToKR(statType);
        string afterText = $"{(int)effectValue} 증가";
        string resultStr = $"{statTypeKR} {afterText}";
        return resultStr;
    }

    public string GetEnumToKR(Enum enumType)
    {
        Type type = enumType.GetType();
        string krStr = enumToKRSO.GetEnumToKRByType(type, Convert.ToInt32(enumType));
        return krStr;
    }
}