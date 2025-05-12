using Keiwando.BigInteger;
using System.Collections;
using UnityEngine;

public class SingleDropAnimatedSkill : SingleAnimatedSkill, ISkillDrop
{
    private float skillDamagePercent;

    [SerializeField] private float distanceY = 5f;
    [SerializeField] private float arriveTime = 0.5f;

    private float targetSpeed;

    private readonly int TRIGGER_HASH = AnimatorParameters.IS_TRIGGER_HASH;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private SpriteRenderer effectRenderer;

    private float wallTriggerPosX;
    private float monsterTriggerHalfSizeX;

    protected override void Awake()
    {
        base.Awake();
    }

    public IEnumerator CoMoveToTarget(Monster monster, bool isVibrated, BigInteger damage)
    {
        Vector2 targetPos = monster.transform.position;

        bool isTriggered = targetPos.x - monsterTriggerHalfSizeX < wallTriggerPosX;

        Debug.Log($"targetPos.x {targetPos.x}, monsterTriggerHalfSizeX {monsterTriggerHalfSizeX}, wallTriggerPosX, {wallTriggerPosX}");

        float height = targetPos.y + distanceY;

        Vector2 departPos = isTriggered ? new Vector2(targetPos.x, height) : new Vector2(targetPos.x - targetSpeed * arriveTime, height);
        Vector2 arrivePos = new Vector2(departPos.x, targetPos.y);

        float elapsedTime = Time.deltaTime;
        float ratio = elapsedTime / arriveTime;

        while (ratio < 1f)
        {
            Vector2 currentPos = Vector2.Lerp(departPos, arrivePos, ratio);
            transform.position = currentPos;
            elapsedTime += Time.deltaTime;
            ratio = elapsedTime / arriveTime;
            yield return null;
        }

        animator.SetBool(TRIGGER_HASH, true);
        effectRenderer.gameObject.SetActive(true);

        OnDamageTarget(skillDamagePercent, monster, monster.GetDamageTextPivot().position, isVibrated, damage);
        StartCoroutine(CoWaitUntilAnimationFinished(0));
    }

    public override void Use(SkillData skillData, Monster monster, BigInteger damage)
    {
        gameObject.SetActive(true);

        effectRenderer.gameObject.SetActive(false);
        // skillDamagePercent = skillData.skillUpgradableData.damagePerecent / Consts.PERCENT_DIVIDE_VALUE;

        animator.SetBool(TRIGGER_HASH, false);

        targetSpeed = monster.GetSpeed();
        monsterTriggerHalfSizeX = monster.GetColliderWidth();

        StartCoroutine(CoAppear());
        // StartCoroutine(CoMoveToTarget(monster, skillData.isVibrated, damage));
    }

    public IEnumerator CoAppear()
    {
        float elapsedTime = Time.deltaTime;
        float appearTime = arriveTime * Consts.HALF;
        float ratio = elapsedTime / appearTime;
        Color spriteColor = spriteRenderer.color;

        while (ratio < 1f)
        {
            ratio = elapsedTime / appearTime;
            float alpha = Mathf.Lerp(0f, 1f, ratio);
            spriteColor.a = alpha;
            spriteRenderer.color = spriteColor;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    public void SetWallTriggerPosX(float wallTriggerPosX)
    {
        this.wallTriggerPosX = wallTriggerPosX;
    }
}
