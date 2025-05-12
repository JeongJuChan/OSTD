using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

struct Reward
{
    public RewardType rewardType;
    public int amount;
}

public class UI_OffLineReward : UI_Base
{
    [SerializeField] Button confirmBtn;

    [SerializeField] Text timeSpanText;
    [SerializeField] Slider timeSpanSlider;
    [SerializeField] Text monsterCountText;
    [SerializeField] Transform slotArea;

    private RewardManager rewardManager;

    private List<Reward> rewards;
    private List<RewardSlot> slots;
    private OfflineRewardConfig offlineRewardConfig;


    public void Initialize()
    {
        SetReferences();
        AddCallbacks();

        // Resources.Load로 불러오기
        TextAsset offlineRewardText = Resources.Load<TextAsset>("CSV/OfflineReward/OfflineReward CSV");
        offlineRewardConfig = new OfflineRewardConfig(offlineRewardText.text);
    }

    public int GetMaxTime()
    {
        return offlineRewardConfig.MaxTime;
    }

    public int GetMinTime()
    {
        return offlineRewardConfig.MinTime;
    }

    private void SetReferences()
    {
        // stageManager = StageController.;
        rewardManager = RewardManager.instance;

        slots = new List<RewardSlot>();
    }

    private void AddCallbacks()
    {
        confirmBtn.onClick.RemoveAllListeners();
        confirmBtn.onClick.AddListener(GiveReward);
    }

    public void SetUI(int killCount, int timePasssed)
    {
        int hour = timePasssed / 3600;
        int minute = (timePasssed % 3600) / 60;
        int second = ((timePasssed % 3600) % 60);

        string time = "";
        if (hour > 0) time += $"{hour}시간 ";
        if (minute > 0) time += $"{minute}분 ";
        if (second > 0) time += $"{second}초";

        timeSpanText.text = time;
        timeSpanSlider.value = (float)timePasssed / 28800;

        monsterCountText.text = $"{killCount}";

        SetReward(killCount);
        SetSlots();
    }

    private void SetReward(int killCount)
    {
        //TODO: 이 곳에서 스테이지의 보상을 설정해준다.
        // rewardManager.offlineRewardCallback?.Invoke(ES3.Load<int>(Consts.CURRENT_DIFFICULTY, 1), ES3.Load<int>(Consts.CURRENT_MAIN_STAGE_NUM, 1), ES3.Load<int>(Consts.CURRENT_SUB_STAGE_NUM, 1));

        //결과값 = (시작 + (레벨당 * 레벨))*계수% /100 + (시작 + (레벨당 * 레벨))
        int offlineForgeRewardAmount = (offlineRewardConfig.StartGates + (offlineRewardConfig.GatesPerLevel * killCount)) * offlineRewardConfig.GatesPerLevel / 100 + (offlineRewardConfig.StartGates + (offlineRewardConfig.GatesPerLevel * killCount));
        int offlineGoldRewardAmount = (offlineRewardConfig.BaseGold + (offlineRewardConfig.GoldPerLevel * killCount)) * offlineRewardConfig.GoldPerLevel / 100 + (offlineRewardConfig.BaseGold + (offlineRewardConfig.GoldPerLevel * killCount));
        int offlineLevelUpStoneRewardAmount = (offlineRewardConfig.BaseLevelUpStone + (offlineRewardConfig.LevelUpStonePerLevel * killCount)) * offlineRewardConfig.LevelUpStonePerLevel / 100 + (offlineRewardConfig.BaseLevelUpStone + (offlineRewardConfig.LevelUpStonePerLevel * killCount));

        rewards = new List<Reward>()
        {
            new Reward { rewardType = RewardType.Gold, amount = offlineGoldRewardAmount },
        };
    }

    private void SetSlots()
    {
        foreach (RewardSlot slot in slots)
        {
            Destroy(slot.gameObject);
        }

        slots.Clear();

        foreach (Reward reward in rewards)
        {
            // IRewardInfo data = rewardManager.GetRewardInfo(reward.rewardType);
            // RewardSlot slot = rewardManager.GetRewardSlot(reward.rewardType, reward.amount);
            // slot.transform.SetParent(slotArea);
            // slot.transform.localScale = Vector3.one;

            // // slot.SetUI(rewardManager.GetRewardInfo(reward.rewardType), reward.amount);
            // slot.gameObject.SetActive(true);
            // slots.Add(slot);
        }
    }


    private void GiveReward()
    {
        foreach (Reward reward in rewards)
        {
            // rewardManager.GiveReward(reward.rewardType, reward.amount);
        }
        // rewardManager.ShowRewardPanel();
        CloseUI();
    }
}