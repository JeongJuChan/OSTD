using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "UnlockData", menuName = "SO/UnlockData")]
public class UnlockDataSO : ListDataSO<UnlockData>
{
    private Dictionary<FeatureID, UnlockData> unlockDataDict = new Dictionary<FeatureID, UnlockData>();
    /*private List<UnlockData> forgeUnlockDatas = new List<UnlockData>();
    private List<UnlockData> skillUnlockDatas = new List<UnlockData>();

    private const string FORGE_STR = "ForgeSlot";
    private const string SKILL_STR = "SkillSlot";*/

    private List<UnlockData> colleagueUnlockDatas = new List<UnlockData>();
    private const string COLLEAGUE_STR = "ColleagueSlot";


    public override void InitDict()
    {
        unlockDataDict.Clear();
        colleagueUnlockDatas.Clear();
        /*forgeUnlockDatas.Clear();
        skillUnlockDatas.Clear();*/

        foreach (var data in datas)
        {
            if (!unlockDataDict.ContainsKey(data.featureID))
            {
                unlockDataDict.Add(data.featureID, data);
                string featureIDStr = data.featureID.ToString();
                if (featureIDStr.Contains(COLLEAGUE_STR))
                {
                    colleagueUnlockDatas.Add(data);
                }
                /*if (featureIDStr.Contains(FORGE_STR))
                {
                    forgeUnlockDatas.Add(data);
                }
                else if (featureIDStr.Contains(SKILL_STR))
                {
                    skillUnlockDatas.Add(data);
                }*/
            }
        }
    }

    public List<UnlockData> GetColleagueUnlockDatas()
    {
        return colleagueUnlockDatas;
    }

    /*public List<UnlockData> GetForgeUnlockDatas()
    {
        return forgeUnlockDatas;
    }

    public List<UnlockData> GetSkillUnlockDatas()
    {
        return skillUnlockDatas;
    }*/

    public UnlockData GetUnlockData(FeatureID featureID)
    {
        if (unlockDataDict.Count == 0)
        {
            InitDict();
        }

        if (!unlockDataDict.ContainsKey(featureID))
        {
            return default;
        }

        return unlockDataDict[featureID];
    }
}
