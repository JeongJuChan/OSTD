using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CryoGun : TargetingWeapon, ITickAttack
{
    [SerializeField] private ParticleSystem FireIdle;
    [SerializeField] private ParticleSystem FireAtk;
    [SerializeField] private ParticleSystem FireSP;

    [SerializeField] private CommonRangeAttackTrigger commonAttackTrigger;
    [SerializeField] private SkillRangeAttackTrigger skillAttackTrigger;

    private List<MonsterBase> monsters = new List<MonsterBase>();
    private List<MonsterBase> waitForRemoveMonsters = new List<MonsterBase>();

    [SerializeField] protected float shootingElapsedTime = 0f;

    private void OnEnable() 
    {
        Reset();
    }

    private void FixedUpdate() 
    {
        if (isSkillActive)
        {
            ApplySkillDamage();
            FireAttack();
            return;
        }

        if (isShootable && targetMonster != null)
        {
            TryShoot();
        }
    }

    public override void Init()
    {
        base.Init();
        GameManager.instance.OnReset += () => UpdateAttackTriggerActiveState(false);
        UIManager.instance.GetUIElement<UI_Battle>().OnClickGameStart += () => UpdateAttackTriggerActiveState(true);
        UpdateAttackTriggerActiveState(false);
    }

    private void Reset()
    {
        FireIdle.Play();
        FireAtk.Stop();
        FireSP.Stop();
        ChangeSkillActiveState(false);
    }

    public void ApplyDamage()
    {
        monsters.Clear();
        waitForRemoveMonsters.Clear();
        monsters.AddRange(commonAttackTrigger.monsters);
        waitForRemoveMonsters.AddRange(commonAttackTrigger.waitForRemoveMonsters);

        foreach (Monster monster in monsters)
        {
            if (!monster.isDead)
            {
                BattleManager.instance.OnMonsterAttacked(monster, this, monster.GetDamageTextPivot().position, false, weaponData.damage);
            }
        }

        foreach (Monster removingMonster in waitForRemoveMonsters)
        {
            commonAttackTrigger.monsters.Remove(removingMonster);
        }

        commonAttackTrigger.waitForRemoveMonsters.Clear();
    }

    public override void UseSkill()
    {
        ChangeSkillActiveState(true);
        commonAttackTrigger.ClearMonsterSets();

        if (preFindTargetCoroutine != null)
        {
            StopCoroutine(CoUsingSkill());
        }

        if (!gameObject.activeInHierarchy)
        {
            return;
        }

        StartCoroutine(CoUsingSkill());
    }

    protected override IEnumerator CoUsingSkill()
    {
        float elapsedTime = Time.deltaTime;
        while (elapsedTime < skillDuration)
        {
            Vector3 targetPos = BoxManager.instance.transform.position;
            targetPos.x += boxSizeX + skillRotateMod * elapsedTime;
            Vector3 dir = targetPos - transform.position;

            transform.rotation = Quaternion.FromToRotation(Vector3.right, dir);

            yield return null;
            elapsedTime += Time.deltaTime;
        }

        ChangeSkillActiveState(false);
    }

    protected override void ChangeSkillActiveState(bool isActive)
    {
        base.ChangeSkillActiveState(isActive);
        if (isActive)
        {
            commonAttackTrigger.ClearMonsterSets();
            commonAttackTrigger.UpdateColliderActiveState(false);
            skillAttackTrigger.UpdateColliderActiveState(true);
        }
        else
        {
            skillAttackTrigger.ClearMonsterSets();
            skillAttackTrigger.UpdateColliderActiveState(false);
            commonAttackTrigger.UpdateColliderActiveState(true);
        }
    }

    protected void ApplySkillDamage()
    {
        if (skillAttackTrigger.monsters.Count == 0)
        {
            return;
        }

        monsters.Clear();
        waitForRemoveMonsters.Clear();
        monsters.AddRange(skillAttackTrigger.monsters);
        waitForRemoveMonsters.AddRange(skillAttackTrigger.waitForRemoveMonsters);

        foreach (MonsterBase monster in monsters)
        {
            if (!monster.isDead)
            {
                BattleManager.instance.OnMonsterAttacked(monster, this, monster.GetDamageTextPivot().position, false, weaponData.skillDamage);
            }
        }

        foreach (MonsterBase removingMonster in waitForRemoveMonsters)
        {
            skillAttackTrigger.monsters.Remove(removingMonster);
        }

        skillAttackTrigger.waitForRemoveMonsters.Clear();
    }

    private void FireAttack()
    {
        FireIdle.Stop();
        if (isSkillActive)
        {
            FireAtk.Stop();
            FireSP.Play();
        }
        else
        {
            FireAtk.Play();
            FireSP.Stop();
        }
    }

    protected override void TryShoot()
    {
        CheckRay();
        if (hit)
        {
            if (targetMonster == null)
            {
                return;
            }

            if (hit.collider.CompareTag(Consts.MONSTER_TAG))
            {
                if (commonAttackTrigger.monsters.Count == 0)
                {
                    Reset();
                    return;
                }

                FireAttack();
                ApplyDamage();
                shootingElapsedTime += Time.deltaTime;
                if (shootingElapsedTime >= shootingDuration)
                {
                    if (preFindTargetCoroutine != null)
                    {
                        StopCoroutine(CoReLoad());
                    }

                    if (!gameObject.activeInHierarchy)
                    {
                        return;
                    }

                    Reset();
                    preFindTargetCoroutine = StartCoroutine(CoReLoad());
                    shootingElapsedTime = 0f;
                }
            }
            else
            {
                Reset();
            }
        }
        else
        {
            Reset();
        }
    }

    public override void UpdateWeaponData(WeaponData weaponData)
    {
        base.UpdateWeaponData(weaponData);
        int index = weaponData.level - 1;

        if (index >= animators.Length)
        {
            return;
        }

        FireIdle.transform.localPosition = animPosArr[index];
        FireAtk.transform.localPosition = animPosArr[index];
        FireSP.transform.localPosition = animPosArr[index];
    }

    private void UpdateAttackTriggerActiveState(bool isActive)
    {
        commonAttackTrigger.UpdateColliderActiveState(isActive);
        skillAttackTrigger.UpdateColliderActiveState(isActive);
    }
}
