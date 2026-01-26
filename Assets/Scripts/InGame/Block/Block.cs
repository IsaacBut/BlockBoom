using UnityEngine;
using UnityEngine.SceneManagement;

public class Block : MonoBehaviour
{
    public bool canSourePlus = true;

    public void BlockDestroy()
    {
        if (canSourePlus)
        {
            InGame.Instance.ScorePlus();
            Debug.Log("Plus");
        }

        Destroy(this.gameObject);
    }


}
