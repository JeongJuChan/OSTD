using Keiwando.BigInteger;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;

public abstract class Monster : MonsterBase, IPoolable<Monster>
{
    public event Action<Monster> OnRemoveTargetAction;

    public event Action<Monster> OnTryResetTarget;

    public event Action<Monster> OnReturnAction;

    [SerializeField] protected float totalSpeed;
    protected float offsetSpeed;

    protected AttackAnimationEventHandler attackEventHandler;

    protected readonly int ATTACK_HASH = AnimatorParameters.IS_ATTACK_HASH;
    protected readonly int ATTACK_SPEED_RATE_HASH = AnimatorParameters.ATTACK_SPEED_RATE_HASH;

    [Header("Physics")]
    [field: SerializeField] public bool isGrounded;

    private float groundPosY;

    [SerializeField] protected Animator animator;


    [SerializeField] public bool isForward { get; private set; } = true;

    private float backMovingDuration;

    private Vector2 backRayOrigin;

    private Vector2 groundCheckRayOriginVec;

    [SerializeField] private SpriteLibrary spriteLibrary;
    private SpriteLibraryAsset spriteLibraryAsset;

    [SerializeField] private SpriteResolver[] spriteResolvers;

    private List<string> categories;
    private Dictionary<string, List<string>> labelDict = new Dictionary<string, List<string>>();

    private int labelCount;

    protected Collider2D otherCollider;

    [field: SerializeField] public MonsterMovementStateType movementStateType { get; protected set; } = MonsterMovementStateType.Idle;

    public MonsterStateModule monsterStateModule { get; protected set; }

    protected Transform boxManagerTrans;

    protected float boxHalfSizeX;
    private float backwardTargetPosX;

    private float gravityVelocityY;
    private float jumpTargetPosY;
    private float fallTargetPosY;
    private bool isGravityEnabled;

    protected float rayMinimumLength = 0.2f;
    private readonly RaycastHit2D[] TEMP_RAY_CAST_HIT_2DS = new RaycastHit2D[20];

    protected bool isHeroDead;

    protected float movingRate = 1f;
    protected float movingFastRate = Mathf.Sqrt(1);
    protected float movingNormalRate = 1f;

    private float attackCheckingPosYMod = 1.75f;

    #region UnityMethods
    protected override void Awake()
    {
        base.Awake();
        groundCheckRayOriginVec = transform.position;
        // TODO : 하드 코딩 바꿀 것
        groundCheckRayOriginVec.y -= 0.1f;
        if (attackEventHandler == null)
        {
            attackEventHandler = GetComponentInChildren<AttackAnimationEventHandler>();
        }

        blinkCalculator = new BlinkCalculator<MonsterBase>(this, spriteRenderers, blinkDuration);

        categories = new List<string>(spriteLibrary.spriteLibraryAsset.GetCategoryNames());
        spriteLibraryAsset = spriteLibrary.spriteLibraryAsset;

        foreach (string categoryName in categories)
        {
            if (!labelDict.ContainsKey(categoryName))
            {
                labelDict.Add(categoryName, new List<string>());
            }

            labelDict[categoryName].AddRange(spriteLibraryAsset.GetCategoryLabelNames(categoryName));
        }

        string category = categories[0];
        List<string> labels = labelDict[category];
        labelCount = labels.Count;
        boxManagerTrans = BoxManager.instance.transform;

        monsterStateModule = new MonsterStateModule();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        circleCollider.enabled = true;
        isGrounded = true;
        InvokeDefaultEnableEvent();
        monsterStateModule.ResetState();

        if (attackEventHandler != null)
        {
            attackEventHandler.AddAttackAction(OnAttack);
        }

        if (categories.Count > 0)
        {
            int labelIndex = UnityEngine.Random.Range(0, labelCount);
            for (int i = 0; i < categories.Count; i++)
            {
                string category = categories[i];
                string label = labelDict[category][labelIndex];
                spriteResolvers[i].SetCategoryAndLabel(category, label);
            }

            blinkCalculator.UpdateTextures();
        }

        boxHalfSizeX = BoxManager.instance.GetBoxSizeX() * Consts.HALF;
    }

