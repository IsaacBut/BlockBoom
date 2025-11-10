using UnityEngine;
using UnityEngine.SceneManagement;
using Data;

public class LevelSelect : MonoBehaviour
{
    public RectTransform canvasRect; // Canvas 的 RectTransform
    public RectTransform backGroundRect;

    public RectTransform levelSelectTitleRect; // Canvas 的 RectTransform

    const int level = 3;
    public RectTransform[] levelSelectRect;  // Image 的 RectTransform
    public RectTransform button_BackToTitle;

    public void Button_Level_01()
    {
        GameManager.instance.level = 1;
        GameManager.instance.stage = 1;
        SceneManager.LoadScene("InGame", LoadSceneMode.Single);

    }
    public void Button_Leve0_00()
    {
        GameManager.instance.level = 0;
        GameManager.instance.stage = 0;
        SceneManager.LoadScene("InGame", LoadSceneMode.Single);

    }
    public void Button_Level_02()
    {
        GameManager.instance.level = 2;
        GameManager.instance.stage = 1;
        SceneManager.LoadScene("InGame", LoadSceneMode.Single);

    }

    public void Button_Level_03()
    {
        GameManager.instance.level = 3;
        GameManager.instance.stage = 1;
        SceneManager.LoadScene("InGame", LoadSceneMode.Single);

    }

    public void Button_BackToTitle()
    {
        SceneManager.LoadScene("GameTitle", LoadSceneMode.Single);
    }

}
