using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunBullet : Bullet
{
    [SerializeField] protected GameObject bulletObject;

    protected override void OnEnable()
    {
        base.OnEnable();
        bulletObject.SetActive(true);
        rigid.freezeRotation = false;
        rigid.isKinematic = false;
    }

    public override void Init()
    {
        base.Init();
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
                    BattleManager.instance.OnMonsterAttacked(monster, monster.transform.position, false, damage);
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
            isCollided = true;
            bulletObject.SetActive(false);
            transform.rotation = Quaternion.identity;
            rigid.velocity = Vector2.zero;
            rigid.freezeRotation = true;
            rigid.isKinematic = true;
            ShowHoleParticle(other);
        }
    }
}
