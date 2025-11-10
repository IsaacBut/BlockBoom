using UnityEngine;

public class Devolopment : MonoBehaviour
{
#if DEVELOPMENT_BUILD || UNITY_EDITOR

#else
    private void Awake()
    {
      Destroy(gameObject);
    }
#endif
}
