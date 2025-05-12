using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : TargetingWeapon
{
    [SerializeField] private int bulletCount = 3;
    private int currentBulletCount;
    [SerializeField] private int offsetSkillBulletCount = 12;
    [SerializeField] private float shootingBulletInterval = 0.5f;
    [SerializeField] private float shotPower = 5f;
    private WaitForSeconds shootingBulletWaitForSeconds;
    [SerializeField] private ParticleSystem MuzzleStart;
    [SerializeField] private Transform firePoint; // 발사 위치

    private ProjectilePooler projectilePooler;

    [SerializeField] private ProjectileType projectileType;
    [SerializeField] private ProjectileType skillProjectileType;

    private Vector2 dotVector = new Vector2(1, 1).normalized;
    private Vector3 maxDir;

    private Coroutine shootingCoroutine;
    private Coroutine skillCoroutine;

    private float skillInterval;
    private WaitForSeconds skillSeconds;

    [SerializeField] private float explosionPower = 4f;
    [SerializeField] private float explosionRadius = 2f;
    private float offsetExplosionRadius;

    [SerializeField] private float angleMax = 45f;
    [SerializeField] private float skillAngle = 30f;

    [SerializeField] private Sprite[] projectileSprites;
    private Sprite currentProjectileSprite;

    [SerializeField] private float targetRadius = 2f;
    private Transform rocketCircleTrans;

    [SerializeField] private float rocketFallingSpeedMod = 1.25f;

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
        projectilePooler.AddPoolInfo((int)skillProjectileType, offsetSkillBulletCount * offsetSkillBulletCount, offsetSkillBulletCount * offsetSkillBulletCount * offsetSkillBulletCount);
        skillInterval = skillDuration / offsetSkillBulletCount;
        skillSeconds = CoroutineUtility.GetWaitForSeconds(skillInterval);

        currentBulletCount = this.bulletCount;
        offsetExplosionRadius = explosionRadius;

        maxDir = Quaternion.Euler(new Vector3(0, 0, -angleMax)) * Vector3.right;

        rocketCircleTrans = BoxManager.instance.rocketCircleTrans;
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
        int skillBulletCount = offsetSkillBulletCount;
        float elapsedTime = skillInterval;
        transform.rotation = Quaternion.FromToRotation(Vector3.right, Quaternion.Euler(new Vector3(0, 0, angleMax)) * Vector3.right);

        while (skillBulletCount > 0)
        {
            MuzzleStart.Play();
            float targetDistanceX = boxSizeX * Consts.HALF + skillRotateMod * elapsedTime;

            skillBulletCount--;
            RocketSkillProjectile rocketSkillProjectile = 
                projectilePooler.Pool((int)skillProjectileType, firePoint.position, transform.rotation) as RocketSkillProjectile;
            rocketSkillProjectile.Init();
            rocketSkillProjectile.SetSprite(currentProjectileSprite);
            rocketSkillProjectile.SetTargetCirlce(rocketCircleTrans);
            rocketSkillProjectile.UpdateDamage(weaponData.skillDamage);
            rocketSkillProjectile.SetExplosionPower(explosionPower);
            rocketSkillProjectile.SetRadius(explosionRadius);
            rocketSkillProjectile.SetShotPower(shotPower);
            rocketSkillProjectile.SetTargetRadius(targetRadius);
            rocketSkillProjectile.SetTargetDistance(targetDistanceX);
            rocketSkillProjectile.SetFallingSpeed(rocketFallingSpeedMod);
            rocketSkillProjectile.Fire();

            yield return skillSeconds;
            elapsedTime += skillInterval;
        }

        UpdateIsShootableState(true);
        ChangeSkillActiveState(false);
    }

    protected override void TryShoot()
    {
        // CheckRay();
        // if (hit)
        // {
        if (targetMonster == null)
        {
            return;
        }

        // if (hit.collider.CompareTag(Consts.MONSTER_TAG))
        // {
        if (isShootable)
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }

            UpdateIsShootableState(false);
            shootingCoroutine = StartCoroutine(CoShoot());
        }
        // }
        // }
    }

    protected override void UpdateIsShootableState(bool isShootable)
    {
        base.UpdateIsShootableState(isShootable);
        if (isShootable)
        {
            currentBulletCount = bulletCount;
        }
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
        while (currentBulletCount > 0)
        {
            MuzzleStart.Play();
            currentBulletCount--;
            RocketProjectile rocketProjectile = projectilePooler.Pool((int)projectileType, firePoint.position, transform.rotation) as RocketProjectile;
            float firstPosX = (firePoint.position.y - BoxManager.instance.transform.position.y) * Consts.HALF;
            firstPosX = firstPosX >= 0 ? firstPosX : -firstPosX;
            rocketProjectile.Init();
            rocketProjectile.SetSprite(currentProjectileSprite);
            rocketProjectile.SetFirstPosX(firstPosX);
            rocketProjectile.UpdateDamage(weaponData.damage);
            rocketProjectile.SetExplosionPower(explosionPower);
            rocketProjectile.SetRadius(explosionRadius);
            rocketProjectile.SetTarget(target);
            rocketProjectile.SetShotPower(shotPower);
            rocketProjectile.Fire();
            yield return shootingBulletWaitForSeconds;
        }

        ReLoad();
    }

    public void UpdateExplosionRadius(float explosionMod)
    {
        explosionRadius = offsetExplosionRadius + offsetExplosionRadius * explosionMod;
    }

    public override void UpdateWeaponData(WeaponData weaponData)
    {
        base.UpdateWeaponData(weaponData);

        this.weaponData = weaponData;
        int index = weaponData.level - 1;
        index = index >= animators.Length ? animators.Length - 1 : index;
        currentProjectileSprite = projectileSprites[index];
        // firePoint.localPosition = animPosArr[index];
        // MuzzleStart.transform.localPosition = animPosArr[index];
    }

    protected override void UpdateRotation()
    {
        Vector3 dir = target.position - transform.position;

        // Dot
        Vector3 dirNormal = dir.normalized;
        float dot = dotVector.x * dirNormal.x + dotVector.y * dirNormal.y;
        if (dot < 0)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.FromToRotation(Vector3.right, maxDir), 
                rotateSpeed * Time.deltaTime);
        }
        else
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.FromToRotation(Vector3.right, dir), 
                rotateSpeed * Time.deltaTime);
        }
        // currentOffsetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        // transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(new Vector3(0, 0, currentOffsetAngle)), rotateSpeed * Time.deltaTime);
    }
}
