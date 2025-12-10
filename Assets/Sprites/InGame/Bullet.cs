using Unity.VisualScripting;
using UnityEngine;
using Data;
using UnityEngine.Rendering.Universal;
using static Unity.Collections.AllocatorManager;


public class Bullet : MonoBehaviour
{
    float moveSpeed = 25f;
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
        if (Player.instance.lazerMode) flyingTime += Time.deltaTime * 100;
        else BulletMove();
        
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
        if(Player.instance.lazerMode) return;
        //Debug.Log(collision.gameObject);
        switch (Manager.instance.FindObjectTag(collision.gameObject))
        {
            case "Player":

                break;

            case "CanBrake":
                touchBlock = true;

                boom = collision.gameObject.GetComponent<Boom>();
                if (boom != null)
                {
                    boom.GoDestroy();
                    BulletDestroy();

                    break;
                }

                wallBoom = collision.gameObject.GetComponent<WallBoom>();
                if (wallBoom != null)
                {
                    wallBoom.IsDestroy();
                    BulletDestroy();
                    break;
                }

                block = collision.gameObject.GetComponent<Block>();
                if (block != null)
                {
                    block.canSourePlus = false;
                    block.BlockDestroy();
                    BulletDestroy();

                    break;
                }

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
