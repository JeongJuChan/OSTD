using System;

[Serializable]
public struct EffectData
{
    public int index;
    public string name;
    public EffectType effectType;

    public EffectData(int index, string name, EffectType effectType)
    {
        this.index = index;
        this.name = name;
        this.effectType = effectType;
    }
}
