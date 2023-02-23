using UnityEngine;

/// <summary>
/// GizmosHelper offers methods for drawing useful shapes for visual debugging that are not available in the Gizmos class.
/// </summary>
public static class GizmosHelper
{
    /// <summary>
    /// Draw a rectangle.
    /// </summary>
    /// <param name="interactZonePointA">One corner of the rectangle.</param>
    /// <param name="interactZonePointB">Diagonally opposite the point A corner of the rectangle.</param>
    /// <param name="offset">Rectangle offset.</param>
    public static void DrawArea(Vector2 interactZonePointA, Vector2 interactZonePointB, Vector2 offset = default)
    {
        var direction = interactZonePointB - interactZonePointA;

        var x = Mathf.Abs(interactZonePointA.x - interactZonePointB.x);
        var y = Mathf.Abs(interactZonePointA.y - interactZonePointB.y);

        Gizmos.DrawWireCube((interactZonePointA + direction / 2) + offset, new Vector2(x, y));
    }
}
