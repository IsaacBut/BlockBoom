using UnityEngine;

public class Wall : MonoBehaviour
{
    public Vector2[] fourPoints;
    public BoxCollider2D wallCollider;
    public enum WallType { BreakAble, NonBreakAble }
    public WallType type;
    public WallType GetWallType() => type;
    public void SetWallType(WallType targetType) => type = targetType;


}
