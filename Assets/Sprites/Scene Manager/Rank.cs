using UnityEngine;
using UnityEngine.SceneManagement;
using Data;
using TMPro;
using System.Security.Cryptography;


public class Rank : MonoBehaviour
{
    public RectTransform backGroundRect;

    public TMP_Text[] rank_PlayerName;
    public TMP_Text[] rank_Score;



    void RankInit()
    {

    }


    void Rank_PlayerNameInit()
    {
        for (int i = 0; i < ScoreManager.instance.Rank_PlayerName().Length; i++)
        {
            rank_PlayerName[i].text = ScoreManager.instance.Rank_PlayerName()[i];
        }
    }
    void Rank_ScoreInit()
    {
        for (int i = 0; i < ScoreManager.instance.Rank_TotalScore().Length; i++)
        {
            rank_Score[i].text = ScoreManager.instance.Rank_TotalScore()[i].ToString("D5");
        }
    }
    private void Start()
    {
        Rank_PlayerNameInit();
        Rank_ScoreInit();
    }

    public void Button_BackToTitle()
    {
        SceneManager.LoadScene("GameTitle", LoadSceneMode.Single);
    }

}
