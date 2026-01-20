using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float moveSpeed = 25f;
    private float boundsX, boundsY;
    private Collider2D cd;
    private Block block;
    private Boom boom;
    private WallBoom wallBoom;

    private const float flytime = 1.5f;
    private float flyingTime = 0;
    private bool touchBlock = false;
    private void Awake()
    {
        cd = this.GetComponent<Collider2D>();
        boundsX = cd.bounds.extents.x;
        boundsY = cd.bounds.extents.y;
    }

    private void Update()
    {
        //if (Player.instance.lazerMode) flyingTime += Time.deltaTime * 100;
        /*else */BulletMove();

    }
    private void BulletDestroy() => Destroy(gameObject);

    private void BulletMove()
    {
        if (flyingTime < flytime)
            this.transform.position += Vector3.up * Time.deltaTime * moveSpeed;
        else Destroy(gameObject);
    }

    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (touchBlock) return;
        //if (Player.instance.lazerMode) return;
        Debug.Log(collision.gameObject);
        switch (collision.gameObject.tag)
        {
            case "Player": break;

            case "Block":

                touchBlock = true;
                block = collision.gameObject.GetComponent<Block>();
                if (block != null)
                {
                    block.canSourePlus = false;
                    block.BlockDestroy();
                }

                BulletDestroy();
                break;

            case "Boom":

                touchBlock = true;
                boom = collision.gameObject.GetComponent<Boom>();
                if (boom != null) boom.GoDestroy();
                BulletDestroy();
                break;

            case "WallBoom":
                touchBlock = true;
                wallBoom = collision.gameObject.GetComponent<WallBoom>();
                if (wallBoom != null) wallBoom.IsDestroy();
                BulletDestroy();
                break;


            case "Wall":
                touchBlock = true;
                BulletDestroy();
                break;

            default:
                break;

        }
    }

}
