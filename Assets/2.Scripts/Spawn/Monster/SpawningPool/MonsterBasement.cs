using Keiwando.BigInteger;
using System;
using System.Collections;
using UnityEngine;

public class MonsterBasement : MonsterBase
{
    [SerializeField] private GameObject shadowObject;
    private float colliderWidth;
    private Vector2 offsetPos;

    private WaitForSeconds appearWaitForSeconds;

    private BigInteger nextGoldEarningHp = 0;
    private BigInteger earningGoldUnitByHp = 0;
    private BigInteger hpUnit = 0;

    public event Action<BigInteger> OnUpdateInGameCurrency;

    public event Action<MonsterBasement> OnForceReturnAction;

    public event Action<MonsterBase> OnReturnAction;

    protected override void Awake()
    {
        base.Awake();
        isMove = false;
        circleCollider.enabled = false;
        shadowObject.SetActive(false);
        colliderWidth = GetColliderWidth();
        offsetPos = new Vector2(transform.position.x - colliderWidth * Consts.HALF, transform.position.y);
        // appearWaitForSeconds = CoroutineUtility.GetWaitForSeconds(animator.GetCurrentAnimatorClipInfo(0).Length);
    }

    public override void Init()
    {
        base.Init();
        deadWaitForSeconds = CoroutineUtility.GetWaitForSeconds(0.5f);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        isDead = false;
        InvokeDefaultEnableEvent();
        transform.position = offsetPos;

        StartCoroutine(CoWaitForAppearing());
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        ToggleInvincible(true);
        shadowObject.SetActive(false);
        if (BoxManager.instance.boxMoveController != null)
        {
            BoxManager.instance.boxMoveController.UpdateMonsterBasementEncounterState(false);
        }
        monsterData.health = maxHp;
    }

    private void Reset()
    {
        monsterData.health = maxHp;
        OnResetHPUI?.Invoke();
    }

    public override void SetMonsterBaseData(MonsterData monsterData)
    {
        base.SetMonsterBaseData(monsterData);
        nextGoldEarningHp = maxHp - earningGoldUnitByHp;
    }

    private IEnumerator CoWaitForAppearing()
    {
        yield return appearWaitForSeconds;
        circleCollider.enabled = true;
        shadowObject.SetActive(true);
        isInvincible = true;
    }

    public void SetGoldEarningUnitByHp(BigInteger earningGoldUnitByHp, BigInteger hpUnit)
    {
        this.earningGoldUnitByHp = earningGoldUnitByHp;
        this.hpUnit = hpUnit;
    }

    public override void TakeDamage(BigInteger damage)
    {
        CalculateDamage(damage);

        if (monsterData.health < nextGoldEarningHp)
        {
            CalculateNextGoldEarningHp();
        }

        UpdateStateAfterDamaged();
    }
    
    #region IDie
    public override void Die(bool isCausedByBattle)
    {
        isDead = true;
        circleCollider.enabled = false;

        if (isCausedByBattle)
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }

            preCoroutine = StartCoroutine(CoOnDie());
        }
        else
        {
            SetDeadSettings();
            OnForceReturnAction?.Invoke(this);
            Reset();
        }
    }
    #endregion

    public void UpdateSprite(Sprite sprite)
    {
        spriteRenderers[0].sprite = sprite;
        blinkCalculator.UpdateTextures();
    }

    private void CalculateNextGoldEarningHp()
    {
        BigInteger diff = nextGoldEarningHp - monsterData.health;
        if (diff > earningGoldUnitByHp)
        {
            BigInteger div = diff / hpUnit;
            BigInteger rest = diff % hpUnit;

            // gold 얻기
            nextGoldEarningHp = nextGoldEarningHp - hpUnit * (div + 1);
            OnUpdateInGameCurrency?.Invoke((div + 1) * earningGoldUnitByHp);
        }
    }

    protected virtual IEnumerator CoOnDie()
    {
        OnActiveHpUI?.Invoke(false);
        SetDeadSettings();
        OnDeadCallback?.Invoke(transform.position);
        OnDead?.Invoke();

        foreach (GameObject gameObject in activeObjects)
        {
            gameObject.SetActive(false);
        }

        deadEffect.Play();

        yield return deadWaitForSeconds;

        OnReturnAction?.Invoke(this);
        OnResetHPUI?.Invoke();
    }
}
