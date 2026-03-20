using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test_PlayerMove : MonoBehaviour
{
    public RectTransform mainRoad;
    public RectTransform[] roadPoint;
    public RectTransform[] hardAndTail;

    private UI ui => UIManager.Instance.ui;


#if DEVELOPMENT_BUILD || UNITY_EDITOR
    public void RoadInit(List<Vector3> targetPos)
    {
        transform.position = new Vector3(0, targetPos[0].y, 0);

        float posY = transform.position.y;


        hardAndTail[0].sizeDelta = new Vector2(10, 250);
        //hardAndTail[0].position = new Vector2(ui.moveDelta[0], posY);
        hardAndTail[0].position = new Vector3(targetPos[0].x, targetPos[0].y, 0);


        hardAndTail[1].sizeDelta = new Vector2(10, 250);
        //hardAndTail[1].position = new Vector2(ui.moveDelta[9], posY);
        hardAndTail[1].position = new Vector3(targetPos[targetPos.Count - 1].x, targetPos[targetPos.Count - 1].y, 0);


        float sizeX = Mathf.Abs(hardAndTail[0].localPosition.x) + Mathf.Abs(hardAndTail[1].localPosition.x);

        mainRoad.sizeDelta = new Vector2(sizeX, 10);
        //mainRoad.position = new Vector3(0, -75, -1);



        for (int i = 0; i < roadPoint.Length; i++)
        {
            roadPoint[i].sizeDelta = new Vector2(10, 500);
            roadPoint[i].position = new Vector3(targetPos[i + 1].x, targetPos[i + 1].y, 0);


            //roadPoint[i].position = new Vector2(ui.moveDelta[i + 1], posY);
        }



    }

#else

    public void RoadInit(List<Vector3> targetPos)
    {
        Destroy(this.gameObject);
    }


#endif



}
