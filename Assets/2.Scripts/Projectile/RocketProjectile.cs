using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class RocketProjectile : ExplosionProjectile
{
    [SerializeField] private ParticleSystem explosionParticle;
    [SerializeField] private ParticleSystem explosionDustParticle;
    [SerializeField] private ParticleSystem movingSmokeParticle;
    [SerializeField] private ParticleSystem movingFireParticle;

    private Transform target;

    private float firstDistance;

    private SpriteRenderer spriteRenderer;

    private Vector2 missingTargetPos;

    protected override void OnEnable()
    {
        base.OnEnable();
        movingSmokeParticle.gameObject.SetActive(true);
        movingFireParticle.gameObject.SetActive(true);
        movingSmokeParticle.Play();
        movingFireParticle.Play();
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (isCollided)
        {
            return;
        }

        if (other.gameObject.CompareTag(Consts.MONSTER_TAG))
        {
            isCollided = true;
            explosionParticle.Play();
            Explode();
        }
        else if (other.gameObject.layer == Consts.LayerInder.LAYER_3)
        {
            isCollided = true;
            explosionParticle.Play();
            explosionDustParticle.Play();
            Explode();
        }
    }

    public override void Init()
    {
        base.Init();
        disableDelaySeconds = CoroutineUtility.GetWaitForSeconds(explosionParticle.main.duration);
        spriteRenderer = rendererObject.GetComponent<SpriteRenderer>();
    }

    protected override IEnumerator DisableDelay()
    {
        rendererObject.SetActive(false);
        movingSmokeParticle.gameObject.SetActive(false);
        movingFireParticle.gameObject.SetActive(false);
        movingSmokeParticle.Stop();
        movingFireParticle.Stop();
        transform.rotation = Quaternion.identity;
        rigid.velocity = Vector2.zero;
        rigid.freezeRotation = true;
        circleCollider2D.enabled = false;
        rigid.isKinematic = true;

        yield return disableDelaySeconds;

        ReturnToPool();
    }

    public override void Fire()
    {
        StartCoroutine(CoMoveParabola());
    }

    public override void SetRadius(float radius)
    {
        base.SetRadius(radius);
        explosionParticle.transform.localScale = new Vector3(radius, radius, radius);
    }

    public void SetSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
        missingTargetPos = target ? new Vector2(target.position.x, myTransform.position.y): 
            new Vector2(myTransform.position.x + firstDistance, myTransform.position.y);
    }

    public void SetFirstPosX(float firstDistance)
    {
        this.firstDistance = firstDistance;
    }

    private IEnumerator CoMoveParabola()
    {
        Vector2 myOffsetPos = myTransform.position;
        
        float targetPosX = target ? myTransform.position.x + firstDistance : missingTargetPos.x;

        Vector2 targetPos = target ? new Vector2(target.position.x, target.position.y) : missingTargetPos;

        targetPos.y = targetPos.y < missingTargetPos.y ? missingTargetPos.y : targetPos.y;

        float targetingYMod = myTransform.position.y > targetPos.y && (myTransform.position.y + targetPos.y) * Consts.HALF < 1 ?
            myTransform.position.y : (myTransform.position.y + targetPos.y) * Consts.HALF;

        float targetPosY = myTransform.position.y > targetPos.y ? targetingYMod : myTransform.position.y;
        
        Vector2 targetingPos = new Vector2(targetPosX, targetPosY);
        float offsetDistance = Vector2.Distance(targetPos, myOffsetPos);
        float distance = offsetDistance;
        float ratio = 0f;

        while (ratio < 1f)
        {
            if (isCollided)
            {
                yield break;
            }
            distance -= shotPower * Time.deltaTime;

            targetPos = target ? new Vector2(target.position.x, target.position.y) : missingTargetPos;

            ratio = 1 - (distance / offsetDistance) + Time.deltaTime;
            Vector2 firstPos = Vector2.Lerp(myOffsetPos, targetingPos, ratio);
            Vector2 secondPos = Vector2.Lerp(targetingPos, targetPos, ratio);

            Vector2 lastPos = Vector2.Lerp(firstPos, secondPos, ratio);

            Vector3 toDirection = new Vector3(lastPos.x, lastPos.y, 0) - myTransform.position;

            Quaternion targetingRotation = Quaternion.FromToRotation(Vector3.right, toDirection);
            myTransform.SetPositionAndRotation(lastPos, targetingRotation);

            yield return null;
        }

        ratio = 0f;

        while (ratio < 1f && !isCollided)
        {
            ratio += Time.deltaTime;

            Vector3 direction = transform.right;
            direction.x = direction.x < 0 ? -direction.x : direction.x;

            transform.position += direction * shotPower * Time.deltaTime;

            Vector3 directionLerp = Vector3.Slerp(transform.right, new Vector3(1, -1, 0).normalized, ratio);

            myTransform.rotation = Quaternion.FromToRotation(Vector3.right, directionLerp);
            yield return null;
        }

        Explode();
    }
}
