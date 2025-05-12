using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct QuestDatas
{
    public Dictionary<int, QuestInfoData> questDatas;

    public QuestDatas(Dictionary<int, QuestInfoData> questDatas)
    {
        this.questDatas = questDatas;

#if UNITY_EDITOR
        /*foreach (var questData in questDatas)
        {
            Debug.Log("퀘스트 데이터 : " + questData.Key + " : " + questData.Value.Description + " : " + questData.Value.QuestType + " : " + questData.Value.EventQuestType + " : " + questData.Value.GoalCount + " : " + questData.Value.Importance + " : " + questData.Value.RewardType + " : " + questData.Value.RewardAmount);

        }*/
#endif
    }

    public QuestInfoData GetQuestData(int index)
    {
        if (index == 0) index = 1;
        if (!questDatas.ContainsKey(index))
        {
            // Debug.LogError($"Data not found for index: {index}");
            var temp = default(QuestInfoData);
            temp.QuestType = GetRecurringQuestType(index);
            return temp;
        }

        return questDatas[index];
    }

    private QuestType GetRecurringQuestType(int currentNumber)
    {
        var typeNumber = (currentNumber % (Enum.GetValues(typeof(QuestType)).Length - 1)) + 1;
        return (QuestType)typeNumber;
    }
}

public class QuestManager : MonoBehaviourSingleton<QuestManager>
{
    private struct QuestManagerData
    {
        public QuestManagerData(int num, int rep, int type, bool reward)
        {
            structQuestManagerDataSave = true;
            currentNumber = num;
            repeatedIndex = rep;
            currentType = type;
            isRewardAvailable = reward;
        }

        public int currentNumber;
        public int repeatedIndex;
        public int currentType;
        public bool isRewardAvailable;
        public bool structQuestManagerDataSave;
    }

    private int currentNumber;
    public QuestData currentQuest;
    private int repeatIndex;

    private bool isRewardAvailable;

    private EventQuestData eventQuest;

    public event Action<bool> OnRewardAvailable;
    public event Action<int> OnQuestNumberChange;
    public event Action<string, RewardType, int, int> OnQuestChange;    //description, rewardType, rewardAmount
    public event Action<int, int> OnCountChange;

    public event Action<bool> OnQuestStarted;
    public event Action<EventQuestType> OnStepQuestStarted;
    public event Action<bool> OnQuestAchieved;
    public event Action<EventQuestType> OnStepQuestCompleted;

    //public event Action<FeatureType> OnUpdateFeature;
    public event Action<int> OnTryShowDialogue;

    private Dictionary<QuestType, Action> GetQuestTypeAction = new Dictionary<QuestType, Action>();
    private Dictionary<EventQuestType, Action> GetEventQuestTypeAction = new Dictionary<EventQuestType, Action>();

    QuestDatas questDatas;
    int[] repeatQuestUpgradeValues;

    private bool isInitialized;


    public void Initialze()
    {
        if (isInitialized) return;

        LoadDatas();

        //OnUpdateFeature += UnlockManager.Instance.CheckUnlocks;

        isInitialized = true; 
    }

    public void StartInit()
    {
        //UpdateCurrentQuest();
    }

    private void LoadDatas()
    {
        //var repeatQuestUpgradeValueDatas = Resources.Load<RepeatQuestUpgradeValueDataSO>("ScriptableObjects/RepeatQuestUpgradeValueDataSO/RepeatQuestUpgradeValueData");
        //repeatQuestUpgradeValues = repeatQuestUpgradeValueDatas.RepeatQuestUpgradeValues;
        questDatas = DataParser.ParseQuestData(Resources.Load<TextAsset>("CSV/Quest/QuestData"));
        currentQuest = new QuestData(questDatas.GetQuestData(1)); // 4의 의미는 반복퀘스트가 처음 불리는 숫자
        eventQuest = new EventQuestData(questDatas.GetQuestData(12)); // 게임의 첫 퀘스트가 eventQuest이기 때문에 1.
    }

    private bool CheckQuestIndex(int index)
    {
        switch (questDatas.GetQuestData(index).QuestType)
        {
            case QuestType.Event:
                return eventQuest.CheckQuestOnNumber(index);
            default:
                return currentQuest.CheckQuestOnNumber(index);
        }
    }

