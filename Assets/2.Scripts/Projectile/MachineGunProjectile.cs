using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGunProjectile : Bullet
{
    [SerializeField] private GameObject bulletObject;

    protected override void OnEnable()
    {
        base.OnEnable();
        rigid.freezeRotation = false;
        rigid.isKinematic = false;
        bulletObject.SetActive(true);
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
            if (other.TryGetComponent(out MonsterBase monster))
            {
                if (!monster.isDead)
                {
                    BattleManager.instance.OnMonsterAttacked(monster, monster.GetDamageTextPivot().position, false, damage);
                }

                if (disableCoroutine != null)
                {
                    StopCoroutine(disableCoroutine);
                }
                ReturnToPool();
            }
        }
        else if (other.gameObject.layer == Consts.LayerInder.LAYER_3)
        {
            bulletObject.SetActive(false);
            transform.rotation = Quaternion.identity;
            rigid.velocity = Vector2.zero;
            rigid.freezeRotation = true;
            rigid.isKinematic = true;
            ShowHoleParticle(other);
        }
    }

    protected override IEnumerator CoDisableDelay()
    {
        yield return disableDelaySeconds;
        ReturnToPool();
    }
}
