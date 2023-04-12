using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyManager : MonoBehaviour
{
    [Header("Pathfinding")]
    [SerializeField] private LayerMask unwalkableLayer;

    [Header("Player transform")]
    [SerializeField] private Transform playerTransform;

    private Player playerScript;
    private Vector2 playerPositionWalkable;
    private Tilemap floorTilemap;

    [HideInInspector] public bool isPlayerDead;

    public Vector2 GetPlayerPosition() => playerTransform.position;
    public Vector2 GetPlayerPositionWalkable() => playerPositionWalkable;
    public bool IsPlayerSneaking() => playerScript.IsSneaking;
    public bool IsPlayerInDialogueState() => (playerScript.CurrentState == playerScript.DialogueState);

    public void UpdatePathfindingFloorTilemap()
    {
        floorTilemap = GameObject.Find("Floor").GetComponent<Tilemap>();
        PathGrid.floorTilemap = floorTilemap;
    }

    private void Awake()
    {
        playerScript = playerTransform.gameObject.GetComponent<Player>();
        PathGrid.unwalkableLayer = unwalkableLayer;
        UpdatePathfindingFloorTilemap();
    }

    private void Start()
    {
        isPlayerDead = false;
    }

    private void FixedUpdate()
    {
        if (floorTilemap == null) {
            UpdatePathfindingFloorTilemap();
        }
        if (PathGrid.IsWalkable(Vector2Int.FloorToInt(playerTransform.position))) {
            playerPositionWalkable = playerTransform.position;
        }
    }
}
