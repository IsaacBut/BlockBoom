using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static GameManager;

public class Release : MonoBehaviour
{
    public static Release Instance { get; private set; }
    private GameManager gameManager => GameManager.Instance;

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
        //nowStage = new Vector2Int(gameManager.nowLevel, gameManager.nowStage);

        //if (gameManager.isGameClear)
        //{
        //    title.sprite = Resources.Load<Sprite>("Image/Release/Release_GameClear_Title");
        //    Score.sprite = Resources.Load<Sprite>("Image/Release/Release_GameClear_Score");

        //    gameManager.NextPlayStage();

        //    if (gameManager.isEndAllStage) nextStage.gameObject.SetActive(false);
        //    else retire.gameObject.SetActive(false);
        //}
        //else
        //{
        //    title.sprite = Resources.Load<Sprite>("Image/Release/Release_GameOver_Title");
        //    Score.sprite = Resources.Load<Sprite>("Image/Release/Release_GameOver_Score");

        //    nextStage.gameObject.SetActive(false);
        //}
        Debug.Log("Release Init");
    }

    public void Button_Retry()
    {
        gameManager.SetPlayStage(nowStage.x, nowStage.y);
        //gameManager.ScenesChange(Scenes.InGame);
    }

    public void Button_Retire() => gameManager.ScenesChange(Scenes.GameTitle);
    public void Button_NextStage() => gameManager.ScenesChange(Scenes.InGame);


}
