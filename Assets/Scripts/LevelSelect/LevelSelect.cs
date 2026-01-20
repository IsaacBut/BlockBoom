using UnityEngine;
using static GameManager;

public class LevelSelect : MonoBehaviour
{
    public static LevelSelect Instance { get; private set; }
    private GameManager gameManager => GameManager.Instance;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    public void Init()
    {
        gameManager.SetPlayStage(1, 1);
        Debug.Log("LevelSelect Init");
    }

    public void Button_BackToTitle() => GameManager.Instance.ScenesChange(Scenes.GameTitle);


    public void Button_Level01()
    {
        gameManager.SetPlayStage(1, 1);
        GameManager.Instance.ScenesChange(Scenes.InGame);
    }
    public void Button_Level02()
    {
        gameManager.SetPlayStage(2, 1);
        GameManager.Instance.ScenesChange(Scenes.InGame);
    }
    public void Button_Level03()
    {
        gameManager.SetPlayStage(3, 1);
        GameManager.Instance.ScenesChange(Scenes.InGame);
    }
}
