using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketSkillProjectile : ExplosionProjectile
{
    [SerializeField] private ParticleSystem explosionParticle;
    [SerializeField] private ParticleSystem explosionDustParticle;
    [SerializeField] private ParticleSystem movingSmokeParticle;
    [SerializeField] private ParticleSystem movingFireParticle;

    private SpriteRenderer spriteRenderer;

    private Transform targetCircleTransform;
    private float targetRadius;
    private float targetDistanceX;
    private float targetPosY;

    private float fallingSpeed;

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
        else if (other.gameObject.CompareTag(Consts.FLOOR_TAG))
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
        targetPosY = BoxManager.instance.transform.position.y - 1;
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

    public void SetTargetCirlce(Transform targetCircleTransform)
    {
        this.targetCircleTransform = targetCircleTransform;
    }

    public void SetTargetRadius(float targetRadius)
    {
        this.targetRadius = targetRadius;
    }

    public void SetTargetDistance(float targetDistanceX)
    {
        this.targetDistanceX = targetDistanceX;
    }

    public void SetFallingSpeed(float fallingSpeed)
    {
        this.fallingSpeed = fallingSpeed;
    }

    private IEnumerator CoMoveParabola()
    {
        bool isLowerThanCirclePosYMin = transform.position.y < (targetCircleTransform.position.y - targetRadius);

        Vector2 firstTargetPos =  isLowerThanCirclePosYMin? 
            new Vector2(targetCircleTransform.position.x + targetRadius, targetCircleTransform.position.y) : 
            new Vector2(targetCircleTransform.position.x, targetCircleTransform.position.y + targetRadius);

        Vector2 targetPos = Vector2.zero;

        float offsetDistance = Vector2.Distance(firstTargetPos, myTransform.position);
        float distance = offsetDistance;
        float ratio = 0f;

        Vector2 offsetPos = myTransform.position;

        Vector3 firstdirection = Vector3.zero;

        Vector2 offsetDirection = transform.right;

        Vector2 targetingPos = isLowerThanCirclePosYMin ?
                new Vector2(targetCircleTransform.position.x + targetRadius, myTransform.position.y) :
                new Vector2(myTransform.position.x, targetCircleTransform.position.y + targetRadius);

        while (ratio < 1f)
        {
            if (isCollided)
            {
                yield break;
            }
            distance -= shotPower * Time.deltaTime;

            targetPos = isLowerThanCirclePosYMin ?
                new Vector2(targetCircleTransform.position.x + targetRadius, targetCircleTransform.position.y) :
                new Vector2(targetCircleTransform.position.x, targetCircleTransform.position.y + targetRadius);

            ratio = 1f - (distance / offsetDistance);

            Vector2 lastPos = Vector2.Lerp(offsetPos, targetPos, ratio);

            firstdirection = isLowerThanCirclePosYMin ? Vector3.up : Vector3.right;

            Vector3 directionLerp = Vector3.Slerp(offsetDirection, firstdirection, ratio);

            Quaternion targetingRotation = Quaternion.FromToRotation(Vector3.right, directionLerp);
            myTransform.SetPositionAndRotation(lastPos, targetingRotation);

            yield return null;
        }

        Vector2 secondTargetPos = isLowerThanCirclePosYMin ?
            new Vector2(targetCircleTransform.position.x, targetCircleTransform.position.y + targetRadius) :
            new Vector2(targetCircleTransform.position.x + targetRadius, targetCircleTransform.position.y);

        offsetDistance = Vector2.Distance(secondTargetPos, myTransform.position);
        distance = offsetDistance;
        ratio = 0f;
        offsetPos = myTransform.position;

        Vector3 secondDirection = Vector3.zero;

        targetingPos = new Vector2(targetCircleTransform.position.x + targetRadius, targetCircleTransform.position.y + targetRadius);

        while (ratio < 1f)
        {
            if (isCollided)
            {
                yield break;
            }

            distance -= shotPower * Time.deltaTime;

            targetPos = isLowerThanCirclePosYMin ?
                new Vector2(targetCircleTransform.position.x, targetCircleTransform.position.y + targetRadius) :
                new Vector2(targetCircleTransform.position.x + targetRadius, targetCircleTransform.position.y);


            ratio = 1 - (distance / offsetDistance);
            Vector2 firstPos = Vector2.Lerp(offsetPos, targetingPos, ratio);
            Vector2 secondPos = Vector2.Lerp(targetingPos, targetPos, ratio);

            Vector2 lastPos = Vector2.Lerp(firstPos, secondPos, ratio);

            secondDirection = isLowerThanCirclePosYMin ? Vector3.left : Vector3.down;

            Vector3 directionLerp = Vector3.Slerp(firstdirection, secondDirection, ratio);

            Quaternion targetingRotation = Quaternion.FromToRotation(Vector3.right, directionLerp);
            myTransform.SetPositionAndRotation(lastPos, targetingRotation);

            yield return null;
        }

        

        if (isLowerThanCirclePosYMin)
        {
            targetPos = new Vector2(targetCircleTransform.position.x - targetRadius, targetCircleTransform.position.y);

            targetingPos = new Vector2(targetCircleTransform.position.x - targetRadius, targetCircleTransform.position.y + targetRadius);

            offsetDistance = Vector2.Distance(targetPos, myTransform.position);
            distance = offsetDistance;
            ratio = 0f;

            offsetPos = myTransform.position;

            while (ratio < 1f)
            {
                if (isCollided)
                {
                    yield break;
                }

                distance -= shotPower * Time.deltaTime;

                targetPos = new Vector2(targetCircleTransform.position.x - targetRadius, targetCircleTransform.position.y);

                ratio = 1 - (distance / offsetDistance);
                Vector2 firstPos = Vector2.Lerp(offsetPos, targetingPos, ratio);
                Vector2 secondPos = Vector2.Lerp(targetingPos, targetPos, ratio);

                Vector2 lastPos = Vector2.Lerp(firstPos, secondPos, ratio);

                Vector3 directionLerp = Vector3.Slerp(secondDirection, Vector3.down, ratio);

                Quaternion targetingRotation = Quaternion.FromToRotation(Vector3.right, directionLerp);
                myTransform.SetPositionAndRotation(lastPos, targetingRotation);

                yield return null;
            }
        }

        targetPos = new Vector2(BoxManager.instance.transform.position.x + targetDistanceX, targetPosY);

        offsetDistance = Vector2.Distance(targetPos, myTransform.position);
        distance = offsetDistance;
        ratio = 0f;

        offsetPos = myTransform.position;

        int fallingCount = 0;

        while (ratio < 1f)
        {
            if (isCollided)
            {
                yield break;
            }
            
            distance -= (shotPower + fallingSpeed * fallingCount) * Time.deltaTime;
            targetPos = new Vector2(BoxManager.instance.transform.position.x + targetDistanceX, targetPosY);
            ratio = 1 - (distance / offsetDistance);
            Vector2 lastPos = Vector2.Lerp(offsetPos, targetPos, ratio);
            Vector3 directionLerp = Vector3.Slerp(transform.right, targetPos - offsetPos, ratio);

            Quaternion targetingRotation = Quaternion.FromToRotation(Vector3.right, directionLerp);
            myTransform.SetPositionAndRotation(lastPos, targetingRotation);

            fallingCount++;
            yield return null;
        }
    }
}
