using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RankDataHandler
{
    private Dictionary<Rank, Sprite> rankBackgroundSpriteDict = new Dictionary<Rank, Sprite>();
    private Dictionary<Rank, Color> rankColorDict = new Dictionary<Rank, Color>();
    private Dictionary<int, Proportion> proportionDict = new Dictionary<int, Proportion>();

    private Sprite[] colorSpritesForText;

    private Rank[] ranks;

    private Rank[] rankProbabilityArray = new Rank[100000];

    private GameData rankProbabilityData;

    private int probabilityMaxlevel;


    public Sprite GetRandomRankBackgroundSprite()
    {
        return GetRankBackgroundSprite(GetRandomRank());
    }

    public Rank GetRandomRank()
    {
        int index = UnityEngine.Random.Range(0, rankProbabilityArray.Length);
        return rankProbabilityArray[index];
    }

    public Sprite GetRankBackgroundSprite(Rank rank)
    {
        if (rankBackgroundSpriteDict.ContainsKey(rank))
        {
            return rankBackgroundSpriteDict[rank];
        }

        return default;
    }

    public Color GetRankColor(Rank rank)
    {
        if (rankColorDict.ContainsKey(rank))
        {
            return rankColorDict[rank];
        }

        return default;
    }

    public Rank[] GetRanks()
    {
        return ranks;
    }

    public void Init()
    {
        ranks = (Rank[])Enum.GetValues(typeof(Rank));
        for (int i = 1; i < ranks.Length; i++)
        {
            Rank rank = ranks[i];
            Sprite sprite = Resources.Load<Sprite>($"Sprites/Ranks/{rank}");
            if (!rankBackgroundSpriteDict.ContainsKey(rank))
            {
                rankBackgroundSpriteDict.Add(rank, sprite);
            }

            Color color = sprite.texture.GetPixel(
                sprite.texture.width / 2, sprite.texture.height / 2);
            rankColorDict.Add(rank, color);
        }

        rankProbabilityData = Resources.Load<GameData>($"{Consts.GAME_DATA}/EquipmentProbabilityData");

        proportionDict.Clear();
        List<SerializableRow> rows = rankProbabilityData.GetDataRows();

        for (int i = 0; i < rows.Count; i++)
        {
            List<string> elements = rows[i].rowData;

            int level = int.Parse(elements[0]);

            int[] rankProbabilities = new int[Enum.GetValues(typeof(Rank)).Length - 1];

            for (int j = 0; j < rankProbabilities.Length && j < elements.Count - 1; j++)
            {
                if (j == rankProbabilities.Length - 1)
                {
                    rankProbabilities[j] = (int)(decimal.Parse(elements[j + 1].Trim('\r')) * Consts.PERCENT_UNIT_EQUIPMENT_VALUE);
                }
                else
                {
                    rankProbabilities[j] = (int)(decimal.Parse(elements[j + 1]) * Consts.PERCENT_UNIT_EQUIPMENT_VALUE);
                }
            }

            if (!proportionDict.ContainsKey(level))
            {
                proportionDict.Add(level, new Proportion(level, rankProbabilities));
            }
        }

        probabilityMaxlevel = proportionDict.Count;
    }

    public void SetRanks(int level)
    {
        int[] proportion = GetCurrentProportion(level);

        int count = 0;
        for (int i = 0; i < proportion.Length; i++)
        {
            int repetition = proportion[i];
            for (int j = 0; j < repetition; j++)
            {
                rankProbabilityArray[count] = (Rank)(i + 1);
                count++;
            }
        }
    }

    private int[] GetProbabillitiesOfLevel(int level)
    {
        return proportionDict[level].proportionArray;
    }

    public int[] GetCurrentProportion(int level)
    {
        int[] proportion = GetProbabillitiesOfLevel(level);

#if UNITY_EDITOR
        #region Assertion
        Debug.Assert(proportion != null, "Proportion of current level does not exist.");

        int sum = 0;
        foreach (int num in proportion)
        {
            sum += num;
        }
        Debug.Assert(sum == 100000, "Elements of the proportion does not sum up 1000.");
        #endregion
#endif
        return proportion;
    }

    public int GetProbabilityMaxLevel()
    {
        return probabilityMaxlevel;
    }
}