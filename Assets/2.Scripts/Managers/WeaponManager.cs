using System;
using System.Collections;
using System.Collections.Generic;
using Keiwando.BigInteger;
using UnityEngine;

public class WeaponManager : Singleton<WeaponManager>
{
    private Dictionary<WeaponSlotType, List<WeaponType>> weaponTypeDictBySlotType = new Dictionary<WeaponSlotType, List<WeaponType>>();

    public event Action<int> OnActivateUpgradeWeaponPanel;

    public event Action<int, BigInteger, BigInteger> OnUpdateUpgradeButtonState;
    public event Action<int, int, int, BigInteger, BigInteger, BigInteger> OnUpdateLevelUpWeaponUI;
    private Action OnActivateAbilityPanel;

    private WeaponSlotType[] weaponSlotTypes;
    private WeaponType[] currentWeaponTypes;

    public event Action<int, WeaponType, WeaponSlotType> OnUpdateCurrentWeaponType;

    private CurrencyManager currencyManager;

    private WeaponResourceDataHandler weaponResourceDataHandler;

    private Dictionary<int, Weapon> weaponDict = new Dictionary<int, Weapon>();
    private Dictionary<int, WeaponData> weaponDataDict = new Dictionary<int, WeaponData>();

    private List<WeaponData> weaponDatas = new List<WeaponData>();

    private int upgradeBlockCountMax;

    private int boxCountMax;

    private WeaponObjectPooler weaponPooler;

    public event Action OnUpdateInGameWeapons;

    private Currency goldCurrency;

    private bool isWeaponSlotInitialized = false;

    public WeaponUpgradeModule weaponUpgradeModule { get; private set; }

    public WeaponAbilityModule weaponAbilityModule { get; private set; }

    private Action<WeaponType, BigInteger, BigInteger, BigInteger, BigInteger> OnUpdateWeaponCurrencyUIUpgraded;

    public void Init()
    {
        weaponResourceDataHandler = ResourceManager.instance.weapon;

        WeaponType[] weaponTypes = (WeaponType[])Enum.GetValues(typeof(WeaponType));

        weaponSlotTypes = (WeaponSlotType[])Enum.GetValues(typeof(WeaponSlotType));
        weaponTypeDictBySlotType.Clear();
        for (int i = 1; i < weaponSlotTypes.Length; i++)
        {
            WeaponSlotType weaponSlotType = weaponSlotTypes[i];
            weaponTypeDictBySlotType.Add(weaponSlotType, new List<WeaponType>());
        }

        weaponUpgradeModule = new WeaponUpgradeModule();

        UI_UpgradeWeaponPanel ui_UpgradeWeaponPanel = UIManager.instance.GetUIElement<UI_Weapon>().ui_UpgradeWeaponPanel;
        weaponUpgradeModule.OnUpdateWeaponUIUpgraded += ui_UpgradeWeaponPanel.UpdateWeaponUI;
        OnUpdateWeaponCurrencyUIUpgraded += ui_UpgradeWeaponPanel.UpdateCurrencyUI;
        
        currencyManager = CurrencyManager.instance;

        for (int i = 1; i < weaponTypes.Length; i++)
        {
            WeaponType weaponType = weaponTypes[i];

            currencyManager.GetCurrency(EnumUtility.GetCurrencyTypeByWeaponType(weaponType)).OnCurrencyChange += (amount) => 
                UpdateWeaponCurrencyUI(weaponType, amount);
            currencyManager.GetCurrency(CurrencyType.Research).OnCurrencyChange += (amount) =>
                UpdateWeaponResearchUI(weaponType, amount);

            ui_UpgradeWeaponPanel.SetUpgradeEvent(weaponType, () => weaponUpgradeModule.UpgradeWeapon(weaponType));

            WeaponUpgradeData weaponUpgradeData = weaponUpgradeModule.GetCurrentWeaponUpgradeData(weaponType);
        }

        weaponUpgradeModule.OnUpgradeWeapon += ApplyStats;

        weaponUpgradeModule.Init();

        currentWeaponTypes = new WeaponType[weaponSlotTypes.Length - 1];
        goldCurrency = currencyManager.GetCurrency(CurrencyType.Gold);

        StageManager.instance.OnUpdateWeaponTypes += UpdateWeaponList;

        weaponResourceDataHandler = ResourceManager.instance.weapon;

        GameManager.instance.OnStart += SetGameState;

        upgradeBlockCountMax = weaponResourceDataHandler.GetUpgradeBlockCountMax();

        weaponPooler = PoolManager.instance.weapon;

        boxCountMax = ResourceManager.instance.box.GetBoxMaxCount();

        weaponDatas = DataBaseManager.instance.Load(Consts.WEAPON_DATAS, new List<WeaponData>());

        weaponAbilityModule = new WeaponAbilityModule(weaponSlotTypes, weaponResourceDataHandler);

        UI_AbilityPanel ui_AbilityPanel = UIManager.instance.GetUIElement<UI_Battle>().ui_AbilityPanel;
        weaponAbilityModule.OnUpdateChooseableAbilityPanel += ui_AbilityPanel.UpdateChooseableAbilityUI;
        weaponAbilityModule.OnUpdateAbilityPanel += ui_AbilityPanel.UpdateCurrentHavingAbilityUI;
        weaponAbilityModule.OnUpdateWeaponAbilities += ApplyStats;
        OnActivateAbilityPanel += ui_AbilityPanel.OpenUI;
        ui_AbilityPanel.Init();
        ui_AbilityPanel.rerollButton.AddButtonAction(weaponAbilityModule.ReRollWeaponAbility);

        for (int i = 0; i < weaponDatas.Count; i++)
        {
            WeaponData weaponData = weaponDatas[i];
            if (weaponData.level != 0)
            {
                SetWeaponInBox(i, weaponData.level, weaponData.upgradeBlockCount, weaponData.weaponType);
            }
            else
            {
                weaponDict.Add(i, null);
                weaponDataDict.Add(i, default);
            }
        }

        GameManager.instance.OnReset += ResetSkill;
    }

