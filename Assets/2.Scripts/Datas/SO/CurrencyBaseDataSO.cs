using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/CurrencyBaseDataSO")]
public class CurrencyBaseDataSO : ScriptableObject
{
    [SerializeField] CurrencyInfo[] currencies;

    public CurrencyInfo[] Currencies => currencies;
}
