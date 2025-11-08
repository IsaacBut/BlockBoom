using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Data;

public class GameOver : MonoBehaviour
{
    public RectTransform canvasRect; // Canvas “I RectTransform

    public RectTransform backGroundImageRect;  // Image “I RectTransform
    public RectTransform gameOverLogoRect;

    const int scoreSize = 3;
    public RectTransform[] scoreRect;
    public TMP_Text score;

    public RectTransform button_BackToTitleRect;
    public RectTransform button_RetryRect;

    const int stageLimit = 5;

    public void Button_Retry()
    {
        SceneManager.LoadScene("InGame", LoadSceneMode.Single);
    }

    public void Button_Quit()
    {
        SceneManager.LoadScene("GameTitle", LoadSceneMode.Single);
    }

}
