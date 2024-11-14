using System;
using System.Collections;
using System.Collections.Generic;
using Keiwando.BigInteger;
using UnityEngine;
using UnityEngine.Rendering;

public abstract class MonsterBase : MonoBehaviour, IIndex, IDamageable, IHasHpUI, IDie, IAlive
{
    public event Action OnAlive;

    [field: SerializeField] public int index { get; private set; }

    [field: Header("IHasUI")]
    public event Action<BigInteger, BigInteger> OnUpdateMaxHPUI;
    public event Action<BigInteger> OnUpdateCurrenHPUI;
    public Action OnResetHPUI { get; set; }
    public Action<bool> OnActiveHpUI { get; set; }
    public Action<Vector2> OnDeadCallback { get; set; }


    public Action OnDead { get; set; }

    [SerializeField] protected MonsterData monsterData;

    [SerializeField] protected CircleCollider2D circleCollider;
    [SerializeField] protected Transform DamagePivot;
    [SerializeField] protected Transform damageTextPivot;
    [SerializeField] protected SortingGroup[] sortingGroups;


    public event Action<MonsterBase> OnResetData;
    public event Action OnDamaged;

    public bool isDead { get; protected set; } = false;

    [Header("Blink")]
    [SerializeField] protected SpriteRenderer[] spriteRenderers;
    [SerializeField] protected float blinkDuration = 0.25f;
    protected BlinkCalculator<MonsterBase> blinkCalculator;

    protected readonly int DEAD_HASH = AnimatorParameters.IS_DEAD_HASH;

    protected WaitForSeconds deadWaitForSeconds;

    [SerializeField] protected bool isInvincible = false;
    protected HPUIPanel hpUIPanel;
    [SerializeField] protected bool isMove = true;
    protected BigInteger maxHp;

    protected Coroutine preCoroutine;
    [field: SerializeField] public ParticleSystem deadEffect { get; set; }

    [SerializeField] protected GameObject[] activeObjects;

    protected virtual void Awake() 
    {
        
    }

    public virtual void Init()
    {
        // blinkCalculator = new BlinkCalculator<MonsterBase>(this, spriteRenderer, blinkDuration);
        blinkCalculator = new BlinkCalculator<MonsterBase>(this, spriteRenderers, blinkDuration);
        hpUIPanel = GetComponentInChildren<HPUIPanel>();
        hpUIPanel.init(this);
        deadWaitForSeconds = CoroutineUtility.GetWaitForSeconds(deadEffect.main.duration);
    }

    protected virtual void OnEnable()
    {
        foreach (GameObject go in activeObjects)
        {
            go.SetActive(true);
        }

        if (blinkCalculator != null)
        {
            blinkCalculator.ResetBlink();
        }
    }

    protected virtual void OnDisable()
    {
        isDead = false;
        foreach (GameObject go in activeObjects)
        {
            go.SetActive(false);
        }
    }

    public virtual void ToggleInvincible(bool isInvincible)
    {
        this.isInvincible = isInvincible;
    }

    protected virtual void InvokeDefaultEnableEvent()
    {
        OnAlive?.Invoke();
        OnResetData?.Invoke(this);
    }

    #region Pivot

    public Transform GetDamagePivot()
    {
        return DamagePivot;
    }

    public Transform GetDamageTextPivot()
    {
        return damageTextPivot;
    }
    #endregion

    #region IIndex
    public void SetIndex(int index)
    {
        this.index = index;
    }
    #endregion

    public virtual void SetMonsterBaseData(MonsterData monsterData)
    {
        this.monsterData = monsterData;
        maxHp = monsterData.health;
        OnUpdateMaxHPUI?.Invoke(maxHp, maxHp);
    }

    public void SetLayer(int layerNum)
    {
        foreach (SortingGroup sortingGroup in sortingGroups)
        {
            sortingGroup.sortingOrder = layerNum;
        }
    }

    public float GetColliderWidth()
    {
        return circleCollider.radius * 2;
    }

    public float GetRadius()
    {
        return circleCollider.radius;
    }

    public virtual void TakeDamage(BigInteger damage)
    {
        CalculateDamage(damage);
        UpdateStateAfterDamaged();
    }


    #region IDamagable


    protected void UpdateStateAfterDamaged()
    {
        OnUpdateCurrenHPUI?.Invoke(monsterData.health);

        if (monsterData.health != 0)
        {
            OnDamaged?.Invoke();
        }
        else
        {
            Die(true);
        }
    }

    protected void CalculateDamage(BigInteger damage)
    {
        if (isInvincible)
        {
            return;
        }

        if (isDead)
        {
            return;
        }

        BigInteger health = monsterData.health - damage;
        monsterData.health = health < 0 ? 0 : health;
        OnActiveHpUI?.Invoke(true);
    }
    #endregion

    protected virtual void SetDeadSettings()
    {
        isMove = false;
        circleCollider.enabled = false;
    }

    public MonsterType GetMonsterType()
    {
        return monsterData.monsterType;
    }

    public abstract void Die(bool isCausedByBattle);
}
