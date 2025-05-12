using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UI_QuestBar : UI_Base
{
    private QuestManager questManager;

    [SerializeField] TextMeshProUGUI descText;
    [SerializeField] TextMeshProUGUI countText;
    [SerializeField] TextMeshProUGUI questNumberText;
    [SerializeField] Image icon;

    [SerializeField] Button questButton;
    [SerializeField] TextMeshProUGUI rewardText;
    // [SerializeField] GameObject rewardDot;
    [SerializeField] GameObject clearGlow;
    [SerializeField] Transform iconArea;

    Sequence buttonAnim;

    bool isRewardAvailable;

    Dictionary<RewardType, GameObject> rewardIconDic = new Dictionary<RewardType, GameObject>();

    UIAnimations UIAnimations;

    private void Start()
    {
        Initialize(QuestManager.instance);
    }

    private void Initialize(QuestManager questManager)
    {
        this.questManager = questManager;

        UIAnimations = UIAnimations.instance;

        /*if (!ES3.Load<bool>(Consts.FirstOpen, false, ES3.settings))
        {
            DialogManager.instance.OnUnlockQuestUI += UnlockQuestPanel;
            questButton.interactable = false;
        }*/

        AddCallbacks();
        questManager.GetQuestInfo();
        questManager.GetQuestNumber();
    }

    private void UnlockQuestPanel()
    {
        questButton.interactable = true;
    }

    private void AddCallbacks()
    {
        questManager.OnQuestChange += UpdateQuestInfoUI;
        questManager.OnCountChange += UpdateCountUI;
        questManager.OnRewardAvailable += ActivateRewardButtonUI;

        questButton.onClick.AddListener(QuestButtonCallback);
    }

    private void RemoveCallbacks()
    {
        questManager.OnQuestChange -= UpdateQuestInfoUI;
        questManager.OnCountChange -= UpdateCountUI;
        questManager.OnRewardAvailable -= ActivateRewardButtonUI;

        questButton.onClick.RemoveAllListeners();
    }

    private void UpdateQuestInfoUI(string desc, RewardType type, int amount, int currentNumber)
    {
        descText.text = desc;
        rewardText.text = $"X{amount}";
        questNumberText.text = $"<color=green>Quest {currentNumber}</color>";

        // icon.sprite = RewardManager.instance.GetIcon(type);

        if (!rewardIconDic.ContainsKey(type))
        {
            Debug.Log("너 뭔데 ?" + type);
            // GameObject prefab = RewardManager.Instance.GetIcon(type);
            // GameObject icon = Instantiate(prefab, iconArea);
            // rewardIconDic[type] = icon;
        }

        foreach (KeyValuePair<RewardType, GameObject> kvp in rewardIconDic)
        {
            if (kvp.Key == type) kvp.Value.SetActive(true);
            else kvp.Value.SetActive(false);
        }
        if (questManager.currentQuest.Type == QuestType.StageClear && questManager.CheckQuestIndex())
            countText.gameObject.SetActive(false);
        else countText.gameObject.SetActive(true);
    }

    private void UpdateCountUI(int currentCount, int goalCount)
    {
        countText.text = $"{currentCount} / {goalCount}";
    }

    private void QuestButtonCallback()
    {
        if (isRewardAvailable)
        {
            //if (!ES3.KeyExists(Consts.DIALOGUE_TYPE_ACQUIRE_COMPANION)) return;

            if (buttonAnim == null) buttonAnim = UIAnimations.PucnchScaleEmphasize(questButton.gameObject);
            buttonAnim.Restart();

            GiveReward();
        }
        else questManager.GetGuid();
    }

    private void ActivateRewardButtonUI(bool isActivating)
    {
        isRewardAvailable = isActivating;
        clearGlow.SetActive(isActivating);
    }

    private void GiveReward()
    {
        questManager.GiveReward();
    }

    private void OnDestroy()
    {
        RemoveCallbacks();
    }
}
