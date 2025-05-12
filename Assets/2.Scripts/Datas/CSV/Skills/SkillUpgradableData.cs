using System;

[Serializable]
public struct SkillUpgradableData
{
    public int targetCount;
    public float damagePerecent;
    public float duration;
    public float coolTime;
    public int equipIndex;
    public bool isEquipped;

    public SkillUpgradableData(float damagePerecent, int targetCount, float duration, float coolTime)
    {
        this.damagePerecent = damagePerecent;
        this.targetCount = targetCount;
        this.duration = duration;
        this.coolTime = coolTime;
        this.equipIndex = -1;
        this.isEquipped = false;
    }

    /*public void SaveSkillData()
    {
        ES3.Save<int>($"{index}{Consts.SKILL_LEVEL}", level, ES3.settings);
        ES3.Save<float>($"{index}{Consts.SKILL_DAMAGE_PERCENT}", damagePerecent, ES3.settings);
        ES3.Save<int>($"{index}{Consts.SKILL_EQUIP_INDEX}", equipIndex, ES3.settings);
        ES3.Save<bool>($"{index}{Consts.SKILL_IS_EQUIPPED}", isEquipped, ES3.settings);

        ES3.StoreCachedFile();
    }

    public void LoadSkillData()
    {
        if (!ES3.KeyExists($"{index}{Consts.SKILL_LEVEL}")) return;
        level = ES3.Load<int>($"{index}{Consts.SKILL_LEVEL}");
        damagePerecent = ES3.Load<float>($"{index}{Consts.SKILL_DAMAGE_PERCENT}");
        equipIndex = ES3.Load<int>($"{index}{Consts.SKILL_EQUIP_INDEX}");
        isEquipped = ES3.Load<bool>($"{index}{Consts.SKILL_IS_EQUIPPED}");
    }*/
}
