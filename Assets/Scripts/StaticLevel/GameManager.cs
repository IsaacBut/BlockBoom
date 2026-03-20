using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Keyboard keyboard => Keyboard.current;
    public Touchscreen touchscreen => Touchscreen.current;
    public Mouse mouse => Mouse.current;


    [Header("Static Manager")]

    private UIManager uiManager;
    private ScoreManager scoreManager;
    private AudioManager audioManager;
    private CSVReader csvReader;

    [Header("Game Scenes")]

    private bool isSceneInit = false;
    
    private bool canSceneUpdate = false;

    public enum Scenes
    {
        Loading,
        GameTitle,
        LevelSelect,
        InGame,
        Release,
    }
    public Scenes nowScene;
    
    private string SceneName(Scenes targetScene)
    {
        switch (targetScene)
        {
            case Scenes.Loading:        return null;
            case Scenes.GameTitle:      return "GameTitle";
            case Scenes.LevelSelect:    return "LevelSelect";
            case Scenes.InGame:         return "InGame";
            case Scenes.Release:        return "Release";
            default:                    return null;
        }
    }


    public void ScenesChange(Scenes targetScene)
    {
        if (targetScene == Scenes.Loading)
        {
            Debug.LogError("[GameManager] Loading Scenes cant be set");
            return;
        }

        SceneManager.LoadScene(SceneName(targetScene), LoadSceneMode.Single);
        nowScene = targetScene;
        isSceneInit = false;
        canSceneUpdate = false;
    }

    private void ScenesInit()
    {
        if (isSceneInit) return;
        isSceneInit = true;

        switch (nowScene)
        {
            case Scenes.Loading:        StartCoroutine(LoadingInit());      break;
            case Scenes.GameTitle:      StartCoroutine(GameTitleInit());    break;
            case Scenes.LevelSelect:    StartCoroutine(LevelSelectInit());  break;
            case Scenes.InGame:         StartCoroutine(InGameInit());       break;
            case Scenes.Release:        StartCoroutine(ReleaseInit());      break;

            default: break;
        }

    }
    private void ScenesUpdate()
    {
        if (!canSceneUpdate) return;

        switch (nowScene)
        {
            case Scenes.Loading:        LoadingUpdate();        break;
            case Scenes.GameTitle:      GameTitleUpdate();      break;
            case Scenes.LevelSelect:    LevelSelectUpdate();    break;
            case Scenes.InGame:         InGameUpdate();         break;
            case Scenes.Release:        ReleaseUpdate();        break;

                default: break;

        }

    }

    private void ScenesLateUpdate()
    {
        if (!canSceneUpdate) return;

        switch (nowScene)
        {
            case Scenes.Loading: break;
            case Scenes.GameTitle: break;
            case Scenes.LevelSelect:break;
            case Scenes.InGame: inGame.InGameLateUpdate(); break;
            case Scenes.Release: break;

            default: break;

        }

    }

    #region Loading
    [Header("Loading")]
    public bool isGameStart;
    private IEnumerator LoadingInit()
    {
        yield return null;

        uiManager.Init();
        csvReader.Init();
        scoreManager.RankInit();
        isGameStart = true;
        canSceneUpdate = true;
    }

    private void LoadingUpdate()
    {
        if (isGameStart) ScenesChange(Scenes.GameTitle);
    }
    #endregion

    #region GameTitle
    [Header("GameTitle")]
    private GameTitle gameTitle;
    private IEnumerator GameTitleInit()
    {

        yield return new WaitUntil(() => GameTitle.Instance != null);
        gameTitle = GameTitle.Instance;
        gameTitle.Init();
        canSceneUpdate = true;
    }

    private void GameTitleUpdate()
    {

    }
    #endregion

    #region Level Select
    [Header("Level Select")]
    private LevelSelect levelSelect;

    private const int minLevel = 1;
    private const int maxLevel = 3;
    public int nowLevel;

    private const int minStage = 1;
    private const int maxStage = 5;
    public int nowStage;

    public bool IsMaxStage() => nowLevel >= maxLevel && nowStage >= maxStage;
    //public bool isMaxStage = false;

    public void NextStage()
    {
        if (IsMaxStage()) return;

        nowStage++;

        if (nowStage > maxStage)
        {
            nowStage = minStage;
            nowLevel++;
        }

        if (nowLevel > maxLevel)
        {
            nowLevel = maxLevel;
            nowStage = maxStage;
            return;
        }

        SetPlayStage(nowLevel, nowStage);
    }

    public void SetPlayStage(int targetLevel, int targetStage)
    {
        if (targetLevel > maxLevel|| targetLevel < minLevel) 
        {
            Debug.LogError($"[GameManager] TargetLevel{targetLevel} is not inside the Range{minLevel} to {maxLevel}");
            return;
        }
        else if (targetStage > maxStage|| targetStage < minStage)
        {
            Debug.LogError($"[GameManager] TargetStage{targetStage} is not inside the Range{minStage} to {maxStage}");
            return;
        }

        nowLevel = targetLevel;
        nowStage = targetStage;
    }

    private IEnumerator LevelSelectInit()
    {
        yield return new WaitUntil(() => LevelSelect.Instance != null);
        levelSelect = LevelSelect.Instance;
        levelSelect.Init();
        canSceneUpdate = true;
    }

    private void LevelSelectUpdate()
    {


    }


    #endregion

    #region InGame

    [Header("InGame")]
    private InGame inGame;
    public int finalScore;

    private IEnumerator InGameInit()
    {
        yield return new WaitUntil(() => InGame.Instance != null);
        inGame = InGame.Instance;
        yield return StartCoroutine(inGame.Init());

        inGame.GameStart();
        canSceneUpdate = true;
    }

    private void InGameUpdate()
    {
        inGame.InGameUpdate();
    }


    #endregion

    #region Release
    [Header("Release")]
    private Release release;
    

    public enum GameResult
    {
        GameClear,
        GameOver
    }
    public GameResult gameResult;

    public void GameClear() => gameResult = GameResult.GameClear;
    public void GameOver() => gameResult = GameResult.GameOver;


    private IEnumerator ReleaseInit()
    {
        yield return new WaitUntil(() => Release.Instance != null);
        release = Release.Instance;
        release.Init();
        canSceneUpdate = true;
    }

    private void ReleaseUpdate()
    {

    }


    #endregion

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else Destroy(this);
    }

    private void Start()
    {
        Application.targetFrameRate = 120;
        Debug.Log(AudioSettings.dspTime);
        uiManager = UIManager.Instance;
        scoreManager = ScoreManager.Instance;
        audioManager = AudioManager.Instance;
        csvReader = CSVReader.Instance;

    }

    private void Update()
    {
        ScenesInit();
        ScenesUpdate();
    }

    private void LateUpdate()
    {
        ScenesLateUpdate();
    }



}
