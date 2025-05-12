using Keiwando.BigInteger;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviourSingleton<SkillManager>
{
    public static Action<int, int, bool> OnUpdateSkillEquipUI;

    private Dictionary<WeaponType, SkillType> skillTypeDictByWeaponType = new Dictionary<WeaponType, SkillType>();

    [field: Header("Energy")]
    public event Action<EnergyData> OnUpdateEnergyUI;
    public event Action<bool> OnUpdateEnergyUpgradeButtonState;
    public int energyLevel { get; private set; } = 1;
    private EnergyData energyData;
    private EnergyDataHandler energyDataHandler;

    [field: Header("InGameEnergy")]
    public event Action<int, bool> OnUpdateSkillButtonInteractable;
    public event Action<int, int> OnUpdateSkillCostUI;
    public EnergyCalculator energyCalculator { get; private set; }
    private int energy;
    private List<SkillType> skillTypes;
    private Dictionary<SkillType, List<IUsingSkill>> usingSkillDict = new Dictionary<SkillType, List<IUsingSkill>>();

    private GameManager gameManager;

    public event Action<int, SkillType> OnUpdateUsingSkillUI;
    public event Action<int, bool> OnUpdateSkillUIActiveState;
    public event Action<int> OnUpdateEnergy;

    private int skillCount = 4;

    private Queue<int> waitingSkillDurationQueue = new Queue<int>();

    [SerializeField] private int defaultEnergyAdsAmount = 9;

    private Action<int> OnUpdateEnergyText;
    
    public string[] guideStrArr { get; private set; } = new string[]
        {
            Consts.GRANADE_GUIDE,
            Consts.USING_FIRST_SKILL_GUIDE,
            Consts.USING_SECOND_SKILL_GUIDE,
            Consts.USING_THIRD_SKILL_GUIDE,
        };


    private void Update() 
    {
        if (GameManager.instance.isGameState && !HeroManager.instance.hero.isDead)
        {
            energyCalculator.Update();
        }
        
        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.E))
        {
            ChangeAmountInGameEnergy(100);
        }
        #endif
    }

    #region Initialize
    public void Init()
    {
        WeaponType[] weaponTypes = (WeaponType[])Enum.GetValues(typeof(WeaponType));

        for (int i = 0; i < weaponTypes.Length; i++)
        {
            if (!skillTypeDictByWeaponType.ContainsKey(weaponTypes[i]))
            {
                skillTypeDictByWeaponType.Add(weaponTypes[i], EnumUtility.GetEqualValue<SkillType>(weaponTypes[i].ToString()));
            } 
        }

        gameManager = GameManager.instance;
        OnUpdateSkillEquipUI = null;
        energyCalculator = new EnergyCalculator();
        energyDataHandler = ResourceManager.instance.energy;

        UI_InGameEnergyPanel ui_InGameEnergyPanel = UIManager.instance.GetUIElement<UI_InGameEnergyPanel>();
        energyCalculator.OnUpdateInGameEnergyUI += ui_InGameEnergyPanel.UpdateEnergySlider;
        ui_InGameEnergyPanel.Init();
        energyCalculator.OnIncreaseInGameEnergy += IncreaseInGameEnergy;
        OnUpdateEnergy += ui_InGameEnergyPanel.UpdateEnergy;
        Currency currency = CurrencyManager.instance.GetCurrency(CurrencyType.Gold);
        currency.OnCurrencyChange += UpdateEnergyUpgradeButtonInteractableState;

        skillTypes = new List<SkillType>(skillCount);

        GameManager.instance.OnReset += ResetInGameEnergy;

        StageManager.instance.OnClearStage += ResetStageEnergy;

        energyLevel = DataBaseManager.instance.Load(Consts.CURRENT_ENERGY_LEVEL, 1);
        UpdateEnergyUI();
        UpdateEnergyUpgradeButtonInteractableState(currency.GetCurrencyValue());

        UI_EnergyAdsPanel ui_EnergyAdsPanel = UIManager.instance.GetUIElement<UI_EnergyAdsPanel>();
        ui_EnergyAdsPanel.OnAddEnergyByAds += IncreaseIngameEnergyByAds;
        OnUpdateEnergyText += ui_EnergyAdsPanel.UpdateEnergyAdsText;

        gameManager.OnStart += UpdateInGameAdsReward;
    }

    public void UpdateSkillDict(WeaponType weaponType, IUsingSkill skill, bool isAdding)
    {
        SkillType skillType = EnumUtility.GetEqualValue<SkillType, WeaponType>(weaponType);

        if (isAdding)
        {
            if (!usingSkillDict.ContainsKey(skillType))
            {
                usingSkillDict.Add(skillType, new List<IUsingSkill>());
            }

            if (!usingSkillDict[skillType].Contains(skill))
            {
                usingSkillDict[skillType].Add(skill);
            }
        }
        else
        {
            if (usingSkillDict.ContainsKey(skillType))
            {
                usingSkillDict[skillType].Remove(skill);
            }
        }
    }
    #endregion

    #region Energy
    public void UpgradeEnergyLevel()
    {
        if (!DataBaseManager.instance.ContainsKey(Consts.ENERGY_ENFORCE_GUIDE))
        {
            GuideManager.instance.ToggleGuideWithBackgroundPanel(Consts.ENERGY_ENFORCE_GUIDE, false);
            DataBaseManager.instance.Save(Consts.ENERGY_ENFORCE_GUIDE, true);
        }

        CurrencyManager.instance.TryUpdateCurrency(CurrencyType.Gold, -energyDataHandler.GetEnergyData(energyLevel).cost);
        energyLevel++;
        DataBaseManager.instance.Save(Consts.CURRENT_ENERGY_LEVEL, energyLevel);

        UpdateEnergyUI();
    }

    public void UseSkill(int index)
    {
        SkillType skillType = skillTypes[index];
        int energyCost = GetSkillCost(index);
        if (energy < energyCost)
        {
            return;
        }

        string guideStr = guideStrArr[index];
        if (!DataBaseManager.instance.ContainsKey(guideStr))
        {
            GuideManager.instance.ToggleGuide(guideStr, false);
            DataBaseManager.instance.Save(guideStrArr[index], true);
        }

        if (skillType == SkillType.Granade)
        {
            HeroManager.instance.PoolGranade();
        }
        else
        {
            DeActivateSkillButtonDuringSkillDuraiton(index);
            foreach (IUsingSkill skill in usingSkillDict[skillType])
            {
                skill.UseSkill();
            }
        }

        bool isSkillFree = false;

        if (index != 0)
        {
            isSkillFree = WeaponManager.instance.IsSkillFree(index - 1);
        }

        if (!isSkillFree)
        {
            ChangeAmountInGameEnergy(-energyCost);
        }
        UpdateSkillGuides();
    }

    private void UpdateSkillGuides()
    {
        for (int i = 0; i < guideStrArr.Length; i++)
        {
            string tempGuideStr = guideStrArr[i];
            if (!DataBaseManager.instance.ContainsKey(tempGuideStr))
            {
                if (skillTypes.Count <= i)
                {
                    return;
                }
                // SkillType tempSkillType = skillTypes[i];
                int tempEnergyCost = GetSkillCost(i);
                if (tempEnergyCost > energy)
                {
                    GuideManager.instance.ToggleGuide(tempGuideStr, false);
                }
            }
        }
    }

    public float GetSkillDuration(int index)
    {
        SkillType skillType = skillTypes[index];
        if (usingSkillDict.ContainsKey(skillType))
        {
            if (usingSkillDict[skillType].Count > 0)
            {
                return usingSkillDict[skillType][0].skillDuration;
            }
        }
        return 0f;
    }

    private void ResetInGameEnergy()
    {
        energyCalculator.Reset();
        energy = 0;

        waitingSkillDurationQueue.Clear();

        for (int i = 0; i < skillTypes.Count; i++)
        {
            OnUpdateSkillButtonInteractable?.Invoke(i, energy >= GetSkillCost(i));
        }

        OnUpdateEnergy?.Invoke(energy);

        energyCalculator.Reset();
        UpdateInGameAdsReward();
        for (int i = 0; i < guideStrArr.Length; i++)
        {
            GuideManager.instance.ToggleGuide(guideStrArr[i], false);
        }
    }

    private void UpdateInGameAdsReward()
    {
        int energyAmount = defaultEnergyAdsAmount + energyLevel;
        OnUpdateEnergyText?.Invoke(energyAmount);
    }

    private void ResetStageEnergy()
    {
        energyLevel = 1;
        UpdateEnergyUI();
        ResetInGameEnergy();
        for (int i = 0; i < guideStrArr.Length; i++)
        {
            GuideManager.instance.ToggleGuide(guideStrArr[i], false); 
        }
    }

    public void UpdateEnergyUI()
    {
        energyData = energyDataHandler.GetEnergyData(energyLevel);
        OnUpdateEnergyUI?.Invoke(energyData);
        UpdateEnergyUpgradeButtonInteractableState(CurrencyManager.instance.GetCurrencyValue(CurrencyType.Gold));
        UpdateEnergy(energyData.recoveryAmountPerSec);
    }

    private void UpdateEnergy(float energyRecoveryTime)
    {
        energyCalculator.Reset();
        energyCalculator.UpdateEnergyRecoveryTime(energyRecoveryTime);
    }

    private void IncreaseInGameEnergy()
    {
        ChangeAmountInGameEnergy(1);
    }

    private void ChangeAmountInGameEnergy(int amount)
    {
        energy += amount;

        for (int i = 0; i < skillTypes.Count; i++)
        {
            int index = i;
            SkillType skillType = skillTypes[index];
            int energyCost = GetSkillCost(index);
            bool isEnergyFilled = energy >= energyCost;

            if (isEnergyFilled)
            {
                string guideStr = guideStrArr[index];
                if (!DataBaseManager.instance.ContainsKey(guideStr))
                {
                    GuideManager.instance.ToggleGuide(guideStr, true);
                }
            }

            OnUpdateSkillButtonInteractable?.Invoke(i, isEnergyFilled && !waitingSkillDurationQueue.Contains(i));
        }

        OnUpdateEnergy?.Invoke(energy);
    }

    private void UpdateEnergyUpgradeButtonInteractableState(BigInteger amount)
    {
        OnUpdateEnergyUpgradeButtonState?.Invoke(amount >= energyData.cost);
    }

    private void TryAddSkill(SkillType skillType)
    {
        if (skillType == SkillType.None)
        {
            return;
        }
        
        if (!skillTypes.Contains(skillType))
        {
            skillTypes.Add(skillType);
        }
    }

    #endregion

    public void ApplySkill(WeaponType weaponType)
    {
        TryAddSkill(skillTypeDictByWeaponType[weaponType]);
    }

    public void Clear()
    {
        ChangeAmountInGameEnergy(0);
        skillTypes.Clear();
        TryAddSkill(SkillType.Granade);
    }

    public void UpdateSkillUI()
    {
        skillTypes.Sort((x, y) => energyDataHandler.GetSkillEnergyData(x).CompareTo(energyDataHandler.GetSkillEnergyData(y)));

        for (int i = 0; i < skillCount; i++)
        {
            OnUpdateSkillUIActiveState?.Invoke(i, false);
        }

        for (int i = 0; i < skillTypes.Count; i++)
        {
            OnUpdateSkillCostUI?.Invoke(i, GetSkillCost(i));
            OnUpdateUsingSkillUI?.Invoke(i, skillTypes[i]);
            OnUpdateSkillUIActiveState?.Invoke(i, true);
        }
    }

    public void DeActiveSkillUIByWeaponType(WeaponType weaponType)
    {
        SkillType skillType = GetSkilTypeByWeaponType(weaponType);
        int index = skillTypes.IndexOf(skillType);
        OnUpdateSkillUIActiveState?.Invoke(index, false);
        
        string guideStr = guideStrArr[index];
        if (!DataBaseManager.instance.ContainsKey(guideStr))
        {
            GuideManager.instance.ToggleGuide(guideStr, false);
        }
    }

    public SkillType GetSkilTypeByWeaponType(WeaponType weaponType)
    {
        if (skillTypeDictByWeaponType.ContainsKey(weaponType))
        {
            return skillTypeDictByWeaponType[weaponType];
        }

        return default;
    }
    private void DeActivateSkillButtonDuringSkillDuraiton(int index)
    {
        StartCoroutine(CoWaitForSkillDuration(index));
    }

    private IEnumerator CoWaitForSkillDuration(int index)
    {
        float skillDuration = GetSkillDuration(index);
        if (skillDuration == 0f)
        {
            yield break;
        }
        waitingSkillDurationQueue.Enqueue(index);
        OnUpdateSkillButtonInteractable?.Invoke(index, false);
        yield return CoroutineUtility.GetWaitForSeconds(skillDuration);
        if (waitingSkillDurationQueue.Count == 0)
        {
            yield break;
        }
        waitingSkillDurationQueue.Dequeue();
        OnUpdateSkillButtonInteractable?.Invoke(index, energy >= GetSkillCost(index));
    }

    private void IncreaseIngameEnergyByAds()
    {
        int energyAmount = defaultEnergyAdsAmount + energyLevel;
        ChangeAmountInGameEnergy(energyAmount);
    }

    private int GetSkillCost(int skillIndex)
    {
        int skillCost = energyDataHandler.GetSkillEnergyData(skillTypes[skillIndex]);
        int skillCostMod = WeaponManager.instance.GetSkillCostMod(skillIndex);
        return skillCost + skillCostMod;
    }
}