using System;
using System.Collections.Generic;
using UnityEngine;

public struct QuestInfoData
{
    public int Index;
    public string Description;
    public QuestType QuestType;
    public EventQuestType EventQuestType;
    public int GoalCount;
    public int Importance;
    public RewardType RewardType;
    public int RewardAmount;
    public bool CountResetNeeded;
    public bool IstheFirstQuestOftheType;

    public QuestInfoData(int index, string description, QuestType questType, EventQuestType eventQuestType, int goalCount, int importance, RewardType rewardType, int rewardAmount, bool countResetNeeded, bool istheFirstQuestOftheType)
    {
        Index = index;
        Description = description;
        QuestType = questType;
        EventQuestType = eventQuestType;
        GoalCount = goalCount;
        Importance = importance;
        RewardType = rewardType;
        RewardAmount = rewardAmount;
        CountResetNeeded = countResetNeeded;
        IstheFirstQuestOftheType = istheFirstQuestOftheType;
    }

    public string GetDescription()
    {
        string s = Description.Replace("n", ReplaceNewLine(QuestType, GoalCount));
        return s.Replace("#", Environment.NewLine);

    }
    private string ReplaceNewLine(QuestType questType, int goalCount)
    {
        if (questType == QuestType.StageClear)
        {
            return TransformStageNumber(goalCount);
        }
        else
        {
            return goalCount.ToString();
        }
    }

    private string TransformStageNumber(int stageNum)
    {
        string s = stageNum.ToString();

        string P = s.Length > 3 ? s.Substring(0, s.Length - 3) : "";
        string X = s.Length > 2 ? s.Substring(s.Length - 3, 1) : "";
        string YZ = s.Length > 1 ? s.Substring(s.Length - 2) : s;

        return $"{Difficulty.GetDifficulty(int.Parse(P))}\n{X}-{int.Parse(YZ)}";
    }
}
public class QuestData
{


    // protected RewardManager rewardManager;
    protected QuestManager questManager;

    public QuestType Type;
    protected QuestInfoData data;

    protected int count;
    protected int goalCount;
    private int index;

    public event Action<int, int> OnQuestCountUpdated;
    public event Action OnQuestAchieved;

    // protected List<Reward> rewards;

    public QuestData(QuestInfoData data)
    {
        // this.rewardManager = rewardManager;
        this.questManager = QuestManager.instance;

        // rewards = new List<Reward>();

        this.data = data;
        this.Type = data.QuestType;

        LoadDatas();
    }

    public void QuestCompleate()
    {
        AddCount(data.GoalCount, data.Importance);
    }

    public virtual void NewQuestInfoData(QuestInfoData data)
    {
        this.data = data;
        this.Type = data.QuestType;

        count = 0;
        goalCount = data.GoalCount;

        LoadDatas();
    }

    public virtual void AddCount(int amount, int importance)
    {
        if (!CheckAddCountCondition(importance)) return;

        count += amount;
        OnQuestCountUpdated?.Invoke(count, goalCount);

        if (CheckQuestAchievedState(count, goalCount))
            OnQuestAchieved?.Invoke();

        SaveDatas();
    }

    public void DeactivateGuides()
    {
        OnQuestAchieved?.Invoke();
    }

    public virtual bool CheckAddCountCondition(int grade)
    {
        if (data.Importance != grade) return false;
        if (data.CountResetNeeded) count = 0;

        return true;
    }

    public virtual bool CheckQuestAchievedState(int count, int goalcount)
    {
        return (count >= goalcount);
    }

    public virtual void UpdateIndex()
    {
        index = data.Index;
        goalCount = data.GoalCount;
        count = 0;

        SaveDatas();
    }

    public virtual void GiveReward()
    {
        // RewardManager.instance.GiveReward(data.RewardType, data.RewardAmount);
    }

    public virtual int GetCurrentGoalCount()
    {
        return data.GoalCount;
    }

    public virtual int GetCurrentCount()
    {
        return count;
    }

    public int GetCurrentImportance()
    {
        return data.Importance;
    }

    public virtual string GetCurrentDescription()
    {
        return data.GetDescription();
    }

    public virtual RewardType GetCurrentRewardType()
    {
        return data.RewardType;
    }

    public virtual int GetCurrentRewardAmount()
    {
        return data.RewardAmount;
    }

