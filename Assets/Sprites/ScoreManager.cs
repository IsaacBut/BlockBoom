using System.Collections.Generic;
using System.IO;
using Data;
using NUnit.Framework;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance { get; private set; }
    public string nowPlayer {  get; set; }

    public RankList rankList = new RankList();
    //public StageBestScoreList stageBestScoreList = new StageBestScoreList();

    private string rankPath;
    private const int MaxRank = 5;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else Destroy(this);
    }
    public void RankInit()
    {
        rankPath = Path.Combine(Application.persistentDataPath, "BlockBoom_Rank.json");
        if (File.Exists(rankPath))
        {
            LoadRank();
        }
        else
        {
            rankList = new RankList();
            SaveRank();
        }
    }

    public void ResetRank()
    {
        File.Delete(rankPath);
        RankInit();
    }
    void SaveRank()
    {
        string json = JsonUtility.ToJson(rankList, true);
        File.WriteAllText(rankPath, json);
        Debug.LogWarning("保存成功: " + rankPath);
    }

    void LoadRank()
    {
        if (File.Exists(rankPath))
        {
            string json = File.ReadAllText(rankPath);
            rankList = JsonUtility.FromJson<RankList>(json);
            Debug.Log("读取成功");
        }
        else
        {
            Debug.Log("没有找到存档，创建新文件");
            rankList = new RankList();
        }
    }

    RankData TargetRankData(string name)
    {
        foreach (var rankData in rankList.ranks)
        {
            if (rankData.playerName == name) return rankData;
        }

        Debug.LogWarning("NO Player Data");
        return null;
    }

    public void PlayerInit(string name)
    {
        nowPlayer = name;
        if (TargetRankData(name) != null) return;
        RankData rankData = new RankData();
        rankData.playerName = name;
        rankList.ranks.Add(rankData);
        SaveRank();
    }

    public void AddRank(string name, int level, int score)
    {
        var rankData = TargetRankData(name);
        if (rankData == null)
        {
            Debug.LogWarning($"Player {name} not found, auto creating new record.");
            PlayerInit(name);
            rankData = TargetRankData(name);
        }

        switch (level)
        {
            case 1: rankData.score_Level01 = score; break;
            case 2: rankData.score_Level02 = score; break;
            case 3: rankData.score_Level03 = score; break;
            default: Debug.LogWarning("Wrong Level"); break;
        }

        SaveRank();
    }

    public void RankShowInit()
    {
        rankList.ranks.Sort((a, b) => (b.score_Level01 + b.score_Level02 + b.score_Level03).CompareTo((a.score_Level01 + a.score_Level02 + a.score_Level03)));
        if (rankList.ranks.Count > MaxRank) rankList.ranks.RemoveRange(MaxRank, rankList.ranks.Count - MaxRank);

    }

    public string[] Rank_PlayerName()
    {
        if (rankList == null || rankList.ranks == null) return new string[0];

        string[] rank_PlayerName = new string[rankList.ranks.Count];
        for (int i = 0; i < rankList.ranks.Count; i++)
        {
            rank_PlayerName[i] = rankList.ranks[i].playerName;
        }
        return rank_PlayerName;
    }

    public int[] Rank_TotalScore()
    {
        if (rankList == null || rankList.ranks == null) return new int[0];

        int[] rank_TotalScore = new int[rankList.ranks.Count];
        for (int i = 0; i < rankList.ranks.Count; i++)
        {
            rank_TotalScore[i] = rankList.ranks[i].score_Level01 + rankList.ranks[i].score_Level02 + rankList.ranks[i].score_Level03;
        }
        return rank_TotalScore;
    }

}
