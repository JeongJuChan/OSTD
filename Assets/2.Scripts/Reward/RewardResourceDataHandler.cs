using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class RewardResourceDataHandler
{
    private Dictionary<int, Dictionary<int, List<RewardData>>> stageRewardDict = new Dictionary<int, Dictionary<int, List<RewardData>>>();

    private GameData rewardData;
    private GameData rewardFixedData;

    public void Init()
    {
        rewardData = Resources.Load<GameData>($"{Consts.GAME_DATA}/RewardData");
        rewardFixedData = Resources.Load<GameData>($"{Consts.GAME_DATA}/RewardFixedData");

        List<SerializableRow> serializableRows = rewardData.GetDataRows();

        List<RewardData> stageClearRewards = new List<RewardData>();

        string[] clearElements = rewardFixedData.GetDataRows()[0].rowData[0].Split();
        for (int i = 0; i < clearElements.Length; i++)
        {
            string amountStr = GetNumericStr(clearElements[i]);
            string clearElement = clearElements[i];
            string str = clearElement.Remove(clearElement.Length -amountStr.Length, amountStr.Length);

            RewardType rewardType = EnumUtility.GetEqualValue<RewardType>(str);
            int amount = int.Parse(amountStr);
            stageClearRewards.Add(new RewardData(rewardType, amount));
        }

        int stageNum = 0;
        for (int i = 0; i < serializableRows.Count; i++)
        {
            List<string> elements = serializableRows[i].rowData;

            if (elements[0] != null && elements[0] != "")
            {
                stageNum = int.Parse(elements[0]);
            }

            int checkPointNum = int.Parse(elements[1]);

            string[] rewards = elements[2].Split();

            if (!stageRewardDict.ContainsKey(stageNum))
            {
                stageRewardDict.Add(stageNum, new Dictionary<int, List<RewardData>>());
            }

            if (!stageRewardDict[stageNum].ContainsKey(checkPointNum))
            {
                stageRewardDict[stageNum].Add(checkPointNum, new List<RewardData>());
            }

            for (int k = 0; k < rewards.Length; k++)
            {
                string amountStr = GetNumericStr(rewards[k]);
                string str = rewards[k].Remove(rewards[k].Length - amountStr.Length, amountStr.Length);

                RewardType rewardType = EnumUtility.GetEqualValue<RewardType>(str);

                if (rewardType == RewardType.StageClearReward)
                {
                    stageRewardDict[stageNum][checkPointNum].AddRange(stageClearRewards);
                }
                else
                {
                    int amount = int.Parse(amountStr);
                    RewardData tempData = new RewardData(rewardType, amount);
                    stageRewardDict[stageNum][checkPointNum].Add(tempData);
                }
            }
        }
    }

    public List<RewardData> GetRewardDatas(int stageNum, int checkPointNum)
    {
        if (stageRewardDict.ContainsKey(stageNum))
        {
            if (stageRewardDict[stageNum].ContainsKey(checkPointNum))
            {
                return stageRewardDict[stageNum][checkPointNum];
            }
        }

        return default;
    }

    private string GetNumericStr(string str)
    {
        return Regex.Replace(str, @"\D", "");
    }
}
