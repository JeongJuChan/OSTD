using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Analytics;

public class WeaponAbilityModule
{
    public event Action<string, Sprite, List<Sprite>> OnUpdateAbilityPanel;
    public event Action<int, string, Sprite, string> OnUpdateChooseableAbilityPanel;
    public event Action<WeaponType> OnUpdateWeaponAbilities;

    private Dictionary<WeaponSlotType, List<AbilityType>> abilityTypesByWeaponSlotType = new Dictionary<WeaponSlotType, List<AbilityType>>();
    private Dictionary<WeaponSlotType, int[]> abilityCountArrDict = new Dictionary<WeaponSlotType, int[]>();

    private WeaponType currentWeaponType;
    private AbilityType[] currentChooseableAbilityTypes = new AbilityType[2];
    private List<AbilityType> currentHavingAbilityTypes;

    private WeaponResourceDataHandler weaponResourceDataHandler;

    public WeaponAbilityModule(WeaponSlotType[] weaponSlotTypes, WeaponResourceDataHandler weaponResourceDataHandler)
    {
        AbilityType[] abilityTypes = (AbilityType[])Enum.GetValues(typeof(AbilityType));

        for (int i = 1; i < weaponSlotTypes.Length; i++)
        {
            int[] tempAbilityCountArr = DataBaseManager.instance.Load($"{weaponSlotTypes[i]}_{Consts.WEAPON_ABILITY_COUNT_ARR}", 
                new int[abilityTypes.Length - 1]);
            abilityCountArrDict.Add(weaponSlotTypes[i], tempAbilityCountArr);
        }

        for (int i = 0; i < weaponSlotTypes.Length; i++)
        {
            if (!abilityTypesByWeaponSlotType.ContainsKey(weaponSlotTypes[i]))
            {
                abilityTypesByWeaponSlotType.Add(weaponSlotTypes[i], new List<AbilityType>());
            }

            abilityTypesByWeaponSlotType[weaponSlotTypes[i]] = DataBaseManager.instance.Load($"{weaponSlotTypes[i]}_{Consts.WEAPON_ABILITIES}",
                new List<AbilityType>());
        }

        this.weaponResourceDataHandler = weaponResourceDataHandler;

        StageManager.instance.OnClearStage += ResetAbilityAll;

    }

    public void ReRollWeaponAbility()
    {
        AdsManager.instance.ShowRewardedAdByTime(Consts.ABILITY_REROLL_ADS, (reward, adInfo) =>
        {
            FirebaseAnalytics.LogEvent($"Reward_battle_gold_{AdsManager.instance.GetAdCount(Consts.ABILITY_REROLL_ADS)}");
            RollWeaponAbility(currentWeaponType);
        });
    }

    public void RollWeaponAbility(WeaponType weaponType)
    {
        UpdateCurrentAbilityCount(weaponType);

        int[] abilityCountArr = abilityCountArrDict[EnumUtility.GetWeaponSlotTypeByWeaponType(weaponType)];

        List<AbilityType> removingTypes = new List<AbilityType>();
        for (int i = 0; i < abilityCountArr.Length; i++)
        {
            AbilityType tempAbilityType = (AbilityType)(i + 1);
            int countMax = weaponResourceDataHandler.GetEvolutionCountMax(tempAbilityType);
            if (countMax <= abilityCountArr[i])
            {
                removingTypes.Add(tempAbilityType);
            }
        }

        (AbilityType, AbilityType) abilityTypePair = removingTypes.Count == 0 ? weaponResourceDataHandler.GetRandomAbilityTypePair(weaponType) :
            weaponResourceDataHandler.GetRandomAbilityTypePair(weaponType, removingTypes);

        currentChooseableAbilityTypes[0] = abilityTypePair.Item1;
        currentChooseableAbilityTypes[1] = abilityTypePair.Item2;
        DataBaseManager.instance.Save(Consts.WEAPON_CURRENT_CHOOSEABLE_ABILITY_TYPES, currentChooseableAbilityTypes);

        string weaponTypeKR = EnumUtility.GetWeaponTypeKR(weaponType);

        Sprite weaponIcon = ResourceManager.instance.skill.GetSkillSprite(SkillManager.instance.GetSkilTypeByWeaponType(weaponType));

        List<Sprite> abilitySprites = new List<Sprite>();
        for (int i = 0; i < currentHavingAbilityTypes.Count; i++)
        {
            Sprite abilitySprite = weaponResourceDataHandler.GetAbilitySprite(weaponType, currentHavingAbilityTypes[i]);
            abilitySprites.Add(abilitySprite);
        }

        for (int i = 0; i < currentChooseableAbilityTypes.Length; i++)
        {
            AbilityType abilityType = currentChooseableAbilityTypes[i];
            Sprite abilitySprite = weaponResourceDataHandler.GetAbilitySprite(weaponType, abilityType);
            WeaponAbilityData weaponAbilityData = weaponResourceDataHandler.GetWeaponAbilityData(abilityType);
            int count = abilityCountArr[(int)abilityType - 1];
            int totalAmount = weaponAbilityData.initEvolutionValue + count * weaponAbilityData.increasingEvolutionValue;
            string abilityStr = weaponAbilityData.arithmeticStatType == ArithmeticStatType.Base ?
                $"{totalAmount}\n{EnumUtility.GetAbilityKR(abilityType)}" : $"{totalAmount}%\n{EnumUtility.GetAbilityKR(abilityType)}";
            abilityStr = totalAmount >= 0 ? $"+{abilityStr}" : abilityStr;
            OnUpdateChooseableAbilityPanel?.Invoke(i, weaponTypeKR, abilitySprite, abilityStr);
        }

        OnUpdateAbilityPanel?.Invoke(weaponTypeKR, weaponIcon, abilitySprites);
    }

