using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TargetingWeapon : Weapon, ITargeting
{
    [Header("TargetDatas")]
    protected List<Monster> targets = new List<Monster>();
    public Func<List<Monster>> OnGetTargetFunc { get; protected set; }
    protected MonsterBase targetMonster;
    public Transform target { get; protected set; }

    [Header("Skill Effects")]
    public bool isSkillActive = false; // 스킬 활성화 여부 체크
    [SerializeField] protected float skillRotateMod = 3f;

    protected bool isShootable = true;
    [SerializeField] protected float rotateSpeed = 180f;
    [SerializeField] protected float offsetRotation = 30f;
    [SerializeField] protected float shootingOffsetInterval = 1f;
    protected float shootingInterval = 0.5f;
    [SerializeField] protected float shootingOffsetDuration = 2f;
    protected float shootingDuration = 2f;
    protected WaitForSeconds shootingIntervalSeconds;
    protected Coroutine preFindTargetCoroutine;

    protected RaycastHit2D hit;

    [SerializeField] protected float lineLength = 15f;

    protected float boxSizeX;

    [SerializeField] protected Vector2[] animPosArr;


    private void OnEnable()
    {
        transform.rotation = Quaternion.identity;    
    }

    protected virtual void Update()
    {
        if (target != null)
        {
            UpdateRotation();
        }
    }

    public override void Init()
    {
        OnGetTargetFunc += BattleManager.instance.targetHandler.GetActiveTargets;
        shootingDuration = shootingOffsetDuration;
        shootingInterval = shootingOffsetInterval;
        shootingIntervalSeconds = CoroutineUtility.GetWaitForSeconds(shootingInterval);
        boxSizeX = BoxManager.instance.GetBoxSizeX();
    }

    public virtual void SetTargetMonster()
    {
        if (target != null)
        {
            if (!target.gameObject.activeInHierarchy)
            {
                target = null;
                targetMonster = null;
                return;
            }
        }

        targets = OnGetTargetFunc.Invoke();

        if (targets.Count == 0)
        {
            targetMonster = StageManager.instance.GetMonsterBasement();
            target = targetMonster == null ? null : targetMonster.GetDamagePivot();
            return;
        }

        int targetCount = targets.Count;
        int index = UnityEngine.Random.Range(0, targetCount);

        if (targets.Count > index)
        {
            if (targets[index].isDead)
            {
                SetTargetMonster();
            }

            targetMonster = targets[index];
            target = targetMonster.GetDamagePivot();
        }
    }

    protected abstract void TryShoot();

    protected IEnumerator CoReLoad()
    {
        UpdateIsShootableState(false);
        yield return shootingIntervalSeconds;
        UpdateIsShootableState(true);
    }

    public virtual void SetTarget(Monster monster)
    {
        if (targetMonster == monster)
        {
            // if (preFindTargetCoroutine != null)
            // {
            //     StopCoroutine(preFindTargetCoroutine);
            // }

            if (!gameObject.activeInHierarchy)
            {
                return;
            }

            SetTargetMonster();
            // preFindTargetCoroutine = StartCoroutine(CoFindTargetDelay());
            return;
        }

        if (targetMonster != null)
        {
            target = targetMonster.GetDamagePivot();
        }
    }

    protected void CheckRay()
    {
        Vector2 fireDirection = gameObject.transform.right;  // 레이저 발사 방향

        // TODO: 하드 코딩
        hit = Physics2D.Raycast(transform.position, fireDirection, lineLength, Consts.LAYER_1 | Consts.LAYER_2 | Consts.LAYER_3);
        Debug.DrawRay(transform.position, fireDirection * lineLength);
    }

    #region Skill
    // 스킬 활성화 처리
    protected virtual void ChangeSkillActiveState(bool activate)
    {
        isSkillActive = activate;
    }

    protected abstract IEnumerator CoUsingSkill();

    protected virtual void UpdateIsShootableState(bool isShootable)
    {
        this.isShootable = isShootable;
    }

    #endregion

    public void UpdateAttackDuration(float durationMod)
    {
        shootingDuration = shootingOffsetDuration + shootingOffsetDuration * durationMod;
    }

    public void UpdateAttackInterval(float intervalMod)
    {
        shootingInterval = shootingOffsetInterval + shootingOffsetInterval * intervalMod;
        shootingIntervalSeconds = CoroutineUtility.GetWaitForSeconds(shootingInterval);
    }

    protected virtual void UpdateRotation()
    {
        Vector3 dir = target.position - transform.position;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.FromToRotation(Vector3.right, dir), rotateSpeed * Time.deltaTime);
        // currentOffsetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        // transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(new Vector3(0, 0, currentOffsetAngle)), rotateSpeed * Time.deltaTime);
    }
}
