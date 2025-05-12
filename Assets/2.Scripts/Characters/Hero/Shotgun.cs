using System;
using System.Collections;
using System.Collections.Generic;
using Keiwando.BigInteger;
using UnityEngine;
using UnityEngine.EventSystems;

public class Shotgun : MonoBehaviour
{
    [SerializeField] private ProjectileType bulletType;
    [SerializeField] private float rotateSpeed = 180f;
    [SerializeField] private float attackInterval = 1f;

    [SerializeField] private float elapsedTime = 1f;

    [SerializeField] private float anglePerBullet = 2.5f;

    [SerializeField] private float shotPower = 20f;

    [SerializeField] private float offsetRotation = 30f;
    [SerializeField] private float minRotation = -75f;
    [SerializeField] private float maxRotation = 75f;

    private int bulletAmount = 6;

    [SerializeField] private int bulletInitMod = 5;

    [SerializeField] private Transform firePivot;

    [SerializeField] private Transform target;

    private ProjectilePooler projectilePooler;

    private float currentOffsetAngle;

    private BigInteger damage;

    public Action OnPlayShotAnim;
    public Action OnPlayGunloadAnim;

    private bool isDraggingState = false;

    private void Update()
    {
        if (!GameManager.instance.isGameState)
        {
            return;
        }

        CheckDraggingState();
        if (isDraggingState)
        {
            UpdateDirectionByInput();
            TryShoot();
            return;
        }

        if (target != null)
        {
            UpdateRotation();
            TryShoot();
        }
        else if (elapsedTime < attackInterval)
        {
            UpdateElapsedTimeAfterShot();
        }
    }

    private void UpdateDirectionByInput()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = (mousePos - firePivot.position).normalized;
        UpdateRotationByDirection(dir);
    }

    private void CheckDraggingState()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDraggingState = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDraggingState = false;
        }
    }

    private void TryShoot()
    {
        if (elapsedTime >= attackInterval)
        {
            OnPlayShotAnim?.Invoke();
            for (int i = 0; i < bulletAmount; i++)
            {
                Projectile bullet = projectilePooler.Pool((int)bulletType, firePivot.position, Quaternion.identity);
                bullet.SetShotPower(shotPower);
                bullet.UpdateDamage(damage);
                // TODO : remove Instantiate 
                // Projectile bullet = Instantiate(bulletPrefab, firePivot.position, Quaternion.identity, null);

                float randomAngle = UnityEngine.Random.Range(0, anglePerBullet);

                int halfIndex = i / 2;
                float modAngle = i % 2 == 0 ? -randomAngle * -(halfIndex + 1) : randomAngle * (halfIndex + 1);

                bullet.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, modAngle + currentOffsetAngle));

                bullet.Fire();
            }

            OnPlayGunloadAnim?.Invoke();
            elapsedTime = 0f;
        }
        else
        {
            UpdateElapsedTimeAfterShot();
        }
    }

    private void UpdateElapsedTimeAfterShot()
    {
        elapsedTime += Time.deltaTime;
        elapsedTime = elapsedTime > attackInterval ? attackInterval : elapsedTime;
    }

    private void UpdateRotation()
    {
        Vector3 dir = target.position - transform.position;
        UpdateRotationByDirection(dir);
    }

    private void UpdateRotationByDirection(Vector2 dir)
    {
        currentOffsetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        currentOffsetAngle = currentOffsetAngle < minRotation ? minRotation : currentOffsetAngle;
        currentOffsetAngle = currentOffsetAngle > maxRotation ? maxRotation : currentOffsetAngle;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(new Vector3(0, 0, currentOffsetAngle - offsetRotation)), rotateSpeed * Time.deltaTime);
    }

    #region Initialize
    public void InitPoolSize()
    {
        int initCount = bulletAmount * bulletInitMod;
        projectilePooler = PoolManager.instance.projectile;
        //TODO : this is used by test when is finished then remove 
        projectilePooler.AddPoolInfo((int)bulletType, initCount, initCount * bulletInitMod);
        // PoolManager.instance.projectile.AddPoolInfo(bullet.index, initCount, initCount * 2);
    }
    #endregion

    public void SetTarget(MonsterBase targetMonster)
    {
        if (targetMonster == null)
        {
            target = null;
            return;
        }
        
        target = targetMonster.GetDamagePivot();
    }

    public void UpdateDamage(BigInteger damage)
    {
        this.damage = damage;
    }
}
