using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyManager : MonoBehaviour
{
    [Header("Pathfinding")]
    [SerializeField] private LayerMask unwalkableLayer;

    [Header("Player transform")]
    [SerializeField] private Transform player;
    
    private Vector2 playerPositionWalkable;
    private Tilemap floorTilemap;

    public Vector2 GetPlayerPosition() => player.position;
    public Vector2 GetPlayerPositionWalkable() => playerPositionWalkable;

    public void UpdatePathfindingFloorTilemap()
    {
        floorTilemap = GameObject.Find("Floor").GetComponent<Tilemap>();
        PathGrid.floorTilemap = floorTilemap;
    }

    private void Awake()
    {
        PathGrid.unwalkableLayer = unwalkableLayer;
        UpdatePathfindingFloorTilemap();
    }

    private void FixedUpdate()
    {
        if (floorTilemap == null) {
            UpdatePathfindingFloorTilemap();
        }
        if (PathGrid.IsWalkable(Vector2Int.RoundToInt(player.position))) {
            playerPositionWalkable = player.position;
        }
    }
}
