using Data;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ゲーム全体の進行管理を行うメインマネージャー。
/// ・UI の初期化
/// ・ゲームデータのセットアップ
/// ・シーン遷移（ゲーム開始 → GameTitle）
/// ・スコア管理との連携
/// など、ゲーム全体の基盤となるクラス。
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// GameManager のシングルトンインスタンス。
    /// </summary>
    public static GameManager instance { get; private set; }

    /// <summary>UIManager シングルトンへの参照</summary>
    private UIManager uiManager => UIManager.instance;
    /// <summary>UI データ（UI サイズ、ポジションなど）へアクセスするためのエイリアス</summary>
    public UI _ui => uiManager.ui;
    /// <summary>ScoreManager シングルトンへの参照</summary>
    private ScoreManager scoreManager => ScoreManager.instance;
    /// <summary>現在 UI に設定された画面幅</summary>
    public int screenWidth => uiManager.screenSize.x;
    /// <summary>現在 UI に設定された画面高さ</summary>
    public int screenHeight => uiManager.screenSize.y;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;                // シングルトン初期化
            DontDestroyOnLoad(this);        // シーンが変わっても破棄しない
        }
        else Destroy(this);                 // 2個目が生成された場合は破棄



    }
    private void Start()
    {
        Application.targetFrameRate = 120;
        uiManager.UIInit();                 // UI の初期化
        GameData.GameBasicDataInit();       // 基本ゲームデータ読み込み（CSVから）
        level = 1;                          // 初期レベル/ステージ
        stage = 1;
        scoreManager.RankInit();            // ランキングデータの初期化
    }

    /// <summary>
    /// ゲーム開始フラグ。
    /// GameEnd UI が準備完了したら GameTitle に遷移する。
    /// </summary>
    bool gameStart = false;

    void Update()
    {
        // GameEnd Screen が初期化済み & まだシーン遷移していない
        if (uiManager.IsGameEndInit() && !gameStart)
        {
            //if (Input.GetKeyDown(KeyCode.Space))
            //{
            //    SceneManager.LoadScene("GameTitle", LoadSceneMode.Single);
            //    gameStart = true;
            //}
            SceneManager.LoadScene("GameTitle", LoadSceneMode.Single);
            gameStart = true;
            //ebug.Log(CSVReader.instance.ReadTargetCellIndex("CSVes/Level_00/Stage_00", "O", 17));
            //gameStart = true;
        }
        else if (!uiManager.IsGameEndInit())    // GameEnd 画面でないとき → ゲーム画面の UI 初期化
        {
            uiManager.GameInit();
        }

    }

    /// <summary>現在のゲームレベル</summary>
    public int level { get; set; }
    /// <summary>現在のステージ番号</summary>
    public int stage { get; set; }
    /// <summary>入力されたプレイヤー名</summary>
    public static string playerName { get; set; }
    /// <summary>
    /// ゲームプレイ結果から保持された点数。
    /// スコア画面引き継ぎなどに利用。
    /// </summary>
    public int pastScoreFromInGame { get; set; }
    /// <summary>
    /// 新しいプレイヤーデータを初期化する。
    /// スコア管理側にプレイヤー情報を渡す。
    /// </summary>
    /// <param name="name">登録するプレイヤー名</param>
    public void PlayerInit(string name) => scoreManager.PlayerInit(name);


}