    protected virtual void FixedUpdate()
    {
        monsterStateModule.FixedUpdate();
    }

    protected virtual void Update()
    {

        UpdateGravityIfEnabled();
        Move();

        monsterStateModule.Update();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (HeroManager.instance.hero != null)
        {
            HeroManager.instance.hero.ResetTarget(this);
        }
        WeaponManager.instance.ResetTarget(this);
        attackEventHandler.RemoveAction(OnAttack);
        ToggleInvincible(true);
    }

    protected void OnDestroy()
    {
        OnAlive -= SetDeadFalse;
    }
    #endregion

    #region InitMethods


    public void UpdateSpeed(float monsterSpawnPosX, float obstaclePosX, float arriveDuration)
    {
        offsetSpeed = (monsterSpawnPosX - obstaclePosX) / arriveDuration;
        totalSpeed = offsetSpeed * monsterData.speed;
    }

    public override void Init()
    {
        base.Init();
        OnAlive += SetDeadFalse;
        backRayOrigin = new Vector2(GetColliderWidth(), circleCollider.radius);
    }

    #endregion

    #region IPoolable
    public void Initialize(Action<Monster> returnAction)
    {
        OnReturnAction += returnAction;
    }

    public void ReturnToPool()
    {
        OnResetHPUI?.Invoke();
        OnReturnAction?.Invoke(this);
    }
    #endregion

    #region MoveMethods
    protected virtual void Move()
    {
        float targetPosX = transform.position.x;
        float targetPosY = transform.position.y;

        if (movementStateType != MonsterMovementStateType.Idle)
        {
            float moveSpeed = totalSpeed * movingRate;
            targetPosX += isForward ? -moveSpeed * Time.deltaTime : moveSpeed * Time.deltaTime;
        }
        
        if (!isHeroDead)
        {
            targetPosX = Mathf.Max(boxManagerTrans.position.x + boxHalfSizeX + circleCollider.radius, targetPosX);
        }

        if (movementStateType == MonsterMovementStateType.Backward)
        {
            if (backwardTargetPosX <= targetPosX)
            {
                targetPosX = backwardTargetPosX;
                movementStateType = MonsterMovementStateType.Idle;
            }
        }

        if (movementStateType == MonsterMovementStateType.Jump || movementStateType == MonsterMovementStateType.Fall)
        {
            targetPosY += gravityVelocityY * Time.deltaTime;
            if (movementStateType == MonsterMovementStateType.Jump && gravityVelocityY <= 0f && targetPosY <= jumpTargetPosY)
            {
                targetPosY = jumpTargetPosY;
                if (monsterStateModule.GetCurrentStateType() == MonsterStateType.Forward)
                {
                    movementStateType = MonsterMovementStateType.Forward;
                }
                else
                {
                    movementStateType = MonsterMovementStateType.Idle;
                }
            }
            else if (movementStateType == MonsterMovementStateType.Fall && targetPosY <= fallTargetPosY)
            {
                targetPosY = fallTargetPosY;
                if (monsterStateModule.GetCurrentStateType() == MonsterStateType.Forward)
                {
                    movementStateType = MonsterMovementStateType.Forward;
                }
                else
                {
                    movementStateType = MonsterMovementStateType.Idle;
                }
            }
        }

        isGrounded = targetPosY <= groundPosY;

        if (isGrounded)
        {
            targetPosY = groundPosY;
            StopGravity();
            if (movementStateType == MonsterMovementStateType.Fall)
            {
                movementStateType = MonsterMovementStateType.Idle;
            }
        }

        transform.position = new Vector2(targetPosX, targetPosY);
    }

    public void StartMoveForward()
    {
        movementStateType = MonsterMovementStateType.Forward;
        movingRate = movingNormalRate;
        isForward = true;
    }

    public void StartMoveBackward(float targetPosX)
    {
        movementStateType = MonsterMovementStateType.Backward;
        movingRate = movingNormalRate;
        backwardTargetPosX = targetPosX;
        isForward = false;
    }

