using System;
using System.Collections;
using System.Collections.Generic;
using Keiwando.BigInteger;
using UnityEngine;

public class UI_HeroEquipmentController : UI_Base
{
    [SerializeField] private UI_EquipmentButton[] ui_EquipmentButtons;
    [SerializeField] private EquipmentStatData[] equipmentStatDatas;

    [SerializeField] private UI_HeroEquipmentViewer ui_HeroEquipmentViewer;

    private Hero hero;

    private EquipmentResourceDataHandler equipmentResourceDataHandler;
    private RankDataHandler rankDataHandler;

    public event Action<int, EquipmentType> OnUpdateEquipmentTitle;
    public event Action<Sprite, Sprite, string, string, Sprite, string, BigInteger, BigInteger, string> OnUpdateEquipmentInfo;
    public event Action<int, EquipmentType> OnUpdateEquipPanelTitleText;
    public event Action<BigInteger, BigInteger> OnUpdateFirstCurrencyCost;
    public event Action<BigInteger, BigInteger> OnUpdateSecondCurrencyCost;
    public event Action<Sprite> OnUpdateFirstCurrencySprite;
    public event Action<Sprite> OnUpdateSecondCurrencySprite;
    public event Action OnOpenCurrentChosenEquipment;
    public event Action<bool> OnUpdateLevelUpButtonsInteractable;

    private Currency enforcePowderCurrency;
    private Currency researchCurrency;
    private Currency gemCurrency;

    private CurrencyManager currencyManager;

    private CurrencyType enforcePowderType = CurrencyType.EnforcePowder;

    private CurrencyType[] bluePrints;

    private EquipmentType[] equipmentTypes;

    public override void Init()
    {
        bluePrints = new CurrencyType[]
        {
            CurrencyType.ShotGunBluePrint,
            CurrencyType.GranadeBluePrint,
            CurrencyType.CapBluePrint,
            CurrencyType.ArmorBluePirnt,
        };

        equipmentTypes = (EquipmentType[])Enum.GetValues(typeof(EquipmentType));

        base.Init();
        equipmentStatDatas = new EquipmentStatData[ui_EquipmentButtons.Length];

        EquipmentManager equipmentManager = EquipmentManager.instance;

        equipmentManager.OnUpdateCurrenEquipment += UpdateCurrentEquipment;

        currencyManager = CurrencyManager.instance;
        enforcePowderCurrency = currencyManager.GetCurrency(enforcePowderType);
        enforcePowderCurrency.OnCurrencyChange += TryUpdateRedDotByCurrency;

        researchCurrency = currencyManager.GetCurrency(CurrencyType.Research);
        gemCurrency = currencyManager.GetCurrency(CurrencyType.Gem);

        for (int i = 0; i < ui_EquipmentButtons.Length; i++)
        {
            int index = i;
            ui_EquipmentButtons[i].AddButtonAction(() => ShowEquipmentInfo(index));
            ui_EquipmentButtons[i].AddButtonAction(() => OnOpenCurrentChosenEquipment?.Invoke());
            ui_EquipmentButtons[i].AddButtonAction(() => equipmentManager.UpdateCurrentEquipmentIndex(index));
            if (index == ui_EquipmentButtons.Length - 1)
            {
                currencyManager.GetCurrency(CurrencyType.Research).OnCurrencyChange += (currency) => 
                    TryUpdateBackpackRedDotByCurrency(CurrencyType.Gem, currency);
                currencyManager.GetCurrency(CurrencyType.Gem).OnCurrencyChange += (currency) => 
                    TryUpdateBackpackRedDotByCurrency(CurrencyType.Research, currency);
            }
            else
            {
                currencyManager.GetCurrency(bluePrints[index]).OnCurrencyChange += (currency) => TryUpdateRedDotByCurrency(index, currency);
            }
        }

        hero = HeroManager.instance.hero;
        equipmentResourceDataHandler = ResourceManager.instance.equipment;
        rankDataHandler = ResourceManager.instance.rank;

        equipmentManager.OnRefreshPanel += ShowEquipmentInfo;

        UIManager.instance.GetUIElement<UI_Hero>().OnUIOpened += UpdateBackpackRedDotByCurrency;
    }

