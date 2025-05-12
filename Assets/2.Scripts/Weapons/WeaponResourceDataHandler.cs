using System;
using System.Collections;
using System.Collections.Generic;
using Keiwando.BigInteger;
using UnityEditor.iOS;
using UnityEngine;

public class WeaponResourceDataHandler
{
    private Dictionary<WeaponType, Dictionary<int, Dictionary<int, WeaponData>>> weaponDataDict = 
        new Dictionary<WeaponType, Dictionary<int, Dictionary<int, WeaponData>>>();

    private Dictionary<WeaponType, Weapon> weaponResourceDict = new Dictionary<WeaponType, Weapon>();

    private Dictionary<WeaponType, Sprite> weaponSpriteDict = new Dictionary<WeaponType, Sprite>();

    private Dictionary<WeaponType, Dictionary<int, WeaponUpgradeData>> weaponUpgradeDataDict = 
        new Dictionary<WeaponType, Dictionary<int, WeaponUpgradeData>>();

    private Dictionary<int, List<WeaponType>> weaponTypeDictByUnlockStage = new Dictionary<int, List<WeaponType>>();
    private Dictionary<WeaponType, int> unlockStageDictByWeaponType = new Dictionary<WeaponType, int>();

    private Dictionary<WeaponType, List<AbilityType>> abilityTypeDict = new Dictionary<WeaponType, List<AbilityType>>();

    private Dictionary<AbilityType, WeaponAbilityData> weaponAbilityDataDict = new Dictionary<AbilityType, WeaponAbilityData>();

    private Dictionary<WeaponType, Dictionary<AbilityType, Sprite>> abilitySpriteDict = 
        new Dictionary<WeaponType, Dictionary<AbilityType, Sprite>>();

    private int upgradeLevelMax;
    private int upgradeBlockCountMax;
    private Dictionary<int, int> initWeaponCostDict = new Dictionary<int, int>();


