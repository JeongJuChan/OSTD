using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MonsterRewardData", menuName = "SO/MonsterRewardData")]
public class MonsterRewardDataSO : DataSO<MonsterRewardData>
{
    public MonsterRewardData GetData()
    {
        return data;
    }
}