    public void InitCurrentQuest()
    {
        LoadCurrentQuestInfo();

        GetQuestInfo();

        OnQuestNumberChange?.Invoke(currentNumber);
        OnTryShowDialogue?.Invoke(currentNumber);

        SaveCurrentInfo();
    }

    public void AddQuestTypeAction(QuestType type, Action action)
    {
        if (!GetQuestTypeAction.ContainsKey(type))
        {
            GetQuestTypeAction.Add(type, action);
        }
        else
        {
            GetQuestTypeAction[type] += action;
        }
    }

    public void InvokeQuestTypeAction(QuestType type)
    {
        GetQuestTypeAction[type]?.Invoke();
    }

    public void AddEventQuestTypeAction(EventQuestType type, Action action)
    {
        if (!GetEventQuestTypeAction.ContainsKey(type))
        {
            GetEventQuestTypeAction.Add(type, action);
        }
        else
        {
            GetEventQuestTypeAction[type] += action;
        }
    }

    public void InvokeEventQuestTypeAction(EventQuestType type)
    {
        GetEventQuestTypeAction[type]?.Invoke();
    }

    public void QuestCompleate() // 치트용
    {
        if (questDatas.GetQuestData(currentNumber).QuestType == QuestType.Event)
        {
            eventQuest.QuestCompleate();
        }
        else
        {
            currentQuest.QuestCompleate();
        }
    }

    private void LoadCurrentQuestInfo()
    {
        if (ES3.KeyExists(Consts.QUEST_MANAGER_DATA))
        {
            QuestManagerData load = ES3.Load<QuestManagerData>(Consts.QUEST_MANAGER_DATA);

            currentNumber = load.currentNumber;
            repeatIndex = load.repeatedIndex;

            isRewardAvailable = load.isRewardAvailable;

            if (currentNumber > questDatas.questDatas.Count)
            {
                // 지정된 퀘스트가 끝났을 때 반복 퀘스트
                var questType = GetRecurringQuestType();
                currentQuest.DirectLoad(questType.ToString(), repeatIndex, repeatQuestUpgradeValues[((int)questType)]);

                currentQuest.OnQuestAchieved += SetRewardAvailable;
                currentQuest.OnQuestCountUpdated += UpdateCurrentCount;

                return;
            }

            if (questDatas.GetQuestData(currentNumber).QuestType == QuestType.Event)
            {
                eventQuest.NewQuestInfoData(questDatas.GetQuestData(currentNumber));
                eventQuest.CheckEventInfo(currentNumber);

                eventQuest.OnQuestAchieved += SetRewardAvailable;
                eventQuest.OnQuestCountUpdated += UpdateCurrentCount;
            }
            else
            {
                currentQuest.NewQuestInfoData(questDatas.GetQuestData(currentNumber));

                currentQuest.OnQuestAchieved += SetRewardAvailable;
                currentQuest.OnQuestCountUpdated += UpdateCurrentCount;
            }
        }
        else
        {

            currentNumber = 1;
            repeatIndex = 1; // 왜 -1이였지?

            if (questDatas.GetQuestData(currentNumber).QuestType == QuestType.Event)
            {
                eventQuest.NewQuestInfoData(questDatas.GetQuestData(currentNumber));
                eventQuest.CheckEventInfo(currentNumber);

                eventQuest.OnQuestAchieved += SetRewardAvailable;
                eventQuest.OnQuestCountUpdated += UpdateCurrentCount;
            }
            else
            {
                currentQuest.NewQuestInfoData(questDatas.GetQuestData(currentNumber));

                currentQuest.OnQuestAchieved += SetRewardAvailable;
                currentQuest.OnQuestCountUpdated += UpdateCurrentCount;
            }

            // Firebase.Analytics.FirebaseAnalytics.LogEvent($"current_quest_{(currentNumber)}");
        }
    }

