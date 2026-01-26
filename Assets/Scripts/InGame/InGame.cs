using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;

public class InGame : MonoBehaviour
{
    public static InGame Instance;
    private GameManager gameManager => GameManager.Instance;
    private ScoreManager scoreManager => ScoreManager.Instance;
    public bool isGameStart;

    [Header("Texting")]
    public Test_PlayerMove playerMovePos;

    [Header("Game Timer")]
    public double bpm = 120;

    public double nowTime;
    public double oldTime;
    public double deltaTime;

    public bool isGamePause = false;


    #region Timer
    private void Timer()
    {
        nowTime = AudioSettings.dspTime;
        deltaTime = nowTime - oldTime;
    }

    public float timerLimt = 90.0f;
    private float nowTimer;
    private void PlayweTimer()
    {
        beat.text = nowBeat.ToString("D2");
        if (nowTimer < 0 || isGamePause) return;
        nowTimer -= (float)deltaTime;

        int min = (int)nowTimer / 60;   // 1
        int sec = (int)nowTimer % 60;   // 30

        timer.text = $"{min.ToString("D2")} : {sec.ToString("D2")}";
    }

    private int nowBeat;
    private int beatPenlo = 0;


    public bool CheckOnBeat(Vector3 target)
    {
        float posX = target.x;

        for(int i = 1; i < points.Count-1; i++)
        {
            if (Mathf.Abs(points[i].x - posX) < 0.1) return true;
        }
        return false;
    }

    public void PlusBeat()
    {
        if(nowBeat<25) nowBeat++; beatPenlo = 0;
    }
    public void ReduceBeat()
    {
        beatPenlo++;
        if (beatPenlo != 3) return;
        if (nowBeat > 0) nowBeat--;
        beatPenlo = 0;
    }
    


    #endregion

    #region UI
    [Header("UI")]

    public RectTransform backGround;
    public RectTransform playerInformArea;
    public RectTransform playArea;
    public RectTransform playerArea;
    public RectTransform leftLightArea;
    public RectTransform rightLightArea;

    [Header("Player Inform Area")]
    [SerializeField] private RectTransform buttleImage;
    [SerializeField] private TextMeshProUGUI buttleIndex;
    private RectTransform buttleIndexRT;
    [SerializeField] private RectTransform bestScoreImage;
    [SerializeField] private TextMeshProUGUI bestScoreIndex;
    private RectTransform bestScoreIndexRT;
    [SerializeField] private TextMeshProUGUI nowScoreIndex;
    private RectTransform nowScoreIndexRT;
    [SerializeField] private RectTransform pause;

    public TextMeshProUGUI timer;
    public TextMeshProUGUI beat;


    private UI ui => UIManager.Instance.ui;
    private CSVReader csvReader => CSVReader.Instance;

    private void AreaInit()
    {
        backGround.sizeDelta = ui.canvasSize;
        playerInformArea.sizeDelta = ui.playerInformArea;

        playArea.sizeDelta = ui.playArea;
        playerArea.sizeDelta = ui.playerArea;
        leftLightArea.sizeDelta = ui.lightArea;

        rightLightArea.sizeDelta = ui.lightArea;

        playerInformArea.pivot = new Vector2(playerInformArea.pivot.x, 1f);

        playArea.pivot = new Vector2(playArea.pivot.x, 1f);
        leftLightArea.pivot = new Vector2(playArea.pivot.x, 1f);
        rightLightArea.pivot = new Vector2(playArea.pivot.x, 1f);

        playerInformArea.localPosition = ui.playInformAreaPos;
        playArea.localPosition = ui.playAreaPos;
        leftLightArea.localPosition = ui.leftLightPos;
        rightLightArea.localPosition = ui.rightLightPos;

        playerArea.localPosition = new Vector3(ui.playerAreaPos.x, (ui.playerAreaPos.y - playerArea.sizeDelta.y / 2), ui.playerAreaPos.z);

        playerPosY = playerArea.transform.position.y;
    }

