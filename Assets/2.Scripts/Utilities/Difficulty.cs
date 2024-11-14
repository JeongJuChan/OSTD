using System.Collections.Generic;

public class Difficulty
{
    static Dictionary<int, string> dic = new Dictionary<int, string>()
    {
        {1, "쉬움"},
        {2, "보통"},
        {3, "어려움"},
        {4, "매우 어려움"},
        {5, "악몽"},
        {6, "지옥"},
        {7, "볼지옥"},
        {8, "혼돈"},
        {9, "불가능한"},
        {10, "정예"},
        {11, "미친듯한"},
        {12, "무자비한"},
        {13, "알수없는"},
        {14, "잔혹한"},
        {15, "지독한"},
        {16, "무시무시한"},
        {17, "교활한"},
        {18, "불멸"},
        {19, "끝나지 않는"},
        {20, "심연"},
        {21, "사악한"},
        {22, "황량한"},
        {23, "야만적인"},
        {24, "섬뜩한"},
        {25, "불길한"},
        {26, "고문받는"},
        {27, "파멸"},
        {28, "재앙"},
        {29, "깨지지 않는"}
    };

    public static string GetDifficulty(int difficulty)
    {
        return dic[difficulty];
    }

    public static string TransformStageNumber(int stageNum)
    {
        string s = stageNum.ToString();

        string P = s.Length > 3 ? s.Substring(0, s.Length - 3) : "";
        string X = s.Length > 2 ? s.Substring(s.Length - 3, 1) : "";
        string YZ = s.Length > 1 ? s.Substring(s.Length - 2) : s;

        return $"{GetDifficulty(int.Parse(P))} {X}-{int.Parse(YZ)}";
    }
}