    #region Initialize
    public void Init()
    {
        GameData weaponData = Resources.Load<GameData>("GameData/WeaponData");
        GameData weaponFixedData = Resources.Load<GameData>("GameData/WeaponFixedData");

        GameData weaponUpgradeData = Resources.Load<GameData>("GameData/WeaponUpgradeData");
        GameData weaponUnlockData = Resources.Load<GameData>("GameData/WeaponUnlockData");

        GameData weaponAbilityFixedData = Resources.Load<GameData>("GameData/WeaponAbilityFixedData");


        List<SerializableRow> rows = weaponData.GetDataRows();
        List<SerializableRow> upgradeRows = weaponUpgradeData.GetDataRows();
        List<SerializableRow> fixedRows = weaponFixedData.GetDataRows();
        List<SerializableRow> unlockRows = weaponUnlockData.GetDataRows();

        int upgradeLevel = 0;
        int upgradeBlockLevel = 0; 

        WeaponType weaponType = WeaponType.None;

        List<string> fixedElements = fixedRows[0].rowData;
        upgradeLevelMax = int.Parse(fixedElements[0]);
        upgradeBlockCountMax = int.Parse(fixedElements[1]);
        initWeaponCostDict.Add((int)WeaponSlotType.First, int.Parse(fixedElements[2]));
        initWeaponCostDict.Add((int)WeaponSlotType.Second, int.Parse(fixedElements[3]));
        initWeaponCostDict.Add((int)WeaponSlotType.Third, int.Parse(fixedElements[4]));

        for (int i = 0; i < rows.Count; i++)
        {
            List<string> elements = rows[i].rowData;
            if (i % upgradeBlockCountMax == 0)
            {
                upgradeLevel = int.Parse(elements[0]);
            }

            upgradeBlockLevel = int.Parse(elements[1]);

            weaponType = WeaponType.Saw;
            WeaponData sawUpgradeData = new WeaponData(weaponType, upgradeLevel, upgradeBlockLevel, new BigInteger(elements[2]),
                new BigInteger(elements[3]), new BigInteger(elements[4]));
            TryAddWeaponData(upgradeLevel, upgradeBlockLevel, weaponType, sawUpgradeData);

            weaponType = WeaponType.FlameThrower;
            WeaponData flameThrowerUpgradeData = new WeaponData(weaponType, upgradeLevel, upgradeBlockLevel, new BigInteger(elements[5]),
                new BigInteger(elements[6]), new BigInteger(elements[7]));
            TryAddWeaponData(upgradeLevel, upgradeBlockLevel, weaponType, flameThrowerUpgradeData);

            weaponType = WeaponType.MachineGun;
            WeaponData machineGunUpgradeData = new WeaponData(weaponType, upgradeLevel, upgradeBlockLevel, new BigInteger(elements[8]),
                new BigInteger(elements[9]), new BigInteger(elements[10]));
            TryAddWeaponData(upgradeLevel, upgradeBlockLevel, weaponType, machineGunUpgradeData);

            weaponType = WeaponType.Shocker;
            WeaponData shockerUpgradeData = new WeaponData(weaponType, upgradeLevel, upgradeBlockLevel, new BigInteger(elements[11]),
            new BigInteger(elements[12]), new BigInteger(elements[13]));
            TryAddWeaponData(upgradeLevel, upgradeBlockLevel, weaponType, shockerUpgradeData);

            weaponType = WeaponType.Laser;
            WeaponData LaserUpgradeData = new WeaponData(weaponType, upgradeLevel, upgradeBlockLevel, new BigInteger(elements[14]),
            new BigInteger(elements[15]), new BigInteger(elements[16]));
            TryAddWeaponData(upgradeLevel, upgradeBlockLevel, weaponType, LaserUpgradeData);

            weaponType = WeaponType.Rocket;
            WeaponData RocketUpgradeData = new WeaponData(weaponType, upgradeLevel, upgradeBlockLevel, new BigInteger(elements[17]),
            new BigInteger(elements[18]), new BigInteger(elements[19]));
            TryAddWeaponData(upgradeLevel, upgradeBlockLevel, weaponType, RocketUpgradeData);

            weaponType = WeaponType.Boomerang;
            WeaponData boomerangUpgradeData = new WeaponData(weaponType, upgradeLevel, upgradeBlockLevel, new BigInteger(elements[20]),
            new BigInteger(elements[21]), new BigInteger(elements[22]));
            TryAddWeaponData(upgradeLevel, upgradeBlockLevel, weaponType, boomerangUpgradeData);

            weaponType = WeaponType.CryoGun;
            WeaponData cryoGunUpgradeData = new WeaponData(weaponType, upgradeLevel, upgradeBlockLevel, new BigInteger(elements[23]),
            new BigInteger(elements[24]), new BigInteger(elements[25]));
            TryAddWeaponData(upgradeLevel, upgradeBlockLevel, weaponType, cryoGunUpgradeData);
        }

        for (int i = 0; i < upgradeRows.Count; i++)
        {
            List<string> elements = upgradeRows[i].rowData;
            weaponType = EnumUtility.GetEqualValue<WeaponType>(elements[0]);
            int level = int.Parse(elements[1]);
            BigInteger weaponResearchCost = new BigInteger(elements[2]);
            BigInteger researchCost = new BigInteger(elements[3]);
            BigInteger beforeDamage = new BigInteger(elements[4]);
            BigInteger afterDamage = new BigInteger(elements[5]);

            WeaponUpgradeData weaponUpgradeTempData = new WeaponUpgradeData(weaponType, level, weaponResearchCost, researchCost, beforeDamage, afterDamage);

            if (!weaponUpgradeDataDict.ContainsKey(weaponType))
            {
                weaponUpgradeDataDict.Add(weaponType, new Dictionary<int, WeaponUpgradeData>());
            }

            if (!weaponUpgradeDataDict[weaponType].ContainsKey(level))
            {
                weaponUpgradeDataDict[weaponType].Add(level, weaponUpgradeTempData);
            }
        }

        for (int i = 0; i < unlockRows.Count; i++)
        {
            List<string> unlockElements = unlockRows[i].rowData;

            weaponType = EnumUtility.GetEqualValue<WeaponType>(unlockElements[0]);
            int stageNum = int.Parse(unlockElements[1]);

            if (!unlockStageDictByWeaponType.ContainsKey(weaponType))
            {
                unlockStageDictByWeaponType.Add(weaponType, stageNum);
            }

            if (!weaponTypeDictByUnlockStage.ContainsKey(stageNum))
            {
                weaponTypeDictByUnlockStage.Add(stageNum, new List<WeaponType>());
            }

            weaponTypeDictByUnlockStage[stageNum].Add(weaponType);
        }

        List<SerializableRow> abilityFixedDatas = weaponAbilityFixedData.GetDataRows();

        AbilityType abilityType = AbilityType.None;

        WeaponType[] weaponTypes = (WeaponType[])Enum.GetValues(typeof(WeaponType));

        for (int i = 1; i < weaponTypes.Length; i++)
        {
            abilityTypeDict.Add(weaponTypes[i], new List<AbilityType>());
        }

        for (int i = 0; i < abilityFixedDatas.Count; i++)
        {
            List<string> elements = abilityFixedDatas[i].rowData;

            for (int j = 1; j < weaponTypes.Length; j++)
            {
                if (elements[j - 1] != "")
                {
                    weaponType = weaponTypes[j];
                    abilityType = EnumUtility.GetEqualValue<AbilityType>(elements[j - 1]);

                    if (!abilityTypeDict[weaponType].Contains(abilityType))
                    {
                        abilityTypeDict[weaponType].Add(abilityType);
                    }
                }
            }

            abilityType = EnumUtility.GetEqualValue<AbilityType>(elements[8]);
            bool isRate = elements[9].Contains('%');
            ArithmeticStatType arithmeticStatType = isRate ? ArithmeticStatType.Rate : ArithmeticStatType.Base;
            int initEvolutionValue = isRate ? int.Parse(elements[9].Split('%')[0]) : int.Parse(elements[9]);
            int increasingEvolutionValue = isRate ? int.Parse(elements[10].Split('%')[0]) : int.Parse(elements[10]);
            int evolutionCountMax = int.Parse(elements[11]);
            WeaponAbilityData weaponAbilityData = new WeaponAbilityData(abilityType, arithmeticStatType, initEvolutionValue, increasingEvolutionValue, 
                evolutionCountMax);

            if (!weaponAbilityDataDict.ContainsKey(abilityType))
            {
                weaponAbilityDataDict.Add(abilityType, weaponAbilityData);
            }
        }

        AbilityType[] abilityTypes = (AbilityType[])Enum.GetValues(typeof(AbilityType));

        for (int i = 1; i < weaponTypes.Length; i++)
        {
            weaponType = weaponTypes[i];

            Sprite sprite = Resources.Load<Sprite>($"Sprites/Weapons/{weaponType}");
            if (!weaponSpriteDict.ContainsKey(weaponType))
            {
                weaponSpriteDict.Add(weaponType, sprite);
            }

            Weapon weapon = Resources.Load<Weapon>($"Weapons/{weaponType}");

            if (!weaponResourceDict.ContainsKey(weaponType))
            {
                weaponResourceDict.Add(weaponType, weapon);
            }

            for (int j = 1; j < abilityTypes.Length; j++)
            {
                if (!abilitySpriteDict.ContainsKey(weaponType))
                {
                    abilitySpriteDict.Add(weaponType, new Dictionary<AbilityType, Sprite>());
                }

                abilityType = abilityTypes[j];

                if (!abilitySpriteDict[weaponType].ContainsKey(abilityType))
                {
                    abilitySpriteDict[weaponType].Add(abilityType, Resources.Load<Sprite>($"Sprites/Weapons/Abilities/{weaponType}_{abilityType}"));
                }
            }
        }
    }
    #endregion

