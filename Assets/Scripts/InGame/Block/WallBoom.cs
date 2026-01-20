using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;
using System.Collections;


public class WallBoom : MonoBehaviour
{
    [SerializeField] private HashSet<GameObject> walls = new HashSet<GameObject>();
    [SerializeField] private Dictionary<int, HashSet<Wall>> wallDestroyOrder;

    //bool isFindConnectingWall = false;
    //bool isDestroy = false;

    private static readonly Vector2[] directions =
    {
        new Vector2(1, 0),
        new Vector2(-1, 0),
        new Vector2(0, 1),
        new Vector2(0, -1)
    };

    public void FindConnectingWall()
    {
        if (!this.gameObject.activeSelf) return;
        HashSet<Wall> connectingWall = new HashSet<Wall>();
        HashSet<Wall> nextLayer = new HashSet<Wall>();

        int dis = 1;
        wallDestroyOrder = new Dictionary<int, HashSet<Wall>>();

        for (int d = 0; d < directions.Length - 1; d++) //Didnt Check Down In First Loop
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, directions[d], InGame.Instance.blockSize);

            for (int j = 0; j < hits.Length; j++)
            {
                Wall wall = hits[j].collider.GetComponent<Wall>();
                if (wall != null && wall.GetWallType() == Wall.WallType.BreakAble && !connectingWall.Contains(wall))
                {
                    walls.Add(wall.gameObject);
                    connectingWall.Add(wall);
                    nextLayer.Add(wall);
                }

            }

        }
        wallDestroyOrder.Add(dis, nextLayer);
        bool finish = false;
        while (!finish)
        {
            bool goFinish = true;
            HashSet<Wall> visited = new HashSet<Wall>();
            dis += 1;
            nextLayer = new HashSet<Wall>();

            var currentWalls = connectingWall.ToList();

            foreach (Wall connectedWall in currentWalls)
            {
                for (int d = 0; d < directions.Length; d++)
                {
                    RaycastHit2D[] hits = Physics2D.RaycastAll(connectedWall.transform.position, directions[d], InGame.Instance.blockSize);
                    foreach (RaycastHit2D detectedWall in hits)
                    {
                        Wall wall = detectedWall.collider.GetComponent<Wall>();

                        if (wall == connectedWall) continue;
                        if (wall != null && wall.GetWallType() == Wall.WallType.BreakAble && !connectingWall.Contains(wall))
                        {
                            walls.Add(wall.gameObject);
                            goFinish = false;
                            visited.Add(wall);
                            nextLayer.Add(wall);
                        }
                    }
                }
            }
            wallDestroyOrder.Add(dis, nextLayer);
            //Debug.Log(dis+"   "+wallDestroyOrder[dis].Count());
            foreach (var wall in visited)
                connectingWall.Add(wall);

            if (goFinish)
                finish = true;
        }



    }

    public void IsDestroy() { StartCoroutine(GoDestroy()); }


    IEnumerator GoDestroy()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;
        // 一层一层销毁（自动按顺序）
        foreach (var kvp in wallDestroyOrder.OrderBy(k => k.Key))
        {
            foreach (Wall wall in kvp.Value)
            {
                if (wall != null)
                {
                    //wall.enabled = false;
                    wall.gameObject.SetActive(false);
                }

            }
            yield return new WaitForSeconds(InGame.Instance.wallBoomSpreadSpeed);
            //Debug.Log($"销毁第 {kvp.Key} 层，共 {kvp.Value.Count} 个墙");
        }

        foreach (var wall in walls)
        {
            if (walls != null)
            {
                Destroy(wall);
            }
        }

        Destroy(gameObject);
    }

#if UNITY_EDITOR

    public bool isBoom = false;

#endif

#if UNITY_EDITOR

    private void Update()
    {
        if (isBoom) StartCoroutine(GoDestroy());
    }

#endif

}
