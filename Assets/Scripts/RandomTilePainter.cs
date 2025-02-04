using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class RandomTilePainter : MonoBehaviour
{
    public Tilemap tilemap; 
    public TileBase[] tileVariants;

    public int areaWidth = 10; // Width of area
    public int areaHeight = 10; // Height
    public Vector3Int startPosition;

    void OnEnable()
    {
        ClearAllTiles();
        HandleTiles();
    }


    void HandleTiles()
    {
        // Get the center of the tilemap's grid
        Vector3Int tilemapCenter = tilemap.WorldToCell(tilemap.transform.position);

        // Adjust the start position so the area is centered
        Vector3Int startPosition = new Vector3Int(
            tilemapCenter.x - Mathf.FloorToInt(areaWidth / 2f),
            tilemapCenter.y - Mathf.FloorToInt(areaHeight / 2f),
            0
        );

        // Pass the calculated start position to fill the tiles
        FillAreaWithRandomTiles(startPosition);
    }

    void FillAreaWithRandomTiles(Vector3Int currentStartPosition)
    {
        // Loop through the area defined by areaWidth and areaHeight
        for (int x = 0; x < areaWidth; x++)
        {
            for (int y = 0; y < areaHeight; y++)
            {
                // Calculate the position of the tile relative to the start position
                Vector3Int tilePosition = currentStartPosition + new Vector3Int(x, y, 0);

                // Pick a random tile from the available variants
                TileBase randomTile = tileVariants[Random.Range(0, tileVariants.Length)];

                // Set the tile in the tilemap
                tilemap.SetTile(tilePosition, randomTile);
            }
        }
    }

    void ClearAllTiles()
    {
        if (tilemap == null)
        {
            Debug.LogWarning("Tilemap is not assigned.");
            return;
        }

        // Get the bounds of the Tilemap
        BoundsInt bounds = tilemap.cellBounds;

        // Iterate through all positions in the bounds
        foreach (var position in bounds.allPositionsWithin)
        {
            // Clear the tile at the current position
            tilemap.SetTile(position, null);
        }

        Debug.Log("All tiles cleared from the Tilemap.");
    }
}