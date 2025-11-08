using Unity.VisualScripting;
using UnityEngine;
using Data;
using UnityEngine.Rendering.Universal;
using static Unity.Collections.AllocatorManager;


public class Bullet : MonoBehaviour
{
    float moveSpeed = GameData.bulletMoveSpeed;
    float boundsX, boundsY;
    Collider2D cd;

    Block block;
    Boom boom;
    WallBoom wallBoom;

    const float flytime = 1.5f;
    float flyingTime = 0;

    private void Awake()
    {
        cd = this.GetComponent<Collider2D>();
        boundsX = cd.bounds.extents.x;
        boundsY = cd.bounds.extents.y;
    }   

    private void Update()
    {
        flyingTime += Time.deltaTime;
        BulletMove();
    }

    void BulletMove()
    {
        if (flyingTime< flytime) 
            this.transform.position += Vector3.up * Time.deltaTime * moveSpeed;
        else Destroy(gameObject);
    }

    bool touchBlock = false;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (touchBlock) return;

        //Debug.Log(collision.gameObject);
        switch (Manager.instance.FindObjectTag(collision.gameObject))
        {
            case "Player":

                break;

            case "CanBrake":
                touchBlock = true;

                block = collision.gameObject.GetComponent<Block>();
                boom = collision.gameObject.GetComponent<Boom>();
                wallBoom = collision.gameObject.GetComponent<WallBoom>();

                if (block != null)
                {
                    block.canSourePlus = false;
                    block.BlockDestroy();
                }
                if (boom != null) boom.GoDestroy();
                if (wallBoom != null) wallBoom.IsDestroy();

                BulletDestroy();
                break;

            case "CantBrake":
                touchBlock = true;
                BulletDestroy();

                break;

            default:
                break;
        }


    }

    void BulletDestroy()
    {
        Destroy(gameObject);
    }

}
