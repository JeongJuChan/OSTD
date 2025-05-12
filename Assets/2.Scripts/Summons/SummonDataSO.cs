using System;
using UnityEngine;

[Serializable]
public struct SummonUnitInfo
{
    public int quantity;
    public CurrencyType currencyType;
    public int price;

    public SummonUnitInfo(int quantity, CurrencyType currencyType, int price)
    {
        this.quantity = quantity;
        this.currencyType = currencyType;
        this.price = price;
    }
}

[CreateAssetMenu(menuName = "SO/SummonData")]
public class SummonDataSO : ScriptableObject
{
    [SerializeField] private SummonUnitInfo adsSummonInfo;
    [SerializeField] private SummonUnitInfo smallSummonInfo;
    [SerializeField] private SummonUnitInfo largeSummonInfo;

    public SummonUnitInfo AdsSummonInfo => adsSummonInfo;
    public SummonUnitInfo SmallSummonInfo => smallSummonInfo;
    public SummonUnitInfo LargeSummonInfo => largeSummonInfo;
}
