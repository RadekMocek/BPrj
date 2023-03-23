using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("Pathfinding")]
    [SerializeField] private LayerMask unwalkableLayer;

    [Header("Player transform")]
    [SerializeField] private Transform player;
    
    private Vector2 playerPositionWalkable;

    public Vector2 GetPlayerPosition() => player.position;
    public Vector2 GetPlayerPositionWalkable() => playerPositionWalkable;

    private void Awake()
    {
        PathGrid.unwalkableLayer = unwalkableLayer;
    }

    private void FixedUpdate()
    {
        if (PathGrid.IsWalkable(Vector2Int.RoundToInt(player.position))) {
            playerPositionWalkable = player.position;
        }
    }
}
