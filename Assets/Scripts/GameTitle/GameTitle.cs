using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static GameManager;

public class GameTitle : MonoBehaviour
{
    public static GameTitle Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }
    public void Init()
    {
        Debug.Log("GameTitle Init");

    }

    public void Button_GoLevelSelect() => GameManager.Instance.ScenesChange(Scenes.LevelSelect);



}
