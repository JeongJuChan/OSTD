using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterProjectile : Projectile
{
    private float disableTimeAfterArrived = 5f;
    
    private Transform target;
    private float targetOffsetX = 1f;
    private float targetOffsetY = 0.5f;
    private WaitForSeconds disableDelaySeconds;

    [SerializeField] private float speed = 5f;

    private Coroutine preCoroutine;

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.gameObject.CompareTag(Consts.HERO_TAG))
        {
            if (other.TryGetComponent(out Hero hero))
            {
                BattleManager.instance.OnHeroAttacked(hero, hero.transform.position, false, damage);
            }
            else if (other.TryGetComponent(out DragableBox dragableBox))
            {
                IDamageable boxDamageable = dragableBox.damagable;
                BattleManager.instance.OnHeroAttacked(boxDamageable, dragableBox.transform.position, false, damage);
            }

            ReturnToPool();
        }
    }

    private void OnDisable() 
    {
        target = null;
    }

    public override void Init()
    {
        base.Init();
        disableDelaySeconds = CoroutineUtility.GetWaitForSeconds(disableTimeAfterArrived);
    }

    private IEnumerator CoMoveParabola()
    {
        Vector2 myOffsetPos = myTransform.position;
        float targetPosX = target.position.x + targetOffsetX > myTransform.position.x ? target.position.x : target.position.x + targetOffsetX;
        float targetPosY = target.position.y + targetOffsetY > myTransform.position.y ? target.position.y : target.position.y + targetOffsetY;
        Vector2 targetingPos = new Vector2(targetPosX, targetPosY);
        float offsetDistance = myOffsetPos.x - target.position.x;
        float distance = offsetDistance;
        float ratio = 0f;

        while (ratio < 1f)
        {
            distance -= speed * Time.deltaTime;

            targetingPos.x = (myOffsetPos.x + target.position.x) * Consts.HALF;

            ratio = 1 - (distance / offsetDistance);
            Vector2 firstPos = Vector2.Lerp(myOffsetPos, targetingPos, ratio);
            Vector2 secondPos = Vector2.Lerp(targetingPos, target.position, ratio);

            Vector2 lastPos = Vector2.Lerp(firstPos, secondPos, ratio);

            Vector3 toDirection = new Vector3(lastPos.x, lastPos.y, 0) - myTransform.position;

            Quaternion targetingRotation = Quaternion.FromToRotation(Vector3.right, toDirection);
            myTransform.SetPositionAndRotation(lastPos, targetingRotation);

            yield return null;
        }

        ReturnToPool();
    }

    public override void Fire()
    {
        if (preCoroutine != null)
        {
            StopCoroutine(preCoroutine);
        }
        preCoroutine = StartCoroutine(CoMoveParabola());
    }
    
    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    private IEnumerator DisableDelayed()
    {
        yield return disableDelaySeconds;

        if (gameObject.activeInHierarchy)
        {
            ReturnToPool();
        }
    }
}
