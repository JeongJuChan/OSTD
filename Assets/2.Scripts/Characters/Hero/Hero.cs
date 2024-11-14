using System;
using System.Collections;
using System.Collections.Generic;
using Keiwando.BigInteger;
using UnityEngine;

public class Hero : MonoBehaviour, IIndex, IDie, IDamageable, IHasHpUI
{
    [Header("TargetDatas")]
    private List<Monster> targets = new List<Monster>();
    public event Func<List<Monster>> OnGetTargetFunc;

    [Header("Animation")]
    private Animator animator;

    [Header("ShootingDatas")]
    private AttackAnimationEventHandler attackEventHandler;

    [SerializeField] private SpriteRenderer[] equipmentRenderers;

    [SerializeField] private Shotgun shotgun;

    private readonly int attackHash = AnimatorParameters.HERO_ATTACK;

    [field: SerializeField] public int index { get; private set; }

    private MonsterBase targetMonster;

    private readonly int ATTACK_SPEED_RATE_HASH = AnimatorParameters.ATTACK_SPEED_RATE_HASH;

    public event Action<Vector2> OnHeroPosUpdated;
    public Action OnDead{ get; set; }
    public event Action OnFailed;
    public event Action OnDamaged;
    public event Action<BigInteger, BigInteger> OnUpdateMaxHPUI;
    public event Action<BigInteger> OnUpdateCurrenHPUI;
    public Action OnResetHPUI{ get; set; }
    public Action<bool> OnActiveHpUI{ get; set; }
    public event Action OnReset;

    [Header("Stat")]
    [SerializeField] private HPUIPanel hpUIPanel;
    private HeroStatData heroStatData;
    private BigInteger maxHealth;

    [field: SerializeField] public Transform granadePivot { get; private set; }
    [field: SerializeField] public ParticleSystem deadEffect { get; set; }

    [SerializeField] private Rigidbody2D rigid;

    private Coroutine preCoroutine;
    private WaitForSeconds deadWaitForSeconds;

    public bool isDead { get; private set; }

    [field: SerializeField] public Collider2D collider2D { get; private set; }

    private BoxMoveController boxMoveController;

    private BlinkCalculator<Hero> blinkCalculator;

#if UNITY_EDITOR
    [SerializeField] private bool isInvincible;
    #endif

    #region UnityMethods

    private void OnDestroy()
    {
        attackEventHandler.RemoveAction(Shoot);
    }
    #endregion

    #region Initialize
    public void Init()
    {
        attackEventHandler = GetComponentInChildren<AttackAnimationEventHandler>();
        animator = GetComponentInChildren<Animator>();
        attackEventHandler.AddAttackAction(Shoot);
        shotgun.InitPoolSize();
        hpUIPanel.init(this);
        OnActiveHpUI?.Invoke(false);
        GameManager.instance.OnReset += () => Die(false);
        GameManager.instance.OnReset += () => UpdateKinematicState(true);
        GameManager.instance.OnReset += Reset;
        deadWaitForSeconds = CoroutineUtility.GetWaitForSeconds(deadEffect.main.duration);
        boxMoveController = BoxManager.instance.boxMoveController;
        blinkCalculator = new BlinkCalculator<Hero>(this, equipmentRenderers, 0.25f);
        shotgun.OnPlayShotAnim += PlayShotAnim;
        shotgun.OnPlayGunloadAnim += PlayGunloadAnim;
    }
    #endregion

    #region Shoot
    public void TryPlayShootAnimation()
    {
        if (boxMoveController.isMonsterBasementEncountered)
        {
            return;
        }

        bool isTargetExist = OnGetTargetFunc().Count != 0;
        SetTargetMonster();
        Shoot();
        animator.SetBool(attackHash, isTargetExist);
    }

    private void Shoot()
    {
        shotgun.SetTarget(targetMonster);
    }
    #endregion

