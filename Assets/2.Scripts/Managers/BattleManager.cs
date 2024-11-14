using Keiwando.BigInteger;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviourSingleton<BattleManager>
{
    [field: SerializeField] public TargetInRangeTrigger targetInRangeTrigger { get; private set; }
    [SerializeField] private DamageImage damageImagePrefab;
    public TargetHandler targetHandler { get; private set; }

    private const int DIVIDE_VALUE = 2;

    public event Action<BigInteger, DamageType, int, Vector2> OnSpawnDamageUI;

    private float vibrateDuration = 0.1f;
    private bool isVibratingProgress;

    private bool isCastleDamageable = true;
    private bool isMonsterDamageable = true;

    private DamageImageSpawner damageImageSpawner;

    [SerializeField] private int criticalProbability = 5;
    [SerializeField] private int criticalMultiplication = 150;

    private Transform damageImageParent;

    private DamageTimer damageTimer;

    #region Initialize
    public void Init()
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0));
        float targetInRangePosx = worldPos.x - 1 + targetInRangeTrigger.GetBoxSizeX();
        targetInRangeTrigger.SetOffsetPosX(targetInRangePosx);

        targetHandler = new TargetHandler();
        targetInRangeTrigger.OnTargetAdded += targetHandler.AddActiveTarget;
        HeroManager.instance.hero.OnGetTargetFunc += targetHandler.GetActiveTargets;
        targetHandler.OnActiveTargetStateChanged += HeroManager.instance.hero.TryPlayShootAnimation;
        targetHandler.OnActiveTargetStateChanged += WeaponManager.instance.TryFindTarget;
        GameManager.instance.OnStart += () => targetInRangeTrigger.UpdateGameState(true);

        damageImageSpawner = new DamageImageSpawner();
        damageImageParent = UIManager.instance.GetUIElement<UI_PopupCanvas>().transform;
        damageImageSpawner.SetPrefab(damageImagePrefab, damageImageParent);
        damageTimer = new DamageTimer();
        damageTimer.Init();
    }
    #endregion

    public void OnMonsterAttacked(IDamageable monster, Vector2 pos, bool isVibrated, BigInteger damage)
    {
        if (!isMonsterDamageable)
        {
            return;
        }

        int percent = UnityEngine.Random.Range(0, Consts.PERCENT_TOTAL_VALUE);
        bool isCritical = percent < criticalProbability * Consts.PERCENT_DIVIDE_VALUE;

        DamageType damageType = isCritical ? DamageType.Critical : DamageType.Normal;
        BigInteger totalDamage = isCritical ? damage * criticalMultiplication / Consts.PERCENT_DIVIDE_VALUE : damage;

        if (!isVibratingProgress && isVibrated)
        {
            isVibratingProgress = true;
            StartCoroutine(CoShakeCamera());
        }

        if (monster != null)
        {
            monster.TakeDamage(totalDamage);
            if (damageTimer.GetTextPossible(damage))
            {
                OnSpawnDamageUI?.Invoke(totalDamage, damageType, 1, pos);
            }
        }
    }

    public void OnMonsterAttacked(IDamageable monster, ITickAttack tickAttack, Vector2 pos, bool isVibrated, BigInteger damage)
    {
        if (!isMonsterDamageable)
        {
            return;
        }

        if (!isVibratingProgress && isVibrated)
        {
            isVibratingProgress = true;
            StartCoroutine(CoShakeCamera());
        }

        if (monster != null)
        {
            // TODO : Refactoring
            if (damageTimer.GetTickDamagePossible(tickAttack, monster))
            {
                monster.TakeDamage(damage);
            }

            if (damageTimer.GetTextPossible(damage))
            {
                OnSpawnDamageUI?.Invoke(damage, DamageType.Normal, 1, pos);
            }
        }
    }

    public void TryRemoveMonsterInRangeTickWeaponTarget(IDamageable damagable)
    {
        damageTimer.TryRemoveMonsterInRangeTickWeaponTarget(damagable);
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

    public void OnHeroAttacked(IDamageable hero, Vector2 pos, bool isVibrated, BigInteger damage)
    {
        if (!isCastleDamageable)
        {
            return;
        }

        if (!isVibratingProgress && isVibrated)
        {
            isVibratingProgress = true;
            StartCoroutine(CoShakeCamera());
        }

        if (hero != null)
        {
            hero.TakeDamage(damage);
            // if (damageTimer.GetTextPossible(damage))
            // {
            //     OnSpawnDamageUI?.Invoke(damage, DamageType.Normal, 1, pos);
            // }
        }
    }

    private BigInteger CalculateDamage(BigInteger damage, BigInteger defense)
    {
        return damage - (defense / DIVIDE_VALUE);
    }

    private void ChangeMonsterDamageable(bool isDamageable)
    {
        isMonsterDamageable = isDamageable;
    }

    private void ChangeCastleDamageable(bool isDamageable)
    {
        isCastleDamageable = isDamageable;
    }

}
