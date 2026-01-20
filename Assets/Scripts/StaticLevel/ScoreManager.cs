using System.IO;
using System.Linq;
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
    public static ScoreManager Instance { get; private set; }

    /// <summary>
    /// 全プレイヤーのランキングデータを保持するリスト。
    /// JSON に保存され、起動時に読み込まれる永続データの実体となる。
    /// </summary>
    public RankData rankData = new RankData();
    /// <summary>
    /// ランキングデータを保存する JSON ファイルのフルパス。
    /// Application.persistentDataPath を基に生成される。
    /// </summary>
    private string rankPath;

    private void Awake()
    {
        if (Instance == null) Instance = this;
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
            rankData = new RankData();  //新規 RankList を作成
            SaveRank();                 //保存する
        }
    }

    public void ResetRank()
    {
        File.Delete(rankPath);  // 既存のランキング保存ファイルを削除
        RankInit();             // 初期化処理を再実行し、空のランキングデータを生成して保存
    }

    public void DeleteRank()
    {
        File.Delete(rankPath);  // 既存のランキング保存ファイルを削除
    }

    /// <summary>
    /// 現在のランキングデータ（rankList）を JSON 形式に変換し、
    /// 永続データとしてファイルに保存する。
    /// プレイヤースコア更新後や新規データ作成時に呼び出される。
    /// </summary>
    void SaveRank()
    {
        // RankList を JSON 文字列へ変換（true で読みやすい整形付き）
        string json = JsonUtility.ToJson(rankData, true);
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
            rankData = JsonUtility.FromJson<RankData>(json);
            Debug.Log("読み込み成功");
        }
        else　　// ファイルが存在しない → 新規 RankList を作成
        {
            Debug.Log("新規 RankList を作成");
            rankData = new RankData();
        }
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
        var stageScore = rankData.scoreData.Find(s => s.level == level && s.stage == stage);
        // スコアが未登録なら 0 を返す
        if (stageScore == null) return 0;
        // 正常 → スコアを返す
        return stageScore.score;
    }
}