    private void SetGameState()
    {
        ApplySkills();
        OnUpdateInGameWeapons?.Invoke();
    }

    private void ResetSkill()
    {
        foreach (var weapon in weaponDict.Values)
        {
            if (weapon == null)
            {
                continue;
            }
            WeaponType weaponType = weapon.GetWeaponType();
            SkillManager.instance.UpdateSkillDict(weaponType, weapon, true);
        }
    }

    private void UpdateWeaponList(int difficultyNum, int mainStageNum)
    {
        int totalStageNum = (difficultyNum - 1) * Consts.STAGE_DIVIDE_VALUE + mainStageNum;

        currentWeaponTypes = DataBaseManager.instance.Load($"{totalStageNum}_{Consts.CURRENT_WEAPON_TYPES}", new WeaponType[weaponSlotTypes.Length - 1]);

        if (!isWeaponSlotInitialized)
        {
            InitWeaponTypeDict(totalStageNum);
        }

        if (currentWeaponTypes[0] == WeaponType.None)
        {
            UpdateCurrentWeaponTypes(totalStageNum);
            RollCurrentWeaponTypes(totalStageNum);
        }

        for (int i = 0; i < currentWeaponTypes.Length; i++)
        {
            WeaponType weaponType = currentWeaponTypes[i];
            OnUpdateCurrentWeaponType?.Invoke(i, weaponType, GetWeaponSlotTypeByWeaponType(weaponType));
            ApplyStats(weaponType);
        }

        CurrencyManager.instance.GetCurrency(CurrencyType.Gold).OnCurrencyChange += UpdateLevelUpWeaponActiveState;
    }

    private void UpdateCurrentWeaponTypes(int totalStageNum)
    {
        List<WeaponType> weaponTypes = weaponResourceDataHandler.GetWeaponTypeByStage(totalStageNum);

        if (weaponTypes != null)
        {
            for (int i = 0; i < weaponTypes.Count; i++)
            {
                WeaponType weaponType = weaponTypes[i];
                WeaponSlotType weaponSlotType = EnumUtility.GetWeaponSlotTypeByWeaponType(weaponType);
                currentWeaponTypes[(int)weaponSlotType - 1] = weaponType;
            }
        }
    }

