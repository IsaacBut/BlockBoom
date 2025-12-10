using System.IO;
using System.Linq;
using Data;
using UnityEngine;

/// <summary>
/// ランキングデータの保存・読み込み・管理を担当するマネージャークラス。
/// ゲーム終了後のスコア集計、プレイヤー毎の記録管理、
/// JSON 保存を通して永続データとして保持する役割を持つ。
/// </summary>
public class ScoreManager : MonoBehaviour
{
    /// <summary>
    /// ScoreManager のシングルトンインスタンス。
    /// </summary>
    public static ScoreManager instance { get; private set; }
    /// <summary>
    /// 現在スコアを記録・更新しているプレイヤー名。
    /// AddRank() や GetRank() など、スコア操作時の対象プレイヤーとして使用される。
    /// </summary>
    public string nowPlayer {  get; set; }
    /// <summary>
    /// 全プレイヤーのランキングデータを保持するリスト。
    /// JSON に保存され、起動時に読み込まれる永続データの実体となる。
    /// </summary>
    public RankList rankList = new RankList();
    /// <summary>
    /// ランキングデータを保存する JSON ファイルのフルパス。
    /// Application.persistentDataPath を基に生成される。
    /// </summary>
    private string rankPath;
    /// <summary>
    /// ランキング画面で表示する最大プレイヤー数。
    /// 上位 MaxRank 名のみをランキングとして表示するための制限値。
    /// </summary>
    private const int MaxRank = 5;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;            // シングルトン初期化
            DontDestroyOnLoad(this);
        }
        else Destroy(this);
    }

    /// <summary>
    /// ランキングデータの初期化を行う。
    /// 保存先の JSON ファイルが存在する場合は読み込み、
    /// 存在しない場合は新規 RankList を作成して保存する。
    /// ゲーム起動時に一度だけ呼び出され、永続データの準備を行う。
    /// </summary>
    public void RankInit()
    {
        // ランクデータを保存する JSON ファイルのフルパスを生成
        rankPath = Path.Combine(Application.persistentDataPath, "BlockBoom_Rank.json");

        // JSON ファイルが存在するかどうかを確認
        if (File.Exists(rankPath))
        {
            LoadRank();     // 既存データがある → 読み込む
        }
        else　　// データがない → 新規 RankList を作成し、初期保存する
        {
            rankList = new RankList();  //新規 RankList を作成
            SaveRank();                 //保存する
        }
    }

    public void ResetRank()
    {
        File.Delete(rankPath);  // 既存のランキング保存ファイルを削除
        RankInit();             // 初期化処理を再実行し、空のランキングデータを生成して保存
    }

    /// <summary>
    /// 現在のランキングデータ（rankList）を JSON 形式に変換し、
    /// 永続データとしてファイルに保存する。
    /// プレイヤースコア更新後や新規データ作成時に呼び出される。
    /// </summary>
    void SaveRank()
    {
        // RankList を JSON 文字列へ変換（true で読みやすい整形付き）
        string json = JsonUtility.ToJson(rankList, true);
        // JSON テキストをファイルとして書き込み保存
        File.WriteAllText(rankPath, json);
        Debug.LogWarning("保存成功: " + rankPath);
    }

    /// <summary>
    /// 保存されているランキングデータ（JSON ファイル）を読み込み、
    /// RankList オブジェクトとして復元する。
    /// ファイルが存在しない場合は新規データを作成する。
    /// </summary>
    void LoadRank()
    {
        // ランキングデータの JSON ファイルが存在するかチェック
        if (File.Exists(rankPath))
        {
            // JSON ファイルを読み込み
            string json = File.ReadAllText(rankPath);
            // JSON → RankList オブジェクトへ復元
            rankList = JsonUtility.FromJson<RankList>(json);
            Debug.Log("読み込み成功");
        }
        else　　// ファイルが存在しない → 新規 RankList を作成
        {
            Debug.Log("新規 RankList を作成");
            rankList = new RankList();
        }
    }

    /// <summary>
    /// 指定したプレイヤー名に一致する RankData を検索して返す。
    /// ランキングリスト内を走査し、名前が一致するプレイヤーを探す。
    /// 見つからない場合は null を返し、警告ログを出力する。
    /// </summary>
    /// <param name="name">検索したいプレイヤー名</param>
    /// <returns>一致する RankData。存在しない場合は null。</returns>
    RankData TargetRankData(string name)
    {
        // ランキングリスト内の各 RankData を順にチェック
        foreach (var rankData in rankList.ranks)
        {
            // プレイヤー名が一致したらそのデータを返す
            if (rankData.playerName == name) return rankData;
        }

        Debug.LogWarning("該当プレイヤーがいなかった");
        return null;
    }

    /// <summary>
    /// 指定したプレイヤー名のランキングデータを初期化する。
    /// すでにデータが存在する場合は何もせず、
    /// 新規プレイヤーの場合のみ RankData を作成して保存する。
    /// ゲーム開始時や名前入力後に呼び出される処理。
    /// </summary>
    /// <param name="name">初期化するプレイヤー名</param>
    public void PlayerInit(string name)
    {
        // 現在操作対象のプレイヤー名を更新
        nowPlayer = name;
        // 既にこの名前の RankData が存在するなら何もしない
        if (TargetRankData(name) != null) return;
        // 新規プレイヤー → RankData を作成
        RankData rankData = new RankData();
        rankData.playerName = name;
        // ランキングリストに追加
        rankList.ranks.Add(rankData);
        // 変更内容を JSON に保存
        SaveRank();
    }

    /// <summary>
    /// 指定したレベル＆ステージのスコアを現在のプレイヤーに追加する。
    /// ・プレイヤーデータが存在しない場合は自動的に新規作成
    /// ・同じステージのデータがある場合はスコアを「高い方」で更新
    /// ・更新後は JSON へ保存する
    /// </summary>
    /// <param name="level">ステージのレベル番号</param>
    /// <param name="stage">ステージ番号</param>
    /// <param name="score">今回達成したスコア</param>
    public void AddRank(int level, int stage, int score)
    {
        // 現在のプレイヤーの RankData を取得
        var rankData = TargetRankData(nowPlayer);

        // プレイヤーデータが存在しない場合 → 自動作成
        if (rankData == null)
        {
            Debug.LogWarning($"{nowPlayer} データが存在しないから、作成する");
            // 新規プレイヤーとして初期化
            PlayerInit(nowPlayer);
            // 再取得（今度は必ず存在する）
            rankData = TargetRankData(nowPlayer);
        }
        // レベル＆ステージが一致する ScoreData を検索
        var stageScore = rankData.scoreData.Find(s => s.level == level && s.stage == stage);

        // データが未登録 → 新規スコアとして追加
        if (stageScore == null)
        {
            stageScore = new ScoreData { level = level, stage = stage, score = score };
            rankData.scoreData.Add(stageScore);
        }
        // すでにスコアがある → 今回のスコアが高ければ更新
        else { stageScore.score = Mathf.Max(stageScore.score, score); }

        // 更新したランキングデータを保存
        SaveRank();
    }

    /// <summary>
    /// 現在のプレイヤーが指定したレベル・ステージで記録したスコアを取得する。
    /// ・プレイヤーデータが存在しない場合は 0 を返す
    /// ・該当ステージのスコアが存在しない場合も 0 を返す
    /// </summary>
    /// <param name="level">取得したいスコアのレベル番号</param>
    /// <param name="stage">取得したいスコアのステージ番号</param>
    /// <returns>該当ステージのスコア。存在しない場合は 0。</returns>
    public int GetRank(int level, int stage)
    {
        // 現在のプレイヤーの RankData を取得
        var rankData = TargetRankData(nowPlayer);
        // プレイヤーデータが存在しない場合 → スコアなし扱い
        if (rankData == null) return 0;
        // 指定されたレベル＆ステージのスコアデータを検索  
        var stageScore = rankData.scoreData.Find(s => s.level == level && s.stage == stage);
        // スコアが未登録なら 0 を返す
        if (stageScore == null) return 0;

        // 正常 → スコアを返す
        return stageScore.score;
    }

    /// <summary>
    /// ランキング表示用にプレイヤーリストを並び替え、
    /// 上位 MaxRank 名のみを残すための初期化処理。
    /// ・全プレイヤーの総合スコアを計算して降順でソート
    /// ・MaxRank を超える分はリストから削除
    /// </summary>
    public void RankShowInit()
    {
        // 各プレイヤーの総合スコア（scoreData の合計）を計算し、
        // スコアが高い順（降順）に並び替える
        rankList.ranks.Sort((a, b) => b.scoreData.Sum(s => s.score).CompareTo(a.scoreData.Sum(s => s.score)));

        // 表示する最大人数（MaxRank）を超える場合、上位 MaxRank 名のみ残す
        if (rankList.ranks.Count > MaxRank) rankList.ranks.RemoveRange(MaxRank, rankList.ranks.Count - MaxRank);
    }

    /// <summary>
    /// ランキングに登録されているプレイヤー名の一覧を配列として取得する。
    /// データが存在しない場合は空の配列を返す安全設計になっている。
    /// </summary>
    /// <returns>プレイヤー名の配列。データが無い場合は空配列。</returns>
    public string[] Rank_PlayerName()
    {
        // ランクデータまたはリスト自体が null の場合 → 空配列を返す
        if (rankList == null || rankList.ranks == null) return new string[0];
        // プレイヤー名だけを格納する配列を作成
        string[] rank_PlayerName = new string[rankList.ranks.Count];
        // ランキング順にプレイヤー名をコピー
        for (int i = 0; i < rankList.ranks.Count; i++)
        {
            rank_PlayerName[i] = rankList.ranks[i].playerName;
        }
        // 完成した配列を返す
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
