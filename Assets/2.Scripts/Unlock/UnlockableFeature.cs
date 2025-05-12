using System;

public class UnlockableFeature
{
    public FeatureType Type { get; set; }
    public FeatureID ID { get; set; }
    public bool IsUnlocked { get; set; }
    public int Count { get; set; }
    public Func<int, bool> UnlockCondition { get; set; }
    public Action OnUnlockAction { get; set; }

    public UnlockableFeature(FeatureType type, FeatureID id, int count, Action onUnlockAction)
    {
        Type = type;
        ID = id;
        Count = count;
        OnUnlockAction = onUnlockAction;
        IsUnlocked = false;
    }

    public void SetUnlockCondition(Func<int, bool> condition)
    {
        UnlockCondition = condition;
    }
}