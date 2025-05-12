using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomerangProjectile : ShootingProjectile
{
    [Header("Animator")]
    protected Animator animator;
    [SerializeField] protected Animator[] animators;

    [SerializeField] private Transform rendererTransform;

    [SerializeField] private float rotateValue = 135f;

    private Transform shooterTransform;
    private float middlePosX;
    private float firstDistanceY;
    private float secondDistanceY;
    private float distanceMaxX;

    private void OnEnable()
    {
        if (animator != null)
        {
            rendererTransform.rotation = Quaternion.identity;
        }
    }

    private void Update() 
    {
        rendererTransform.Rotate(0, 0, rotateValue * Time.deltaTime);
    }

    public override void Fire()
    {
        StartCoroutine(CoMoveParabola());
    }

    private IEnumerator CoMoveParabola()
    {
        Vector2 offsetPos = myTransform.position;
        Vector2 targetPos = new Vector2(shooterTransform.position.x + middlePosX, shooterTransform.position.y + firstDistanceY);

        float offsetDistance = Vector2.Distance(targetPos, myTransform.position);
        float distance = offsetDistance;
        float ratio = 0f;

        while (ratio < 1f)
        {
            distance -= shotPower * Time.deltaTime;

            targetPos = new Vector2(shooterTransform.position.x + middlePosX, shooterTransform.position.y + firstDistanceY);

            ratio = 1 - (distance / offsetDistance);

            Vector2 lastPos = Vector2.Lerp(offsetPos, targetPos, ratio);

            myTransform.position = lastPos;

            yield return null;
        }

        Vector2 targetingPos = Vector2.zero;

        offsetPos = myTransform.position;
        targetPos = shooterTransform.position;
        targetPos.x += distanceMaxX;
        offsetDistance = Vector2.Distance(targetPos, offsetPos);
        distance = offsetDistance;
        ratio = 0f;

        while (ratio < 1f)
        {
            distance -= shotPower * Time.deltaTime;

            targetPos = shooterTransform.position;
            targetPos.x += distanceMaxX;

            targetingPos = new Vector2(targetPos.x, offsetPos.y);

            ratio = 1 - (distance / offsetDistance);
            Vector2 firstPos = Vector2.Lerp(offsetPos, targetingPos, ratio);
            Vector2 secondPos = Vector2.Lerp(targetingPos, targetPos, ratio);

            Vector2 lastPos = Vector2.Lerp(firstPos, secondPos, ratio);

            myTransform.position = lastPos;

            yield return null;
        }

        offsetPos = myTransform.position;
        targetPos = new Vector2(shooterTransform.position.x + middlePosX, shooterTransform.position.y + secondDistanceY);
        offsetDistance = Vector2.Distance(targetPos, offsetPos);
        distance = offsetDistance;
        
        ratio = 0f;

        while (ratio < 1f)
        {
            distance -= shotPower * Time.deltaTime;

            targetPos = new Vector2(shooterTransform.position.x + middlePosX, shooterTransform.position.y + secondDistanceY);
            targetingPos = new Vector2(offsetPos.x, shooterTransform.position.y + secondDistanceY);

            ratio = 1 - (distance / offsetDistance);
            Vector2 firstPos = Vector2.Lerp(offsetPos, targetingPos, ratio);
            Vector2 secondPos = Vector2.Lerp(targetingPos, targetPos, ratio);

            Vector2 lastPos = Vector2.Lerp(firstPos, secondPos, ratio);

            myTransform.position = lastPos;

            yield return null;
        }

        offsetPos = myTransform.position;
        targetPos = shooterTransform.position;
        offsetDistance = Vector2.Distance(targetPos, offsetPos);
        distance = offsetDistance;
        ratio = 0f;

        while (ratio < 1f)
        {
            distance -= shotPower * Time.deltaTime;

            targetPos = shooterTransform.position;

            ratio = 1 - (distance / offsetDistance);

            Vector2 lastPos = Vector2.Lerp(offsetPos, targetPos, ratio);

            transform.position = lastPos;
            yield return null;
        }

        ReturnToPool();
    }

    public void SetShooterTransform(Transform shooterTransform)
    {
        this.shooterTransform = shooterTransform;
    }

    public void SetMiddlePosX(float middlePosX)
    {
        this.middlePosX = middlePosX;
    }

    public void SetFirstDistanceY(float firstDistanceY)
    {
        this.firstDistanceY = firstDistanceY;
    }

    public void SetSecondDistance(float secondDistanceY)
    {
        this.secondDistanceY = secondDistanceY;
    }

    public void SetDistanceMaxX(float distanceMaxX)
    {
        this.distanceMaxX = distanceMaxX;
    }

    public void UpdateAnimIndex(int index)
    {
        if (animators.Length <= index)
        {
            return;
        }

        if (index >= 0)
        {
            for (int i = 0; i < animators.Length; i++)
            {
                if (i == index)
                {
                    animator = animators[i];
                    rendererTransform.rotation = Quaternion.identity;
                    animators[i].gameObject.SetActive(true);
                }
                else
                {
                    animators[i].gameObject.SetActive(false);
                }
            }
        }
    }
}