    private void InitWeaponTypeDict(int totalStageNum)
    {
        UI_UpgradeWeaponPanel ui_upgradeWeaponPanel = UIManager.instance.GetUIElement<UI_UpgradeWeaponPanel>();

        WeaponType[] tempWeaponTypes = (WeaponType[])Enum.GetValues(typeof(WeaponType));

        for (int i = 1; i < tempWeaponTypes.Length; i++)
        {
            WeaponType weaponType = tempWeaponTypes[i];
            string weaponName = EnumUtility.GetWeaponTypeKR(weaponType);
            Sprite weaponSprite = weaponResourceDataHandler.GetWeaponSprite(weaponType);
            ui_upgradeWeaponPanel.InitWeaponInfo(weaponType, weaponName, weaponSprite, weaponResourceDataHandler.GetWeaponUnlockStage(weaponType));
        }

        for (int i = 1; i <= totalStageNum; i++)
        {
            List<WeaponType> weaponTypes = weaponResourceDataHandler.GetWeaponTypeByStage(i);

            if (weaponTypes == null)
            {
                continue;
            }

            for (int j = 0; j < weaponTypes.Count; j++)
            {
                WeaponType weaponType = weaponTypes[j];
                CurrencyType currencyType = EnumUtility.GetCurrencyTypeByWeaponType(weaponType);
                ui_upgradeWeaponPanel.UnlockWeaponUI(weaponType, currencyType);
                WeaponSlotType weaponSlotType = EnumUtility.GetWeaponSlotTypeByWeaponType(weaponType);
                if (!weaponTypeDictBySlotType[weaponSlotType].Contains(weaponType))
                {
                    weaponTypeDictBySlotType[weaponSlotType].Add(weaponType);
                }
            }
        }

        isWeaponSlotInitialized = true;
    }

    public WeaponSlotType GetWeaponSlotType(int index)
    {
        WeaponType weaponType = currentWeaponTypes[index];
        return GetWeaponSlotTypeByWeaponType(weaponType);
    }

    private void RollCurrentWeaponTypes(int totalStageNum)
    {
        WeaponSlotType weaponSlotType = WeaponSlotType.First;
        WeaponType firstType = currentWeaponTypes[(int)weaponSlotType - 1];
        if (firstType == WeaponType.None)
        {
            int index = (totalStageNum - 1) % weaponTypeDictBySlotType[weaponSlotType].Count;
            firstType = weaponTypeDictBySlotType[weaponSlotType][index];
        }
        else
        {
            if (!weaponTypeDictBySlotType[weaponSlotType].Contains(firstType))
            {
                weaponTypeDictBySlotType[weaponSlotType].Add(firstType);
            }
        }

        weaponSlotType = WeaponSlotType.Second;
        WeaponType secondType = currentWeaponTypes[(int)weaponSlotType - 1];
        List<WeaponType> weapons = new List<WeaponType>(weaponTypeDictBySlotType[weaponSlotType]);
        if (secondType == WeaponType.None)
        {
            int secondRandomIndex = UnityEngine.Random.Range(0, weapons.Count);
            secondType = weapons[secondRandomIndex];
        }
        else
        {
            if (!weaponTypeDictBySlotType[weaponSlotType].Contains(secondType))
            {
                weaponTypeDictBySlotType[weaponSlotType].Add(secondType);
            }
        }

        if (weapons.Contains(secondType))
        {
            weapons.Remove(secondType);
        }

        weaponSlotType = WeaponSlotType.Third;
        weapons.AddRange(weaponTypeDictBySlotType[weaponSlotType]);

        WeaponType thirdType = currentWeaponTypes[(int)weaponSlotType - 1];
        if (thirdType == WeaponType.None)
        {
            int thirdRandomIndex = UnityEngine.Random.Range(0, weapons.Count);
            thirdType = weapons[thirdRandomIndex];
        }
        else
        {
            if (!weaponTypeDictBySlotType[weaponSlotType].Contains(thirdType))
            {
                weaponTypeDictBySlotType[weaponSlotType].Add(thirdType);
            }
        }

        currentWeaponTypes[(int)WeaponSlotType.First - 1] = firstType;
        currentWeaponTypes[(int)WeaponSlotType.Second - 1] = secondType;
        currentWeaponTypes[(int)WeaponSlotType.Third - 1] = thirdType;

        DataBaseManager.instance.Save($"{totalStageNum}_{Consts.CURRENT_WEAPON_TYPES}", currentWeaponTypes);
    }

    public void ChooseWeapon(int boxIndex, int currentWeaponTypeIndex)
    {
        int weaponLevel = 1;
        int upgradeBlockCount = 0;
        WeaponType weaponType = currentWeaponTypes[currentWeaponTypeIndex];
        SetWeaponInBox(boxIndex, weaponLevel, upgradeBlockCount, weaponType);
    }