    #region Target
    private void SetTargetMonster()
    {
        targets = OnGetTargetFunc.Invoke();

        if (targets.Count == 0)
        {
            targetMonster = null;
            return;
        }

        int index = 0;
        // int index = UnityEngine.Random.Range(0, targets.Count);

        if (targets.Count > index)
        {
            if (targets[index].isDead)
            {
                SetTargetMonster();
            }
            else
            {
                targetMonster = targets[index];
            }
        }
        else
        {
            SetTargetMonster();
        }
    }

    public void ResetTarget(MonsterBase monster)
    {
        if (targetMonster == monster)
        {
            SetTargetMonster();
            shotgun.SetTarget(targetMonster);
        }
    }

    private void ResetTarget()
    {
        targetMonster = null;
        shotgun.SetTarget(null);
    }

    public void SetTarget(MonsterBase monster)
    {
        targetMonster = monster;
        shotgun.SetTarget(monster);
    }

    #endregion

    #region Index
    public void SetIndex(int index)
    {
        this.index = index;
    }

    public int GetIndex()
    {
        return index;
    }
    #endregion

    #region Update Pos
    public void UpdatePos(Vector2 position)
    {
        transform.position = position;
        OnHeroPosUpdated?.Invoke(position);
    }

    public void UpdatePosX(float posX)
    {
        Vector2 pos = transform.position;
        pos.x = posX;
        transform.position = pos;
    }

    #endregion


    public void Die(bool isCausedByBattle)
    {
        isDead = true;

        if (isCausedByBattle)
        {
            if (preCoroutine != null)
            {
                StopCoroutine(preCoroutine);
            }

            preCoroutine = StartCoroutine(CoOnDie());
        }
        else
        {
            OnActiveHpUI?.Invoke(false);
            OnDead?.Invoke();
        }

    }

    #region OnDieMethods
    protected IEnumerator CoOnDie()
    {
        OnActiveHpUI?.Invoke(false);
        deadEffect.Play();

        yield return deadWaitForSeconds;

        OnDead?.Invoke();
        OnFailed?.Invoke();
    }
    #endregion

    public void TakeDamage(BigInteger damage)
    {
        if (isDead)
        {
            return;
        }

        #if UNITY_EDITOR
        if (isInvincible)
        {
            return;
        }
        #endif

        heroStatData.health -= damage;
        heroStatData.health = heroStatData.health < 0 ? 0 : heroStatData.health;
        if (heroStatData.health <= 0)
        {
            Die(true);
        }

        OnDamaged?.Invoke();
        OnActiveHpUI?.Invoke(true);
        hpUIPanel.UpdateCurrentHPUI(heroStatData.health);
    }

    public void UpdateHeroStatData(HeroStatData heroStatData)
    {
        this.heroStatData = heroStatData;
        shotgun.UpdateDamage(heroStatData.damage);
        maxHealth = heroStatData.health;
        hpUIPanel.UpdateMaxHP(maxHealth, heroStatData.health);
    }

    public void Reset()
    {
        gameObject.SetActive(true);
        heroStatData.health = maxHealth;
        hpUIPanel.UpdateCurrentHPUI(maxHealth);
        isDead = false;
        ResetTarget();
        OnReset?.Invoke();
    }

    public void UpdateKinematicState(bool isActive)
    {
        rigid.isKinematic = isActive;
    }

    public void UpdateEquipmentSprite(int index, Sprite sprite)
    {
        if (index == Consts.BACKPACK_INDEX)
        {
            return;
        }

        if (index == Consts.GRANADE_INDEX)
        {
            HeroManager.instance.SetGranadeSprite(sprite);
            return;
        }
        
        equipmentRenderers[index].sprite = sprite;

        blinkCalculator.UpdateTextures();
    }

    private void PlayShotAnim()
    {
        animator.SetTrigger(AnimatorParameters.HERO_ATTACK);
    }

    private void PlayGunloadAnim()
    {
        animator.SetTrigger(AnimatorParameters.GUN_LOAD);
    }
}

