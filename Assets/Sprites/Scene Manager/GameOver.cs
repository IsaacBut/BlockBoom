using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Data;

public class GameOver : MonoBehaviour
{
    public TMP_Text score;

    private void Start()
    {
        score.text = GameManager.instance.pastScoreFromInGame.ToString("D5");
    }


    public void Button_Retry()
    {
        SceneManager.LoadScene("InGame", LoadSceneMode.Single);
    }

    public void Button_Quit()
    {
        SceneManager.LoadScene("GameTitle", LoadSceneMode.Single);
    }

}