    public void ShowEquipmentInfo(int index)
    {
        bool isBackpack = index == (int)EquipmentType.Backpack - 1;
        EquipmentStatData equipmentStatData = equipmentStatDatas[index];
        OnUpdateEquipmentTitle?.Invoke(equipmentStatData.level, equipmentStatData.equipmentType);
        Rank rank = equipmentStatData.rank;
        EquipmentType equipmentType = equipmentStatData.equipmentType;
        Sprite equipmentSprite = equipmentResourceDataHandler.GetEquipmentSprite(equipmentType, rank);
        Sprite rankSprite = rankDataHandler.GetRankBackgroundSprite(rank);

        (BigInteger, BigInteger) levelUpCostData = default;
        // TODO: 로컬라이징 필요
        levelUpCostData = isBackpack ? equipmentResourceDataHandler.GetBackpackLevelUpCost(equipmentStatData.level) :
            equipmentResourceDataHandler.GetEquipmentLevelUpCost(equipmentStatData.level);

        CurrencyType firstType = CurrencyType.None;
        CurrencyType secondType = CurrencyType.None;
        Currency firstTypeCurrency = null;
        Currency secondTypeCurrency = null;


        firstType = isBackpack ? CurrencyType.Research : bluePrints[(int)equipmentType - 1];
        secondType = isBackpack ? CurrencyType.Gem : enforcePowderType;

        firstTypeCurrency = currencyManager.GetCurrency(firstType);
        secondTypeCurrency = currencyManager.GetCurrency(secondType);

        BigInteger firstCurrencyAmount = firstTypeCurrency.GetCurrencyValue();
        BigInteger secondCurrencyAmount = secondTypeCurrency.GetCurrencyValue();

        OnUpdateFirstCurrencyCost?.Invoke(firstCurrencyAmount, levelUpCostData.Item1);
        OnUpdateSecondCurrencyCost?.Invoke(secondCurrencyAmount, levelUpCostData.Item2);

        Sprite firstIcon = levelUpCostData.Item1 == null || levelUpCostData.Item1 == 0 ? null : firstTypeCurrency.GetIcon();
        Sprite secondIcon = levelUpCostData.Item2 == null || levelUpCostData.Item2 == 0 ? null : secondTypeCurrency.GetIcon();
        OnUpdateFirstCurrencySprite?.Invoke(firstIcon);
        OnUpdateSecondCurrencySprite?.Invoke(secondIcon);

        OnUpdateLevelUpButtonsInteractable?.Invoke(firstCurrencyAmount >= levelUpCostData.Item1 && 
            secondCurrencyAmount >= levelUpCostData.Item2);

        OnUpdateEquipPanelTitleText?.Invoke(equipmentStatData.level, equipmentType);

        StatType statType;
        Sprite statTypeSprite = null;
        if (equipmentType != EquipmentType.Backpack)
        {
            statType = equipmentType == EquipmentType.Shotgun || equipmentType == EquipmentType.Granade ? StatType.Attack : StatType.Health;
            statTypeSprite = equipmentResourceDataHandler.GetStatTypeSprite(statType);
        }
        else
        {
            statType = StatType.Capacity;
            statTypeSprite = equipmentResourceDataHandler.GetStatTypeSprite(statType);
        }
        // 시트 필요
        BigInteger nextStat = equipmentResourceDataHandler.GetEquipmentStatData(equipmentType, rank, equipmentStatData.level + 1).amount;

        if (nextStat == null)
        {
            // TODO : levelMax처리
        }
        string description = equipmentResourceDataHandler.GetEquipmentDescription(equipmentType);

        OnUpdateEquipmentInfo?.Invoke(equipmentSprite, rankSprite, EnumUtility.GetEquipmentTypeKR(equipmentType), EnumUtility.GetRankKR(rank),
            statTypeSprite, EnumUtility.GetStatTypeKR(statType), equipmentStatData.amount, nextStat, description);
    }

    private void UpdateCurrentEquipment(int index, EquipmentStatData equipmentStatData)
    {
        equipmentStatDatas[index] = equipmentStatData;
        Sprite rankSprite = ResourceManager.instance.rank.GetRankBackgroundSprite(equipmentStatData.rank);
        Sprite equipmentSprite = equipmentResourceDataHandler.GetEquipmentSprite(equipmentStatData.equipmentType, equipmentStatData.rank);
        ui_EquipmentButtons[index].UpdateUI(equipmentSprite, rankSprite);

        ui_HeroEquipmentViewer.UpdateEquipment(index, equipmentSprite);
        hero.UpdateEquipmentSprite(index, equipmentSprite);
    }

