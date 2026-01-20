using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BoomFlame : MonoBehaviour
{
    public LayerMask blockLayer;
    private float maxRadius = 35;

    private float distance;
    private float speed;


    public GameObject flameObject;

    private HashSet<FlameBone> flameBones = new HashSet<FlameBone>();
    public HashSet<GameObject> canBrakeObject;

    public Vector3 parentScale;

    private int wallNumber;
    private float currentLength = 0.01f;

    private void FindAllTheBones()
    {
        flameBones.Clear();

        for (int i = 0; i < transform.childCount; i++)
        {
            FlameBone bone =
                transform.GetChild(i).GetComponent<FlameBone>();
            
            if (bone != null)
                flameBones.Add(bone);
        }

        Debug.Log($"FlameBones Count = {flameBones.Count}");
    }




    public void FlameInit(float setDistance, float setSpeed)
    {
        //maxRadius = GameData.flameMaxRadius;
        //speed = GameData.spreadSpeed;
        distance = setDistance;
        speed = setSpeed;

        Debug.Log(distance);
        Debug.Log(speed);


        //speed = 0.068f;
        parentScale = transform.lossyScale;
        FindAllTheBones();
        foreach (var bone in flameBones)
        {
            bone.maxLenght = maxRadius;
        }

        FindFlameCanBrakeBlock();

        //Debug.Log(maxRadius);
        //Debug.Log(speed);
        //wallNumber = InGame.Instance.wallGameObjectList.Count;
       // StartCoroutine(FlameGrow());
            
    }

    private void LateUpdate()
    {
        if(currentLength< maxRadius)
        {

            if (InGame.Instance.wallGameObjectList.Count != wallNumber)
            {
                //FindFlameCanBrakeBlock();
                wallNumber = InGame.Instance.wallGameObjectList.Count;
            }

            currentLength += distance * speed * Time.deltaTime;

            foreach (var bone in flameBones)
            {
                float length = Mathf.Min(currentLength, bone.maxLenght);

                if (bone != null) bone.SetLength(length / 2);

            }
            /*flameObject.*/
            flameObject.transform.localScale = new Vector3(currentLength, currentLength, currentLength);

        }
        else
        {
            Destroy(gameObject);
        }

    }

    public void FindFlameCanBrakeBlock()
    {
        canBrakeObject = new HashSet<GameObject>();
        if (!this.gameObject.activeSelf) return;
        Vector2 laserStart = transform.position;
        canBrakeObject.Clear();
        foreach (var bone in flameBones)
        {
            float rad = bone.angle * Mathf.Deg2Rad;
            Vector2 direction = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
            float wallDistance = maxRadius;
            RaycastHit2D[] hits = Physics2D.RaycastAll(laserStart, direction, maxRadius, blockLayer);
            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            foreach (var hit in hits)
            {
                if (hit.collider == null) continue;
                if (hit.collider.CompareTag("Wall"))
                {
                    wallDistance = hit.distance * 2;
                    break;
                }
                if (hit.collider.CompareTag("Block") || hit.collider.CompareTag("Boom"))
                {
                    canBrakeObject.Add(hit.collider.gameObject);
                }
            }
            bone.maxLenght = wallDistance;
        }
    }



}


