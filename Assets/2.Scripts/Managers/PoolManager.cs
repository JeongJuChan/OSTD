using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviourSingleton<PoolManager>
{
    public MonsterObjectPooler monster { get; private set; }
    public ProjectilePooler projectile { get; private set; }
    public EffectObjectPooler effect { get; private set; }
    public WeaponObjectPooler weapon { get; private set; }

    public void Init()
    {
        float duration = ResourceManager.instance.stage.GetSpawningDurationAfterEnemyBaseDestroyed();

        monster = new MonsterObjectPooler(transform, null);

        projectile = new ProjectilePooler(transform);

        effect = new EffectObjectPooler(transform);

        weapon = new WeaponObjectPooler(transform);
    }
}