    private void SetWeaponInBox(int boxIndex, int weaponLevel, int upgradeBlockCount, WeaponType weaponType)
    {
        WeaponData weaponData = weaponResourceDataHandler.GetWeaponData(weaponType, weaponLevel, upgradeBlockCount);

        int weaponTypeIndex = (int)weaponType;

        weaponPooler.AddPoolInfo(weaponTypeIndex, 1, boxCountMax);

        Weapon weapon = weaponPooler.Pool(weaponTypeIndex, Vector2.zero, Quaternion.identity);
        weapon.Init();
        weapon.UpdateWeaponData(weaponData);
        BoxManager.instance.AddWeapon(boxIndex, weapon);
        weapon.transform.localPosition = Vector2.zero;

        if (!weaponDict.ContainsKey(boxIndex))
        {
            weaponDict.Add(boxIndex, weapon);
        }
        else
        {
            weaponDict[boxIndex] = weapon;
        }

        if (!weaponDataDict.ContainsKey(boxIndex))
        {
            weaponDataDict.Add(boxIndex, weaponData);
        }
        else
        {
            weaponDataDict[boxIndex] = weaponData;
            weaponDatas[boxIndex] = weaponData;
        }

        ApplyStats(weaponType, boxIndex);
        DataBaseManager.instance.Save(Consts.WEAPON_DATAS, weaponDatas);

        SkillManager.instance.UpdateSkillDict(weaponType, weapon, true);

        OnActivateUpgradeWeaponPanel?.Invoke(boxIndex);
    }

    public void ResetWeapon()
    {
        DataBaseManager.instance.Delete(Consts.WEAPON_DATAS);

        weaponDict.Clear();
        weaponDataDict.Clear();
    }

    public void LevelUpWeapon(int boxIndex)
    {
        if (!weaponDataDict.ContainsKey(boxIndex))
        {
            return;
        }

        WeaponData weaponData = weaponDataDict[boxIndex];
        WeaponType weaponType = weaponData.weaponType;

        int level = weaponData.level;
        int upgradeBlockCount = weaponData.upgradeBlockCount;

        if (level == weaponResourceDataHandler.GetUpgradeLevelMax())
        {
            if (upgradeBlockCount == weaponResourceDataHandler.GetUpgradeBlockCountMax())
            {
                return;
            }
        }
        else
        {
            if (upgradeBlockCount == weaponResourceDataHandler.GetUpgradeBlockCountMax() - 1)
            {
                bool isAddAbilityPossible = true;

                foreach (WeaponData tempWeaponData in weaponDatas)
                {
                    if (weaponData.weaponType == tempWeaponData.weaponType)
                    {
                        if (level < tempWeaponData.level)
                        {
                            isAddAbilityPossible = false;
                            break;
                        }
                    }
                }

                if (isAddAbilityPossible)
                {
                    OnActivateAbilityPanel?.Invoke();
                    weaponAbilityModule.RollWeaponAbility(weaponType);
                }
            }
        }

        if (currencyManager.GetCurrencyValue(CurrencyType.Gold) < weaponData.goldCost)
        {
            return;
        }

        Currency currency = currencyManager.GetCurrency(CurrencyType.Gold);
        currency.TryUpdateCurrency(-weaponData.goldCost);

        weaponData.upgradeBlockCount++;

        if (weaponData.upgradeBlockCount >= weaponResourceDataHandler.GetUpgradeBlockCountMax())
        {
            weaponData.upgradeBlockCount = 0;
            weaponData.level++;
        }

        WeaponData newWeaponData = weaponResourceDataHandler.GetWeaponData(weaponType, weaponData.level, weaponData.upgradeBlockCount);
        UpdateWeaponData(boxIndex, newWeaponData);
        ApplyStats(weaponType, boxIndex);

        OnUpdateUpgradeButtonState?.Invoke(boxIndex, currency.GetCurrencyValue(), newWeaponData.goldCost);
    }

    private WeaponData GetWeaponDataUpgraded(WeaponData weaponData)
    {
        WeaponType weaponType = weaponData.weaponType;

        BigInteger damageUpgraded = weaponUpgradeModule.GetDamageUpgraded(weaponType, weaponData.damage);
        BigInteger originDamage = weaponResourceDataHandler.GetWeaponData(weaponType, weaponData.level, 0).damage;
        BigInteger skillDamageUpgraded = weaponUpgradeModule.GetSkillDamageUpgraded(weaponType, originDamage, weaponData.skillDamage);
        weaponData.damage = damageUpgraded;
        weaponData.skillDamage = skillDamageUpgraded;
        return weaponData;
    }

