using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ScoreData
{
    public int level;
    public int stage;
    public int score;
}

[System.Serializable]
public class RankData
{
    public List<ScoreData> scoreData = new List<ScoreData>();
}
