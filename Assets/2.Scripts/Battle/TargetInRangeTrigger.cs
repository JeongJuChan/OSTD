using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetInRangeTrigger : MonoBehaviour
{
    public event Action<Monster> OnTargetAdded;

    [SerializeField] protected BoxCollider2D boxCollider;

    private float offsetPosX;

    private Transform boxManagerTrans;

    private bool isInGame;

    #region UnityMethods
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!gameObject.activeInHierarchy)
        {
            return;
        }

        if (collision.TryGetComponent(out MonsterBase monster))
        {
            if (monster.GetMonsterType() == MonsterType.Basement)
            {
                HeroManager.instance.hero.SetTarget(monster);
                WeaponManager.instance.TryFindTarget();
                monster.ToggleInvincible(false);
            }
            else
            {
                OnTargetAdded?.Invoke(monster as Monster);
            }
        }
        
        if (collision.TryGetComponent(out MonsterBasementTrigger basementTrigger))
        {
            basementTrigger.EncounterMonsterBasement();
        }
    }

    private void Update() 
    {
        if (isInGame)
        {
            UpdatePos();
        }
    }

    #endregion

    #region Initialize

    public void Init()
    {
    }

    public void SetOffsetPosX(float posX)
    {
        boxManagerTrans = BoxManager.instance.transform;
        offsetPosX = posX - boxManagerTrans.position.x;
    }

    public float GetOffsetPosX()
    {
        return offsetPosX;
    }

    #endregion

    #region InGame
    public void UpdateGameState(bool isInGame)
    {
        this.isInGame = isInGame;
    }

    private void UpdatePos()
    {
        transform.position = new Vector2(boxManagerTrans.position.x + offsetPosX, transform.position.y);
    }
    #endregion

    public float GetBoxSizeX()
    {
        return boxCollider.size.x;
    }

}
