using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MonsterRewardIncrementData", menuName = "SO/MonsterRewardIncrementData")]
public class MonsterRewardIncrementDataSO : DataSO<MonsterRewardIncrementData>
{
    public MonsterRewardIncrementData GetData()
    {
        return data;
    }
}