    private void UpdateCurrentAbilityCount(WeaponType weaponType)
    {
        WeaponSlotType weaponSlotType = EnumUtility.GetWeaponSlotTypeByWeaponType(weaponType);

        int[] abilityCountArr = abilityCountArrDict[weaponSlotType];

        for (int i = 0; i < abilityCountArr.Length; i++)
        {
            abilityCountArr[i] = 0;
        }

        currentWeaponType = weaponType;
        currentHavingAbilityTypes = abilityTypesByWeaponSlotType[weaponSlotType];

        foreach (AbilityType tempAbilityType in currentHavingAbilityTypes)
        {
            abilityCountArr[(int)tempAbilityType - 1]++;
        }

        abilityCountArrDict[weaponSlotType] = abilityCountArr;
    }

    public void ChooseWeaponAblility(int index)
    {
        WeaponSlotType weaponSlotType = EnumUtility.GetWeaponSlotTypeByWeaponType(currentWeaponType);
        AbilityType abilityType = currentChooseableAbilityTypes[index];
        abilityTypesByWeaponSlotType[weaponSlotType].Add(abilityType);
        UpdateCurrentAbilityCount(currentWeaponType);
        DataBaseManager.instance.Save($"{weaponSlotType}_{Consts.WEAPON_ABILITIES}", abilityTypesByWeaponSlotType[weaponSlotType]);
        DataBaseManager.instance.Save($"{weaponSlotType}_{Consts.WEAPON_ABILITY_COUNT_ARR}", abilityCountArrDict[weaponSlotType]);
        OnUpdateWeaponAbilities?.Invoke(currentWeaponType);
    }

    public List<AbilityType> GetCurrentWeaponAbilityTypes(WeaponType weaponType)
    {
        WeaponSlotType weaponSlotType = EnumUtility.GetWeaponSlotTypeByWeaponType(weaponType);
        if (abilityTypesByWeaponSlotType.ContainsKey(weaponSlotType))
        {
            return abilityTypesByWeaponSlotType[weaponSlotType];
        }

        return default;
    }

    public bool CanSkillUseFree(WeaponType weaponType)
    {
        AbilityType abilityType = AbilityType.Skill_FreeUse;
        int abilityCount = GetCurrentAbilitiesCount(weaponType)[(int)abilityType - 1];
        if (abilityCount == 0)
        {
            return false;
        }

        WeaponAbilityData weaponAbilityData = weaponResourceDataHandler.GetWeaponAbilityData(abilityType);
        int targetValue = weaponAbilityData.initEvolutionValue + (abilityCount - 1) * weaponAbilityData.increasingEvolutionValue;
        int randomValue = UnityEngine.Random.Range(0, Consts.PERCENT_DIVIDE_VALUE);
        return randomValue < targetValue;
    }

    public int[] GetCurrentAbilitiesCount(WeaponType weaponType)
    {
        return abilityCountArrDict[EnumUtility.GetWeaponSlotTypeByWeaponType(weaponType)];
    }

    private void ResetAbilityAll()
    {
        foreach (var abilityPair in abilityTypesByWeaponSlotType)
        {
            DataBaseManager.instance.Save($"{abilityPair.Key}_{Consts.WEAPON_ABILITIES}", abilityTypesByWeaponSlotType[abilityPair.Key]);
            abilityPair.Value.Clear();
        }

        foreach (var abilityCountPair in abilityCountArrDict)
        {
            for (int i = 0; i < abilityCountPair.Value.Length; i++)
            {
                abilityCountPair.Value[i] = 0;
            }

            DataBaseManager.instance.Save($"{abilityCountPair.Key}_{Consts.WEAPON_ABILITY_COUNT_ARR}", abilityCountArrDict[abilityCountPair.Key]);
        }

        OnUpdateAbilityPanel?.Invoke("", null, null);
    }
}
