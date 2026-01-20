using System.Security.Cryptography;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.XR;
public class InGame : MonoBehaviour
{
    public static InGame Instance;
    private GameManager gameManager => GameManager.Instance;
    public bool isGameStart;

    [Header("Texting")]
    public Test_PlayerMove playerMovePos;

    [Header("Game Timer")]
    public double bpm = 120;

    public double nowTime;
    public double oldTime;
    public double deltaTime;

    public bool isGamePause = false;

    //private void Timer()
    //{
    //    double now = AudioSettings.dspTime;

    //    while (now >= nextBeatTime)
    //    {
    //        nextBeatTime += moveTime;
    //    }

    //    while (now >= playerShootTimer)
    //    {
    //        player.canShoot = true;
    //        playerShootTimer += playerShootCd;
    //    }

    //}

    private void Timer()
    {
        nowTime = AudioSettings.dspTime;
        deltaTime = nowTime - oldTime;
    }


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
        bestScoreImage.sizeDelta = ui.bestScoreImageArea;
        bestScoreIndexRT.sizeDelta = ui.bestScoreIndexArea;
        nowScoreIndexRT.sizeDelta = ui.nowScoreIndexArea;
        pause.sizeDelta = ui.pauseArea;

        buttleImage.localPosition = ui.buttleImagePos;
        buttleIndexRT.localPosition = ui.buttleIndexPos;
        bestScoreImage.localPosition = ui.bestScoreImagePos;
        bestScoreIndexRT.localPosition = ui.bestScoreIndexPos;
        nowScoreIndexRT.localPosition = ui.nowScoreIndexPos;

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

    #region Pause 
    [Header("Pause")]

    public GameObject PauseCanvas;

    private void PauseInit()
    {
        PauseCanvas.SetActive(false);
    }

    public void Button_Pause()
    {
        isGamePause = !isGamePause;

        if (isGamePause)
        {
            PauseCanvas.SetActive(true);
            StopMusic();
        }
        else
        {
            PauseCanvas.SetActive(false);
            PlayMusic();
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

    #endregion


    #region PlayGround
    [Header("PlayGround")]

    [SerializeField] private List<Vector2> areaLimit;
    [SerializeField] private List<Boom> boomGameObjectList = new List<Boom>();
    public List<Wall> wallGameObjectList = new List<Wall>();
    [SerializeField] private List<GameObject> normalBlock = new List<GameObject>();

    [SerializeField] private int rowNumber;
    [SerializeField] private int colNumber;

    public float blockSize;
    private float blockSizeY;


    private string levelStageCsv;
    private bool isBlockSetUp = false;
    public float wallBoomSpreadSpeed = 0.25f;
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
                        //WallBoom targetWallBoom = target.GetComponent<WallBoom>();
                        //wallBoomGameObjectList.Add(targetWallBoom);
                        //canBrakeGameObject.Add(target);
                        target.name = $"X={x - 4},Y={y - 4}, Wall";
                    }

                    target.transform.SetParent(parent.transform);

                }
            }


        }

        isBlockSetUp = true;

    }
    #endregion

    #region Player
    [Header("Player")]
    public Player player;
    public float playerPosY;
    public List<Vector3> points;

    public int playerStartIndex = 0;

    private float playerShootCd;

    public float distanceOfBeat;
    public float moveDistance;
    public double timePerOneBeat;
    public float moveSpeed;

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

        if (!IsInTail())
        {
            float newPosX = (float)(player.transform.position.x + distanceOfBeat * moveSpeed * deltaTime);

            player.transform.position = new Vector3(newPosX, playerPosY, -1);
        }
        else
        {
            player.transform.position = points[0];
        }

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

        musicCripPath = csvReader.ReadTargetCellString(levelStageCsv, "B", 14);


        player = Player.Instance;
        playerShootCd = csvReader.ReadTargetCellIndex(levelStageCsv, "B", 16);
        player.bulletMax = (int)csvReader.ReadTargetCellIndex(levelStageCsv, "B", 17);

        timePerOneBeat = 60.0 / bpm;

        moveSpeed = (float)(1.0 / timePerOneBeat);
        distanceOfBeat = Mathf.Abs(points[1].x - points[2].x);
        moveDistance = distanceOfBeat * moveSpeed;

        oldTime = AudioSettings.dspTime;

    }

    public IEnumerator Init()
    {
        isGamePause = true;
        AreaInit();
        PlayerMovePointInit();
        PlayerInformAreaInit();
        GameDataInit();

        AudioInit();
        CaluBlockSize();

        BlocksSetUp();
        PauseInit();

        flameLenght = blockSize * BlockSizeRedia();
        Debug.Log(points[playerStartIndex]);
        player.transform.position = points[playerStartIndex];
        player.Init(playerShootCd);

        yield return new WaitUntil(() => audioManager.musicSounds[0].clip != null);
        audioManager.PlayMusic(musicCripName, 100);
        StopMusic();    
        Debug.Log("InGame Init");
    }
    public void GameStart()
    {
        isGamePause = false;
        PlayMusic();
    }


    public void InGameUpdate()
    {
        Timer();
        player.ShootBullet();
    }

    public void InGameLateUpdate()
    {
        oldTime = nowTime;
        PlayerMove();
    }

}

