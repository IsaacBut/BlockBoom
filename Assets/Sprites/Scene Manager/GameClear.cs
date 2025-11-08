using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;
using Data;
public class GameClear : MonoBehaviour
{
    public RectTransform canvasRect; // Canvas “I RectTransform

    public RectTransform backGroundImageRect;  // Image “I RectTransform
    public RectTransform gameClearLogoRect;

    const int scoreSize = 3;
    public RectTransform[] scoreRect;
    public TMP_Text score;

    public RectTransform button_BackToLevelSelectRect;
    public RectTransform button_NextStageRect;


    const int stageLimit = 5;

    private void Start()
    {

    }

    public void Button_NextStage()
    {
        GameManager.instance.stage += 1;
        if (GameManager.instance.stage > stageLimit)
        {
            GameManager.instance.stage = 1;
            GameManager.instance.level += 1;
        }
        SceneManager.LoadScene("InGame", LoadSceneMode.Single);

    }
    public void Button_BackToLevelSelect()
    {
        SceneManager.LoadScene("LevelSelect", LoadSceneMode.Single);

    }



}
