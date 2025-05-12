using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnumUtility
{
    public static T GetEqualValue<T>(string element)
    {
        var values = Enum.GetValues(typeof(T));
        foreach (T tempValue in values)
        {
            if (tempValue.ToString() == element)
            {
                return tempValue;
            }
        }

        return default;
    }

    public static T1 GetEqualValue<T1, T2>(T2 element)
    {
        var values = Enum.GetValues(typeof(T1));
        foreach (T1 tempValue in values)
        {
            if (tempValue.ToString() == element.ToString())
            {
                return tempValue;
            }
        }

        return default;
    }

    public static CurrencyType ChangeRewardCurrency(RewardType type)
    {
        switch (type)
        {
            case RewardType.Gold:
                return CurrencyType.Gold;
            // case RewardType.Gem:
            //     return CurrencyType.Ruby;
            default:
                return CurrencyType.None;
        }
    }

    // TODO: 로컬라이징 구현 후 지우기
    private readonly static Dictionary<Rank, string> rankKRDict = new Dictionary<Rank, string>()
    {
        {Rank.Common, "흔한"},
        {Rank.Great, "좋은"},
        {Rank.Excellent, "탁월한"},
        {Rank.Rare, "희귀한"},
        {Rank.Unique, "특별한"},
    };
    private readonly static Dictionary<WeaponType, string> weaponTypeKRDict = new Dictionary<WeaponType, string>()
    {
        {WeaponType.Saw, "전기톱"},
        {WeaponType.FlameThrower, "화염방사기"},
        {WeaponType.MachineGun, "기관총"},
        {WeaponType.Shocker, "전기충격기"},
        {WeaponType.Laser, "레이저"},
    };
    private readonly static Dictionary<EquipmentType, string> equipmentTypeKRDict = new Dictionary<EquipmentType, string>()
    {
        {EquipmentType.Shotgun, "샷건"},
        {EquipmentType.Granade, "수류탄"},
        {EquipmentType.Cap, "모자"},
        {EquipmentType.Armor, "방어구"},
        {EquipmentType.Backpack, "백팩"},
    };
    private readonly static Dictionary<StatType, string> statTypeKRDict = new Dictionary<StatType, string>()
    {
        {StatType.Attack, "공격력"},
        {StatType.Health, "체력"},
        {StatType.Capacity, "용량"},
    };
    private readonly static Dictionary<AbilityType, string> abilityTypeKRDict = new Dictionary<AbilityType, string>()
    {
        {AbilityType.Att_DamageUp, "데미지"},
        {AbilityType.Att_CoolDown, "재장전시간 감소"},
        {AbilityType.Att_DurationUp, "지속시간 증가"},
        {AbilityType.Att_RangeUp, "공격 범위 증가"},
        {AbilityType.Skill_AddShot, "추가 스킬"},
        {AbilityType.Skill_CostDown, "스킬 비용 감소"},
        {AbilityType.Skill_FreeUse, "무료 스킬 사용"},
    };


    private readonly static Dictionary<WeaponType, CurrencyType> weaponUpgradeCurrencyDict = new Dictionary<WeaponType, CurrencyType>()
    {
        {WeaponType.Saw, CurrencyType.SawCurrency},
        {WeaponType.FlameThrower, CurrencyType.FlameThrowerCurrency},
        {WeaponType.MachineGun, CurrencyType.MachineGunCurrency},
        {WeaponType.Shocker, CurrencyType.ShockerCurrency},
        {WeaponType.Laser, CurrencyType.LaserCurrency},
        {WeaponType.Rocket, CurrencyType.RocketCurrency},
        {WeaponType.Boomerang, CurrencyType.BoomerangCurrency},
        {WeaponType.CryoGun, CurrencyType.CryoGunCurrency},
    };
    private readonly static Dictionary<WeaponType, WeaponSlotType> weaponSlotTypeByWeaponType = new Dictionary<WeaponType, WeaponSlotType>()
    {
        {WeaponType.Saw, WeaponSlotType.First},
        {WeaponType.FlameThrower, WeaponSlotType.Second},
        {WeaponType.MachineGun, WeaponSlotType.Third},
        {WeaponType.Shocker, WeaponSlotType.First},
        {WeaponType.Laser, WeaponSlotType.Second},
        {WeaponType.Rocket, WeaponSlotType.Third},
        {WeaponType.Boomerang, WeaponSlotType.First},
        {WeaponType.CryoGun, WeaponSlotType.Second},
    };

    public static WeaponSlotType GetWeaponSlotTypeByWeaponType(WeaponType weaponType)
    {
        if (weaponSlotTypeByWeaponType.ContainsKey(weaponType))
        {
            return weaponSlotTypeByWeaponType[weaponType];
        }

        return default;
    }

    public static CurrencyType GetCurrencyTypeByWeaponType(WeaponType weaponType)
    {
        if (weaponUpgradeCurrencyDict.ContainsKey(weaponType))
        {
            return weaponUpgradeCurrencyDict[weaponType];
        }

        return default;
    }

    public static string GetRankKR(Rank rank)
    {
        if (rankKRDict.ContainsKey(rank))
        {
            return rankKRDict[rank];
        }

        return default;
    }

    public static string GetWeaponTypeKR(WeaponType weaponType)
    {
        if (weaponTypeKRDict.ContainsKey(weaponType))
        {
            return weaponTypeKRDict[weaponType];
        }

        return default;
    }

    public static string GetEquipmentTypeKR(EquipmentType equipmentType)
    {
        if (equipmentTypeKRDict.ContainsKey(equipmentType))
        {
            return equipmentTypeKRDict[equipmentType];
        }

        return default;
    }

    public static string GetStatTypeKR(StatType statType)
    {
        if (statTypeKRDict.ContainsKey(statType))
        {
            return statTypeKRDict[statType];
        }

        return default;
    }

    public static string GetAbilityKR(AbilityType abilityType)
    {
        if (abilityTypeKRDict.ContainsKey(abilityType))
        {
            return abilityTypeKRDict[abilityType];
        }

        return default;
    }

}
