using UnityEngine;
using Data;
using NUnit.Framework;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.UIElements;

public class Manager : MonoBehaviour
{
    static public Manager instance { get; private set; }
    private UI _ui => GameManager.instance._ui;

    const string ready_Bgm = "Ready_Bgm";
    const string inGame_Bgm = "InGame_Bgm";

    const int pointPlus = 100;
    [SerializeField] TMP_Text bulletAmount;
    [SerializeField] TMP_Text higestScore;
    [SerializeField] TMP_Text score;


    #region Little Function

    public string FindObjectTag(GameObject targetGameObject)
    {
        return targetGameObject.tag;
    }

    public void ScorePlus()
    {
        nowPoint += pointPlus;
    }

    void PrintPlayerInform()
    {
        score.text = nowPoint.ToString("D5");
        bulletAmount.text = Player.instance.nowBulletAmount.ToString("D2");
    }

    #endregion

    #region Play Area & UI
    public RectTransform pauseCanvas;
    public RectTransform backGround;
    public RectTransform playArea;
    public RectTransform playInformArea;
    public RectTransform playerArea;

    public List<Vector2> areaLimit;

    void PlayArea()
    {
        //// 设置 Image 大小
        //playInformArea.sizeDelta = _ui.playerInformArea;
        playArea.sizeDelta = _ui.playArea;
        playerArea.sizeDelta = _ui.playerArea;

        //playInformArea.pivot = new Vector2(playInformArea.pivot.x, 1f);
        playArea.pivot = new Vector2(playArea.pivot.x, 1f);
        playerArea.pivot = new Vector2(playerArea.pivot.x, 1f);

        //playInformArea.localPosition = _ui.playInformAreaPos;
        playArea.localPosition = _ui.playAreaPos;
        playerArea.localPosition = _ui.playerAreaPos;

        RectTransform rt = playerArea;

        // 获取 playerArea 的底边世界坐标
        Vector3 bottomWorldPos = rt.TransformPoint(new Vector3(0, rt.rect.yMin, 0));

        var player = Player.instance.gameObject;
        var col = player.GetComponent<BoxCollider2D>();

        // 要让玩家正好贴在底边
        float posY = bottomWorldPos.y + col.bounds.extents.y;

        player.transform.position = new Vector2(player.transform.position.x, posY);

        //Vector3 playerAreaCenter = playerArea.TransformPoint(playerArea.rect.center);
        //float playerPosY = playerAreaCenter.y - (Player.instance.gameObject.GetComponent<BoxCollider2D>().bounds.extents.y / 2);
        //Player.instance.gameObject.transform.position = new Vector2(Player.instance.gameObject.transform.position.x, playerPosY);

    }


    #endregion

    #region Pause

    bool isPause = false;

    void PauseCanvasInit()
    {
        pauseCanvas.sizeDelta = UIManager.instance.screenSize;
        pauseCanvas.position = new Vector3(0, 0, -90);
        pauseCanvas.gameObject.SetActive(false);
    }

    void IsPause()
    {
        if (isPause)
        {
            pauseCanvas.gameObject.SetActive(true);
            AudioManager.instance.PauseMusic("InGame_Bgm");
            Time.timeScale = 0;
        }
        else
        {
            pauseCanvas.gameObject.SetActive(false);
            AudioManager.instance.UnPauseMusic("InGame_Bgm");
            Time.timeScale = 1;

        }
    }

    public void Button_Pause()
    {
        isPause = !isPause;
        IsPause();
    }
    public void Button_ReSume() 
    { 
        isPause = false;
        IsPause();
    }
    public void Button_ReTry()
    {
        SceneManager.LoadScene("InGame", LoadSceneMode.Single);
    }
    public void Button_ReTrie()
    {
        SceneManager.LoadScene("LevelSelect", LoadSceneMode.Single);
    }
    public void Button_Title()
    {
        SceneManager.LoadScene("GameTitle", LoadSceneMode.Single);
    }

    void OnApplicationFocus(bool focused)
    {
        if (focused)
        {
            // 玩家回到游戏
            isPause = false;
        }
        else
        {
            // 玩家切出去 / Alt+Tab
            isPause = true;
        }

        IsPause();
    }
    #endregion

