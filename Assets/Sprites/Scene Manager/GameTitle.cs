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

    private void Update()
    {

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
        ScoreManager.instance.RankShowInit();
        SceneManager.LoadScene("Rank", LoadSceneMode.Single);

    }

    public void ResetRankButton() => ScoreManager.instance.ResetRank();

    public void PlayerNameInPutButton()
    {

        inputField.ForceLabelUpdate(); // ✅ 强制刷新文本
        string playerName = inputField.text; // 此时日文已经确认

        Debug.Log("玩家名字：" + playerName);

        if (string.IsNullOrWhiteSpace(playerName)) return;

        GameManager.instance.PlayerInit(playerName);
        inputField.text = "";
        SceneManager.LoadScene("LevelSelect", LoadSceneMode.Single);

    }

 

}
