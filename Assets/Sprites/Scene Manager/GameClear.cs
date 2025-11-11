using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;
using Data;
public class GameClear : MonoBehaviour
{
    public TMP_Text score;

    const int stageLimit = 5;

    private void Start()
    {
        score.text = GameManager.instance.pastScoreFromInGame.ToString("D5");
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
