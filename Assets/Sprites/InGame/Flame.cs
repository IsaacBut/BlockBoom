using System.Collections.Generic;
using UnityEngine;
using Data;
using System;
using UnityEngine.Rendering.Universal;

public class Flame : MonoBehaviour
{
    float maxRadius;
    float speed;

    List<GameObject> CanBrakeObject = new List<GameObject>();

    public void FlameSetList(List<GameObject> targetList) => CanBrakeObject = targetList;

    public void FlameInit()
    {
        maxRadius = GameData.flameMaxRadius;
        speed = GameData.flameSpreadSpeed;

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
            this.transform.localScale += new Vector3(0.1f, 0.1f,0) * Time.deltaTime* speed;
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
        Bigger();
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
