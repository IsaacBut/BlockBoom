using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Data;

public class GameTitle : MonoBehaviour
{
    [SerializeField] private RectTransform canvasRect;
    [SerializeField] private RectTransform backGroundRect;
    [SerializeField] private RectTransform logoRect;
    [SerializeField] private RectTransform button_StartRect; 
    [SerializeField] private RectTransform button_RankRect;

    [SerializeField] private GameObject nameInput;
    [SerializeField] private RectTransform nameInput_BackGroundRect;
    [SerializeField] private RectTransform nameInput_Logo;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private RectTransform nameInput_button;

    public static GameTitle instance { get; private set; }

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
    }

    private void Start()
    {

        nameInput.SetActive(false);

    }


    public void GoLevelSelectButton() 
    {
        //SceneManager.LoadScene("GameClear", LoadSceneMode.Single);

        nameInput.SetActive(true);

        //SceneManager.LoadScene("LevelSelect", LoadSceneMode.Single);

    }
    public void GoRankButton() 
    {
        //SceneManager.LoadScene("GameOver", LoadSceneMode.Single);

        SceneManager.LoadScene("Rank", LoadSceneMode.Single);

    }

    public void ResetRankButton() => ScoreManager.instance.ResetRank();

    public void PlayerNameInPutButton()
    {
        string playerName = inputField.text; 
        Debug.Log("当前输入内容是：[" + playerName + "]");

        if (string.IsNullOrEmpty(playerName)) return;
        Debug.Log("玩家输入的名字是：" + playerName);
        GameManager.instance.PlayerInit(playerName);
        inputField.text = "";
        SceneManager.LoadScene("LevelSelect", LoadSceneMode.Single);

    }

    //public void ReSetRankButton() => GameManager.instance.ResetRank();


}