    #region BlockGenerate
    int wallCount;
    public bool isWallBroke = false;

    public bool isMapBuild = false;

    public List<Wall> wallGameObjectList;
    List<Boom> boomGameObjectList;
    List<WallBoom> wallBoomGameObjectList;
    List<GameObject> canBrakeGameObject;

    void GenerateMap()
    {
        GameObject parent = Instantiate(new GameObject(), GameData.playAreaPosition, Quaternion.identity, transform);

        areaLimit = new List<Vector2>();
        boomGameObjectList = new List<Boom>();
        canBrakeGameObject = new List<GameObject>();
        wallBoomGameObjectList = new List<WallBoom>();


        int row = (int)CSVReader.instance.ReadTargetCellIndex(GameData.levelStage, "B", 3) + 4;
        int col = (int)CSVReader.instance.ReadTargetCellIndex(GameData.levelStage, "B", 4) + 4;

        for (int x = 4; x < row; x++)
        {
            if (x - 4 > GameData.rowNumber) return;

            for (int y = 4; y < col; y++)
            {
                if (y - 4 > GameData.colNumber) continue;

                float posX = GameData.Width(x - 4);
                float posY = GameData.Height(y - 4);


                if (x == 4 || x == row - 1 || y == 4 || y == col - 1)
                {
                    areaLimit.Add(new Vector2(posX, posY));
                }

                string targetCode = CSVReader.instance.ReadTargetCellString(GameData.levelStage, CSVReader.instance.IndexToColumnName(x), y);

                if (targetCode.Length >= 1)
                {

                    GameObject target = Instantiate(GameData.TargetGameObject(targetCode), Vector3.zero, Quaternion.identity);
                    //Debug.Log(target.name);

                    target.transform.localScale = new Vector3(GameData.blockSize, GameData.blockSize, 1);
                    target.transform.position = new Vector3(posX, posY, 0);

                    if (targetCode == "N")
                    {
                        canBrakeGameObject.Add(target);
                        target.name = $"X={x - 4},Y={y - 4}, Block";
                    }
                    else if (targetCode == "B")
                    {
                        Boom targetBoom = target.GetComponent<Boom>();
                        if (targetBoom != null) { boomGameObjectList.Add(targetBoom); }
                    }
                    else if (targetCode == "W")
                    {
                        Wall targetWall = target.GetComponent<Wall>();
                        if (targetWall != null)
                        {
                            targetWall.SetWallType(Wall.WallType.NonBreakAble);
                            wallGameObjectList.Add(targetWall);
                        }
                        target.name = $"X={x - 4},Y={y - 4}, Wall";
                    }
                    else if (targetCode == "CBW")
                    {
                        Wall targetWall = target.GetComponent<Wall>();
                        if (targetWall != null)
                        {
                            targetWall.SetWallType(Wall.WallType.BreakAble);
                            wallGameObjectList.Add(targetWall);
                        }

                        target.name = $"X={x - 4},Y={y - 4}, Wall";
                    }
                    else if (targetCode == "WB")
                    {
                        WallBoom targetWallBoom = target.GetComponent<WallBoom>();
                        wallBoomGameObjectList.Add(targetWallBoom);

                        canBrakeGameObject.Add(target);
                        target.name = $"X={x - 4},Y={y - 4}, Wall";
                    }

                    target.transform.SetParent(parent.transform);

                }
            }


        }
        isMapBuild = true;
        wallCount = wallGameObjectList.Count;

    }
    IEnumerator WallInit()
    {
        bool wallInit = false;

        yield return null; 
        for (int i = 0; i < wallGameObjectList.Count; i++)
        {
            wallGameObjectList[i].FindFourPoint();
            wallInit = true;
        }
        yield return new WaitForSeconds(0.2f);

        if (wallInit)
        {
            BoomInit();
        }

    }
    void BoomInit()
    {
        if (boomGameObjectList.Count > 0)
        {
            for (int i = 0; i < boomGameObjectList.Count; i++)
            {
                boomGameObjectList[i].FindFlameCanBrakeBlock();
            }
        }

        if (wallBoomGameObjectList.Count > 0)
        {
            for (int i = 0; i < wallBoomGameObjectList.Count; i++)
            {
                wallBoomGameObjectList[i].FindConnectingWall();
            }
        }

    }
    #endregion

