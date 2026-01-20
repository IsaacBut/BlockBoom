using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FlameHit : MonoBehaviour
{
    public BoomFlame boomFlame;
    private HashSet<GameObject> canBrakeObject => boomFlame.canBrakeObject;
    private LayerMask blockLayer => boomFlame.blockLayer;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (canBrakeObject.Contains(other.gameObject))
        {
            if (other.CompareTag("Block"))
            {
                Block block = other.GetComponent<Block>();
                if(block != null) block.BlockDestroy();
            }
            else if (other.CompareTag("Boom"))
            {
                Boom boom = other.GetComponent<Boom>();
                if (boom != null) boom.GoDestroy();

            }

        }

    }

}