    private void UpdateWeaponData(int boxIndex, WeaponData newWeaponData)
    {
        weaponDataDict[boxIndex] = newWeaponData;
        weaponDict[boxIndex].UpdateWeaponData(newWeaponData);
        weaponDatas[boxIndex] = newWeaponData;
        DataBaseManager.instance.Save($"{Consts.WEAPON_DATAS}", weaponDatas);
    }

    private void ApplySkills()
    {
        SkillManager.instance.Clear();

        foreach (var item in weaponDataDict)
        {
            SkillManager.instance.ApplySkill(item.Value.weaponType);
        }

        SkillManager.instance.UpdateSkillUI();
    }

    private void UpdateLevelUpWeaponActiveState(BigInteger amount)
    {
        foreach (var item in weaponDataDict)
        {
            if (item.Value.level == 0 || 
                (item.Value.level == weaponResourceDataHandler.GetUpgradeLevelMax() && 
                item.Value.upgradeBlockCount == weaponResourceDataHandler.GetUpgradeBlockCountMax() - 1))
            {
                continue;
            }
            BigInteger cost = item.Value.goldCost;
            OnUpdateUpgradeButtonState?.Invoke(item.Key, amount, cost);
        }
    }

    public void ResetTarget(Monster monster)
    {
        foreach (Weapon weapon in weaponDict.Values)
        {
            ITargeting targeting = weapon as ITargeting;
            if (targeting != null)
            {
                targeting.SetTarget(monster);
            }
        }
    }

    public void TryFindTarget()
    {
        foreach (Weapon weapon in weaponDict.Values)
        {
            ITargeting targeting = weapon as ITargeting;
            if (targeting != null)
            {
                targeting.SetTargetMonster();
            }
        }
    }

    public BigInteger GetWeaponTotalGold(int boxIndex)
    {
        BigInteger totalGold = 0;

        if (weaponDataDict.ContainsKey(boxIndex))
        {
            WeaponData weaponData = weaponDataDict[boxIndex];
            int weaponLevel = weaponData.level;
            int upgradeBlockCount = weaponData.upgradeBlockCount;
            WeaponType weaponType = weaponData.weaponType;

            for (int i = 1; i < weaponSlotTypes.Length; i++)
            {
                if (weaponTypeDictBySlotType[weaponSlotTypes[i]].Contains(weaponData.weaponType))
                {
                    totalGold += weaponResourceDataHandler.GetInitWeaponCost((int)weaponSlotTypes[i]);
                    break;
                }
            }

            for (int i = 1; i <= weaponLevel; i++)
            {
                if (i < weaponLevel)
                {
                    for (int j = 0; j < upgradeBlockCountMax; j++)
                    {
                        totalGold += GetWeaponLevelUpCost(weaponType, i, j);
                    }
                }
                else
                {
                    for (int j = 0; j < upgradeBlockCount; j++)
                    {
                        totalGold += GetWeaponLevelUpCost(weaponType, i, j);
                    }
                }
            }
        }

        return totalGold;
    }

    public void SwitchBoxIndex(int boxIndex, int otherBoxIndex)
    {
        if (weaponDict.ContainsKey(boxIndex) && weaponDict.ContainsKey(otherBoxIndex))
        {
            Weapon weapon = weaponDict[boxIndex];
            weaponDict[boxIndex] = weaponDict[otherBoxIndex];
            weaponDict[otherBoxIndex] = weapon;

            WeaponData weaponData = weaponDataDict[boxIndex];
            weaponDataDict[boxIndex] = weaponDataDict[otherBoxIndex];
            weaponDatas[boxIndex] = weaponDatas[otherBoxIndex];
            weaponDataDict[otherBoxIndex] = weaponData;
            weaponDatas[otherBoxIndex] = weaponData;

            DataBaseManager.instance.Save(Consts.WEAPON_DATAS, weaponDatas);
        }
    }