    private void UpdateCurrentQuest()
    {
        currentQuest.OnQuestAchieved -= SetRewardAvailable;
        currentQuest.OnQuestCountUpdated -= UpdateCurrentCount;
        eventQuest.OnQuestAchieved -= SetRewardAvailable;
        eventQuest.OnQuestCountUpdated -= UpdateCurrentCount;

        currentQuest.UpdateIndex();
        eventQuest.UpdateIndex();

        currentNumber++;

        OnTryShowDialogue?.Invoke(currentNumber);

        Debug.Log("currentNumber : " + currentNumber);
        if (currentNumber > questDatas.questDatas.Count)
        {
            // 지정된 퀘스트가 끝났을 때 반복 퀘스트
            var questType = GetRecurringQuestType();

            if (questType == QuestType.MonsterKillCount) repeatIndex++;

            currentQuest.DirectLoad(questType.ToString(), repeatIndex, repeatQuestUpgradeValues[(int)questType]);

            currentQuest.OnQuestAchieved += SetRewardAvailable;
            currentQuest.OnQuestCountUpdated += UpdateCurrentCount;

            GetQuestInfo();

            currentQuest.UpdateIndex(currentNumber);
            //OnUpdateFeature?.Invoke(FeatureType.Quest);

            OnQuestNumberChange?.Invoke(currentNumber);

            // Firebase.Analytics.FirebaseAnalytics.LogEvent($"current_quest_{(currentNumber)}");

            SaveCurrentInfo();

            return;
        }

        if (questDatas.GetQuestData(currentNumber).QuestType == QuestType.Event)
        {
            eventQuest.NewQuestInfoData(questDatas.GetQuestData(currentNumber));
            eventQuest.CheckEventInfo(currentNumber);

            eventQuest.OnQuestAchieved += SetRewardAvailable;
            eventQuest.OnQuestCountUpdated += UpdateCurrentCount;

            currentQuest.Type = QuestType.Event;
        }
        else
        {
            currentQuest.NewQuestInfoData(questDatas.GetQuestData(currentNumber));

            currentQuest.OnQuestAchieved += SetRewardAvailable;
            currentQuest.OnQuestCountUpdated += UpdateCurrentCount;
            currentQuest.DirectSave();
        }

        GetQuestInfo();


        OnQuestNumberChange?.Invoke(currentNumber);


        //OnUpdateFeature?.Invoke(FeatureType.Quest);
        // Firebase.Analytics.FirebaseAnalytics.LogEvent($"current_quest_{(currentNumber)}");

        SaveCurrentInfo();
    }

    private void SetRewardAvailable()
    {
        isRewardAvailable = true;
        OnRewardAvailable?.Invoke(isRewardAvailable);
        OnQuestAchieved?.Invoke(currentQuest.CheckQuestOnNumber(currentNumber));

        if (questDatas.GetQuestData(currentNumber).QuestType == QuestType.Event)
            OnStepQuestCompleted?.Invoke(eventQuest.GetQuestType());

        SaveCurrentInfo();
    }

    private void UpdateCurrentCount(int count, int goalCount)
    {
        OnCountChange?.Invoke(count, goalCount);
    }


    public void UpdateCount(QuestType type, int amount, int importence = -1)
    {
        if (isRewardAvailable) return;
        if (questDatas.GetQuestData(currentNumber).QuestType != type) return;

        currentQuest.AddCount(amount, importence);
    }

    public void UpdateCount(EventQuestType type, int amount, int importence)
    {
        if (isRewardAvailable) return;
        if (questDatas.GetQuestData(currentNumber).QuestType != QuestType.Event) return;
        if (!eventQuest.IsCurrentType(type, importence)) return;

        eventQuest.AddCount(amount, importence);
    }

    public void GiveReward()
    {
        if (currentNumber == 1)
        {
            // Firebase.Analytics.FirebaseAnalytics.LogEvent($"first_startpoint_3");
        }

        if (questDatas.GetQuestData(currentNumber).QuestType == QuestType.Event)
        {
            eventQuest.GiveReward();
        }
        else
        {
            currentQuest.GiveReward();
        }

        // RewardManager.instance.ShowRewardPanel();

        isRewardAvailable = false;
        UpdateCurrentQuest();
    }

