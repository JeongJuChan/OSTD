using System;
using System.Collections.Generic;
using Keiwando.BigInteger;
using UnityEngine;

public class MonsterResourceHandler
{
    private Dictionary<int, Monster> monsterResourceDict = new Dictionary<int, Monster>();
    private Dictionary<int, MonsterData> monsterDataDict = new Dictionary<int, MonsterData>();
    private const string MONSTER_PATH = "Characters/Monsters";

    private GameData monsterData;

    public int monsterProjectileInitCount { get; private set; } = 10;
    public int monsterProjectileMaxCount { get; private set; } = 500;
    public int monsterInitCount { get; private set; } = 10;
    public int monsterMaxCount { get; private set; } = 200;

    #region Initialize
    public void Init()
    {
        monsterData = Resources.Load<GameData>($"{Consts.GAME_DATA}/MonsterData");
        List<SerializableRow> rows = monsterData.GetDataRows();

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0f, 0f));
        float monsterOffsetPosX = worldPos.x;

        for (int i = 0; i < rows.Count; i++)
        {
            List<string> elements = rows[i].rowData;
            Monster monster = Resources.Load<Monster>($"Monsters/{elements[1]}");

            int index = int.Parse(elements[0]);

            MonsterData tempData = new MonsterData(index, elements[1], EnumUtility.GetEqualValue<MonsterType>(elements[2]), new BigInteger(elements[3]),
            new BigInteger(elements[4]), float.Parse(elements[5]));

            monster.SetIndex(index);

            if (!monsterResourceDict.ContainsKey(index))
            {
                monsterResourceDict.Add(index, monster);
            }

            if (!monsterDataDict.ContainsKey(index))
            {
                monsterDataDict.Add(index, tempData);
            }
        }
    }
    #endregion
    public Monster GetResource(int index)
    {
        if (monsterResourceDict.ContainsKey(index))
        {
            return monsterResourceDict[index];
        }

        return null;
    }

    public MonsterData GetMonsterData(int index)
    {
        if (monsterDataDict.ContainsKey(index))
        {
            return monsterDataDict[index];
        }

        return default;
    }
    
    
}