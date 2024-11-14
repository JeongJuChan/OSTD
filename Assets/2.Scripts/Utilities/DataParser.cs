using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Keiwando.BigInteger;
using UnityEngine;

[Serializable]
public struct OfflineRewardConfig
{
    public int StartGates { get; private set; }
    public int GatesPerLevel { get; private set; }
    public float GatesFactorPercent { get; private set; }
    public int BaseGold { get; private set; }
    public int GoldPerLevel { get; private set; }
    public float GoldFactorPercent { get; private set; }
    public int BaseLevelUpStone { get; private set; }
    public int LevelUpStonePerLevel { get; private set; }
    public float LevelUpStoneFactorPercent { get; private set; }
    public int MaxTime { get; private set; }
    public int MinTime { get; private set; }

    public OfflineRewardConfig(string csvLine)
    {
        string[] rows = csvLine.Split('\n');
        string[] values = rows[1].Split(',');
        StartGates = int.Parse(values[0]);
        GatesPerLevel = int.Parse(values[1]);
        GatesFactorPercent = float.Parse(values[2]);
        BaseGold = int.Parse(values[3]);
        GoldPerLevel = int.Parse(values[4]);
        GoldFactorPercent = float.Parse(values[5]);
        BaseLevelUpStone = int.Parse(values[6]);
        LevelUpStonePerLevel = int.Parse(values[7]);
        LevelUpStoneFactorPercent = float.Parse(values[8]);
        MaxTime = int.Parse(values[9]);
        MinTime = int.Parse(values[10]);
    }
}

public class DataParser
{
    public static QuestDatas ParseQuestData(TextAsset data)
    {
        if (data == null)
        {
            throw new ArgumentNullException("data", "Provided TextAsset for quest data is null.");
        }

        var questDatas = new Dictionary<int, QuestInfoData>();
        string[] lines = data.text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        for (int i = 1; i < lines.Length; i++) // start at 1 to skip header if present
        {
            string[] fields = lines[i].Split(',');


            try
            {
                QuestInfoData questData = new QuestInfoData
                {
                    Index = int.Parse(fields[0].Trim()),
                    Description = fields[1].Trim(),
                    QuestType = (QuestType)Enum.Parse(typeof(QuestType), fields[2].Trim(), true),
                    EventQuestType = ChangeEventQuestType(fields[3].Trim()),
                    GoalCount = int.Parse(fields[4].Trim()),
                    Importance = int.Parse(fields[5].Trim()),
                    RewardType = (RewardType)Enum.Parse(typeof(RewardType), fields[6].Trim(), true),
                    RewardAmount = int.Parse(fields[7].Trim()),
                    CountResetNeeded = bool.Parse(fields[8].Trim()),
                    IstheFirstQuestOftheType = bool.Parse(fields[9].Trim())
                };

                questDatas.Add(questData.Index, questData);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to parse line {i + 1}: {ex.Message}");
            }
        }

        return new QuestDatas(questDatas);
    }

    private static EventQuestType ChangeEventQuestType(string type)
    {
        return string.IsNullOrEmpty(type) ? EventQuestType.None : (EventQuestType)Enum.Parse(typeof(EventQuestType), type, true);
    }

    public static Dictionary<Rank, double> ParseOptionGradePercentageData(TextAsset data)
    {
        var gradePercentage = new Dictionary<Rank, double>();

        string[] lines = data.text.Split('\n');

        for (int i = 1; i < lines.Length; i++)
        {
            string[] fields = lines[i].Split(',');

            Rank grade = Enum.Parse<Rank>(fields[0]);
            double percentage = double.Parse(fields[1]);

            gradePercentage[grade] = percentage;
        }

        return gradePercentage;
    }
}
