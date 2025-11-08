using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using Data;
using System.Linq;
using Unity.VisualScripting;

public class WallBoom : MonoBehaviour
{
    public HashSet<Wall> connectingWall;
    bool isFindConnectingWall = false;
    bool isDestroy = false;

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
        connectingWall = new HashSet<Wall>();

        //List<Wall> visited = new List<Wall>();


        for (int d = 0; d < directions.Length - 1; d++) //Didnt Check Down In First Loop
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, directions[d], GameData.blockSize);

            for (int j = 0; j < hits.Length; j++)
            {
                Wall wall = hits[j].collider.GetComponent<Wall>();
                if (wall != null && wall.GetWallType() == Wall.WallType.BreakAble && !connectingWall.Contains(wall)) 
                {
                    connectingWall.Add(wall);      
                }

            }

        }
        bool finish = false;
        while (!finish)
        {
            bool goFinish = true;
            HashSet<Wall> visited = new HashSet<Wall>();

            foreach(Wall connectedWall in connectingWall)
            {
                for (int d = 0; d < directions.Length; d++)
                {
                    RaycastHit2D[] hits = Physics2D.RaycastAll(connectedWall.transform.position, directions[d], GameData.blockSize);

                    foreach(RaycastHit2D dectedWall in hits)
                    {
                        Wall wall = dectedWall.collider.GetComponent<Wall>();

                        if (wall != null && wall.GetWallType() == Wall.WallType.BreakAble && !connectingWall.Contains(wall))
                        {
                            goFinish = false;
                            visited.Add(wall);
                        }
                    }
                }

            }


            connectingWall.AddRange(visited);
            if (goFinish) finish = true;
        }
        
    }

    public void IsDestroy() { GoDestroy(); }

    void GoDestroy()
    {
        this.gameObject.SetActive(false);

        foreach (Wall wall in connectingWall)
        {
            Destroy(wall.gameObject);
        }

        Destroy(this.gameObject);
    }

#if UNITY_EDITOR

    public bool isBoom = false;

#endif

#if UNITY_EDITOR

    private void Update()
    {
        if (isBoom) GoDestroy();

    }

#endif


}
