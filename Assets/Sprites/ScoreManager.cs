using System.Collections.Generic;
using System.IO;
using System.Linq;
using Data;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

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

    public void AddRank(int level, int stage, int score)
    {
        var rankData = TargetRankData(nowPlayer);
        if (rankData == null)
        {
            Debug.LogWarning($"Player {nowPlayer} not found, auto creating new record.");
            PlayerInit(nowPlayer);
            rankData = TargetRankData(nowPlayer);
        }
        var stageScore = rankData.scoreData.Find(s => s.level == level && s.stage == stage);
        if (stageScore == null)
        {
            stageScore = new ScoreData { level = level, stage = stage, score = score };
            rankData.scoreData.Add(stageScore);
        }
        else { stageScore.score = Mathf.Max(stageScore.score, score); }

            SaveRank();
    }

    public int GetRank(int level, int stage)
    {
        var rankData = TargetRankData(nowPlayer);
        if (rankData == null) return 0;
        var stageScore = rankData.scoreData.Find(s => s.level == level && s.stage == stage);
        if (stageScore == null) return 0;

        return stageScore.score;
    }

    public void RankShowInit()
    {
        rankList.ranks.Sort((a, b) => b.scoreData.Sum(s => s.score).CompareTo(a.scoreData.Sum(s => s.score)));

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
        if (rankList == null || rankList.ranks == null)
            return new int[0];

        int[] rank_TotalScore = new int[rankList.ranks.Count];

        for (int i = 0; i < rankList.ranks.Count; i++)
        {
            rank_TotalScore[i] = rankList.ranks[i].scoreData.Sum(s => s.score);
        }

        return rank_TotalScore;
    }

}
