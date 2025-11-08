using UnityEngine;

public class Wall : MonoBehaviour
{
    public Vector2[] fourPoints;
    public BoxCollider2D wallCollider;
    private const float offset = 0.03f;
    public enum WallType { BreakAble, NonBreakAble }
    public WallType type;
    public WallType GetWallType() => type;
    public void SetWallType(WallType targetType) => type = targetType;

    public Vector2[] GetWallFourPoint() => fourPoints;


    public void FindFourPoint()
    {
        wallCollider = this.GetComponent<BoxCollider2D>();

        Bounds b = wallCollider.bounds; // 世界座標範圍
        Vector2 center = b.center;       // 世界座標中心
        Vector2 extents = b.extents;     // 半寬 & 半高，實際大小的一半
        //Debug.Log($"{gameObject.name} center: {wallCollider.bounds.center}");

        float posX = extents.x + offset;
        float posY = extents.y + offset;

        fourPoints = new Vector2[4];
        fourPoints[0] = center + new Vector2(-posX, -posY); // 左下
        fourPoints[1] = center + new Vector2(posX, -posY);  // 右下
        fourPoints[2] = center + new Vector2(posX, posY);   // 右上
        fourPoints[3] = center + new Vector2(-posX, posY);  // 左上
    }




    //// 🔍 在 Scene 視圖畫出矩形，方便確認
    //private void OnDrawGizmos()
    //{
    //    if (wallCollider == null) return;

    //    FindFourPoint(); // 保證四個角已經更新

    //    Gizmos.color = Color.red;
    //    for (int i = 0; i < fourPoints.Length; i++)
    //    {
    //        Vector2 current = fourPoints[i];
    //        Vector2 next = fourPoints[(i + 1) % fourPoints.Length];
    //        Gizmos.DrawLine(current, next);
    //    }
    //}
}