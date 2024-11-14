using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventQuestData : QuestData
{
    public EventQuestType eventQuestType;

    public EventQuestData(QuestInfoData data) : base(data)
    {
        this.data = data;
    }

    public override void NewQuestInfoData(QuestInfoData data)
    {
        base.NewQuestInfoData(data);
        eventQuestType = data.EventQuestType;
    }

    public bool CheckEventInfo(int number)
    {
        int index = data.Index;

        if (index > -1)
        {
            goalCount = data.GoalCount;
            return true;
        }
        else
        {
            return false;
        }
    }

    public override void GiveReward()
    {
        // RewardManager.instance.GiveReward(data.RewardType, data.RewardAmount);
    }

    public EventQuestType GetQuestType()
    {
        return data.EventQuestType;
    }

    public bool IsCurrentType(EventQuestType type, int importance)
    {
        return type == data.EventQuestType && data.Importance == importance;
    }

    public override bool CheckAddCountCondition(int importance)
    {
        if (data.Importance != importance) return false;
        if (data.CountResetNeeded) count = 0;

        return true;
    }

    public override bool CheckQuestAchievedState(int count, int goalcount)
    {
        return (count >= data.GoalCount);
    }

    public override void UpdateIndex()
    {
        count = 0;
        SaveDatas();
    }

    public override int GetCurrentGoalCount()
    {
        return data.GoalCount;
    }

    public override int GetCurrentCount()
    {
        return count;
    }

    public override string GetCurrentDescription()
    {
        return data.GetDescription();
    }

    public override RewardType GetCurrentRewardType()
    {
        return data.RewardType;
    }

    public override int GetCurrentRewardAmount()
    {
        return data.RewardAmount;
    }

    public override bool CheckQuestOnNumber(int questNumber)
    {
        return questNumber == data.Index;//&& data.IstheFirstQuestOftheType
    }
}