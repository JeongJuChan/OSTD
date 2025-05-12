using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillDataHandler
{
    private Dictionary<SkillType, Sprite> skillSpriteDict = new Dictionary<SkillType, Sprite>();

    private const string TEMPLATE_PATH = "Skills";
    
    public void Init()
    {
        SkillType[] skillTypes = (SkillType[])Enum.GetValues(typeof(SkillType));

        for (int i = 1; i < skillTypes.Length; i++)
        {
            Sprite sprite = Resources.Load<Sprite>($"UI/{TEMPLATE_PATH}/{skillTypes[i]}");
            if (!skillSpriteDict.ContainsKey(skillTypes[i]))
            {
                skillSpriteDict.Add(skillTypes[i], sprite);
            }
        }
    }

    public Sprite GetSkillSprite(SkillType skillType)
    {
        if (skillSpriteDict.ContainsKey(skillType))
        {
            return skillSpriteDict[skillType];
        }

        return default;
    }
}