using System.Collections.Generic;
using UnityEngine;

public class Boom : MonoBehaviour
{
    public GameObject flame;

#if UNITY_EDITOR

    public bool isBoom = false;

#endif

    public void GoDestroy()
    {
        this.gameObject.SetActive(false);

        Vector3 startPoint = new Vector3(this.transform.position.x, this.transform.position.y, -0.11f);
        GameObject flamePrefab = Instantiate(flame, startPoint, Quaternion.identity);

        flamePrefab.GetComponent<BoomFlame>().FlameInit(InGame.Instance.distanceOfBeat, (float)InGame.Instance.beatsPerSecond);

        Destroy(this.gameObject);
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (isBoom) GoDestroy();
#endif

    }

}
