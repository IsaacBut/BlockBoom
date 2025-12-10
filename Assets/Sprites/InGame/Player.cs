using UnityEngine;
using Data;
using System.Collections;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

public class Player : MonoBehaviour
{
    public static Player instance;
    public GameObject bullet;
    public Vector2[] points;
    private float moveTime; // 每段移动时间

    public int nowBulletAmount;
    public bool lazerMode = false;
    Keyboard _keyboard => Manager.instance.keyboard;
    Touchscreen _touchscreen => Manager.instance.touchscreen;
    bool gameStart => Manager.instance.isGameStart;
    Transform image => this.transform.GetChild(0) as Transform;

    public void PlayerInit()
    {
        points = new Vector2[GameData.moveDelta.Length];

        for (int x = 0; x < GameData.moveDelta.Length; x++) 
        {
            points[x] = new Vector2(GameData.moveDelta[x], this.transform.position.y);
        }
        nowBulletAmount = (int)GameData.bulletMaxAmount;

        /*        
        Vector3 playerAreaCenter = playerArea.TransformPoint(playerArea.rect.center);
        float playerPosY = playerAreaCenter.y - (Player.instance.gameObject.GetComponent<BoxCollider2D>().bounds.extents.y / 2);
        Player.instance.gameObject.transform.position = new Vector2(Player.instance.gameObject.transform.position.x, playerPosY);
        */
        moveTime = GameData.playerMoveTime;

        this.transform.position = points[1];
    }
    public bool IsEmptyBullet() => nowBulletAmount <= 0;
    const int startIngIndex = 1;

    private Vector2 shootPos;

    void LazerMode()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(shootPos, Vector2.up, 1000);
        //Debug.DrawLine((Vector2)this.transform.position, (Vector2)this.transform.position + Vector2.up * GameData.distance, Color.red);

        for (int h = 0; h < hits.Length; h++)
        {
            GameObject hitObject = hits[h].collider.gameObject;

            if (hitObject == this) break;

            if (hitObject.CompareTag("CantBrake")) break;

            if (hitObject.CompareTag("CanBrake"))
            {
                Wall wall = hitObject.GetComponent<Wall>();
                if (wall != null) break;

                WallBoom wallBoom = hitObject.GetComponent<WallBoom>();
                if (wallBoom != null)
                {
                    wallBoom.IsDestroy();
                    break;
                }
                Boom boom = hitObject.GetComponent<Boom>();
                if (boom != null)
                {
                    boom.GoDestroy();
                    break;
                }
                Block block = hitObject.GetComponent<Block>();
                if (block != null)
                {
                    block.canSourePlus = false;
                    block.BlockDestroy();

                    break;
                }

            }


        }

        Instantiate(bullet, this.transform.position, Quaternion.identity);
    }

    public IEnumerator MoveThroughPointsLoop()
    {
        int currentIndex = startIngIndex;

        while (true)
        {
            Vector2 startPos = points[currentIndex];
            Vector2 endPos;

            // 判断是否到最后一个点
            if (currentIndex == points.Length - 1)
            {
                // 直接跳回第一个点
                transform.position = new Vector3(points[0].x, points[0].y, transform.position.z);
                currentIndex = 0;
                yield return null;
                continue; // 重新开始下一段移动
            }
            else
            {
                endPos = points[currentIndex + 1];
            }

            float elapsed = 0f;
            while (elapsed < moveTime)
            {
                if (Time.timeScale > 0)
                {
                    elapsed += Time.deltaTime;
                    float t = Mathf.Clamp01(elapsed / moveTime);
                    transform.position = Vector3.Lerp(
                        new Vector3(startPos.x, startPos.y, transform.position.z),
                        new Vector3(endPos.x, endPos.y, transform.position.z),
                        t
                    );
                }
                yield return null;
            }

            transform.position = new Vector3(endPos.x, endPos.y, transform.position.z); // 确保精确到达
            currentIndex++;
        }
    }

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
    }


    private void Update()
    {
        if (!gameStart) return;

        if (_keyboard != null && _keyboard.spaceKey.wasPressedThisFrame)
        {
            shootPos = transform.position;
            BulletGen();
        }

        if (_touchscreen != null)
        {
            var touch = _touchscreen.primaryTouch;

            if (touch.press.wasPressedThisFrame)
            {
                shootPos = transform.position;
                BulletGen();
            }
        }

    }

    Coroutine cantShoot;

    private const float cantShootAnaimeTime = 0.0025f;

    private readonly Vector3 moveDis = new Vector3(0.1f, 0, 0);
    private const int loop = 2;

    void BulletGen()
    {
        if (!Manager.instance.playerCanShoot)
        {
            if (cantShoot == null) StartCoroutine(CantShoot());
            return;
        }
        else if (nowBulletAmount > 0)
        {
            if (lazerMode) LazerMode();
            else Instantiate(bullet, shootPos, Quaternion.identity);
            StartCoroutine(CanShoot());
            //+GameData.blockSize
            nowBulletAmount--;
        }
        Manager.instance.Shot();
    }



    IEnumerator CantShoot()
    {
        int loopCount = 0;
        while (loopCount < loop)
        {
            if (Manager.instance.playerCanShoot) break;
            image.position -= moveDis;
            yield return new WaitForSeconds(cantShootAnaimeTime);
            image.position += moveDis;
            yield return new WaitForSeconds(cantShootAnaimeTime);
            image.position += moveDis;
            yield return new WaitForSeconds(cantShootAnaimeTime);
            image.position -= moveDis;
            yield return new WaitForSeconds(cantShootAnaimeTime);
            loopCount++;
        }
    }


    private const float shootAnaimeTime = 0.1f;
    Vector3 scale => image.localScale;
    Vector3 biggerScale => image.localScale * 1.3f;
    Vector3 smallerScale => image.localScale * 0.9f;



    IEnumerator CanShoot()
    {
        StartCoroutine(ScaleCoroutine(scale, biggerScale, shootAnaimeTime / 4));
        yield return new WaitForFixedUpdate();
        StartCoroutine(ScaleCoroutine(biggerScale, scale, shootAnaimeTime / 4));
        yield return new WaitForFixedUpdate();
        StartCoroutine(ScaleCoroutine(scale, smallerScale, shootAnaimeTime / 4));
        yield return new WaitForFixedUpdate();
        StartCoroutine(ScaleCoroutine(smallerScale, scale, shootAnaimeTime / 4));
        yield return new WaitForFixedUpdate();

    }
    public IEnumerator ScaleCoroutine(Vector3 start, Vector3 end, float second)
    {
        float time = 0f;

        // 设置初始 Scale
        transform.localScale = start;

        while (time < second)
        {
            time += Time.deltaTime;
            float t = time / second;     // 0 → 1
            transform.localScale = Vector3.Lerp(start, end, t);
            yield return null;
        }

        // 确保最终精确到达
        transform.localScale = end;
    }
}