    private void PlayerInformAreaInit()
    {
        buttleIndexRT = buttleIndex.GetComponent<RectTransform>();
        bestScoreIndexRT = bestScoreIndex.GetComponent<RectTransform>();
        nowScoreIndexRT = nowScoreIndex.GetComponent<RectTransform>();

        buttleImage.sizeDelta = ui.buttleImageArea;
        buttleIndexRT.sizeDelta = ui.buttleIndexArea;
        buttleIndex.fontSize = buttleIndexRT.sizeDelta.y;

        bestScoreImage.sizeDelta = ui.bestScoreImageArea;
        
        bestScoreIndexRT.sizeDelta = ui.bestScoreIndexArea;
        bestScoreIndex.fontSize = bestScoreIndexRT.sizeDelta.y;

        nowScoreIndexRT.sizeDelta = ui.nowScoreIndexArea;
        nowScoreIndex.fontSize = nowScoreIndexRT.sizeDelta.y;
        pause.sizeDelta = ui.pauseArea;

        buttleImage.localPosition = ui.buttleImagePos;
        buttleIndexRT.localPosition = ui.buttleIndexPos;
        bestScoreImage.localPosition = ui.bestScoreImagePos;
        bestScoreIndexRT.localPosition = ui.bestScoreIndexPos;
        nowScoreIndexRT.localPosition = ui.nowScoreIndexPos;

        RectTransform timerRT = timer.GetComponent<RectTransform>(); 
        RectTransform beatRT = beat.GetComponent<RectTransform>();

        timerRT.sizeDelta =ui.timerArea; 
        beatRT.sizeDelta = ui.beatArea;

        timerRT.localPosition =ui.timerPos;
        beatRT.localPosition = ui.beatPos;


        pause.localPosition = ui.pausePos;

    }
    #endregion

    #region Audio
    [Header("Audio")]
    public string musicCripPath;
    public AudioManager audioManager => AudioManager.Instance;
    private const string musicCripName = "Stage_Music";

    private void AudioInit()
    {
        audioManager.ChangeCrip(musicCripName, musicCripPath);
    }
    private void PlayMusic() => audioManager.UnPauseMusic(musicCripName);
    private void StopMusic() => audioManager.PauseMusic(musicCripName);
    #endregion

    #region Score
    [Header("Score")]
    public TextMeshProUGUI thisStageNowScoreText;
    public TextMeshProUGUI thisStageBestScoreText;

    [SerializeField] private const int scorePlusIndex = 100;
    [SerializeField] private int thisStageNowScore = 0;
    [SerializeField] private int thisStageBestScore;

    private Coroutine scoreRoutine;
    private int minStep = 5;
    private int maxStep = 50;
    private int targetScore;


    private void ScoreInit()
    {
        thisStageBestScore = scoreManager.GetRank(gameManager.nowLevel, gameManager.nowStage);
    }

    private void ScoreUpdate()
    {
        thisStageBestScoreText.text = thisStageBestScore.ToString("D5");
        thisStageNowScoreText.text = thisStageNowScore.ToString("D5");

    }

    public void ScorePlus()
    {
        targetScore += scorePlusIndex;
    }

    private IEnumerator ScorePlusRoutine()
    {
        if (thisStageNowScore >= targetScore) yield break;
        while (thisStageNowScore < targetScore)
        {
            int diff = targetScore - thisStageNowScore;

            int step = Mathf.Clamp(
                diff / 5,      
                minStep,
                maxStep
            );

            thisStageNowScore += step;
            yield return null;
        }

        thisStageNowScore = targetScore;
        scoreRoutine = null;
    }


    #endregion

    #region Pause 
    [Header("Pause")]

    public GameObject PauseCanvas;
    public GameObject TutorialCanvas;
    private void PauseInit()
    {
        PauseCanvas.SetActive(false);
        TutorialCanvas.SetActive(false);
    }

    public void Button_Pause()
    {
        isGamePause = !isGamePause;
        if (isGamePause)
        {
            PauseCanvas.SetActive(true);
            StopMusic();
            Time.timeScale = 0.0f;

        }
        else
        {
            PauseCanvas.SetActive(false);
            PlayMusic();
            Time.timeScale = 1.0f;

        }
    }
    public void Button_Retry()
    {
        gameManager.ScenesChange(GameManager.Scenes.InGame);
    }
    public void Button_Retrie()
    {
        gameManager.ScenesChange(GameManager.Scenes.LevelSelect);
    }
    public void Button_BackToTitle()
    {
        gameManager.ScenesChange(GameManager.Scenes.GameTitle);
    }

