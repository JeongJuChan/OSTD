using UnityEngine;

public interface IRewardInfo
{
    Sprite GetIcon();
    Color GetBackground();
}

public class GoldRewardInfo : IRewardInfo
{
    public Sprite GetIcon() => CurrencyManager.instance.GetCurrency(CurrencyType.Gold).GetIcon();
    public Color GetBackground() => ResourceManager.instance.rank.GetRankColor(Rank.Common);
}

public class EquipmentRewardInfo : IRewardInfo
{
    private EquipmentType type;
    private Rank rank;
    private int index;
    public string name { get; private set; }

    public EquipmentRewardInfo(EquipmentType type, Rank rank, int index)
    {
        this.type = type;
        this.rank = rank;
        this.index = index;

        name = $"{type}_{rank}_{index}";
    }

    public Sprite GetIcon() => Resources.Load<Sprite>($"Sprites/Equipments/{type}/{name}");
    public Color GetBackground() => ResourceManager.instance.rank.GetRankColor(rank);
}