    public virtual bool CheckQuestOnNumber(int questNumber)
    {
        return questNumber == data.Index;
    }

    public virtual bool IstheFirstQuestOftheType(int questNumber)
    {
        return questNumber == data.Index && data.IstheFirstQuestOftheType;
    }

    public bool CheckQuestAvailability(int questNumber)
    {
        return questNumber > data.Index;
    }

    public bool IsCurrentType()
    {
        return questManager.currentQuest.Type == Type;
    }

    public void UpdateIndex(int index)
    {
        data.Index = index;
    }

    public bool CheckCountResetNeeded()
    {
        return data.CountResetNeeded;
    }

    protected virtual void LoadDatas()
    {
        count = (ES3.KeyExists($"{Consts.QUEST_PREFIX}{Type}{Consts.QUEST_COUNT}")) ? ES3.Load<int>($"{Consts.QUEST_PREFIX}{Type}{Consts.QUEST_COUNT}") : 0;
        index = (ES3.KeyExists($"{Consts.QUEST_PREFIX}{Type}{Consts.QUEST_CURRENT_INDEX}")) ? ES3.Load<int>($"{Consts.QUEST_PREFIX}{Type}{Consts.QUEST_CURRENT_INDEX}") : 0;

        goalCount = data.GoalCount;
    }

    protected virtual void SaveDatas()
    {
        ES3.Save($"{Consts.QUEST_PREFIX}{Type}{Consts.QUEST_COUNT}", count, ES3.settings);
        ES3.Save($"{Consts.QUEST_PREFIX}{Type}{Consts.QUEST_CURRENT_INDEX}", index, ES3.settings);

        ES3.StoreCachedFile();
    }


    public void DirectSave()
    {
        ES3.Save(Type.ToString(), new QuestDataSave(
            data.Index,
            data.Description,
            data.QuestType.ToString(),
            data.GoalCount,
            data.Importance,
            data.RewardType.ToString(),
            data.RewardAmount,
            data.CountResetNeeded
        ), ES3.settings);

        ES3.StoreCachedFile();
    }

    public void DirectLoad(string type, int repeatIndex = 0, int repeatUpgradeValue = 0)
    {
        QuestDataSave save = ES3.Load<QuestDataSave>(type.ToString());
        Type = (QuestType)Enum.Parse(typeof(QuestType), type);
        data.Index = save.Index;
        data.Description = save.Description;
        data.QuestType = (QuestType)Enum.Parse(typeof(QuestType), save.QuestType);
        if (type == QuestType.StageClear.ToString())
        {
            // Extract parts of the number: ab c d
            int difficulty = save.GoalCount / 1000;              // Get the hundreds part
            int chapter = (save.GoalCount / 100) % 10;           // Get the tens part
            int stage = save.GoalCount % 100;                    // Get the units part

            // Increment the stage and manage overflows
            stage += repeatUpgradeValue * repeatIndex;
            while (stage > 20)
            {
                stage -= 20;
                chapter += 1;
                if (chapter > 5)
                {
                    chapter = 1;
                    difficulty += 1;
                }
            }

            // Recompose the number: ab c d
            save.GoalCount = difficulty * 1000 + chapter * 100 + stage;

            data.GoalCount = save.GoalCount;
        }
        else
        {
            data.GoalCount = save.GoalCount + (repeatUpgradeValue * repeatIndex);
        }
        data.Importance = save.Importance;
        data.RewardType = (RewardType)Enum.Parse(typeof(RewardType), save.RewardType);
        data.RewardAmount = save.RewardAmount;
        data.CountResetNeeded = save.CountResetNeeded;

        LoadDatas();
    }

    [Serializable]
    private struct QuestDataSave
    {
        public QuestDataSave(int index, string description, string questType, int goalCount, int importance, string rewardType, int rewardAmount, bool countResetNeeded)
        {
            Index = index;
            Description = description;
            QuestType = questType;
            GoalCount = goalCount;
            Importance = importance;
            RewardType = rewardType;
            RewardAmount = rewardAmount;
            CountResetNeeded = countResetNeeded;
            structQuestDataSave = true;
        }

        public int Index;
        public string Description;
        public string QuestType;
        public int GoalCount;
        public int Importance;
        public string RewardType;
        public int RewardAmount;
        public bool CountResetNeeded;
        public bool structQuestDataSave;

    }
}