    public void JumpTo(float jumpTargetPosY)
    {
        movementStateType = MonsterMovementStateType.Jump;
        movingRate = movingFastRate;
        this.jumpTargetPosY = jumpTargetPosY;
        float height = jumpTargetPosY - transform.position.y;
        StartGravity(height);
    }

    public void Fall(float fallTargetPosY)
    {
        movingRate = movingFastRate;
        movementStateType = MonsterMovementStateType.Fall;
        this.fallTargetPosY = fallTargetPosY;
        StartGravity(0f);
    }

    public void Stop()
    {
        movementStateType = MonsterMovementStateType.Idle;
    }

    #endregion

    #region AttackMethods


    public virtual void PlayAttackAnimation(bool isPlaying)
    {
        animator.SetBool(ATTACK_HASH, isPlaying);
        if (isPlaying && monsterStateModule.GetCurrentStateType() != MonsterStateType.Attack)
        {
            monsterStateModule.ChangeState(MonsterStateType.Attack);
        }
    }

    #endregion

    #region IDie
    public override void Die(bool isCausedByBattle)
    {
        if (isDead)
        {
            return;
        }

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
            ReturnToPool();
        }
    }
    #endregion

    #region OnDieMethods

    #endregion

    protected override void SetDeadSettings()
    {
        base.SetDeadSettings();
        isForward = true;
        OnRemoveTargetAction?.Invoke(this);
        OnTryResetTarget?.Invoke(this);
    }

    public float GetSpeed()
    {
        return totalSpeed;
    }

    private void SetDeadFalse()
    {
        isDead = false;
    }

    #region Physics

    protected override void InvokeDefaultEnableEvent()
    {
        base.InvokeDefaultEnableEvent();
        OnActiveHpUI?.Invoke(false);
    }

    #endregion

    public Monster FindBelowMonster(bool onlyGrounded = false)
    {
        Vector2 belowDirection = Vector2.down;
        Vector2 origin = transform.position;
        origin.y -= circleCollider.radius + rayMinimumLength;
        float rayDistance = circleCollider.radius;
        // int resultCount = Physics2D.CircleCastNonAlloc(origin, circleCollider.radius, belowDirection, TEMP_RAY_CAST_HIT_2DS, rayDistance, 1 << gameObject.layer);
        int resultCount = Physics2D.CircleCastNonAlloc(origin, circleCollider.radius, belowDirection, TEMP_RAY_CAST_HIT_2DS, rayDistance, 1 << gameObject.layer);
        if (0 < resultCount)
        {
            for (int index = 0; index < resultCount; index++)
            {
                if (TEMP_RAY_CAST_HIT_2DS[index].collider.TryGetComponent(out Monster hitMonster))
                {
                    if (hitMonster != this && !onlyGrounded)
                    {
                        continue;
                    }

                    return hitMonster;
                }
            }
        }

        return null;
    }

    public Monster FindBehindAndAboveMonster()
    {
        Vector2 behindDirection = Vector2.right;
        Vector2 origin = transform.position;
        origin.x += circleCollider.radius;
        origin.y += circleCollider.radius;
        float rayDistance = circleCollider.radius * 2;
        int resultCount = Physics2D.RaycastNonAlloc(origin, behindDirection, TEMP_RAY_CAST_HIT_2DS, rayDistance, 1 << gameObject.layer);
        if (0 < resultCount)
        {
            for (int index = 0; index < resultCount; index++)
            {
                if (TEMP_RAY_CAST_HIT_2DS[index].collider.TryGetComponent(out Monster hitMonster))
                {
                    if (hitMonster != this && base.transform.position.x < hitMonster.transform.position.x)
                    {
                        return hitMonster;
                    }
                }
            }
        }

        origin = transform.position;
        rayDistance = circleCollider.radius + rayMinimumLength;
        origin.y += circleCollider.radius;
        Vector2 direction = Vector2.up;
        resultCount = Physics2D.RaycastNonAlloc(origin, direction, TEMP_RAY_CAST_HIT_2DS, rayDistance, 1 << gameObject.layer);
        if (0 < resultCount)
        {
            for (int index = 0; index < resultCount; index++)
            {
                if (TEMP_RAY_CAST_HIT_2DS[index].collider.TryGetComponent(out Monster hitMonster))
                {
                    if (hitMonster != this && base.transform.position.y < hitMonster.transform.position.y)
                    {
                        return hitMonster;
                    }
                }
            }
        }

        return null;
    }

    public Monster FindBehindMonster()
    {
        Vector2 behindDirection = Vector2.right;
        Vector2 origin = transform.position;
        origin.x += rayMinimumLength;
        origin.y += circleCollider.radius;
        float rayDistance = circleCollider.radius * 2;
        int resultCount = Physics2D.RaycastNonAlloc(origin, behindDirection, TEMP_RAY_CAST_HIT_2DS, rayDistance, 1 << gameObject.layer);
        if (0 < resultCount)
        {
            for (int index = 0; index < resultCount; index++)
            {
                if (TEMP_RAY_CAST_HIT_2DS[index].collider.TryGetComponent(out Monster hitMonster))
                {
                    if (hitMonster != this && base.transform.position.x < hitMonster.transform.position.x)
                    {
                        return hitMonster;
                    }
                }
            }
        }

        return null;
    }

    public Monster FindForwardMonsterWithoutJumpOrFall()
    {
        Vector2 forwardDirection = Vector2.left;
        Vector2 origin = transform.position;
        origin.x -= circleCollider.radius + rayMinimumLength;
        origin.y += circleCollider.radius;
        float rayDistance = rayMinimumLength;
        int resultCount = Physics2D.RaycastNonAlloc(origin, forwardDirection, TEMP_RAY_CAST_HIT_2DS, rayDistance, 1 << gameObject.layer);
        Debug.DrawRay(origin, rayDistance * forwardDirection, Color.green);
        if (0 < resultCount)
        {
            for (int index = 0; index < resultCount; index++)
            {
                if (TEMP_RAY_CAST_HIT_2DS[index].collider.TryGetComponent(out Monster hitMonster))
                {
                    if (hitMonster != this && transform.position.x > hitMonster.transform.position.x &&
                        hitMonster.movementStateType != MonsterMovementStateType.Jump)
                    {
                        return hitMonster;
                    }
                }
            }
        }

        return null;
    }

    public Monster FindAboveMonster()
    {
        Vector2 origin = transform.position;
        float rayDistance = circleCollider.radius + rayMinimumLength;
        origin.y += rayDistance;
        Vector2 direction = Vector2.up;
        int resultCount = Physics2D.RaycastNonAlloc(origin, direction, TEMP_RAY_CAST_HIT_2DS, rayDistance, 1 << gameObject.layer);
        if (0 < resultCount)
        {
            for (int index = 0; index < resultCount; index++)
            {
                if (TEMP_RAY_CAST_HIT_2DS[index].collider.TryGetComponent(out Monster hitMonster))
                {
                    if (hitMonster != this && base.transform.position.y < hitMonster.transform.position.y)
                    {
                        return hitMonster;
                    }
                }
            }
        }

        return null;
    }

    public Monster FindForwardMonster()
    {
        Vector2 origin = transform.position;
        float rayDistance = circleCollider.radius + rayMinimumLength;
        origin.x -= circleCollider.radius;
        origin.y += circleCollider.radius;
        Vector2 direction = Vector2.left;
        int resultCount = Physics2D.RaycastNonAlloc(origin, direction, TEMP_RAY_CAST_HIT_2DS, rayDistance, 1 << gameObject.layer);
        if (0 < resultCount)
        {
            for (int index = 0; index < resultCount; index++)
            {
                if (TEMP_RAY_CAST_HIT_2DS[index].collider.TryGetComponent(out Monster hitMonster))
                {
                    if (hitMonster != this && base.transform.position.y < hitMonster.transform.position.y)
                    {
                        return hitMonster;
                    }
                }
            }
        }

        return null;
    }

    protected virtual IEnumerator CoOnDie()
    {
        PlayDyingAnimation(true);

        OnActiveHpUI?.Invoke(false);
        SetDeadSettings();
        OnDeadCallback?.Invoke(transform.position);
        OnDead?.Invoke();
        RewardMovingController.instance.MoveCurrencyByCoroutine(1, CurrencyType.Gold, transform.position);

        foreach (GameObject gameObject in activeObjects)
        {
            gameObject.SetActive(false);
        }

        deadEffect.Play();

        yield return deadWaitForSeconds;

        ReturnToPool();
    }

    protected virtual void OnAttack()
    {
        if (otherCollider == null)
        {
            return;
        }

        if (otherCollider.gameObject.CompareTag(Consts.HERO_TAG))
        {
            if (otherCollider.TryGetComponent(out Hero hero))
            {
                BattleManager.instance.OnHeroAttacked(hero, hero.transform.position, false, monsterData.damage);
            }
            else if (otherCollider.TryGetComponent(out DragableBox dragableBox))
            {
                IDamageable boxDamageable = dragableBox.damagable;
                BattleManager.instance.OnHeroAttacked(boxDamageable, dragableBox.transform.position, false, monsterData.damage);
            }
        }

    }

    protected void StartGravity(float height)
    {
        isGravityEnabled = true;
        gravityVelocityY = height > 0f ? Mathf.Sqrt(-2f * Consts.GRAVITY * height) : 0;
    }

    protected void StopGravity()
    {
        gravityVelocityY = 0f;
        isGravityEnabled = false;
    }

    protected void UpdateGravityIfEnabled()
    {
        if (isGravityEnabled)
        {
            gravityVelocityY += Consts.GRAVITY * Time.deltaTime;
        }
    }

    public void UpdateGroundPosY(float groundPosY)
    {
        this.groundPosY = groundPosY;
    }

    public bool CanAttack()
    {
        Vector2 rayPos = transform.position;
        rayPos.x -= circleCollider.radius + rayMinimumLength;
        rayPos.y += circleCollider.radius * attackCheckingPosYMod;
        RaycastHit2D hit = Physics2D.Raycast(rayPos, Vector2.left, rayMinimumLength, Consts.HERO);
        Debug.DrawRay(rayPos, Vector2.left * rayMinimumLength);
        // Vector2 attackDirection = Vector2.left;
        // Vector2 origin = transform.position;
        // origin.y += circleCollider.radius * attackCheckingPosYMod;
        // float rayDistance = circleCollider.radius + rayMinimumLength;
        // RaycastHit2D raycastHit2D = Physics2D.Raycast(origin, attackDirection, rayDistance, Consts.HERO);
        UpdateAttackState(hit.collider);
        return hit.collider != null;
    }

    public void UpdateAttackState(Collider2D collider2D)
    {
        otherCollider = collider2D;
        bool isAttackState = collider2D != null;
        PlayAttackAnimation(isAttackState);
    }


    protected virtual void PlayDyingAnimation(bool isPlaying)
    {
        animator.SetBool(DEAD_HASH, isPlaying);
    }

    // protected void CheckAttackRay()
    // {
    //     if (monsterStateModule.GetCurrentStateType() == MonsterStateType.Backward)
    //     {
    //         return;
    //     }

    //     Vector2 rayPos = transform.position;
    //     rayPos.x -= circleCollider.radius + rayMinimumLength;
    //     rayPos.y += circleCollider.radius * attackCheckingPosYMod;
    //     RaycastHit2D hit = Physics2D.Raycast(rayPos, Vector2.left, rayMinimumLength, Consts.HERO);

    //     UpdateAttackState(hit.collider);
    // }

    public bool GetBackwardable()
    {
        return groundPosY + circleCollider.radius > transform.position.y;
    }

    public void UpdateIsHeroDead(bool isHeroDead)
    {
        this.isHeroDead = isHeroDead;
        if (isHeroDead)
        {
            monsterStateModule.ChangeState(MonsterStateType.Forward);
        }
    }

    protected void TryAttack()
    {
        Vector2 attackDirection = Vector2.left;
        Vector2 origin = transform.position;
        origin.y += circleCollider.radius;
        float rayDistance = circleCollider.radius + rayMinimumLength;
        RaycastHit2D raycastHit2D = Physics2D.Raycast(origin, attackDirection, rayDistance, 1 << LayerMask.NameToLayer(Consts.HERO_TAG));
        UpdateAttackState(raycastHit2D.collider);
    }
}
