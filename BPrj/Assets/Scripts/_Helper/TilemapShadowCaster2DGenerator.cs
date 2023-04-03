using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// This script allows you to create a ShadowCaster2D with solid border mesh around a tilemap with composite collider. Inspired by https://stackoverflow.com/a/73169914/
/// </summary>
[RequireComponent(typeof(CompositeCollider2D))]
public class TilemapShadowCaster2DGenerator : MonoBehaviour
{
    // Component references
    private CompositeCollider2D compositeCollider;

    // Points that make up the shape of the CompositeCollider2D are stored in this list
    private readonly List<Vector2> colliderPoints = new();

    // Editing of ShadowCaster2D values is done using System.Reflection
    private readonly FieldInfo meshField = typeof(ShadowCaster2D).GetField("m_Mesh", BindingFlags.NonPublic | BindingFlags.Instance);
    private readonly FieldInfo shapePathField = typeof(ShadowCaster2D).GetField("m_ShapePath", BindingFlags.NonPublic | BindingFlags.Instance);
    private readonly MethodInfo generateShadowMeshMethod = typeof(ShadowCaster2D)
            .Assembly
            .GetType("UnityEngine.Rendering.Universal.ShadowUtility")
            .GetMethod("GenerateShadowMesh", BindingFlags.Public | BindingFlags.Static);

    //
    private readonly string childNameBase = "ShadowCaster";
    private GameObject shadowCasterGO;
    private ShadowCaster2D shadowCasterScript;
    private Vector3[] innerPath, outerPath, finalPath;

    /// <summary>
    /// Creates a child GameObject with ShadowCaster2D component and edits its mesh shape according to CompositeCollider2D's shape
    /// </summary>
    [ContextMenu("Generate ShadowCaster mesh shape")]
    public void Generate()
    {
        // Initialization
        compositeCollider = GetComponent<CompositeCollider2D>();
        int nPaths = compositeCollider.pathCount;
        // Foreach composite collider path
        for (int pathIndex = 0; pathIndex < nPaths; pathIndex++) {
            string childName = childNameBase + pathIndex;
            // Clean-up
            colliderPoints.Clear();
            var child = this.transform.Find(childName);
            if (child != null) DestroyImmediate(child.gameObject);

            // Create child and add component
            shadowCasterGO = new GameObject(childName);
            shadowCasterGO.transform.parent = this.transform;
            shadowCasterGO.transform.position = Vector3.zero;
            shadowCasterScript = shadowCasterGO.AddComponent<ShadowCaster2D>();
            shadowCasterScript.selfShadows = true;

            // Get composite collider points and store them to `colliderPoints`
            compositeCollider.GetPath(pathIndex, colliderPoints);

            // ShadowCaster mesh accepts Vector3[], that will be our `finalPath` consisting of points that make up the border around the tilemap
            // `innerPath` points are equal to the composite collider points
            innerPath = colliderPoints.Select(vec => (Vector3)vec).ToArray();

            // `outerPath` points needs to be calculated
            int len = innerPath.Length;
            outerPath = new Vector3[len];

            for (int i = 0; i < len; i++) {
                var point = innerPath[i];
                var prevPoint = innerPath[(i == 0) ? (len - 1) : (i - 1)];
                var nextPoint = innerPath[(i + 1) % len];
                var perpendicular = (nextPoint - prevPoint).NormalizedPerPartes() * 0.45f;
                outerPath[i].Set(point.x + perpendicular.y, point.y - perpendicular.x, 0);
            }

            // Combine inner and outer paths
            finalPath = innerPath.Concat(outerPath.Reverse()).ToArray();

            // System.Reflection
            shapePathField.SetValue(shadowCasterScript, finalPath);
            meshField.SetValue(shadowCasterScript, new Mesh());
            generateShadowMeshMethod.Invoke(shadowCasterScript, new object[] { meshField.GetValue(shadowCasterScript), shapePathField.GetValue(shadowCasterScript) });
            Debug.Log("ShadowCaster mesh shape generated");
        }
    }
}
