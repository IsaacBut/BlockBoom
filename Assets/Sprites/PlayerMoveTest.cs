using Data;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMoveTest : MonoBehaviour
{
    public RectTransform mainRoad;
    public RectTransform[] roadPoint;
    public RectTransform[] hardAndTail;

    public void RoadInit()
    {
        float posY = mainRoad.position.y;


        hardAndTail[0].sizeDelta = new Vector2(10, Manager.instance.playerArea.sizeDelta.y);
        hardAndTail[0].position = new Vector2(GameData.moveDelta[0], posY);

        hardAndTail[1].sizeDelta = new Vector2(10, Manager.instance.playerArea.sizeDelta.y);
        hardAndTail[1].position = new Vector2(GameData.moveDelta[9], posY);

        float sizeX = Mathf.Abs(hardAndTail[0].localPosition.x) + Mathf.Abs(hardAndTail[1].localPosition.x);

        mainRoad.sizeDelta = new Vector2(sizeX, 10);
        mainRoad.localPosition = new Vector3(0, -75, -1);


    
        for (int i = 0; i < roadPoint.Length; i++)
        {
            roadPoint[i].sizeDelta = new Vector2(10, Manager.instance.playerArea.sizeDelta.y);
            roadPoint[i].position = new Vector2(GameData.moveDelta[i+1], posY);
        }



    }


}