    public Sprite GetWeaponSprite(WeaponType weaponType)
    {
        if (weaponSpriteDict.ContainsKey(weaponType))
        {
            return weaponSpriteDict[weaponType];
        }

        return default;
    }

    public Weapon GetWeapon(WeaponType weaponType)
    {
        if (weaponResourceDict.ContainsKey(weaponType))
        {
            return weaponResourceDict[weaponType];
        }

        return default;
    }

    // 레벨업만 된 원본 무기 데이터
    public WeaponData GetWeaponData(WeaponType weaponType, int upgradeLevel, int upgradeBlockCount)
    {
        if (weaponDataDict.ContainsKey(weaponType))
        {
            if (weaponDataDict[weaponType].ContainsKey(upgradeLevel))
            {
                if (weaponDataDict[weaponType][upgradeLevel].ContainsKey(upgradeBlockCount))
                {
                    return weaponDataDict[weaponType][upgradeLevel][upgradeBlockCount];
                }
            }
        }

        return default;
    }

    public WeaponUpgradeData GetWeaponUpgradeData(WeaponType weaponType, int upgradeLevel)
    {
        if (weaponUpgradeDataDict.ContainsKey(weaponType))
        {
            if (weaponUpgradeDataDict[weaponType].ContainsKey(upgradeLevel))
            {
                return weaponUpgradeDataDict[weaponType][upgradeLevel];
            }
        }

        return default;
    }

