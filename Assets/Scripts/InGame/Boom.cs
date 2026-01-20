using UnityEngine;
using System.Collections;

public class Boom : MonoBehaviour
{
    public GameObject flame;

    public IEnumerator GoDestroy()
    {
        this.gameObject.SetActive(false);

        Vector3 startPoint = new Vector3(this.transform.position.x, this.transform.position.y, -0.11f);
        GameObject flamePrefab = Instantiate(flame, startPoint, Quaternion.identity);
        //flamePrefab.GetComponent<BoomFlame>().FlameInit();

        Destroy(this.gameObject);

        yield return null;
    }


}
