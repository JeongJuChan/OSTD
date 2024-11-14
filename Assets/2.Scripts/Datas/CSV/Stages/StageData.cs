using System;
using System.Collections;
using System.Collections.Generic;
using Keiwando.BigInteger;
using UnityEngine;

[Serializable]
public struct StageData
{
    public int stageNum;
    public string stageName;
    public int[] monsterIndexes;
    public int checkpointNum;
    public BigInteger enemyBaseHealth;
    public float speed;
    public float[] monsterInstIntervals;
    public float secondMonsterInstIntervalMultiplication;
    public float thirdMonsterInstIntervalMultiplication;
    public float firstWaveDuration;
    public float secondWaveDuration;

    public StageData(int stageNum, string stageName, int[] monsterIndexes, int checkpointNum, BigInteger enemyBaseHealth, float speed, 
        float[] monsterInstIntervals, float secondMonsterInstIntervalMultiplication, float thirdMonsterInstIntervalMultiplication, float firstWaveDuration,
        float secondWaveDuration)
    {
        this.stageNum = stageNum;
        this.stageName = stageName;
        this.monsterIndexes = monsterIndexes;
        this.checkpointNum = checkpointNum;
        this.enemyBaseHealth = enemyBaseHealth;
        this.speed = speed;
        this.monsterInstIntervals = monsterInstIntervals;
        this.secondMonsterInstIntervalMultiplication = secondMonsterInstIntervalMultiplication;
        this.thirdMonsterInstIntervalMultiplication = thirdMonsterInstIntervalMultiplication;
        this.firstWaveDuration = firstWaveDuration;
        this.secondWaveDuration = secondWaveDuration;
    }
}
