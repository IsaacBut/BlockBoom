using UnityEngine;

public class Block : MonoBehaviour
{

    public bool canSourePlus = true;

    public void BlockDestroy()
    {
        if (canSourePlus)
        {
            Manager.instance.ScorePlus();
            Debug.Log("Plus");
        }

        Destroy(this.gameObject);
    }


}
