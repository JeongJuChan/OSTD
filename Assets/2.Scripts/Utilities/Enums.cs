using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Rank
{
    None,
    Common,
    Great,
    Excellent,
    Rare,
    Unique,
}

public enum SummonType
{
    None,
    Equipment,
}

public enum EffectType
{
    None,
    LaserFloorEffect = 30000,
}

public enum EffectMaterialType
{
    None,
    DamageFlash,

}

public enum StatType
{
    None,
    Attack,
    Health,
    Capacity,
}

public enum MonsterStateType
{
    None,
    Forward,
    Backward,
    Attack,
    Jump,
}

public enum MonsterMovementStateType
{
    None,
    Idle,
    Jump,
    Forward,
    Backward,
    Fall,
}

public enum SkillMoveType
{
    Throw,
    // 설치형
    Trigger,
}

public enum EquipmentType
{
    None,
    Shotgun,
    Granade,
    Cap,
    Armor,
    Backpack,
}

public enum CurrencyType
{
    None,
    Gold,
    Gem,
    EnforcePowder,
    Research,
    Key,
    ShotGunBluePrint,
    GranadeBluePrint,
    CapBluePrint,
    ArmorBluePirnt,
    SawCurrency,
    FlameThrowerCurrency,
    MachineGunCurrency,
    ShockerCurrency,
    LaserCurrency,
    RocketCurrency,
    BoomerangCurrency,
    CryoGunCurrency,
}

public enum DailyAdsType
{
    None,
    FreeGem,
}

public enum UIAnimationType
{
    None, Scale, VerticalPunch
}

public enum RewardType
{
    None,
    Gold,
    Gem,
    Equipment,
    BluePrint,
    EnforcePowder,
    StageClearReward,
    Research,
    Key,
}

// 반복퀘스트 증가값은 이 enum값으로 순서대로 설정함
public enum QuestType
{
    Event,
    MonsterKillCount,
    StageClear, // 반복 시 1씩 증가
}

public enum EventQuestType
{
    None,
}

public enum ArithmeticStatType
{
    Base,
    Rate
}

public enum SceneType
{
    TitleScene,
    MainScene,
}

public enum DamageType
{
    Normal,
    Critical,
}

public enum RedDotIDType
{
    Bottombar_Shop,
    Bottombar_Hero,
    Bottombar_Battle,
    Bottombar_Weapon,
    Bottombar_Boss,
    Shop_Equipment_Summon_Small,
    Shop_Equipment_Summon_Large,
    Shop_Equipment_Summon_Ads,
    Shop_Free_Gem_Ads,
    Hero_Equip_Best,
    Hero_Sell_Duplicates,
    Hero_Shotgun,
    Hero_Granade,
    Hero_Cap,
    Hero_Armor,
    Hero_Backpack,
}

public enum MonsterType
{
    None,
    Common,
    Range,
    Tank,
    Flying,
    Basement,
}

public enum FeatureType
{
    Stage,
}

public enum FeatureID
{
    None,
    FirstOpen,
}

public enum ProjectileType
{ 
    None,
    Bullet = 20000,
    Granade = 20001,
    ElectricShock = 20002,
    DinoProjectile = 20003,
    SawProjectile = 20004,
    MachineGunProjectile = 20005,
    MachineGunSkillProjectile = 20006,
    ZombieProjectile = 20007,
    SlimeProjectile = 20008,
    RocketProjectile = 20009,
    RocketSkillProjectile = 20010,
    BoomerangProjectile = 20011,
}

public enum WeaponType
{
    None,
    Saw,
    FlameThrower,
    MachineGun,
    Shocker,
    Laser,
    Rocket,
    Boomerang,
    CryoGun,
}

public enum WeaponSlotType
{
    None,
    First,
    Second,
    Third,
}

public enum SkillType
{
    None,
    Granade,
    Saw,
    FlameThrower,
    MachineGun,
    Shocker,
    Laser,
    Rocket,
    Boomerang,
    CryoGun,
}

public enum AbilityType
{
    None,
    Att_DamageUp,
    Att_CoolDown,
    Att_DurationUp,
    Skill_CostDown,
    Skill_FreeUse,
    Skill_AddShot,
    Att_RangeUp,
}

public enum DailyQuestType
{
    DEFEAT_ENEMY,
    PRODUCE_ENERGY,
    USE_SAW_ABILITY,
    WATCH_ADS,
    COLLECT_COINS,
    USE_GRENADE,
}