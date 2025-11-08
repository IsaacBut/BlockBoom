using Data;
using NUnit.Framework;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    public static UIManager instance { get; private set; }


    private string uIPath;
    private UI_Size ui_Size;
    public UI ui;

    public Vector2Int screenSize { get; private set; }

    const int level = 3;
    const int scoreSize = 3;

    const float radio = 16f / 9f;       // 使用浮点数除法，结果为 1.777...
    const float turnradio = 9f / 16f;   // 使用浮点数除法，结果为 0.5625f
    private Vector2 scale;
    private bool screenFit;

    [Header("GameInit")]
    [SerializeField] private RectTransform canvasRect;
    [SerializeField] private RectTransform playInformArea;
    [SerializeField] private RectTransform playArea;
    [SerializeField] private RectTransform playerArea;

    bool IsScreenDataReady()
    {
        // 如果 ui_Size 没初始化，肯定不算就绪
        if (ui_Size == null) return false;

        // 如果 size 未设置，也不算就绪
        if (ui_Size.size.x <= 0 || ui_Size.size.y <= 0) return false;

        // 最后检查是否匹配当前屏幕
        return ui_Size.size == screenSize;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else Destroy(this);

        ScreenInit();
        Screen.SetResolution(screenSize.x, screenSize.y,FullScreenMode.FullScreenWindow);
    }

    public void ScreenInit()
    {
        float width = Screen.width;
        float height = Screen.height;

        float nowScreenRadio = width / height;
        float epsilon = 0.001f;
        screenFit = Mathf.Abs(nowScreenRadio - radio) < epsilon;

        float screenWidth;
        float screenHeight;

        if (nowScreenRadio > radio)
        {
            screenWidth = height * radio;
            screenHeight = height;
        }
        else if (nowScreenRadio < radio)
        {
            screenWidth = width;
            screenHeight = width * turnradio;
        }
        else
        {
            screenWidth = width;
            screenHeight = height;
        }

        screenSize = new Vector2Int((int)screenWidth, (int)screenHeight);

        Debug.Log($"当前比例: {nowScreenRadio:F4}, 目标比例: {radio:F4}, 匹配: {screenFit}");
        Debug.Log($"调整后尺寸: {screenSize.x} × {screenSize.y}");
    }

    public void UIInit()
    {
        uIPath = Path.Combine(Application.persistentDataPath, "UIData.json");

        // 第一阶段：加载或初始化 UI 数据
        if (File.Exists(uIPath)) LoadUIData();
        else ui_Size = new UI_Size();



        // 第三阶段：检查 UI 数据是否可用，否则重建
        if (!IsScreenDataReady())
        {
            ui_Size = new UI_Size();
        }

        ui_Size.size = screenSize;

        // 第四阶段：同步 ui（UI_Size → ui）
        ui = ui_Size.ui;
        ui.canvasSize = screenSize;
        scale = new Vector2(screenSize.x / 1920f, screenSize.y / 1080f);
        // 第五阶段：保存一次最终数据
        SaveUIData();


    }

    bool isAreaSizeInit = false;
    bool isAreaPosInit = false;
    bool isPlayAreaInit = false;
    bool isPlayerAreaInit = false;
    public bool IsGameEndInit() => isAreaSizeInit && isAreaPosInit && isPlayAreaInit && isPlayerAreaInit;


    public void GameInit()
    {
        if (!isAreaSizeInit) AreaSizeInit();
        if (!isAreaPosInit) AreaPosInit();
        if (!isPlayAreaInit) PlayAreaInit();
        if (!isPlayerAreaInit) PlayerAreaInit();
    }


    void AreaSizeInit()
    {
        //int screenWidth = Screen.width;
        //int screenHeight = Screen.height;

        float scaleByWidth = screenSize.x / 24f;
        float scaleByHeight = screenSize.y / 9;


        ui.playerInformArea = new Vector2(screenSize.x, 0.75f * scaleByHeight);
        ui.playArea = new Vector2(22f * scaleByWidth, 7f * scaleByHeight);
        ui.playerArea = new Vector2(screenSize.x, 1.25f * scaleByHeight);

        isAreaSizeInit = true;
    }
    void AreaPosInit()
    {
        float canvasHeight = canvasRect.rect.height;

        // 设置 Image 大小
        playInformArea.sizeDelta = ui.playerInformArea;
        playArea.sizeDelta = ui.playArea;
        playerArea.sizeDelta = ui.playerArea;

        playInformArea.pivot = new Vector2(playInformArea.pivot.x, 1f);
        playArea.pivot = new Vector2(playArea.pivot.x, 1f);
        playerArea.pivot = new Vector2(playerArea.pivot.x, 1f);

        float playInformAreaPosY = canvasHeight / 2;
        float playAreaPosY = playInformAreaPosY - ui.playerInformArea.y;
        float playerAreaPosY = playAreaPosY - ui.playArea.y;

        playInformArea.localPosition = new Vector3(0, playInformAreaPosY, playInformArea.localPosition.z);
        playArea.localPosition = new Vector3(0, playAreaPosY, playInformArea.localPosition.z);
        playerArea.localPosition = new Vector3(0f, playerAreaPosY, playInformArea.localPosition.z);

        ui.playInformAreaPos = playInformArea.localPosition;
        ui.playAreaPos = playArea.localPosition;
        ui.playerAreaPos = playerArea.localPosition;

        Debug.Log(ui.playInformAreaPos);
        Debug.Log(ui.playAreaPos);
        Debug.Log(ui.playerAreaPos);

        isAreaPosInit = true;

    }
    void PlayAreaInit()
    {

        GameData.playAreaPosition = playArea.position;
        GameData.worldCorners = new Vector3[4];
        playArea.GetWorldCorners(GameData.worldCorners);
        isPlayAreaInit = true;
    }
    void PlayerAreaInit()
    {
        Vector3[] canvasCorners = new Vector3[4];
        canvasRect.GetWorldCorners(canvasCorners);

        float dx = Mathf.Abs(canvasCorners[0].x - canvasCorners[2].x);
        float delta = dx / GameData.deltaLine;

        GameData.moveDelta = new float[GameData.deltaLine + 2];

        GameData.moveDelta[0] = canvasCorners[0].x - delta / 2;

        for (int x = 1; x < GameData.moveDelta.Length; x++)
        {
            GameData.moveDelta[x] = GameData.moveDelta[0] + x * delta;
            Debug.Log(GameData.moveDelta[x]);
        }
        isPlayerAreaInit = true;
    }





    void LoadUIData()
    {
        string json = File.ReadAllText(uIPath);
        ui_Size = JsonUtility.FromJson<UI_Size>(json);
        ui = ui_Size.ui;
        Debug.Log("?取成功");
    }

    void SaveUIData()
    {
        string json = JsonUtility.ToJson(ui_Size, true);
        ui = ui_Size.ui;
        File.WriteAllText(uIPath, json);
        Debug.Log("保存成功: " + uIPath);
    }

    void DeleteJson()
    {
        if (File.Exists(uIPath))
        {
            File.Delete(uIPath);
            Debug.Log("已删除 JSON 文件");
        }
        else
        {
            Debug.Log("找不到可删除的 JSON 文件");
        }
    }


}
