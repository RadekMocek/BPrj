using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelBuilder : MonoBehaviour
{

    [Header("Floor")]

    [SerializeField] private Tilemap floorTilemap;

    [SerializeField] private RuleTile floor1;

    private void Start()
    {
        var location = new Vector2(0, 0);

        var tileLocation = floorTilemap.WorldToCell(location);

        floorTilemap.SetTile(tileLocation, floor1);
    }

}
