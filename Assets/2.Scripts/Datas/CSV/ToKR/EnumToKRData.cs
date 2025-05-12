using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct EnumToKRData
{
    public Type type;
    public int index;
    public Enum enumType;
    public string enumToKR;

    public EnumToKRData(Type type, int index, Enum enumType, string enumToKR)
    {
        this.type = type;
        this.index = index;
        this.enumType = enumType;
        this.enumToKR = enumToKR;
    }
}
