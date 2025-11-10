using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Data;
using System.Linq;
using Unity.VisualScripting;
using static Unity.Collections.AllocatorManager;
using UnityEngine.Rendering.Universal;

public class Boom : MonoBehaviour
{
    public GameObject flame;

    public List<GameObject> flameCanBrakeBlock = new List<GameObject>();

    public bool isFound = false;
    int flameCanBrakeBlockNum = 0;
    float eps = 0.001f;

#if UNITY_EDITOR

    public bool isBoom = false;

#endif


    public void GoDestroy()
    {
        this.gameObject.SetActive(false);

        Vector3 startPoint = new Vector3(this.transform.position.x, this.transform.position.y, -0.11f);
        GameObject flamePrefab = Instantiate(flame, startPoint, Quaternion.identity);
        flamePrefab.GetComponent<Flame>().FlameSetList(flameCanBrakeBlock);
        flamePrefab.GetComponent<Flame>().FlameInit();

        Destroy(this.gameObject);
    }

    public void FindFlameCanBrakeBlock()
    {
        if (!this.gameObject.activeSelf) return;
        if (!Manager.instance.isMapBuild) return;

        flameCanBrakeBlock.Clear();
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
                        WallBoom wallBoom = hitObject.GetComponent<WallBoom>();
                        if (wallBoom != null) break;
                        //Debug.Log(hitObject.name);
                        flameCanBrakeBlock.Add(hitObject);

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
                    WallBoom wallBoom = hitObject.GetComponent<WallBoom>();
                    if (wallBoom != null) break;
                    //Debug.Log(hitObject.name);
                    flameCanBrakeBlock.Add(hitObject);

                }
            }

        }

        flameCanBrakeBlock = flameCanBrakeBlock.Distinct().ToList();

        flameCanBrakeBlockNum = flameCanBrakeBlock.Count;

        isFound = true;
    }

    void Test()
    {
        if (flameCanBrakeBlock.Count != 0)
        {
            foreach (GameObject obj in flameCanBrakeBlock)
            {
                Destroy(obj);

            }
            flameCanBrakeBlock.RemoveAll(item => item == null);

        }

    }

    private void Update()
    {

        if (flameCanBrakeBlock.Count != flameCanBrakeBlockNum)
        {
            isFound = false;
        }
        if (!isFound) { FindFlameCanBrakeBlock(); }

#if UNITY_EDITOR

        if (isBoom) GoDestroy();

#endif


    }

}
