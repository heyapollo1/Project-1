using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowField
{
    [HideInInspector] public Vector2[,] flowDirections;
    [HideInInspector] public SceneGrid grid;

    private int flowGridWidth, flowGridHeight;
    private float flowCellSize;
    private Transform playerTransform;

    public FlowField(SceneGrid grid, Transform player, float cellSizeMultiplier = 3f)
    {
        this.grid = grid;
        playerTransform = player;

        // Calculate flow cell size based on the A* grid's node diameter
        flowCellSize = grid.nodeDiameter * cellSizeMultiplier;

        // Calculate dimensions based on world size and flow cell size
        flowGridWidth = Mathf.CeilToInt(grid.gridWorldSize.x / flowCellSize);
        flowGridHeight = Mathf.CeilToInt(grid.gridWorldSize.y / flowCellSize);

        Debug.Log($"FlowFields initialized with grid dimensions: {flowGridWidth} x {flowGridHeight}, flow cell size: {flowCellSize}");

        // Initialize flow directions with the calculated size
        flowDirections = new Vector2[flowGridWidth, flowGridHeight];
    }

    public void UpdateFlowField()
    {
        if (playerTransform == null)
        {
            Debug.LogWarning("Player Transform is null! Ensure it is set before updating the FlowField.");
            return;
        }

        Vector2 playerPosition = playerTransform.position;

        for (int x = 0; x < flowGridWidth; x++)
        {
            for (int y = 0; y < flowGridHeight; y++)
            {
                Vector2 flowCellPosition = GetFlowCellWorldPosition(x, y);
                Vector2 directionToPlayer = (playerPosition - flowCellPosition).normalized;
                flowDirections[x, y] = directionToPlayer;
            }
        }
    }

    public Vector2 GetFlowDirection(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt((worldPosition.x - grid.transform.position.x + (grid.gridWorldSize.x / 2)) / flowCellSize);
        int y = Mathf.FloorToInt((worldPosition.y - grid.transform.position.y + (grid.gridWorldSize.y / 2)) / flowCellSize);

        if (x >= 0 && x < flowGridWidth && y >= 0 && y < flowGridHeight)
        {
            return flowDirections[x, y];
        }

        return Vector2.zero;  // Default direction if outside the flow field
    }

    private Vector2 GetFlowCellWorldPosition(int x, int y)
    {
        Vector3 worldBottomLeft = grid.transform.position - Vector3.right * grid.gridWorldSize.x / 2 - Vector3.up * grid.gridWorldSize.y / 2;
        Vector3 flowCellOffset = new Vector3(x * flowCellSize + flowCellSize / 2, y * flowCellSize + flowCellSize / 2, 0);
        return (worldBottomLeft + flowCellOffset);
    }

    // Method to draw flow field grid cells and directions
    public void DrawFlowFieldGizmos()
    {
        if (flowDirections == null || grid == null) return;

        for (int x = 0; x < flowGridWidth; x++)
        {
            for (int y = 0; y < flowGridHeight; y++)
            {
                Vector3 cellCenter = GetFlowCellWorldPosition(x, y);

                // Draw the cell outline
                Gizmos.color = new Color(0, 1, 1, 0.3f);
                Gizmos.DrawWireCube(cellCenter, Vector3.one * flowCellSize);

                // Draw the flow direction arrow
                if (flowDirections[x, y] != Vector2.zero)
                {
                    Gizmos.color = Color.red;
                    Vector3 endPosition = cellCenter + (Vector3)flowDirections[x, y] * flowCellSize * 0.5f;
                    Gizmos.DrawLine(cellCenter, endPosition);
                    Gizmos.DrawSphere(endPosition, 0.1f);
                }
            }
        }
    }
}