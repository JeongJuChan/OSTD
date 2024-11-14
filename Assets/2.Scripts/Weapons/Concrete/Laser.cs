using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : TargetingWeapon, ITickAttack
{
    [Header("Laser Settings")]
    public ParticleSystem laserStartParticles;
    public ParticleSystem laserEndParticles;

    [Header("Laser Width")]
    public float normalWidth = 0.25f;  // 기본 레이저 굵기
    public float specialWidth = 0.5f;  // 스킬 사용 시 레이저 굵기

    [Header("Skill Effects")]
    public GameObject floorEffectPrefab;  // 잔상 이펙트 프리팹
    public float burnMarkDuration = 5f;   // 잔상 유지 시간

    private LineRenderer line;
    private RaycastHit2D[] skillHits;

    [SerializeField] private MeleeAttackTrigger meleeAttackTrigger;

    private EffectObjectPooler effect;

    [SerializeField] protected float shootingElapsedTime = 0f;

    private void Awake() 
    {
        Init();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        StopLaser();
    }

    private void FixedUpdate()
    {
        UpdateLaserWidth();

        if (isSkillActive)
        {
            CheckSkillRay();
            ShowSkillEffect();
            return;
        }

        if (isShootable && targetMonster != null)
        {
            TryShoot();
        }
        else
        {
            StopLaser();
        }

    }

    protected override void Update()
    {
        if (isSkillActive)
        {
            line.enabled = true;
            return;
        }

        if (target != null)
        {
            UpdateRotation();
        }
    }

    #region Initialize
    // 초기 레이저 설정
    public override void Init()
    {
        base.Init();
        line = GetComponent<LineRenderer>();
        line.startWidth = normalWidth;
        line.endWidth = normalWidth;
        line.enabled = false;  // 레이저 비활성화
        OnGetTargetFunc += BattleManager.instance.targetHandler.GetActiveTargets;
        shootingIntervalSeconds = CoroutineUtility.GetWaitForSeconds(shootingInterval);
        boxSizeX = BoxManager.instance.GetBoxSizeX();
        meleeAttackTrigger.OnMonsterAttacked += ApplySkillDamage;
        // TDOO: Effect 풀 시트 구현
        effect = PoolManager.instance.effect;
        effect.AddPoolInfo(30000, 100, 1000);
        GameManager.instance.OnReset += Reset;
    }
    #endregion

    private void Reset()
    {
        transform.rotation = Quaternion.identity;
    }

    public void ApplyDamage()
    {
        if (!targetMonster.isDead)
        {
            BattleManager.instance.OnMonsterAttacked(targetMonster, this, targetMonster.GetDamageTextPivot().position, false, weaponData.damage);
        }
    }

    // 레이저 굵기 업데이트
    private void UpdateLaserWidth()
    {
        float currentWidth = isSkillActive ? specialWidth : normalWidth;
        line.startWidth = currentWidth;
        line.endWidth = currentWidth;
    }

    // 레이저가 벽이나 바닥에 맞았을 때 처리
    private void HandleHitLaser()
    {
        if (targetMonster == null)
        {
            return;
        }

        if (hit.collider.CompareTag(Consts.MONSTER_TAG))
        {
            line.enabled = true;
            HandleParticles(laserStartParticles);
            ShowLaser(target.position);

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

                StopLaser();
                preFindTargetCoroutine = StartCoroutine(CoReLoad());
                shootingElapsedTime = 0f;
            }
        }
        else
        {
            StopLaser();
        }    
    }

    private void ShowLaser(Vector2 pos)
    {
        Vector2 targetPos = pos;

        HandleParticles(laserEndParticles);

        laserEndParticles.transform.position = targetPos;

        float distance = Vector2.Distance(transform.position, targetPos);
        line.SetPosition(1, new Vector3(distance, 0, 0));
    }

    // 레이저가 맞지 않았을 때 처리
    private void HandleMissLaser()
    {
        line.SetPosition(1, new Vector3(lineLength, 0, 0));

        if (laserEndParticles.isPlaying)
        {
            StopParticles(laserEndParticles);
        }
    }

    // 레이저 중지 처리
    private void StopLaser()
    {
        line.enabled = false;
        line.SetPosition(1, new Vector3(lineLength, 0, 0));

        StopParticles(laserStartParticles);
        StopParticles(laserEndParticles);
    }

    // 잔상 효과 생성
    private void CreateBurnMark(Vector2 position)
    {
        Effect burnMark = effect.Pool(30000, position, Quaternion.identity);
        burnMark.gameObject.SetActive(true);
        StartCoroutine(ReturnBurnMarkDelay(burnMark));
    }

    private IEnumerator ReturnBurnMarkDelay(Effect effect)
    {
        yield return CoroutineUtility.GetWaitForSeconds(burnMarkDuration);
        effect.ReturnToPool();
    }

    // 파티클 재생 처리
    private void HandleParticles(ParticleSystem particles)
    {
        if (!particles.isPlaying)
        {
            particles.Play(true);
        }
    }

    // 파티클 정지 처리
    private void StopParticles(ParticleSystem particles)
    {
        if (particles.isPlaying)
        {
            particles.Stop(true);
        }
    }

    #region Skill
    public override void UseSkill()
    {
        ChangeSkillActiveState(true);
        meleeAttackTrigger.UpdateColliderActiveState(true);

        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(CoUsingSkill());
        }
    }

    protected override IEnumerator CoUsingSkill()
    {
        float elapsedTime = Time.deltaTime;
        while (elapsedTime < skillDuration)
        {
            Vector3 targetPos = BoxManager.instance.transform.position;
            targetPos.x += boxSizeX + skillRotateMod * elapsedTime;
            Vector3 dir = targetPos - transform.position;

            // transform.right = dir;

            transform.rotation = Quaternion.FromToRotation(Vector3.right, dir);

            yield return null;
            elapsedTime += Time.deltaTime;
        }

        ChangeSkillActiveState(false);
        meleeAttackTrigger.UpdateColliderActiveState(false);
    }

    private void ApplySkillDamage(MonsterBase monster)
    {
        if (!monster.isDead)
        {
            BattleManager.instance.OnMonsterAttacked(monster, monster.GetDamageTextPivot().position, false, weaponData.skillDamage);
        }
    }
    #endregion

    protected override void TryShoot()
    {
        CheckRay();
        if (hit)
        {
            HandleHitLaser();
        }
        else
        {
            StopLaser();
        }
    }

    private void CheckSkillRay()
    {
        HandleParticles(laserStartParticles);
        Vector2 fireDirection = gameObject.transform.right;  // 레이저 발사 방향
        // TODO: 하드 코딩
        skillHits = Physics2D.RaycastAll(transform.position, fireDirection, lineLength, Consts.LAYER_3);
    }

    private void ShowSkillEffect()
    {
        foreach (RaycastHit2D hit in skillHits)
        {
            if (hit.collider == null)
            {
                continue;
            }

            if (hit.collider.CompareTag(Consts.FLOOR_TAG))
            {
                ShowLaser(hit.point);
                CreateBurnMark(hit.point);
            }
        }
    }

    private void UpdateRotation()
    {
        Vector3 dir = target.position - transform.position;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.FromToRotation(Vector3.right, dir), rotateSpeed * Time.deltaTime);
        // currentOffsetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        // transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(new Vector3(0, 0, currentOffsetAngle)), rotateSpeed * Time.deltaTime);
    }

    public override void UpdateWeaponData(WeaponData weaponData)
    {
        base.UpdateWeaponData(weaponData);
        int index = weaponData.level - 1;

        if (index >= animators.Length)
        {
            return;
        }

        line.SetPosition(0, animPosArr[index]);
    }
}
