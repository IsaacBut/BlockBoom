using System.Collections.Generic;
using UnityEngine;
using Data;

using System.Linq;
using Unity.VisualScripting;
using static Unity.Collections.AllocatorManager;
using UnityEngine.Rendering.Universal;

public class Flame : MonoBehaviour
{
    float maxRadius;
    float speed;

    public bool isFound = false;
    float eps = 0.001f;
    int flameCanBrakeBlockNum = 0;

    int wallNum;


    List<GameObject> CanBrakeObject = new List<GameObject>();

    //public void FlameSetList(List<GameObject> targetList) => CanBrakeObject = targetList;

    public void FlameInit()
    {
        maxRadius = GameData.flameMaxRadius;
        speed = GameData.spreadSpeed;
        wallNum = Manager.instance.wallGameObjectList.Count;

        Debug.Log(maxRadius);
        Debug.Log(speed);

    }

    bool IsCanBrake(GameObject targetGameObject)
    {
        for (int i = 0; i < CanBrakeObject.Count; i++)
        {
            if (targetGameObject == CanBrakeObject[i]) return true;
        }
        return false;
    }

    bool IsMaxRadius()
    {
        if (this.transform.localScale.x >= maxRadius && this.transform.localScale.y >= maxRadius) return true;
        return false;
    }

    void Bigger()
    {

        if (!IsMaxRadius())
        {
            this.transform.localScale += new Vector3(0.1f, 0.1f, 0) * (1 + speed);
            //this.transform.localScale *= (1 + GameData.spreadSpeed);

        }
        else
        {
            FlameDestroy();
        }

    }

    void FlameDestroy()
    {
        Destroy(gameObject);
    }

    void Update()
    {
        if (Manager.instance.gameSet) FlameDestroy();

        if (wallNum != Manager.instance.wallGameObjectList.Count)
        {
            isFound = false;
        }
        if (!isFound) { FindFlameCanBrakeBlock(); }
        Bigger();
    }

    public void FindFlameCanBrakeBlock()
    {
        if (!this.gameObject.activeSelf) return;
        if (!Manager.instance.isMapBuild) return;

        CanBrakeObject.Clear();
        Vector2 laserStart = new Vector2(this.transform.position.x, this.transform.position.y);

        //By Wall
        for (int i = 0; i < Manager.instance.wallGameObjectList.Count; i++)
        {
            Vector2[] targetWallFourPoint = Manager.instance.wallGameObjectList[i].GetWallFourPoint();

            for (int j = 0; j < targetWallFourPoint.Length; j++)
            {
                Vector2 delta = targetWallFourPoint[j] - laserStart;
                Vector2 direction = delta.normalized;

                RaycastHit2D[] hits = Physics2D.RaycastAll(laserStart, direction, GameData.distance);
                Debug.DrawLine(laserStart, laserStart + direction * GameData.distance, Color.red);

                for (int h = 0; h < hits.Length; h++)
                {
                    GameObject hitObject = hits[h].collider.gameObject;

                    if (hitObject == this) break;

                    if (hitObject.CompareTag("CantBrake"))
                    {
                        //Debug.Log(hitObject.name);
                        //Debug.Log("Cant Brake");
                        break;
                    }

                    if (hitObject.CompareTag("CanBrake"))
                    {
                        Wall wall = hitObject.GetComponent<Wall>();
                        if (wall != null) break;

                        WallBoom wallBoom = hitObject.GetComponent<WallBoom>();
                        if (wallBoom != null) break;

                        //Debug.Log(hitObject.name);
                        CanBrakeObject.Add(hitObject);

                    }
                }
            }
        }

        //By Diection
        for (int i = 0; i < Manager.instance.areaLimit.Count; i++)
        {
            if ((laserStart - Manager.instance.areaLimit[i]).sqrMagnitude <= eps * eps) continue;

            Vector2 dir = (Manager.instance.areaLimit[i] - laserStart).normalized;
            RaycastHit2D[] hits = Physics2D.RaycastAll(laserStart, dir, GameData.distance);
            Debug.DrawLine(laserStart, laserStart + Manager.instance.areaLimit[i] * GameData.distance, Color.red);


            for (int h = 0; h < hits.Length; h++)
            {
                GameObject hitObject = hits[h].collider.gameObject;

                if (hitObject == this) break;

                if (hitObject.CompareTag("CantBrake"))
                {
                    //Debug.Log(hitObject.name);
                    //Debug.Log("Cant Brake");
                    break;
                }

                if (hitObject.CompareTag("CanBrake"))
                {
                    Wall wall = hitObject.GetComponent<Wall>();
                    if (wall != null) break;
                    WallBoom wallBoom = hitObject.GetComponent<WallBoom>();
                    if (wallBoom != null) break;
                    //Debug.Log(hitObject.name);
                    CanBrakeObject.Add(hitObject);

                }
            }

        }

        CanBrakeObject = CanBrakeObject.Distinct().ToList();

        flameCanBrakeBlockNum = CanBrakeObject.Count;

        isFound = true;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log(collision.gameObject);
        switch (Manager.instance.FindObjectTag(collision.gameObject))
        {
            case "Player":

                break;

            case "CanBrake":

                if (IsCanBrake(collision.gameObject))
                {
                    Block block = collision.gameObject.GetComponent<Block>();
                    Boom boom = collision.gameObject.GetComponent<Boom>();
                    if (boom != null) boom.GoDestroy();
                    if (block != null) block.BlockDestroy();
                }

                break;

            case "CantBrake":

                break;

            default:
                break;
        }


    }



}
