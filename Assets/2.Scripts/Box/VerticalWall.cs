using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalWall : MonoBehaviour
{
    private HashSet<MonsterCollisionHandler> monsters = new HashSet<MonsterCollisionHandler>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out MonsterCollisionHandler monsterCollisionHandler))
        {
            AddMonster(monsterCollisionHandler);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out MonsterCollisionHandler monsterCollisionHandler))
        {
            RemoveMonster(monsterCollisionHandler);
        }
    }

    public void Init()
    {
        GameManager.instance.OnReset += Reset;
    }

    private void Reset()
    {
        monsters.Clear();
    }

    public HashSet<MonsterCollisionHandler> GetMonsters()
    {
        return monsters;
    }

    

    #region MonsterSet
    private void AddMonster(MonsterCollisionHandler monsterCollisionHandler)
    {
        monsters.Add(monsterCollisionHandler);
    }

    private void RemoveMonster(MonsterCollisionHandler monsterCollisionHandler)
    {
        monsters.Remove(monsterCollisionHandler);
    }
    #endregion
}
