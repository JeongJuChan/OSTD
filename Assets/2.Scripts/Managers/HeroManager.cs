using System;
using System.Collections;
using System.Collections.Generic;
using Keiwando.BigInteger;
using UnityEngine;

public class HeroManager : MonoBehaviourSingleton<HeroManager>
{
    public Hero hero { get; private set; }

    [Header("Granade")]
    [SerializeField] private int granadeInitPoolSize = 10;
    [SerializeField] private int granadePoolSizeMax = 100;
    [SerializeField] private float throwPower = 6f;
    [SerializeField] private float disalbeDelayTime = 2f;
    [SerializeField] private float explosionPower = 2f;
    [SerializeField] private float explosionRadius = 2f;
    [SerializeField] private float vibrateDuration = 0.5f;

    private BigInteger skillDamage = 0;

    private bool isVibratingProgress;

    private Sprite granadeSprite;

    public void Init()
    {
        hero = Instantiate(Resources.Load<Hero>("Hero/Hero"), BoxManager.instance.frameTransform.position, Quaternion.identity, transform);
        hero.Init();
        hero.OnDead += () => ChangeHeroActiveState(false);
        InitPoolSize();
    }

    private void InitPoolSize()
    {
        PoolManager.instance.projectile.AddPoolInfo((int)ProjectileType.Granade, granadeInitPoolSize, granadePoolSizeMax);
    }

    public void PoolGranade()
    {
        Granade granade = PoolManager.instance.projectile.Pool((int)ProjectileType.Granade, hero.granadePivot.position, Quaternion.identity) as Granade;
        granade.SetShotPower(throwPower);
        granade.UpdateDamage(skillDamage);
        granade.SetExplosionPower(explosionPower);
        granade.SetRadius(explosionRadius);
        granade.SetDelayTime(disalbeDelayTime);
        granade.Fire();
        granade.SetSprite(granadeSprite);
        granade.OnShakeCamera += TryShakeCamera;
    }

    private void TryShakeCamera()
    {
        if (!isVibratingProgress)
        {
            isVibratingProgress = true;
            StartCoroutine(CoShakeCamera());
        }
    }

    private IEnumerator CoShakeCamera()
    {
        float elapsedTime = Time.deltaTime;

        UIAnimations.instance.ShakeCamera();

        while (elapsedTime < vibrateDuration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        isVibratingProgress = false;
    }

    public void ChangeHeroActiveState(bool isActive)
    {
        hero.gameObject.SetActive(isActive);
    }

    public void SetSkillDamage(BigInteger amount)
    {
        skillDamage = amount;
    }

    public void SetGranadeSprite(Sprite sprite)
    {
        granadeSprite = sprite;
    }
}
