using System;
using System.Collections;
using System.Collections.Generic;
using Keiwando.BigInteger;
using UnityEngine;

[Serializable]
public struct ProjectileData
{
    public int index;
    public string name;

    public ProjectileData(int index, string name)
    {
        this.index = index;
        this.name = name;
    }
}
