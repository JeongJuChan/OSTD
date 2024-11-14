using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SummonInfo : UI_Base
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Button prevBtn;
    [SerializeField] private Button nextBtn;
    [SerializeField] private Image[] rankImages;
    [SerializeField] private TextMeshProUGUI[] rankTitles;
    [SerializeField] private TextMeshProUGUI[] proportionTitles;
    [SerializeField] private TextMeshProUGUI[] proportions;
    [SerializeField] private Button closeBtn;

    private Dictionary<SummonType, SummonProbabilityDataSO> dataDic;

    private SummonType currentType;

    int currentLevel;
    int maxLevel;

    public void Initialize()
    {
        SetCollections();
        AddCallbacks();
        SetTextsInfo();
        maxLevel = ResourceManager.instance.rank.GetProbabilityMaxLevel();
    }

    private void SetTextsInfo()
    {
        Rank[] ranks = (Rank[])Enum.GetValues(typeof(Rank));

        for (int i = 1; i < ranks.Length; i++)
        {
            Color color = ResourceManager.instance.rank.GetRankColor(ranks[i]);
            rankTitles[i - 1].text = EnumUtility.GetRankKR(ranks[i]);
            proportionTitles[i - 1].color = color;
            proportions[i - 1].color = color;
            rankImages[i - 1].sprite = ResourceManager.instance.rank.GetRankBackgroundSprite(ranks[i]);
        }        
    }

    private void SetCollections()
    {
        dataDic = new Dictionary<SummonType, SummonProbabilityDataSO>();
    }

    private void AddCallbacks()
    {
        closeBtn.onClick.AddListener(CloseUI);
        prevBtn.onClick.AddListener(DecreaseLevel);
        nextBtn.onClick.AddListener(IncreaseLevel);
    }

    public void ShowUI(SummonType type, int level = 1)
    {
        titleText.text = $"{GetSummonTypeKR(type)} 소환 확률";

        GetProportionData(type);
        currentType = type;

        currentLevel = level;

        prevBtn.enabled = true;
        nextBtn.enabled = (currentLevel < maxLevel);

        ChangeContents();
        OpenUI();
    }

    private string GetSummonTypeKR(SummonType type)
    {
        switch (type)
        {
            case SummonType.Equipment:
                return "장비";
            default:
                return "";
        }
    }


    private void GetProportionData(SummonType type)
    {
    }

    private void IncreaseLevel()
    {
        currentLevel++;
        currentLevel = Mathf.Min(currentLevel, maxLevel);


        prevBtn.enabled = true;
        nextBtn.enabled = (currentLevel < maxLevel);

        ChangeContents();
    }

    private void DecreaseLevel()
    {
        currentLevel--;

        currentLevel = Mathf.Max(currentLevel, 1);

        prevBtn.enabled = (currentLevel > 1);
        nextBtn.enabled = true;

        ChangeContents();
    }

    private void ChangeContents()
    {
        levelText.text = $"소환 레벨 {currentLevel}";

        int[] currentProportions = ResourceManager.instance.rank.GetCurrentProportion(currentLevel);

        for (int i = 0; i < proportions.Length; i++)
        {
            float currentProportion;
            if (currentType != SummonType.Equipment)
            {
                currentProportion = (float)currentProportions[i] / Consts.THOUSAND_DIVIDE_VALUE;
            }
            else
            {
                currentProportion = (float)currentProportions[i] / Consts.THOUSAND_DIVIDE_VALUE;
            }
            proportions[i].text = (currentProportion != 0) ? $"{currentProportion}%" : "-";
        }
    }
}