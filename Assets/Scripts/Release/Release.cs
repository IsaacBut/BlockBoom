using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static GameManager;

public class Release : MonoBehaviour
{
    public static Release Instance { get; private set; }
    private GameManager gameManager => GameManager.Instance;
    private ScoreManager scoreManager => ScoreManager.Instance;

    public Image title;
    public Image Score;
    public TextMeshProUGUI scoreText;
    public Button retire;
    public Button nextStage;

    private Vector2Int nowStage;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    public void Init()
    {
        nowStage = new Vector2Int(gameManager.nowLevel, gameManager.nowStage);

        switch(gameManager.gameResult)
        {

            case GameManager.GameResult.GameClear:
                title.sprite = Resources.Load<Sprite>("Image/Release/Release_GameClear_Title");
                Score.sprite = Resources.Load<Sprite>("Image/Release/Release_GameClear_Score");

                if (gameManager.IsMaxStage()) nextStage.gameObject.SetActive(false);
                else retire.gameObject.SetActive(false);

                break;
            case GameManager.GameResult.GameOver:
                title.sprite = Resources.Load<Sprite>("Image/Release/Release_GameOver_Title");
                Score.sprite = Resources.Load<Sprite>("Image/Release/Release_GameOver_Score");

                nextStage.gameObject.SetActive(false);
                break;

            default: 
                Debug.LogError($"[Release] gameResult is Error{gameManager.gameResult}");
                break;


        }
        scoreText.text = scoreManager.GetRank(nowStage.x, nowStage.y).ToString("D5");

        Debug.Log("Release Init");
    }

    public void Button_Retry()
    {
        gameManager.SetPlayStage(nowStage.x, nowStage.y);
        gameManager.ScenesChange(Scenes.InGame);
    }

    public void Button_Retire() => gameManager.ScenesChange(Scenes.GameTitle);
    public void Button_NextStage()
    {
        gameManager.NextStage();
        gameManager.ScenesChange(Scenes.InGame);
    }

}
