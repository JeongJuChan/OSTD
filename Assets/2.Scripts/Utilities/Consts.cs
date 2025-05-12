using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class Consts
{
    public static class LayerInder
    {
        public const int LAYER_1 = 6;
        public const int LAYER_2 = 7;
        public const int LAYER_3 = 8;
        public const int HERO = 9;
        public const int WALL = 10;
        public const int GRANADE = 11;
        public const int FLOOR = 12;
    }

    public const float HALF = 0.5f;
    public const float DEFAULT_PIXEL_PER_UNIT = 100;
    public const int THOUSAND_DIVIDE_VALUE = 1000;
    public const string DEFAULT_ADS_GOLD = "500";
    public const float PERCENT_MUTIPLY_VALUE = 0.01f;
    public const int PERCENT_DIVIDE_VALUE = 100;
    public const int PERCENT_TOTAL_VALUE = 10000;
    public const int PERCENT_UNIT_EQUIPMENT_VALUE = 1000;
    public const int PERCENT_TOTAL_EQUIPMENT_VALUE = 100000;
    public const int COMPOSIT_UNIT = 4;
    public const int MAX_HOUR = 24;
    public const int MAX_MINUTE = 60;
    public const int MAX_SECOND = 60;

    public const int STAGE_DIVIDE_VALUE = 10;

    public const int REWARD_MOVING_CURRENCY_COUNT = 5;

    public const int GRANADE_INDEX = (int)EquipmentType.Granade - 1;
    public const int BACKPACK_INDEX = (int)EquipmentType.Backpack - 1;

    public static readonly Color32 DISABLE_COLOR = new Color32(142, 142, 142, 255);
    public static readonly Color32 HALF_TRANSPARENT_COLOR = new Color32(255, 255, 255, 128);

    public readonly static float GRAVITY = Physics2D.gravity.y; 

    #region Tag
    public const string FLOOR_TAG = "Floor";
    public const string MONSTER_TAG = "Monster";
    public const string HERO_TAG = "Hero";
    #endregion

    #region Layer
    public const int LAYER_1 = 1 << LayerInder.LAYER_1;
    public const int LAYER_2 = 1 << LayerInder.LAYER_2;
    public const int LAYER_3 = 1 << LayerInder.LAYER_3;
    public const int HERO = 1 << LayerInder.HERO;
    public const int WALL = 1 << LayerInder.WALL;
    public const int GRANADE = 1 << LayerInder.GRANADE;
    public const int FLOOR = 1 << LayerInder.FLOOR;
    #endregion
    #region SortingLayer
    public const string PLAYER_LAYER = "Player";
    public const string POPUP_UI_LAYER = "UIPopup";

    #endregion

    public const string GAME_DATA = "GameData";

    #region Ads
    public const string IN_GAME_GOLD_ADS = "InGameGoldAds";
    public const string IN_GAME_ENERGY_ADS = "InGameEnergy_Ads";
    public const string IN_GAME_DOUBLE_GOLD_ADS = "InGameDoubleGoldAds";
    public const string ABILITY_REROLL_ADS = "AbilityRerollAds";
    #endregion

    #region Guide
    public const string CURRENT_TUTORIAL_NAME = "CurrentTutorialName";
    public const string GRANADE_GUIDE = "GranadeGuide";
    public const string ADD_BOX_GUIDE = "AddBoxGuide";
    public const string MOVE_BOX_GUIDE = "MoveBoxGuide";
    public const string ENERGY_ENFORCE_GUIDE = "EnergyEnforceGuide";
    public const string ADD_WEAPON_GUIDE = "AddWeaponGuide";
    public const string USING_FIRST_SKILL_GUIDE = "UsingFirstSkillGuide";
    public const string USING_SECOND_SKILL_GUIDE = "UsingSecondSkillGuide";
    public const string USING_THIRD_SKILL_GUIDE = "UsingThridSkillGuide";
    public const string EQUIPMENT_ENFORCE_REWARDED = "EquipmentEnforceRewarded";
    public const string HERO_TAP_TOUCHED_GUIDE_START = "HeroTapTouchedGuideStart";
    public const string HERO_TAP_TOUCHED_GUIDE = "HeroTapTouchedGuide";
    public const string GRANADE_TOUCHED_GUIDE = "GranadeTouchedGuide";
    public const string GRANADE_EQUIPPED_GUIDE = "GranadeEquippedGuide";
    public const string EQUIPPED_GRANADE_TOUCHED_GUIDE = "EquippedGranadeTouchedGuide";
    public const string EQUIPPED_GRANADE_LEVELUP_GUIDE = "EquippedGranadeLevelUpGuide";
    public const string SELL_DUPLICATES_GUIDE = "SellDuplicatesGuide";
    public const string SHOP_TAP_TOUCHED_GUIDE = "ShopTapTouchedGuide";
    public const string SUMMON_EQUIPMENT_GUIDE = "SummonEquipmentGuide";
    public const string AUTO_EQUIP_GUIDE = "AutoEquipGuide";
    #endregion

    #region SaveStrings
    public const string DATA_BASE_KEY_HASH = "KeyHash";

    public const string ADS_SHOWN_TODAY_SAVE = "adsShownToday";
    public const string LAST_AD_SHOW_DATE_SAVE = "lastAdShowDate";
    public const string REWARD_TIME = "currentTime";
    public const string REWARD_DAY = "currentDay";

    public const string QUEST_MANAGER_DATA = "QuestManager_Data";
    public const string QUEST_PREFIX = "Quest_";
    public const string QUEST_COUNT = "_COUNT";
    public const string QUEST_CURRENT_INDEX = "_CurrentIndex";

    public const string MAIN_STAGE_NUM = "MainStageNum";
    public const string CURRENT_DIFFICULTY = "CurrentDifficulty";
    public const string STAGE_PRE_CHECK_POINT_NUM = "stagePreCheckPointNum";

    public const string SUMMON_PREFIX = "Summon_";
    public const string CURRENT_SUMMON_EXP = "_CurrentSummonExp";
    public const string CURRENT_SUMMON_LEVEL = "_CurrentSummonLevel";
    public const string MAX_SUMMON_EXP = "_MaxSummonExp";
    public const string SUMMON_IS_LOCKED = "_IsLocked";

    public const string IS_LOCKED = "IsLocked";

    public const string IS_FIRST_TIME_INSTALLED = "IsFirstTimeInstalled";

    public const string IS_SHOWING_SUMMON_ANIM_STATE = "isShowingSummonAnimState";

    public const string FirstOpen = "wasFirstOpenFinished";

    public const string CURRENT_EQUIPMENT_STAT_DATA = "CurrentEquipmentStatData";

    public const string EQUIPMENTS_IN_INVENTORY = "EquipmentsInInvetory";

    public const string BOX_LEVELS = "BoxLevels";

    public const string CURRENT_WEAPON_TYPES = "CurrentWeaponTypes";

    public const string WEAPON_DATAS = "WeaponDatas";

    public const string CURRENT_ENERGY_LEVEL = "CurrentEnergyLevel";

    public const string BONUS_GOLD_TIME = "BonusGoldTime";

    public const string INTERSTITIAL_TIME = "InterstitialTime";
    public const string HAVE_STAGE_CLEARED = "HaveStageCleared";
    public const string IS_RECENTLY_REWARDED = "IsRecentlyRewarded";

    public const string HIGHEST_INGAME_GOLD = "HighestIngameGold";

    public const string PLAY_COUNT = "PlayCount";

    public const string WEAPON_UPGRADE_LEVELS = "WeaponUpgradeLevels";

    public const string WEAPON_ABILITIES = "WeaponAbilities";

    public const string WEAPON_CURRENT_CHOOSEABLE_ABILITY_TYPES = "CurrentChooseableAbilityTypes";

    public const string WEAPON_ABILITY_COUNT_ARR = "WeaponAbilityCountArr";
    #endregion
}