    private void TryUpdateRedDotByCurrency(int index, BigInteger currentBluePrintAmount)
    {
        BigInteger currentEnforcePowderAmount = currencyManager.GetCurrencyValue(enforcePowderType);
        int equipmentLevel = EquipmentManager.instance.GetEquipmentStatData(equipmentTypes[index + 1]).level;
        (BigInteger, BigInteger) equipmentLevelUpCost = equipmentResourceDataHandler.GetEquipmentLevelUpCost(equipmentLevel);
        BigInteger bluePrintCost = equipmentLevelUpCost.Item1;
        BigInteger enforcePowderCost = equipmentLevelUpCost.Item2;

        bool isLevelUpPossible = currentBluePrintAmount >= bluePrintCost && currentEnforcePowderAmount >= enforcePowderCost;
        NotificationManager.instance.SetNotification(ui_EquipmentButtons[index].redDotController.notificationKey, isLevelUpPossible);
        NotificationManager.instance.SetNotification(RedDotIDType.Bottombar_Hero, UIManager.instance.GetUIElement<UI_Hero>().GetRedDotActiveState());
    }

    private void TryUpdateBackpackRedDotByCurrency(CurrencyType otherCurrencyType, BigInteger currentCurrencyAmount)
    {
        int index = equipmentTypes.Length - 1;
        BigInteger currentOtherCurrencyAmount = currencyManager.GetCurrencyValue(otherCurrencyType);
        int equipmentLevel = EquipmentManager.instance.GetEquipmentStatData(equipmentTypes[index]).level;
        (BigInteger, BigInteger) backpackLevelUpCost = equipmentResourceDataHandler.GetBackpackLevelUpCost(equipmentLevel);
        BigInteger researchCost = backpackLevelUpCost.Item1;
        BigInteger gemCost = backpackLevelUpCost.Item2;

        bool isLevelUpPossible = false;

        isLevelUpPossible = currentCurrencyAmount >= researchCost && currentOtherCurrencyAmount >= gemCost;

        NotificationManager.instance.SetNotification(ui_EquipmentButtons[index - 1].redDotController.notificationKey, isLevelUpPossible);
        NotificationManager.instance.SetNotification(RedDotIDType.Bottombar_Hero, UIManager.instance.GetUIElement<UI_Hero>().GetRedDotActiveState());
    }

    private void UpdateBackpackRedDotByCurrency()
    {
        int index = equipmentTypes.Length - 1;
        BigInteger researchCurrencyAmount = currencyManager.GetCurrencyValue(CurrencyType.Research);
        BigInteger gemCurrencyAmount = currencyManager.GetCurrencyValue(CurrencyType.Gem);
        int equipmentLevel = EquipmentManager.instance.GetEquipmentStatData(equipmentTypes[index]).level;
        (BigInteger, BigInteger) backpackLevelUpCost = equipmentResourceDataHandler.GetBackpackLevelUpCost(equipmentLevel);
        BigInteger researchCost = backpackLevelUpCost.Item1;
        BigInteger gemCost = backpackLevelUpCost.Item2;

        bool isLevelUpPossible = false;

        isLevelUpPossible = researchCurrencyAmount >= researchCost && gemCurrencyAmount >= gemCost;

        NotificationManager.instance.SetNotification(ui_EquipmentButtons[index - 1].redDotController.notificationKey, isLevelUpPossible);
        NotificationManager.instance.SetNotification(RedDotIDType.Bottombar_Hero, UIManager.instance.GetUIElement<UI_Hero>().GetRedDotActiveState());
    }

    private void TryUpdateRedDotByCurrency(BigInteger amount)
    {
        for (int i = 0; i < bluePrints.Length; i++)
        {
            BigInteger currentBluePrintAmount = currencyManager.GetCurrencyValue(bluePrints[i]);
            BigInteger currentEnforcePowderAmount = currencyManager.GetCurrencyValue(enforcePowderType);
            int equipmentLevel = EquipmentManager.instance.GetEquipmentStatData(equipmentTypes[i + 1]).level;
            (BigInteger, BigInteger) equipmentLevelUpCost = equipmentResourceDataHandler.GetEquipmentLevelUpCost(equipmentLevel);
            BigInteger bluePrintCost = equipmentLevelUpCost.Item1;
            BigInteger enforcePowderCost = equipmentLevelUpCost.Item2;

            bool isLevelUpPossible = currentBluePrintAmount >= bluePrintCost && currentEnforcePowderAmount >= enforcePowderCost;
            NotificationManager.instance.SetNotification(ui_EquipmentButtons[i].redDotController.notificationKey, isLevelUpPossible);
        }

        NotificationManager.instance.SetNotification(RedDotIDType.Bottombar_Hero, UIManager.instance.GetUIElement<UI_Hero>().GetRedDotActiveState());
    }
}
