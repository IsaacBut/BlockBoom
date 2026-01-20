using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    public static Player Instance;
    private InGame inGame;
    Transform image => this.transform.GetChild(0) as Transform;
    private const float cantShootAnaimeTime = 0.0025f;
    private readonly Vector3 moveDis = new Vector3(0.1f, 0, 0);

    private Keyboard keyboard => GameManager.Instance.keyboard;
    private Touchscreen touchscreen => GameManager.Instance.touchscreen;
    private Mouse mouse => GameManager.Instance.mouse;

    public GameObject bulletPrefab;
    public int bulletMax;
    public int nowbullet;

    public bool canShoot = true;
    private float shootCD;
    private float shootCDTimer;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void Init(float setShootCD)
    {
        shootCD = setShootCD;
        nowbullet = bulletMax;
        inGame = InGame.Instance;
    }

#if UNITY_STANDALONE
    public void ShootBullet()
    {
        if (!canShoot)
        {
            shootCDTimer += Time.deltaTime;
            if (shootCDTimer > shootCD)
            {
                shootCDTimer = 0;
                canShoot = true;
            }
        }
        if (keyboard != null && keyboard.spaceKey.wasPressedThisFrame)
        {
            if (!canShoot) { StartCoroutine(CantShoot()); return; }

            else if(nowbullet > 0)
            {
                Instantiate(bulletPrefab, transform.position, Quaternion.identity);
                StartCoroutine(CanShoot());
                //+GameData.blockSize
                nowbullet--;
                canShoot = false;
            }

                
        }




        
    
    }

#endif

#if UNITY_ANDROID
    public void ShootBullet()
    {
        if (!canShoot)
        {
            shootCDTimer += Time.deltaTime;
            if (shootCDTimer > shootCD)
            {
                shootCDTimer = 0;
                canShoot = true;
            }
        }

        if (touchscreen != null)
        {
            var touch = touchscreen.primaryTouch;
            if (touch.press.wasPressedThisFrame)
            {
                if (!canShoot) { StartCoroutine(CantShoot()); return; }
                else if (nowbullet > 0)
                {
                    Instantiate(bulletPrefab, transform.position, Quaternion.identity);
                    StartCoroutine(CanShoot());
                    //+GameData.blockSize
                    nowbullet--;
                    canShoot = false;
                }
            }

        }
    }

#endif

    private IEnumerator CantShoot()
    {
        int loopCount = 0;
        while (loopCount < 2)
        {
            if (canShoot) break;
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

        transform.localScale = start;

        while (time < second)
        {
            time += Time.deltaTime;
            float t = time / second;     // 0 ¨ 1
            transform.localScale = Vector3.Lerp(start, end, t);
            yield return null;
        }

        transform.localScale = end;
    }
}