    public void Button_BackToTutorial()
    {
        TutorialCanvas.SetActive(false);
    }

    #endregion

    #region PlayGround
    [Header("PlayGround")]

    [SerializeField] private List<Vector2> areaLimit;
    [SerializeField] private List<Boom> boomGameObjectList = new List<Boom>();
    public List<Wall> wallGameObjectList = new List<Wall>();
    [SerializeField] private List<GameObject> normalBlock = new List<GameObject>();
    [SerializeField] private List<WallBoom> wallBooms = new List<WallBoom>();

    [SerializeField] private int rowNumber;
    [SerializeField] private int colNumber;

    public float blockSize;
    private float blockSizeY;

    private string levelStageCsv;
    private bool isBlockSetUp = false;
    public float wallBoomSpreadSpeed = 0.01f;
    public float flameLenght;

    private int BlockSizeRedia()
    {
        int row = 7;

        switch (rowNumber)
        {
            case 8:     row *= 1;   break;
            case 15:    row *= 2;   break;
            case 22:    row *= 3;   break;
            case 29:    row *= 4;   break;
            case 36:    row *= 5;   break;
            default: return 0;
        }

        return row;
    }
    private void CaluBlockSize()
    {
        float dx = Mathf.Abs(ui.worldCorners[0].x - ui.worldCorners[2].x);
        dx /= BlockSizeRedia();
        float dy = Mathf.Abs(ui.worldCorners[1].y - ui.worldCorners[3].y);

        //float xLong = dx / rowNumber;
        float yLong = dy / colNumber;

        blockSize = dx;
        blockSizeY = yLong;
    }
    private float Width(int index)
    {
        float posX;

        posX = ui.worldCorners[0].x;
        if (index < 0 || index > rowNumber) Debug.LogError("Out of range");
        posX += blockSize * index;
        //posX -= blockSize / 2;
        return posX;
    }
    private float Height(int index)
    {
        float posY;
        posY = ui.worldCorners[1].y;
        if (index < 0 || index > colNumber) Debug.LogError("Out of range");
        posY -= blockSize * index;
        posY -= blockSize / 2;
        return posY;

    }

    private GameObject TargetBlock(string targetCode)
    {
        GameObject targetGameObject = null;
        if (targetCode == "N") targetGameObject = Resources.Load<GameObject>("Prefab/Block");
        else if (targetCode == "B") targetGameObject = Resources.Load<GameObject>("Prefab/Boom");
        else if (targetCode == "W") targetGameObject = Resources.Load<GameObject>("Prefab/Wall");
        else if (targetCode == "WB") targetGameObject = Resources.Load<GameObject>("Prefab/WallBoom_Boom");
        else if (targetCode == "CBW") targetGameObject = Resources.Load<GameObject>("Prefab/WallBoom_Wall");

        return targetGameObject;
    }

    private void BlocksSetUp()
    {
        GameObject parent = Instantiate(new GameObject(), ui.playAreaPosition, Quaternion.identity, transform);

        areaLimit = new List<Vector2>();
        boomGameObjectList = new List<Boom>();
        wallGameObjectList = new List<Wall>();
        normalBlock = new List<GameObject>();

        int row = (int)csvReader.ReadTargetCellIndex(levelStageCsv, "B", 3) + 4;
        int col = (int)csvReader.ReadTargetCellIndex(levelStageCsv, "B", 4) + 4;

        for (int x = 4; x < row; x++)
        {
            if (x - 4 > rowNumber) return;

            for (int y = 4; y < col; y++)
            {
                if (y - 4 > colNumber) continue;

                float posX = Width(x - 4);
                float posY = Height(y - 4);


                if (x == 4 || x == row - 1 || y == 4 || y == col - 1)
                {
                    areaLimit.Add(new Vector2(posX, posY));
                }

                string targetCode = csvReader.ReadTargetCellString(levelStageCsv, csvReader.IndexToColumnName(x), y);

                if (targetCode.Length >= 1)
                {

                    GameObject target = Instantiate(TargetBlock(targetCode), Vector3.zero, Quaternion.identity);
                    //Debug.Log(target.name);

                    target.transform.localScale = new Vector3(blockSize, blockSize, 1);
                    target.transform.position = new Vector3(posX, posY, 0);

                    if (targetCode == "N")
                    {
                        normalBlock.Add(target);
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
                            //targetWall.SetWallType(Wall.WallType.NonBreakAble);
                            wallGameObjectList.Add(targetWall);
                        }
                        target.name = $"X={x - 4},Y={y - 4}, Wall";
                    }
                    else if (targetCode == "CBW")
                    {
                        Wall targetWall = target.GetComponent<Wall>();
                        if (targetWall != null)
                        {
                            //targetWall.SetWallType(Wall.WallType.BreakAble);
                            wallGameObjectList.Add(targetWall);
                        }

                        target.name = $"X={x - 4},Y={y - 4}, Wall";
                    }
                    else if (targetCode == "WB")
                    {
                        WallBoom targetWallBoom = target.GetComponent<WallBoom>();
                        normalBlock.Add(target);
                        wallBooms.Add(targetWallBoom);
                        target.name = $"X={x - 4},Y={y - 4}, Wall";
                    }

                    target.transform.SetParent(parent.transform);

                }
            }


        }



