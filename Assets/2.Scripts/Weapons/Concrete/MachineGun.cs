using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGun : TargetingWeapon
{
    [SerializeField] private int skillBulletCount = 12;
    [SerializeField] private float shootingBulletInterval = 0.08f;
    [SerializeField] private float shotPower = 10f;
    private WaitForSeconds shootingBulletWaitForSeconds;
    [SerializeField] private ParticleSystem MuzzleStart;
    [SerializeField] private Transform firePoint; // 발사 위치

    private ProjectilePooler projectilePooler;

    [SerializeField] private ProjectileType projectileType;
    [SerializeField] private ProjectileType skillProjectileType;

    private Coroutine shootingCoroutine;
    private Coroutine skillCoroutine;

    private float skillInterval;
    private WaitForSeconds skillSeconds;

    [Header("Skill")]
    [SerializeField] private float explosionPower = 4f;
    [SerializeField] private float explosionRadius = 2f;

    [SerializeField] private float shootingElapsedTime;

    private void OnEnable() 
    {
        isShootable = true;
    }

    private void FixedUpdate() 
    {
        if (isSkillActive)
        {
            return;
        }

        if (isShootable && targetMonster != null)
        {
            TryShoot();
        }
    }

    protected override void Update()
    {
        if (isSkillActive)
        {
            return;
        }

        base.Update();
    }

    public override void Init()
    {
        base.Init();
        shootingBulletWaitForSeconds = CoroutineUtility.GetWaitForSeconds(shootingBulletInterval);
        projectilePooler = PoolManager.instance.projectile;

        int bulletCount = (int)(shootingDuration / shootingBulletInterval);
        projectilePooler.AddPoolInfo((int)projectileType, bulletCount * bulletCount, bulletCount * bulletCount * bulletCount);
        projectilePooler.AddPoolInfo((int)skillProjectileType, skillBulletCount * skillBulletCount, skillBulletCount * skillBulletCount * skillBulletCount);
        skillInterval = skillDuration / skillBulletCount;
        skillSeconds = CoroutineUtility.GetWaitForSeconds(skillInterval);
    }

    public override void UseSkill()
    {
        ChangeSkillActiveState(true);
        StopShooting();

        if (skillCoroutine != null)
        {
            StopCoroutine(skillCoroutine);
        }

        if (gameObject.activeInHierarchy)
        {
            skillCoroutine = StartCoroutine(CoUsingSkill());
        }
    }

    protected override IEnumerator CoUsingSkill()
    {
        int skillBulletCount = this.skillBulletCount;
        float elapsedTime = skillInterval;

        while (skillBulletCount > 0)
        {
            Vector3 targetPos = BoxManager.instance.transform.position;
            targetPos.x += boxSizeX + skillRotateMod * elapsedTime;
            Vector3 dir = targetPos - transform.position;
            transform.rotation = Quaternion.FromToRotation(Vector3.right, dir);

            skillBulletCount--;
            ExplosionProjectile explosionProjectile = projectilePooler.Pool((int)skillProjectileType, firePoint.position, transform.rotation) as ExplosionProjectile;
            explosionProjectile.UpdateDamage(weaponData.skillDamage);
            explosionProjectile.SetExplosionPower(explosionPower);
            explosionProjectile.SetRadius(explosionRadius);
            explosionProjectile.SetShotPower(shotPower);
            explosionProjectile.Fire();

            yield return skillSeconds;
            elapsedTime += skillInterval;
        }

        UpdateIsShootableState(true);
        ChangeSkillActiveState(false);
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
                if (isShootable)
                {
                    if (!gameObject.activeInHierarchy)
                    {
                        return;
                    }

                    UpdateIsShootableState(false);
                    shootingCoroutine = StartCoroutine(CoShoot());
                }
            }
        }
    }

    protected override void UpdateIsShootableState(bool isShootable)
    {
        base.UpdateIsShootableState(isShootable);
        shootingElapsedTime = isShootable ? 0f : shootingElapsedTime;
    }

    private void StopShooting()
    {
        if (shootingCoroutine != null)
        {
            StopCoroutine(shootingCoroutine);
            MuzzleStart.Stop();
        }

    }

    private void ReLoad()
    {
        if (preFindTargetCoroutine != null)
        {
            StopCoroutine(CoReLoad());
        }

        if (!gameObject.activeInHierarchy)
        {
            return;
        }
 
        preFindTargetCoroutine = StartCoroutine(CoReLoad());
    }

    private IEnumerator CoShoot()
    {
        while (shootingElapsedTime < shootingDuration)
        {
            MuzzleStart.Play();
            Bullet projectile = projectilePooler.Pool((int)projectileType, firePoint.position, transform.rotation) as Bullet;
            projectile.SetShotPower(shotPower);
            projectile.UpdateDamage(weaponData.damage);
            projectile.Fire();
            shootingElapsedTime += shootingBulletInterval;
            yield return shootingBulletWaitForSeconds;
        }

        ReLoad();
    }

    public override void UpdateWeaponData(WeaponData weaponData)
    {
        base.UpdateWeaponData(weaponData);

        this.weaponData = weaponData;
        int index = weaponData.level - 1;
        index = index >= animators.Length ? animators.Length - 1 : index;

        firePoint.localPosition = animPosArr[index];
        MuzzleStart.transform.localPosition = animPosArr[index];
    }
}
