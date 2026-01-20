using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class UI
{
    public Vector2 canvasSize;

    public Vector2 playerInformArea;
    public Vector2 playArea;
    public Vector2 playerArea;
    public Vector2 lightArea;

    public Vector3 playInformAreaPos;
    public Vector3 playAreaPos;
    public Vector3 leftLightPos;
    public Vector3 rightLightPos;
    public Vector3 playerAreaPos;

    public float[] moveDelta;
    public Vector3 playAreaPosition;
    public Vector3[] worldCorners;
    public float distance;

    public Vector2 buttleImageArea;
    public Vector2 buttleIndexArea;
    public Vector2 bestScoreImageArea;
    public Vector2 bestScoreIndexArea;
    public Vector2 nowScoreIndexArea;
    public Vector2 pauseArea;

    public Vector3 buttleImagePos;
    public Vector3 buttleIndexPos;
    public Vector3 bestScoreImagePos;
    public Vector3 bestScoreIndexPos;
    public Vector3 nowScoreIndexPos;
    public Vector3 pausePos;

}

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public UI ui;

    public Vector2Int screenSize { get; private set; }
    const float radio = 16f / 9f;
    private Vector2 scale;
    private bool screenFit;

    [Header("GameInit")]
    [SerializeField] private RectTransform canvasRect;
    [SerializeField] private RectTransform playInformArea;
    [SerializeField] private RectTransform playArea;
    [SerializeField] private RectTransform leftLight;
    [SerializeField] private RectTransform rightLight;
    [SerializeField] private RectTransform playerArea;

    [Header("Player Inform Area")]
    [SerializeField] private RectTransform buttleImage;
    [SerializeField] private RectTransform buttleIndex;
    [SerializeField] private RectTransform bestScoreImage;
    [SerializeField] private RectTransform bestScoreIndex;
    [SerializeField] private RectTransform nowScoreIndex;
    [SerializeField] private RectTransform pause;

    




    private int deltaLine = 8;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    public void Init()
    {
        ui = new UI();
        ScreenInit();
        Screen.SetResolution(screenSize.x, screenSize.y, FullScreenMode.FullScreenWindow);
        PlayGroundInit();
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
        else
        {
            screenWidth = width;
            screenHeight = height;
        }

        screenSize = new Vector2Int((int)screenWidth, (int)screenHeight);
        ui.canvasSize = screenSize;
        Debug.Log($"Now Radio: {nowScreenRadio:F4}, TargetRadio : {radio:F4}, is Fit => {screenFit}");
        Debug.Log($"Now Side: {screenSize.x} × {screenSize.y}");
    }

    bool isAreaSizeInit = false;
    bool isAreaPosInit = false;
    bool isPlayAreaInit = false;
    bool isPlayerAreaInit = false;
    bool isPlayerInformInit = false;
    public bool IsGameEndInit() => isAreaSizeInit && isAreaPosInit && isPlayAreaInit && isPlayerAreaInit && isPlayerInformInit;

    public void PlayGroundInit()
    {
        if (!isAreaSizeInit) AreaSizeInit();
        if (!isAreaPosInit) AreaPosInit();
        if (!isPlayerAreaInit) PlayerAreaInit();
        if (!isPlayAreaInit) PlayAreaInit();
        if (!isPlayerInformInit) PlayerInformInit();

    }
    private void AreaSizeInit()
    {

        float scaleByWidth = screenSize.x / 24f;
        float scaleByHeight = screenSize.y / 9;

        ui.playerInformArea = new Vector2(screenSize.x, 0.75f * scaleByHeight);
        ui.playArea = new Vector2(scaleByWidth * 22, 7f * scaleByHeight);
        ui.lightArea = new Vector2(scaleByWidth * 1f, 7f * scaleByHeight);
        ui.playerArea = new Vector2(screenSize.x, 1.25f * scaleByHeight);

        isAreaSizeInit = true;
    }
    private void AreaPosInit()
    {
        float canvasHeight = canvasRect.rect.height;
        float canvasWidth = canvasRect.rect.width;

        // 设置 Image 大小
        playInformArea.sizeDelta = ui.playerInformArea;
        playArea.sizeDelta = ui.playArea;
        playerArea.sizeDelta = ui.playerArea;
        leftLight.sizeDelta = ui.lightArea;
        rightLight.sizeDelta = ui.lightArea;

        playInformArea.pivot = new Vector2(playInformArea.pivot.x, 1f);
        playArea.pivot = new Vector2(playArea.pivot.x, 1f);
        leftLight.pivot = new Vector2(playArea.pivot.x, 1f);
        rightLight.pivot = new Vector2(playArea.pivot.x, 1f);
        playerArea.pivot = new Vector2(playerArea.pivot.x, 1f);

        float playInformAreaPosY = canvasHeight / 2;
        float playAreaPosY = playInformAreaPosY - ui.playerInformArea.y;
        float lightAreaPosX = canvasWidth / 2 - ui.lightArea.x / 2;
        //float rightLightAreaPosX = playAreaPosY - ui.playArea.y;
        float playerAreaPosY = playAreaPosY - ui.playArea.y;


        playInformArea.localPosition = new Vector3(0, playInformAreaPosY, playInformArea.localPosition.z);
        leftLight.localPosition = new Vector3(-lightAreaPosX, playAreaPosY, playInformArea.localPosition.z);
        playArea.localPosition = new Vector3(0, playAreaPosY, playInformArea.localPosition.z);
        rightLight.localPosition = new Vector3(lightAreaPosX, playAreaPosY, playInformArea.localPosition.z);
        playerArea.localPosition = new Vector3(0f, playerAreaPosY, playInformArea.localPosition.z);


        ui.playInformAreaPos = playInformArea.localPosition;
        ui.leftLightPos = leftLight.localPosition;
        ui.playAreaPos = playArea.localPosition;
        ui.rightLightPos = rightLight.localPosition;
        ui.playerAreaPos = playerArea.localPosition;

        //Debug.Log(ui.playInformAreaPos);
        //Debug.Log(ui.playAreaPos);
        //Debug.Log(ui.playerAreaPos);

        isAreaPosInit = true;

    }
    private void PlayerAreaInit()
    {
        Vector3[] canvasCorners = new Vector3[4];
        playerArea.GetWorldCorners(canvasCorners);

        float dx = Mathf.Abs(canvasCorners[0].x - canvasCorners[2].x);
        float delta = dx / deltaLine;

        ui.moveDelta = new float[deltaLine + 2];

        ui.moveDelta[0] = canvasCorners[0].x - delta / 2;

        for (int x = 1; x < ui.moveDelta.Length; x++)
        {
            ui.moveDelta[x] = ui.moveDelta[0] + x * delta;
            Debug.Log(ui.moveDelta[x]);
        }


        isPlayerAreaInit = true;
    }
    private void PlayAreaInit()
    {
        ui.playAreaPosition = playArea.position;

        // 获取 Canvas 及其相机
        Canvas rootCanvas = playArea.GetComponentInParent<Canvas>();
        Camera cam = rootCanvas.worldCamera;

        // 获取 Canvas 的 RectTransform（根对象）
        RectTransform canvasRect = rootCanvas.GetComponent<RectTransform>();

        // 将世界坐标转换到 Canvas 局部坐标
        Vector2 localPoint1, localPoint8;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            RectTransformUtility.WorldToScreenPoint(cam, new Vector3(ui.moveDelta[1], 0f, 0f)),
            cam,
            out localPoint1
        );
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            RectTransformUtility.WorldToScreenPoint(cam, new Vector3(ui.moveDelta[8], 0f, 0f)),
            cam,
            out localPoint8
        );

        // 计算在 UI 坐标下的宽度
        float width = Mathf.Abs(localPoint8.x - localPoint1.x);

        // 设置 UI 元素宽度
        playArea.sizeDelta = new Vector2(width, playArea.sizeDelta.y);

        //Debug.Log($"转换后宽度: {width} (Canvas Mode: {rootCanvas.renderMode}, Camera: {cam.name})");

        // 获取世界坐标四角（如需）
        ui.worldCorners = new Vector3[4];
        playArea.GetWorldCorners(ui.worldCorners);
        ui.distance = Vector3.Distance(ui.worldCorners[1], ui.worldCorners[2]);
        isPlayAreaInit = true;
    }

    private void PlayerInformInit()
    {
        float areaOffsetX = screenSize.x / 16;
        float areaY = ui.playerInformArea.y;

        float posY = -ui.playerInformArea.y / 2;
        float posZ = ui.playInformAreaPos.z;

        ui.buttleImageArea = new Vector2(areaOffsetX, areaY);
        ui.buttleIndexArea = new Vector2(areaOffsetX, areaY);
        ui.bestScoreImageArea = new Vector2(areaOffsetX * 3, areaY);
        ui.bestScoreIndexArea = new Vector2(areaOffsetX * 3, areaY);
        ui.nowScoreIndexArea = new Vector2(areaOffsetX * 3, areaY);
        ui.pauseArea = new Vector2(screenSize.x / 6, areaY);

        buttleImage.sizeDelta = ui.buttleImageArea;
        buttleIndex.sizeDelta = ui.buttleIndexArea;
        bestScoreImage.sizeDelta = ui.bestScoreImageArea;
        bestScoreIndex.sizeDelta = ui.bestScoreIndexArea;
        nowScoreIndex.sizeDelta = ui.nowScoreIndexArea;
        pause.sizeDelta = ui.pauseArea;

        ui.buttleImagePos = new Vector3(-(screenSize.x / 2) + (ui.buttleImageArea.x / 2), posY, posZ);
        ui.buttleIndexPos = new Vector3(ui.buttleImagePos.x + ui.buttleIndexArea.x, posY, posZ);
        ui.bestScoreImagePos = new Vector3(-ui.bestScoreImageArea.x, posY, posZ);
        ui.bestScoreIndexPos = new Vector3(0, posY, posZ);
        ui.pausePos = new Vector3((screenSize.x / 2) - (ui.pauseArea.x / 2), posY, posZ);
        float nowScoreIndexPosX = ui.pausePos.x - (ui.pauseArea.x / 2) - (ui.nowScoreIndexArea.x / 2);
        ui.nowScoreIndexPos = new Vector3(nowScoreIndexPosX, posY, posZ);

        buttleImage.localPosition = ui.buttleImagePos;
        buttleIndex.localPosition = ui.buttleIndexPos;
        bestScoreImage.localPosition = ui.bestScoreImagePos;
        bestScoreIndex.localPosition = ui.bestScoreIndexPos;
        nowScoreIndex.localPosition = ui.nowScoreIndexPos;
        pause.localPosition = ui.pausePos;



        isPlayerInformInit = true;
    }

}
