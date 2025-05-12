using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct SkillData
{
    public SkillType skillType;
    public int typeNumber;

    public SkillData(SkillType skillType, int typeNumber)
    {
        this.skillType = skillType;
        this.typeNumber = typeNumber;
    }
}