    #region Music
    private float timer;
    private float gameMusicLenght;

    [SerializeField] TMP_Text musicTimer;

    private int timer_M => (int)(timer / 60);
    private int timer_S => (int)(timer % 60);
    private int music_M => (int)(gameMusicLenght / 60);
    private int music_S => (int)(gameMusicLenght % 60);

    private void MusicTimer() => musicTimer.text = $"{timer_M:D2} :{timer_S:D2} / {music_M:D2} : {music_S:D2} ";

    bool isReady;

    private void MusicInit()
    {
        AudioManager.instance.ChangeCrip(inGame_Bgm,CSVReader.instance.ReadTargetCellString(GameData.levelStage, "B", 14));

    }

    private IEnumerator Ready()
    {
        //if (!AudioManager.instance.IsPlaying()) AudioManager.instance.PlayMusic(ready_Bgm, 1);

        //isReady = true;
        yield return new WaitForSeconds(1f);
        AudioManager.instance.PlayMusic(inGame_Bgm,100);
        gameMusicLenght = AudioManager.instance.CripLenght(inGame_Bgm);
        Debug.Log($"{gameMusicLenght}");

        isGameStart = true;
        yield return new WaitForSeconds(0.01f);
        StartCoroutine(Player.instance.MoveThroughPointsLoop());

    }


    #endregion

    #region GameSet

    int nowPoint;

    bool IsAllBlockGone() => canBrakeGameObject.Count == 0;
    bool IsNoBullet() => Player.instance.nowBulletAmount <= 0;

    bool IsMusicDone() => timer >= gameMusicLenght;

    bool IsGameSet()
    {
        return IsAllBlockGone() || IsNoBullet() || IsMusicDone();
    }

    IEnumerator GameSet()
    {
        GameManager.instance.pastScoreFromInGame = nowPoint;
        ScoreManager.instance.AddRank(GameManager.instance.level, GameManager.instance.stage, nowPoint);

        yield return new WaitForSeconds(0.5f);
        AudioManager.instance.EndMusic(inGame_Bgm);
        if (IsAllBlockGone()) SceneManager.LoadScene("GameClear", LoadSceneMode.Single);
        else if (IsNoBullet() && !IsAllBlockGone()) SceneManager.LoadScene("GameOver", LoadSceneMode.Single);
        // else if (IsMusicDone() && !IsAllBlockGone()) SceneManager.LoadScene("GameOver", LoadSceneMode.Single);
        else { SceneManager.LoadScene("GameOver", LoadSceneMode.Single); }
    }




    #endregion



    bool isGameStart = false;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
    }
    public PlayerMoveTest playerMoveTest;
    private void Start()
    {
        PlayArea();

        GameData.GameDataInit(GameManager.instance.level, GameManager.instance.stage);
        PauseCanvasInit();

        GenerateMap();
        StartCoroutine(WallInit());
        nowPoint = 0;

        MusicInit();

        Player.instance.PlayerInit();
        //if (!AudioManager.instance.IsPlaying()) AudioManager.instance.PlayMusic("InGame_Bgm", 1);
        //Test
        //if (!AudioManager.instance.IsPlaying()) AudioManager.instance.PlayMusic("Moonlight", 1);
        playerMoveTest.RoadInit();

        higestScore.text = ScoreManager.instance.GetRank(GameManager.instance.level, GameManager.instance.stage).ToString("D5");
        StartCoroutine(Ready());
    }



    private void Update()
    {
        if (!isGameStart)
        {

        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Button_Pause();
            }
            if (!isPause) timer += Time.deltaTime;

#if DEVELOPMENT_BUILD || UNITY_EDITOR
            MusicTimer();
#endif

            if (isMapBuild)
            {

                wallGameObjectList.RemoveAll(item => item == null);
                wallBoomGameObjectList.RemoveAll(item => item == null);
                boomGameObjectList.RemoveAll(item => item == null);
                canBrakeGameObject.RemoveAll(item => item == null);

                if (wallGameObjectList.Count != wallCount)
                {
                    BoomInit();
                    wallCount = wallGameObjectList.Count;

                }

            }
            if (IsGameSet()) {  StartCoroutine(GameSet()); }
            PrintPlayerInform();
        }
      
    }



}