    public void RemoveWeaponByBoxIndex(int boxIndex)
    {
        if (weaponDict.ContainsKey(boxIndex))
        {
            Weapon weapon = weaponDict[boxIndex];
            if (weapon != null)
            {
                weaponDict[boxIndex].ReturnToPool();
                WeaponType weaponType = weapon.GetWeaponType();
                SkillManager.instance.UpdateSkillDict(weaponType, weapon, false);
            }
            weaponDict.Remove(boxIndex);
        }

        if (weaponDataDict.ContainsKey(boxIndex))
        {
            WeaponData weaponData = weaponDataDict[boxIndex];
            weaponDatas.Remove(weaponData);
            weaponDataDict.Remove(boxIndex);
            DataBaseManager.instance.Save(Consts.WEAPON_DATAS, weaponDatas);
        }
    }

    public void AddWeaponData(int boxIndex)
    {
        if (weaponDatas.Count == 0)
        {
            weaponDatas = DataBaseManager.instance.Load(Consts.WEAPON_DATAS, new List<WeaponData>());
        }

        if (weaponDatas.Count <= boxIndex)
        {
            weaponDatas.Add(default);
            weaponDict.Add(boxIndex, null);
            weaponDataDict.Add(boxIndex, default);
            DataBaseManager.instance.Save(Consts.WEAPON_DATAS, weaponDatas);
        }
    }

    public WeaponType GetWeaponTypeByBoxIndex(int boxIndex)
    {
        if (weaponDict.ContainsKey(boxIndex))
        {
            if (weaponDict[boxIndex] != null)
            {
                return weaponDict[boxIndex].GetWeaponType();
            }
        }

        return default;
    }

    public Weapon GetWeaponByBoxIndex(int boxIndex)
    {
        if (weaponDict.ContainsKey(boxIndex))
        {
            return weaponDict[boxIndex];
        }

        return default;
    }

    public int GetSkillCostMod(int skillIndex)
    {
        // GRANADE
        if (skillIndex == 0)
        {
            return 0;
        }

        AbilityType abilityType = AbilityType.Skill_CostDown;

        WeaponType weaponType = currentWeaponTypes[skillIndex - 1];

        int[] abilitiesCount = weaponAbilityModule.GetCurrentAbilitiesCount(weaponType);
        int abilitiesCountIndex = (int)abilityType - 1;

        WeaponAbilityData weaponAbilityData = weaponResourceDataHandler.GetWeaponAbilityData(abilityType);

        if (abilitiesCount[abilitiesCountIndex] == 0)
        {
            return 0;
        }

        int abilityMod = weaponAbilityData.initEvolutionValue +
            (abilitiesCount[abilitiesCountIndex] - 1) * weaponAbilityData.increasingEvolutionValue;

        return abilityMod;
    }

    public bool IsSkillFree(int index)
    {
        WeaponType weaponType = currentWeaponTypes[index];
        return weaponAbilityModule.CanSkillUseFree(weaponType);
    }

    private WeaponSlotType GetWeaponSlotTypeByWeaponType(WeaponType weaponType)
    {
        foreach (var weaponTypePair in weaponTypeDictBySlotType)
        {
            if (weaponTypePair.Value.Contains(weaponType))
            {
                return weaponTypePair.Key;
            }
        }

        return default;
    }

    private BigInteger GetWeaponLevelUpCost(WeaponType weaponType, int i, int j)
    {
        WeaponData tempWeaponData = weaponResourceDataHandler.GetWeaponData(weaponType, i, j);
        return tempWeaponData.goldCost;
    }

    private void UpdateWeaponCurrencyUI(WeaponType weaponType, BigInteger currentWeaponCurrencyAmount)
    {
        WeaponUpgradeData weaponUpgradeData = weaponUpgradeModule.GetCurrentWeaponUpgradeData(weaponType);
        BigInteger currentResearchAmount = currencyManager.GetCurrencyValue(CurrencyType.Research);
        OnUpdateWeaponCurrencyUIUpgraded?.Invoke(weaponType, currentWeaponCurrencyAmount, weaponUpgradeData.weaponCurrencyCost, currentResearchAmount,
            weaponUpgradeData.researchCost);
    }

    private void UpdateWeaponResearchUI(WeaponType weaponType, BigInteger currentResearchAmount)
    {
        WeaponUpgradeData weaponUpgradeData = weaponUpgradeModule.GetCurrentWeaponUpgradeData(weaponType);
        BigInteger currentWeaponCurrencyAmount = currencyManager.GetCurrencyValue(EnumUtility.GetCurrencyTypeByWeaponType(weaponType));
        OnUpdateWeaponCurrencyUIUpgraded?.Invoke(weaponType, currentWeaponCurrencyAmount, weaponUpgradeData.weaponCurrencyCost, currentResearchAmount,
            weaponUpgradeData.researchCost);
    }

