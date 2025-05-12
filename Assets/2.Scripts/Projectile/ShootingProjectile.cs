using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingProjectile : Projectile
{
    private float disableDelayTime;

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag(Consts.MONSTER_TAG))
        {
            if (other.TryGetComponent(out MonsterBase monster))
            {
                if (!monster.isDead)
                {
                    BattleManager.instance.OnMonsterAttacked(monster, monster.GetDamageTextPivot().position, false, damage);
                }
            }
        }
    }

    public override void Fire()
    {
        rigid.velocity = transform.right * shotPower;

        if (disableCoroutine != null)
        {
            StopCoroutine(DelayDisable());
        }
        disableCoroutine = StartCoroutine(DelayDisable());
    }

    public void SetDisableDelayTime(float disableDelayTime)
    {
        this.disableDelayTime = disableDelayTime;
    }

    private IEnumerator DelayDisable()
    {
        yield return CoroutineUtility.GetWaitForSeconds(disableDelayTime);
        ReturnToPool();
    }
}
