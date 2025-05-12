using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct DailyQuestData
{
    public int starCount;
    public DailyQuestType dailyQuestType;
    public int currentAchievementCount;
    public int goalCount;

    public DailyQuestData(int starCount, DailyQuestType dailyQuestType, int currentAchievementCount, int goalCount)
    {
        this.starCount = starCount;
        this.dailyQuestType = dailyQuestType;
        this.currentAchievementCount = currentAchievementCount;
        this.goalCount = goalCount;
    }
    

}
