using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawnTimer
{
    private float spawnInterval;
    private float spawnElapsedTime;
    private float offsetInterval;

    public void UpdateInterval(float spawnInterval)
    {
        offsetInterval = spawnInterval;
        this.spawnInterval = spawnInterval;
        spawnElapsedTime = 0f;
    }

    public void MultiplyInterval(float multipleAmount)
    {
        spawnInterval = offsetInterval * multipleAmount;
    }

    public bool UpdateElapsedTime()
    {
        if (offsetInterval == 0f)
        {
            return false;
        }

        spawnElapsedTime += Time.deltaTime;

        bool isSpawnable = spawnElapsedTime >= spawnInterval;

        if (isSpawnable)
        {
            spawnElapsedTime = 0f;
        }
        return isSpawnable;
    }
}