    public void ApplyWeaponUpgrade(int boxIndex)
    {
        WeaponData weaponData = weaponDataDict[boxIndex];
        WeaponData newWeaponData = weaponResourceDataHandler.GetWeaponData(weaponData.weaponType, weaponData.level, weaponData.upgradeBlockCount);
        newWeaponData = GetWeaponDataUpgraded(newWeaponData);

        UpdateWeaponData(boxIndex, newWeaponData);
    }

    private void ApplyAbilitiesToBoxWeapon(List<AbilityType> abilityTypes, int boxIndex)
    {
        WeaponData weaponData = weaponDataDict[boxIndex];
        WeaponType weaponType = weaponData.weaponType;

        Weapon weapon = weaponDict[boxIndex];

        for (int i = 0; i < abilityTypes.Count; i++)
        {
            AbilityType abilityType = abilityTypes[i];

            int[] abilitiesCount = weaponAbilityModule.GetCurrentAbilitiesCount(weaponType);
            int abilitiesCountIndex = (int)abilityType - 1;

            WeaponAbilityData weaponAbilityData = weaponResourceDataHandler.GetWeaponAbilityData(abilityType);

            int abilityMod = abilitiesCount[abilitiesCountIndex] == 0 ? 0 :
                weaponAbilityData.initEvolutionValue + (abilitiesCount[abilitiesCountIndex] - 1) * weaponAbilityData.increasingEvolutionValue;

            switch (abilityType)
            {
                case AbilityType.Att_DamageUp:
                    weaponData.damage = weaponData.damage + weaponData.damage * abilityMod / Consts.PERCENT_DIVIDE_VALUE;
                    weaponData.skillDamage = weaponData.skillDamage + weaponData.skillDamage * abilityMod / Consts.PERCENT_DIVIDE_VALUE;
                    break;
                case AbilityType.Att_CoolDown:
                    TargetingWeapon targetingWeapon = weapon as TargetingWeapon;
                    if (targetingWeapon != null)
                    {
                        targetingWeapon.UpdateAttackInterval(abilityMod * Consts.PERCENT_MUTIPLY_VALUE);
                    }
                    break;
                case AbilityType.Att_DurationUp:
                    targetingWeapon = weapon as TargetingWeapon;
                    if (targetingWeapon != null)
                    {
                        targetingWeapon.UpdateAttackDuration(abilityMod * Consts.PERCENT_MUTIPLY_VALUE);
                    }
                    break;
                case AbilityType.Skill_AddShot:
                    MeleeTickWeapon meleeTickWeapon = weapon as MeleeTickWeapon;
                    if (meleeTickWeapon != null)
                    {
                        meleeTickWeapon.UpdateSkillCount(abilityMod);
                    }
                    break;
                case AbilityType.Att_RangeUp:
                    Rocket rocket = weapon as Rocket;
                    if (rocket != null)
                    {
                        rocket.UpdateExplosionRadius(abilityMod * Consts.PERCENT_MUTIPLY_VALUE);
                    }
                    break;
            }
        }

        UpdateWeaponData(boxIndex, weaponData);
    }

    private void ApplyStats(WeaponType weaponType)
    {
        foreach (Box box in BoxManager.instance.boxSpawner.readiedBoxes)
        {
            int boxIndex = box.index;
            if (weaponDatas[boxIndex].weaponType == weaponType)
            {
                ApplyStats(weaponType, boxIndex);
            }
        }
    }

    private void ApplyStats(WeaponType weaponType, int boxIndex)
    {
        List<AbilityType> abilityTypes = weaponAbilityModule.GetCurrentWeaponAbilityTypes(weaponType);

        ApplyWeaponUpgrade(boxIndex);

        ApplyAbilitiesToBoxWeapon(abilityTypes, boxIndex);

        WeaponData weaponData = weaponDataDict[boxIndex];
        OnUpdateLevelUpWeaponUI?.Invoke(boxIndex, weaponData.level, weaponData.upgradeBlockCount, weaponData.damage,
            weaponData.goldCost, goldCurrency.GetCurrencyValue());
    }
}
