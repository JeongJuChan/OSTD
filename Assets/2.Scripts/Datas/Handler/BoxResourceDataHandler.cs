using System.Collections;
using System.Collections.Generic;
using Keiwando.BigInteger;
using UnityEngine;

public class BoxResourceDataHandler
{
    private GameData boxData;
    private GameData boxFixedData;

    private Dictionary<int, BoxData> boxCostDict = new Dictionary<int, BoxData>();
    private Dictionary<int, Sprite> boxSpriteDict = new Dictionary<int, Sprite>();

    private int boxCost;
    private int boxMaxCount;

    #region Initialize
    public void Init()
    {
        boxData = Resources.Load<GameData>($"{Consts.GAME_DATA}/BoxData");
        boxFixedData = Resources.Load<GameData>($"{Consts.GAME_DATA}/BoxFixedData");

        List<SerializableRow> rows = boxData.GetDataRows();

        boxCostDict.Clear();

        for (int i = 0; i < rows.Count; i++)
        {
            List<string> elements = rows[i].rowData;
            BoxData boxData = new BoxData(int.Parse(elements[0]), new BigInteger(elements[1]), new BigInteger(elements[2]));

            int boxLevel = boxData.level;

            if (!boxCostDict.ContainsKey(boxLevel))
            {
                boxCostDict.Add(boxLevel, boxData);
            }

            if (!boxSpriteDict.ContainsKey(boxLevel))
            {
                Sprite sprite = Resources.Load<Sprite>($"Sprites/Boxes/BoxLevel_{boxLevel}");
                boxSpriteDict.Add(boxLevel, sprite);
            }
        }

        boxCost = int.Parse(boxFixedData.GetDataRows()[0].rowData[0]);
        boxMaxCount = int.Parse(boxFixedData.GetDataRows()[0].rowData[1]);

        
    }
    #endregion

    #region BoxData
    public BoxData GetBoxData(int level)
    {
        Debug.Assert(boxCostDict.ContainsKey(level), "None Matched BoxData By Level");

        if (boxCostDict.ContainsKey(level))
        {
            return boxCostDict[level];
        }
        
        return default;
    }

    public Sprite GetBoxSprite(int level)
    {
        if (boxSpriteDict.ContainsKey(level))
        {
            return boxSpriteDict[level];
        }

        return default;
    }

    public int GetNewBoxCost()
    {
        return boxCost;
    }

    public int GetBoxMaxCount()
    {
        return boxMaxCount;
    }
    #endregion
}
