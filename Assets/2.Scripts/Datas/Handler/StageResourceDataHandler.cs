using System;
using System.Collections;
using System.Collections.Generic;
using Keiwando.BigInteger;
using UnityEngine;

public class StageResourceDataHandler
{
    private GameData stageData;
    private GameData stageFixedData;

    private Dictionary<int, Dictionary<int, StageData>> stageDataDict = new Dictionary<int, Dictionary<int, StageData>>();
    private Dictionary<int, (Sprite, Sprite)> monsterBaseResourceDict = new Dictionary<int, (Sprite, Sprite)>();
    private Dictionary<int, (Sprite, Sprite)> stageBackgroundDict = new Dictionary<int, (Sprite, Sprite)>();

    private int checkPointMax = 5;

    private float spawningDurationAfterEnemyBaseDestroyed;
    private int monsterKillGold;
    private int goldMultiply;
    private int goldByEnemyBaseHealth;

    private const string DEFAULT_MONSTER_BASE_PATH = "Monsters/MonsterBase/";
    private const string DEFAULT_BACKGROUND_SPRITE = "Sprites/Background/";

    #region Initialize
    public void Init()
    {
        stageData = Resources.Load<GameData>($"{Consts.GAME_DATA}/stageData");
        stageFixedData = Resources.Load<GameData>($"{Consts.GAME_DATA}/stageFixedData");

        List<SerializableRow> rows = stageData.GetDataRows();

        stageDataDict.Clear();

        int stageNum = 1;
        string stageName = "";
        int[] monsterIndexses = null;
        MonsterType[] monsterTypes = (MonsterType[])Enum.GetValues(typeof(MonsterType));


        for (int i = 0; i < rows.Count; i++)
        {
            List<string> elements = rows[i].rowData;
            int checkPoint = int.Parse(elements[3]);
            BigInteger enemyBaseHealth = new BigInteger(elements[4]);
            float speed = float.Parse(elements[5]);
            float[] spawningInterval = new float[monsterTypes.Length - 2];
            float secondMonsterInstIntervalMultiplication = float.Parse(elements[12]);
            float thirdMonsterInstIntervalMultiplication = float.Parse(elements[13]);
            float firstWaveDuration = float.Parse(elements[14]);
            float secondWaveDuration = float.Parse(elements[15]);

            for (int j = 0; j < spawningInterval.Length; j++)
            {
                spawningInterval[j] = float.Parse(elements[j + 6]);
            }

            if (i % checkPointMax == 0)
            {
                stageNum = int.Parse(elements[0]);
                stageName = elements[1];
                string[] indexes = elements[2].Split();
                monsterIndexses = new int[indexes.Length];
                for (int j = 0; j < indexes.Length; j++)
                {
                    monsterIndexses[j] = int.Parse(indexes[j]);
                }

                if (!monsterBaseResourceDict.ContainsKey(stageNum))
                {
                    monsterBaseResourceDict.Add(stageNum, (Resources.Load<Sprite>($"{DEFAULT_MONSTER_BASE_PATH}{elements[16]}"), Resources.Load<Sprite>($"{DEFAULT_MONSTER_BASE_PATH}{elements[17]}")));
                }

                if (!stageBackgroundDict.ContainsKey(stageNum))
                {
                    stageBackgroundDict.Add(stageNum, (Resources.Load<Sprite>($"{DEFAULT_BACKGROUND_SPRITE}Stage{stageNum}Load"), Resources.Load<Sprite>($"{DEFAULT_BACKGROUND_SPRITE}Stage{stageNum}Background")));
                }
            }

            StageData stageData = new StageData(stageNum, stageName, monsterIndexses, checkPoint, enemyBaseHealth, speed, spawningInterval, 
            secondMonsterInstIntervalMultiplication, thirdMonsterInstIntervalMultiplication, firstWaveDuration, secondWaveDuration);

            if (!stageDataDict.ContainsKey(stageNum))
            {
                stageDataDict.Add(stageNum, new Dictionary<int, StageData>());
            }

            if (!stageDataDict[stageNum].ContainsKey(checkPoint))
            {
                stageDataDict[stageNum].Add(checkPoint, stageData);
            }

            stageDataDict[stageNum][checkPoint] = stageData;
        }

        spawningDurationAfterEnemyBaseDestroyed = float.Parse(stageFixedData.GetDataRows()[0].rowData[0]);
        monsterKillGold = int.Parse(stageFixedData.GetDataRows()[0].rowData[1]);
        goldMultiply = int.Parse(stageFixedData.GetDataRows()[0].rowData[2]);
        goldByEnemyBaseHealth = int.Parse(stageFixedData.GetDataRows()[0].rowData[3]);
    }
    #endregion

    #region StageData
    public StageData GetStageData(int stageNum, int checkpointNum)
    {
        if (stageDataDict.ContainsKey(stageNum))
        {
            if (stageDataDict[stageNum].ContainsKey(checkpointNum))
            {
                return stageDataDict[stageNum][checkpointNum];
            }
        }

        return default;
    }

    public (Sprite, Sprite) GetMonsterBasementSprites(int stageNum)
    {
        if (monsterBaseResourceDict.ContainsKey(stageNum))
        {
            return monsterBaseResourceDict[stageNum];
        }

        return default;
    }

    public float GetSpawningDurationAfterEnemyBaseDestroyed()
    {
        return spawningDurationAfterEnemyBaseDestroyed;
    }

    public int GetMonsterKillGold()
    {
        return monsterKillGold;
    }

    public int GetStageClearGoldMultiply()
    {
        return goldMultiply;
    }

    public int GetGoldByEnemyBaseHealth()
    {
        return goldByEnemyBaseHealth;
    }

    public int GetCheckPointMax()
    {
        return checkPointMax;
    }

    public (Sprite, Sprite) GetBackgroundSprites(int mainstageNum)
    {
        if (stageBackgroundDict.ContainsKey(mainstageNum))
        {
            return stageBackgroundDict[mainstageNum];
        }

        return default;
    }

    public int GetTotalMainStageNum()
    {
        return stageDataDict.Count;
    }
    #endregion
}