    public List<WeaponType> GetWeaponTypeByStage(int stageNum)
    {
        if (weaponTypeDictByUnlockStage.ContainsKey(stageNum))
        {
            return weaponTypeDictByUnlockStage[stageNum];
        }

        return default;
    }

    public int GetWeaponUnlockStage(WeaponType weaponType)
    {
        if (unlockStageDictByWeaponType.ContainsKey(weaponType))
        {
            return unlockStageDictByWeaponType[weaponType];
        }

        return default;
    }

    public int GetInitWeaponCost(int index)
    {
        if (initWeaponCostDict.ContainsKey(index))
        {
            return initWeaponCostDict[index];
        }

        return default;
    }

    public (AbilityType, AbilityType) GetRandomAbilityTypePair(WeaponType weaponType, List<AbilityType> removingTypes = null)
    {
        if (abilityTypeDict.ContainsKey(weaponType))
        {
            List<AbilityType> abilityTypes = new List<AbilityType>(abilityTypeDict[weaponType].Count);
            abilityTypes.AddRange(abilityTypeDict[weaponType]);

            if (removingTypes != null)
            {
                for (int i = 0; i < removingTypes.Count; i++)
                {
                    abilityTypes.Remove(removingTypes[i]);
                }
            }

            int firstIndex = UnityEngine.Random.Range(0, abilityTypes.Count);
            AbilityType firstAbilityType = abilityTypes[firstIndex];
            abilityTypes.Remove(firstAbilityType);
            int secondIndex = UnityEngine.Random.Range(0, abilityTypes.Count);
            AbilityType secondAbilityType = abilityTypes[secondIndex];
            return (firstAbilityType, secondAbilityType);
        }

        return default;
    }

    public int GetEvolutionCountMax(AbilityType abilityType)
    {
        if (weaponAbilityDataDict.ContainsKey(abilityType))
        {
            return weaponAbilityDataDict[abilityType].evolutionCountMax;
        }

        return default;
    }

    public Sprite GetAbilitySprite(WeaponType weaponType, AbilityType abilityType)
    {
        if (abilitySpriteDict.ContainsKey(weaponType))
        {
            if (abilitySpriteDict[weaponType].ContainsKey(abilityType))
            {
                return abilitySpriteDict[weaponType][abilityType];
            }
        }

        return default;
    }

    public WeaponAbilityData GetWeaponAbilityData(AbilityType abilityType)
    {
        if (weaponAbilityDataDict.ContainsKey(abilityType))
        {
            return weaponAbilityDataDict[abilityType];
        }

        return default;
    }

    public int GetUpgradeLevelMax()
    {
        return upgradeLevelMax;
    }

    public int GetUpgradeBlockCountMax()
    {
        return upgradeBlockCountMax;
    }

    private void TryAddWeaponData(int upgradeLevel, int upgradeBlockLevel, WeaponType weaponType, WeaponData weaponData)
    {
        if (!weaponDataDict.ContainsKey(weaponType))
        {
            weaponDataDict.Add(weaponType, new Dictionary<int, Dictionary<int, WeaponData>>());
        }

        if (!weaponDataDict[weaponType].ContainsKey(upgradeLevel))
        {
            weaponDataDict[weaponType].Add(upgradeLevel, new Dictionary<int, WeaponData>());
        }

        if (!weaponDataDict[weaponType][upgradeLevel].ContainsKey(upgradeBlockLevel))
        {
            weaponDataDict[weaponType][upgradeLevel].Add(upgradeBlockLevel, weaponData);
        }
    }
}
