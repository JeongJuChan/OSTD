using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MeleeTickWeapon : Weapon, ITickAttack
{
    [SerializeField] protected float skillDisableTime = 5f;
    [SerializeField] protected ProjectileType projectileType;
    protected ProjectilePooler pooler;

    protected HashSet<Monster> monsters = new HashSet<Monster>();
    protected HashSet<Monster> waitForRemoveMonsters = new HashSet<Monster>();

    [SerializeField] protected Transform firePoint; // 발사 위치
    [SerializeField] protected float projectileSpeed = 10f; // 발사체 속도

    [SerializeField] private BoxCollider2D boxCollider2D;

    [SerializeField] private float skillInterval = 0.2f;
    protected WaitForSeconds skillIntervalSeconds;

    protected int skillCount = 1;
    [SerializeField] private int offsetSkillCount = 1;

    void FixedUpdate()
    {
        ApplyDamage();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(Consts.MONSTER_TAG))
        {
            if (other.TryGetComponent(out Monster monster))
            {
                monsters.Add(monster);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(Consts.MONSTER_TAG))
        {
            if (other.TryGetComponent(out Monster monster))
            {
                waitForRemoveMonsters.Add(monster);
            }
        }
    }

    public override void Init()
    {
        pooler = PoolManager.instance.projectile;
        skillCount = offsetSkillCount;
        int skillCountMaxMod = ResourceManager.instance.weapon.GetWeaponAbilityData(AbilityType.Skill_AddShot).evolutionCountMax + 1;
        int boxCountMax = ResourceManager.instance.box.GetBoxMaxCount();
        int skillCountMax = skillCountMaxMod * boxCountMax;
        PoolManager.instance.projectile.AddPoolInfo((int)projectileType, skillCountMax * 2, skillCountMax * 2);

        UpdateBoxEnableState(false);

        GameManager.instance.OnStart += () => UpdateBoxEnableState(true);
        GameManager.instance.OnReset += () => UpdateBoxEnableState(false);

        skillIntervalSeconds = CoroutineUtility.GetWaitForSeconds(skillInterval);
    }

    public void UpdateSkillCount(int skillCount)
    {
        this.skillCount = offsetSkillCount + skillCount;
    }

    private void UpdateBoxEnableState(bool isEnabled)
    {
        boxCollider2D.enabled = isEnabled;
    }

    public abstract void ApplyDamage();

    protected abstract IEnumerator CoUseSkill();
}
