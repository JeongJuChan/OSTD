using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceManager : Singleton<ResourceManager>
{
    public UnlockDataSO unlockDataSO { get; private set; }
    public MonsterResourceHandler monster { get; private set; }
    public EffectDataHandler effect { get; private set; }
    public SkillDataHandler skill { get; private set; }
    public RankDataHandler rank { get; private set; }
    public EnumToKRSO enumToKRSO { get; private set; }
    public BoxResourceDataHandler box { get; private set; }
    public StageResourceDataHandler stage { get; private set; }
    public ProjectileDataHandler projectile { get; private set; }
    public EnergyDataHandler energy { get; private set; }
    public EquipmentResourceDataHandler equipment { get; private set; }
    public WeaponResourceDataHandler weapon { get; private set; }
    public RewardResourceDataHandler reward { get; private set; }
    public QuestResourceDataHandler quest { get; private set; }

    public void Init()
    {
        box = new BoxResourceDataHandler();
        box.Init();
        monster = new MonsterResourceHandler();
        monster.Init();
        stage = new StageResourceDataHandler();
        stage.Init();
        projectile = new ProjectileDataHandler();
        projectile.Init();
        energy = new EnergyDataHandler();
        energy.Init();
        rank = new RankDataHandler();
        rank.Init();
        equipment = new EquipmentResourceDataHandler();
        equipment.Init();
        weapon = new WeaponResourceDataHandler();
        weapon.Init();
        skill = new SkillDataHandler();
        skill.Init();
        effect = new EffectDataHandler();
        effect.Init();
        reward = new RewardResourceDataHandler();
        reward.Init();
        quest = new QuestResourceDataHandler();
        quest.Init();
    }
}
