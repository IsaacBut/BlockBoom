using UnityEngine;

public class FlameBone : MonoBehaviour
{
    public float maxLenght;
    public float angle => transform.eulerAngles.z + 90;
    public float angleShow;

    private float width = 0.05f;
    [SerializeField] private float minLength = 0.01f;

    //public void SetLength(float length)
    //{
    //    // 先限制范围
    //    length = Mathf.Clamp(length, minLength, maxLenght);
    //    // 设置缩放
    //    transform.localScale = new Vector3(width, length, 1f);

    //    // 只改 Y，避免影响 X/Z
    //    Vector3 pos = transform.localPosition;
    //    pos.y = length * 0.5f;
    //    transform.localPosition = pos;
    //}

    public void SetLength(float length)
    {
        if (transform.localScale.y > maxLenght)
        {
            transform.localScale = new Vector3(width, maxLenght, 1f);
            transform.position = transform.up * (maxLenght * 0.5f);
            return;
        }

        angleShow = angle;
        length = Mathf.Max(length, minLength);
        transform.localScale = new Vector3(width, length, 1f);
        transform.localPosition = transform.up * (length * 0.5f);
    }
}
