using UnityEngine;

/// <summary>
/// VectorHelper offers additional methods for working with UnityEngine.Vector2/3 data type.
/// </summary>
public static class VectorHelper
{
    private static Vector2 tempVector2;
    private static Vector3 tempVector3;

    /// <summary>
    /// Returns this vector with every element normalized separately.
    /// </summary>
    public static Vector2 NormalizedPerPartes(this Vector2 value)
    {
        tempVector2.Set(value.x / Mathf.Abs(value.x), value.y / Mathf.Abs(value.y));
        return tempVector2;
    }

    /// <summary>
    /// Returns this vector with every element normalized separately.
    /// </summary>
    public static Vector3 NormalizedPerPartes(this Vector3 value)
    {
        tempVector3.Set(value.x / Mathf.Abs(value.x), value.y / Mathf.Abs(value.y), value.z / Mathf.Abs(value.z));
        return tempVector3;
    }
}
