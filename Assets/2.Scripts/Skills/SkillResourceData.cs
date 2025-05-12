using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SkillResourceData
{
    public Sprite skillIcon;
    public Skill skill;

    public SkillResourceData(Sprite skillIcon, Skill skill)
    {
        this.skillIcon = skillIcon;
        this.skill = skill;
    }
}