        isBlockSetUp = true;

    }

    private void BlockListUpdate()
    {
        boomGameObjectList.RemoveAll(item => item == null);
        normalBlock.RemoveAll(item => item == null);
        wallGameObjectList.RemoveAll(item => item == null);
    }

    #endregion

    #region Player
    [Header("Player")]

    public Player player;
    public float playerPosY;
    public List<Vector3> points;

    public int playerStartIndex = 0;

    private float playerShootCd;
    public TextMeshProUGUI playerNowBullet;

    public float distanceOfBeat;
    public double timePerOneBeat;
    public double beatsPerSecond;



    public bool IsInTail()
    {
        var x = player.transform.position.x;
        var tailX = points[points.Count - 1].x;
        if (x > tailX) return true;
        else return false;
    }
    private void PlayerMovePointInit()
    {
        points = new List<Vector3>();
        for (int i = 0; i < ui.moveDelta.Length; i++)
        {
            Vector3 Pos = new Vector3(ui.moveDelta[i], playerPosY, -1);
            points.Add(Pos);
            Debug.Log(Pos);
        }
        if (playerStartIndex < 0) playerStartIndex = 0;
        else if (playerStartIndex > points.Count - 1) playerStartIndex = points.Count - 1;

        playerMovePos.RoadInit(points);
    }

    private void PlayerMove()
    {
        if (isGamePause) return;

        float newPosX = player.transform.position.x + (float)((distanceOfBeat * beatsPerSecond) * deltaTime);
        //float newPosX = player.transform.position.x + (float)((distanceOfBeat * timePerOneBeat) * deltaTime);

        player.transform.position = new Vector3(newPosX, playerPosY, -1);

        if (IsInTail())
        {
            var extraX = Mathf.Abs(points[points.Count - 1].x - player.transform.position.x);
            newPosX = points[0].x + extraX;
            player.transform.position = new Vector3(newPosX, playerPosY, -1);

        }

    }



    private double test_StartTime;
    private float test_BeatOfDistance;
    private double test_BeatOfTime;
    private float test_LoopOfDistance;
    private double test_LoopOfTime;



    private void Text_Init()
    {
        test_BeatOfDistance = distanceOfBeat;
        test_BeatOfTime = timePerOneBeat;
        test_LoopOfDistance = test_BeatOfDistance * 8;
        test_LoopOfTime = test_BeatOfTime * 8;
    }

    private double test_nowTime;
    private double test_pauseTime;
    private double test_totalpauseTime;


    private double test_totaDeltaTime;
    private int test_timeRedio;
    private double test_deltaTime;
    private double test_timePerloopTime;
    private float test_distance;


    private void NewPlayMove()
    {
        if (isGamePause)
        {

            test_pauseTime = AudioSettings.dspTime - test_nowTime;
            return;
        }
        test_nowTime = AudioSettings.dspTime;
        test_totalpauseTime += test_pauseTime;
        test_totaDeltaTime = nowTime - test_StartTime - test_totalpauseTime;

        test_timeRedio = (int)(test_totaDeltaTime / test_LoopOfTime);
        test_deltaTime = test_totaDeltaTime - (test_timeRedio * test_LoopOfTime);
        test_timePerloopTime = test_deltaTime / test_LoopOfTime;
        test_distance = (float)(test_timePerloopTime * test_LoopOfDistance);

        float newPosX = points[0].x + test_distance;
        player.transform.position = new Vector3(newPosX, playerPosY, -1);


    }


    #endregion

    #region GameSet
    public List<Bullet> bulletList = new List<Bullet>();
    private Coroutine gameSet;

    private bool IsAllBlockGone() => normalBlock.Count == 0;

    private bool IsBulletEmpty() => player.nowbullet == 0 && bulletList.Count == 0;
    private bool IsTimeUp() => nowTime < 0;

    private bool IsGameSet() => IsAllBlockGone() || IsBulletEmpty()|| IsTimeUp();

    private void GameResult()
    {
        if (IsAllBlockGone())
        {
            GameManager.Instance.GameClear();
        }
        else GameManager.Instance.GameOver();
    }

    private IEnumerator GameSet()
    {
        isGamePause = false;
        StopMusic();
        yield return new WaitForSeconds(0.1f);

        GameResult();
        gameManager.finalScore = thisStageNowScore;
        scoreManager.AddRank(gameManager.nowLevel, gameManager.nowStage, thisStageNowScore);

        yield return new WaitForSeconds(0.1f);
        gameManager.ScenesChange(GameManager.Scenes.Release);


    }



    #endregion

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }
    private void GameDataInit()
    {
        levelStageCsv = $"CSVes/Level_0{gameManager.nowLevel}/Stage_0{gameManager.nowStage}";
        Debug.Log(levelStageCsv);
        bpm = csvReader.ReadTargetCellIndex(levelStageCsv, "B", 13);

        rowNumber = (int)csvReader.ReadTargetCellIndex(levelStageCsv, "B", 3);
        colNumber = (int)csvReader.ReadTargetCellIndex(levelStageCsv, "B", 4);

        //musicCripPath = csvReader.ReadTargetCellString(levelStageCsv, "B", 14);
        musicCripPath = "135bpm_game_loop";

        player = Player.Instance;
        playerShootCd = csvReader.ReadTargetCellIndex(levelStageCsv, "B", 16);
        player.bulletMax = (int)csvReader.ReadTargetCellIndex(levelStageCsv, "B", 17);

        //timePerOneBeat = bpm / 60.0;
        timePerOneBeat = 60.0 / bpm;

        beatsPerSecond = (float)(1.0 / timePerOneBeat);
        distanceOfBeat = Mathf.Abs(points[1].x - points[2].x);
        Text_Init();
    }






    public IEnumerator Init()
    {
        isGamePause = true;
        AreaInit();
        PlayerMovePointInit();
        PlayerInformAreaInit();
        GameDataInit();

        AudioInit();
        ScoreInit();
        CaluBlockSize();
        BlocksSetUp();
        PauseInit();
        yield return null;

        flameLenght = blockSize * BlockSizeRedia();
        Debug.Log(points[playerStartIndex]);
        player.transform.position = points[0];
        player.Init(playerShootCd);

        foreach (WallBoom wallBoom in wallBooms)
        {
            wallBoom.startFind = true;
            yield return null;
        }

        yield return new WaitUntil(() => audioManager.musicSounds[0].clip != null);



        audioManager.PlayMusic(musicCripName, 100);
        StopMusic();    
        Debug.Log("InGame Init");
    }
    public void GameStart()
    {
        isGamePause = false;
        PlayMusic();
        nowTimer = timerLimt;
        oldTime = AudioSettings.dspTime;
        test_StartTime = AudioSettings.dspTime;
    }


    public void InGameUpdate()
    {
        Timer();
        PlayweTimer();
        ScoreUpdate();
        player.ShootBullet();
        playerNowBullet.text = player.nowbullet.ToString("D2");
    }

    public void InGameLateUpdate()
    {
        if (!IsGameSet())
        {
            NewPlayMove();
            //PlayerMove();
            if (scoreRoutine == null) scoreRoutine = StartCoroutine(ScorePlusRoutine());
            BlockListUpdate();
            bulletList.RemoveAll(item => item == null);

            oldTime = AudioSettings.dspTime;
        }
        else
        {
            if(gameSet == null)
            {
                gameSet = StartCoroutine(GameSet());
            }
        }

    }

}

