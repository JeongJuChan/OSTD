using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public struct SkillUIData
{
    public int index;
    public string skillName;
    public string skillNameKR;
    public string description;
    public Sprite mainSprite;
    public Sprite equipStateSprite;
    public SkillUpgradableData skillUpgradableData;

    public SkillUIData(int index, string skillName, string skillNameKR, string description, Sprite mainSprite, Sprite equipStateSprite, 
        SkillUpgradableData skillUpgradableData)
    {
        this.index = index;
        this.skillName = skillName;
        this.skillNameKR = skillNameKR;
        this.description = description;
        this.mainSprite = mainSprite;
        this.equipStateSprite = equipStateSprite;
        this.skillUpgradableData = skillUpgradableData;
    }
}
