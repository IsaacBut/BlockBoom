using UnityEngine;
using Data;
using System.Collections;
public class Player : MonoBehaviour
{
    public static Player instance;
    public GameObject bullet;
    public Vector2[] points;
    private float moveTime; // 每段移动时间

    public int nowBulletAmount;
    private bool canShoot = true;


    public float moveSpeed;


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

        moveTime = 60 / CSVReader.instance.ReadTargetCellIndex(GameData.levelStage, "B", 13);

        Debug.Log(nowBulletAmount);
        StartCoroutine(MoveThroughPointsLoop());
    }
    public bool IsEmptyBullet() => nowBulletAmount <= 0;
    const int startIngIndex = 1;


    void BulletGen()
    {
        if(nowBulletAmount > 0)
        {
            Instantiate(bullet, new Vector3(this.transform.position.x , this.transform.position.y, 0), Quaternion.identity);
            //+GameData.blockSize
            nowBulletAmount--;
        }
        canShoot = false;
    }


    IEnumerator MoveThroughPointsLoop()
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
                canShoot = true;
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

        if (Input.GetKeyDown(KeyCode.Space)&& canShoot) 
        {
            BulletGen();
        }
    }



}
