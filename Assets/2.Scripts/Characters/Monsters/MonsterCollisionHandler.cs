using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterCollisionHandler : MonoBehaviour
{
    private int collideCount;

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag(Consts.MONSTER_TAG))
        {
            UpdateCount(true);
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag(Consts.MONSTER_TAG))
        {
            UpdateCount(false);
        }
    }

    public int GetMonsterCount()
    {
        return collideCount;
    }

    private void UpdateCount(bool isIncrease)
    {
        collideCount += isIncrease ? 1 : -1;
    }
}
