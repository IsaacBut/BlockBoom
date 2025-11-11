using Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    

    private UIManager uiManager => UIManager.instance;
    public UI _ui => uiManager.ui;
    private ScoreManager scoreManager => ScoreManager.instance;
    public int screenWidth => uiManager.screenSize.x;
    public int screenHeight => uiManager.screenSize.y;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else Destroy(this);



    }
    private void Start()
    {
        uiManager.UIInit();
        GameData.GameBasicDataInit();
        level = 1;
        stage = 1;
        scoreManager.RankInit();
    }

    bool gameStart = false;
    // Update is called once per frame
    void Update()
    {
        if (uiManager.IsGameEndInit() && !gameStart)
        {
            SceneManager.LoadScene("GameTitle", LoadSceneMode.Single);
            //ebug.Log(CSVReader.instance.ReadTargetCellIndex("CSVes/Level_00/Stage_00", "O", 17));
            gameStart = true;
        }
        else if (!uiManager.IsGameEndInit())
        {
            uiManager.GameInit();
        }

    }

    #region GameLevel
    public int level { get; set; }
    public int stage { get; set; }

    #endregion
    public static string playerName { get; set; }

    public int pastScoreFromInGame { get; set; }

    public void PlayerInit(string name) => scoreManager.PlayerInit(name);


}
