using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteAlways]
public class BoundaryGenerator : MonoBehaviour
{
    public GameObject wallPrefab;
    public Vector2Int obstacleSize;

    private List<GameObject> generatedWalls = new List<GameObject>();
    
    public void GenerateBoundaries(SceneGrid grid)
    {
        Debug.Log($"Generating boundaries for {grid}");
        if (grid == null)
        {
            Debug.LogError("Grid is null. Cannot generate boundaries.");
            return;
        }

        ClearAllWalls();

        int gridSizeX = grid.gridNodeAmountX;
        int gridSizeY = grid.gridNodeAmountY;
        float nodeDiameter = grid.nodeDiameter;

        int step = 2;
        // Generate bottom and top boundaries
        for (int x = 0; x < gridSizeX - 1; x += step) // Stop at gridSizeX - 1
        {
            // Bottom boundary (y = 0)
            Vector3 bottomPosition = CalculateGridPosition(grid, x, 0, nodeDiameter);
            InstantiateBoundaryWall(grid, bottomPosition);

            // Top boundary (y = gridSizeY - 1)
            Vector3 topPosition = CalculateGridPosition(grid, x, gridSizeY - obstacleSize.x, nodeDiameter);
            InstantiateBoundaryWall(grid, topPosition);
        }

        // Generate left and right boundaries
        for (int y = 0; y < ((gridSizeY - 1) - (obstacleSize.y * 2)); y += step) // Stop at gridSizeY - 
        {
            // Left boundary (x = 0)
            Vector3 leftPosition = CalculateGridPosition(grid, 0, y + step, nodeDiameter);
            InstantiateBoundaryWall(grid, leftPosition);

            // Right boundary (x = sgridSizeX - 1x)
            Vector3 rightPosition = CalculateGridPosition(grid, gridSizeX - obstacleSize.x, y + step, nodeDiameter);
            InstantiateBoundaryWall(grid, rightPosition);

            //Debug.Log($"Boundaries {obstacleSize.x} and {obstacleSize.y}.");
        }

        GridManager.Instance.InitializeObstacles();
        //Debug.Log("Boundaries generated.");
    }

    private Vector3 CalculateGridPosition(SceneGrid grid, int x, int y, float nodeDiameter)
    {
        return grid.worldBottomLeft + new Vector3(x * nodeDiameter, y * nodeDiameter, 0);
    }

    private void InstantiateBoundaryWall(SceneGrid grid, Vector3 position)
    {
        if (wallPrefab == null)
        {
            Debug.LogError("Wall prefab or grid is null.");
            return;
        }
        
        Vector3 snappedPosition = grid.SnappedPosition(position, obstacleSize); // Assuming 2x2 wall size

        GameObject wall = Instantiate(wallPrefab, snappedPosition, Quaternion.identity, transform);
        if (wall != null)
        {
            Obstacle obstacle = wall.GetComponent<Obstacle>();
            if (obstacle != null)
            {
                obstacle.UpdateWalkability(transform.position, false);
            }
            generatedWalls.Add(wall); // Ensure all walls are tracked
        }
        else
        {
            Debug.LogWarning("Fasiled to instantiate a wall. It will not be added to the list.");
        }
    }

    void ClearAllWalls()
    {
        var tempArray = new GameObject[transform.childCount];

        for (int i = 0; i < tempArray.Length; i++)
        {
            tempArray[i] = transform.GetChild(i).gameObject;
        }

        foreach (var child in tempArray)
        {
            DestroyImmediate(child);
        }
    }
}