    public void GetQuestInfo()
    {
        string desc = "";
        RewardType type = RewardType.None;
        int amount = 0;
        int count = 0;
        int goal = 0;

        if (questDatas.GetQuestData(currentNumber).QuestType == QuestType.Event)
        {
            if (eventQuest.CheckQuestOnNumber(currentNumber))
            {
                OnQuestStarted?.Invoke(true);
                OnStepQuestStarted?.Invoke(eventQuest.GetQuestType());
            }
            if (eventQuest.CheckCountResetNeeded())
                InvokeEventQuestTypeAction(eventQuest.GetQuestType());
            // GetEventQuestTypeAction[eventQuest.GetQuestType()]?.Invoke();

            desc = eventQuest.GetCurrentDescription();
            type = eventQuest.GetCurrentRewardType();
            amount = eventQuest.GetCurrentRewardAmount();
            count = eventQuest.GetCurrentCount();
            goal = eventQuest.GetCurrentGoalCount();

        }
        else
        {
            if (currentQuest.IstheFirstQuestOftheType(currentNumber))
            {
                OnQuestStarted?.Invoke(true);
            }
            if (currentQuest.CheckCountResetNeeded())
                InvokeQuestTypeAction(currentQuest.Type);
            // GetQuestTypeAction[currentQuest.Type]?.Invoke();

            desc = currentQuest.GetCurrentDescription();
            type = currentQuest.GetCurrentRewardType();
            amount = currentQuest.GetCurrentRewardAmount();
            count = currentQuest.GetCurrentCount();
            goal = currentQuest.GetCurrentGoalCount();
            Debug.Log("변환 ! " + desc + " " + type + " " + amount + " " + count + " " + goal);
        }

        OnQuestChange?.Invoke(desc, type, amount, currentNumber);
        OnCountChange?.Invoke(count, goal);
        OnRewardAvailable?.Invoke(isRewardAvailable);
        if (isRewardAvailable)
        {
            eventQuest.DeactivateGuides();
        }
    }

    public void GetQuestNumber()
    {
        OnQuestNumberChange?.Invoke(currentNumber);
    }

    public void GetGuid()
    {
        OnQuestStarted?.Invoke(true);
        if (currentQuest.Type == QuestType.Event)
        {
            OnStepQuestStarted?.Invoke(eventQuest.GetQuestType());
        }
    }

    public int GetImportence()
    {
        return currentQuest.GetCurrentImportance();
    }

    public string GetTargetDescription(int targetIndex)
    {
        var target = questDatas.GetQuestData(targetIndex);

        return target.GetDescription();
    }

    public QuestType GetQuestType()
    {
        return currentQuest.Type;
    }

    public EventQuestType GetEventQuestType()
    {
        return eventQuest.GetQuestType();
    }

    public bool CheckQuestIndex()
    {
        currentQuest.UpdateIndex(currentNumber);
        return currentQuest.CheckQuestOnNumber(currentNumber);
    }

    public bool CheckQuestType(QuestType type)    // 현재 진행 중인 퀘스트가 인자로 받은 퀘스트 타입인지
    {
        return questDatas.GetQuestData(currentNumber).QuestType == type;
    }

    public bool CheckQuestOnNumber(int questNumber)    // 인자로 받은 번호의 퀘스트가 지금 최초로 시작되었는지 여부를 return (달성되지 않은 경우)
    {
        return !currentQuest.CheckQuestAvailability(questNumber) && !isRewardAvailable;
    }

    public bool CheckQuestType(EventQuestType type, int importence)
    {
        
        if (questDatas.GetQuestData(currentNumber).QuestType != QuestType.Event) return false;
        return eventQuest.IsCurrentType(type, importence);
    }

    //  

    public bool IsQuestOnNumber(QuestType type)   // 인자로 받은 타입의 퀘스트가 지금 최초로 시작되었는지 여부를 return (달성되지 않은 경우)
    {
        return currentQuest.CheckQuestOnNumber(currentNumber) && !isRewardAvailable;
    }

    public bool IsQuestOnNumber(EventQuestType type)
    {
        return eventQuest.CheckQuestOnNumber(currentNumber) && !isRewardAvailable;
    }

    private void SaveCurrentInfo()
    {
        QuestManagerData save = new QuestManagerData(
            num: currentNumber,
            rep: repeatIndex,
            type: currentNumber > questDatas.questDatas.Count ? ((int)GetRecurringQuestType()) : ((int)questDatas.GetQuestData(currentNumber).QuestType),
            reward: isRewardAvailable
            );

        ES3.Save<QuestManagerData>(Consts.QUEST_MANAGER_DATA, save, ES3.settings);
        ES3.StoreCachedFile();
        // ES3.DirectSave("QuestManager_Data", save);


        // isSaveNeeded = true;
    }

    public QuestType GetRecurringQuestType()
    {
        var typeNumber = (currentNumber % (Enum.GetValues(typeof(QuestType)).Length - 1)) + 1;
        var questType = (QuestType)typeNumber;

        return questType;
    }

    private void OnApplicationPause(bool pause)
    {
        // TODO: 퀘스트 살려내기
        //SaveCurrentInfo();
    }
}