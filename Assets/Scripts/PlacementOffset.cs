using UnityEngine;

public class PlacementOffset : MonoBehaviour
{
    public Vector2 offset;

    public Vector3 GetOffsetPosition(Vector3 basePosition)
    {
        return basePosition + new Vector3(offset.x, offset.y, 0f);
    }
}
