using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class SceneGrid : MonoBehaviour
{
    public Vector2 gridWorldSize;//example: 40 - divide by diameter(size of grid node)
    public float nodeRadius;//size of each node
    public Node[,] nodeGrid;//[,] is cause the call has [thing,thing] - a comma inbetween. ITS THE PARENT OF NODE!!
    public int gridNodeAmountX { get; private set; }// amt of nodes in x
    public int gridNodeAmountY { get; private set; }// amt of nodes in y
    [HideInInspector] public Vector3 worldBottomLeft;
    [HideInInspector] public float nodeDiameter;
    public bool showGizmos = true;
    
    public BoundaryGenerator boundaryGenerator;
   
    public void InitializeGrid()
    {
        boundaryGenerator = GetComponent<BoundaryGenerator>();
        Debug.Log("Grid initializing");
        nodeDiameter = nodeRadius * 2;
        gridNodeAmountX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);//NODE(x) = size of ENTIRE GRID(x) divided by SIZE OF EACH NODE
        gridNodeAmountY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);//(y)

        // Adjust gridWorldSize to match actual grid size
        gridWorldSize.x = gridNodeAmountX * nodeDiameter; //ENTIRE GRID(x) = NODE(x) * SIZE OF EACH NODE.
        gridWorldSize.y = gridNodeAmountY * nodeDiameter; //(y)
        Debug.Log($"Raw value: {gridWorldSize.x}, Div: {gridWorldSize.x / nodeDiameter}");
        CreateGrid();
    }

    private void CreateGrid()
    {
        nodeGrid = new Node[gridNodeAmountX, gridNodeAmountY]; //grid created with node amounts.

        // Bottom-left corner of grid calculation AKA origin point of grid creation.
        // worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.up * gridWorldSize.y / 2; //right in relation to ENTIRE GRID x, up for y.
        Debug.Log($"SceneGrid position: {transform.position}");
        worldBottomLeft = transform.position
                  - Vector3.right * (gridWorldSize.x / 2 - nodeDiameter / 2)
                  - Vector3.up * (gridWorldSize.y / 2 - nodeDiameter / 2);
        //is divide by 2 because its HALF of the node?
        // Populate the grid
        for (int x = 0; x < gridNodeAmountX; x++)//Trigger for each node in x, x=0,1,2,3,4 - each node.
        {
            for (int y = 0; y < gridNodeAmountY; y++)//Trigger for each node in y
            {
                // Center each node in its cell
                //Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter + nodeRadius);
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter) + Vector3.up * (y * nodeDiameter);//Finding node location to create based off worldbottomLeft
                //LOCATION = [WBL+(1,0,0)] * (x(which is 1,2,3,etc based on node number) * size of node.x), AKA: Bottom left right * Node Size.
                nodeGrid[x, y] = new Node(true, worldPoint, x, y);//using parent to create real.
            }
        }
        if (Application.isPlaying)
        {
            EventManager.Instance.TriggerEvent("GridReady", this);
        }
    }

    public void CreateBoundaries()
    {
        boundaryGenerator.GenerateBoundaries(this);
    }

    public Vector3 SnappedPosition(Vector3 worldPosition, Vector2Int obstacleSize)
    {
        // Snap the world position directly to the nearest grid node center
        float snappedX = Mathf.RoundToInt((worldPosition.x - worldBottomLeft.x) / nodeDiameter) * nodeDiameter + worldBottomLeft.x;
        float snappedY = Mathf.RoundToInt((worldPosition.y - worldBottomLeft.y) / nodeDiameter) * nodeDiameter + worldBottomLeft.y;

        // Adjust for obstacle size to align with the bottom-left corner
        Vector3 offset = GetObstacleOffset(obstacleSize);
        return new Vector3(snappedX, snappedY, 0) + offset;
    }

    private Vector3 GetObstacleOffset(Vector2Int obstacleSize)
    {
        // Calculate the offset to align the bottom-left corner
        float xOffset = (obstacleSize.x - 1) * nodeDiameter / 2f;
        float yOffset = (obstacleSize.y - 1) * nodeDiameter / 2f;
        return new Vector3(xOffset, yOffset, 0);
    }

    public Node GetNodeFromWorldPosition(Vector3 worldPosition)
    {
        int x = Mathf.RoundToInt((worldPosition.x - worldBottomLeft.x) / nodeDiameter);
        int y = Mathf.RoundToInt((worldPosition.y - worldBottomLeft.y) / nodeDiameter);

        // Ensure indices are within bounds
        if (x < 0 || x >= gridNodeAmountX || y < 0 || y >= gridNodeAmountY)
        {
            return null;
        }

        return nodeGrid[x, y];
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 1));
        if (nodeGrid != null && showGizmos)
        {
            foreach (Node n in nodeGrid)
            {
                Color nodeColor = (n.walkable) ? new Color(0, 1, 1, 0.05f) : new Color(1, 0, 0, 0.06f); // Green for walkable, red for unwalkable, with 0.3f alpha
                Gizmos.color = nodeColor;

                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - 0.025f));
            }
        }
    }
}
/*{
    public Vector2 gridWorldSize;
    public float nodeRadius;
    public LayerMask unwalkableMask; // Only for reference if needed, no longer in use for setting nodes unwalkable in `CreateGrid`

    public Node[,] nodeGrid; // 2D array to hold grid nodes
    public float nodeDiameter;
    public int gridSizeX { get; private set; }
    public int gridSizeY { get; private set; }
    public bool showGizmos = true;

    public List<Vector3> staticObstaclePositions = new List<Vector3>();

    void OnValidate()
    {
        if (!Application.isPlaying)
        {
            Initialize();
        }
    }

    public void Initialize()
    {
        Debug.LogWarning("Grid initialized");
        nodeDiameter = nodeRadius * 2; // Node diameter is twice the radius
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }

    void CreateGrid()
    {
        nodeGrid = new Node[gridSizeX, gridSizeY];

        // Calculate bottom-left corner based on the grid's position and world size
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.up * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter + nodeRadius);
                bool walkable = true; // Default walkable

                nodeGrid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }

        EventManager.Instance.TriggerEvent("GridReady");
        foreach (Vector3 position in staticObstaclePositions)
        {
            UpdateNodeWalkability(position, false);
        }
    }

    public Node GetNodeFromWorldPosition(Vector3 worldPosition)
    {
        // Calculate position as a percentage within the grid bounds
        float percentX = (worldPosition.x - (transform.position.x - gridWorldSize.x / 2)) / gridWorldSize.x;
        float percentY = (worldPosition.y - (transform.position.y - gridWorldSize.y / 2)) / gridWorldSize.y;

        // Convert percentage to grid indices without clamping
        int x = Mathf.FloorToInt((gridSizeX) * Mathf.Clamp01(percentX));
        int y = Mathf.FloorToInt((gridSizeY) * Mathf.Clamp01(percentY));

        // Check if calculated indices are within valid range before accessing
        if (x < 0 || x >= gridSizeX || y < 0 || y >= gridSizeY)
        {
            Debug.LogWarning("Position out of grid bounds");
            return null; // Position is out of bounds
        }

        return nodeGrid[x, y];
    }

    // Update specific node walkability status at a given world position
    public void UpdateNodeWalkability(Vector3 position, bool walkable)
    {
        Node node = GetNodeFromWorldPosition(position);
        if (node != null)
        {
            node.walkable = walkable;
        }
    }

    public Vector3 GetSnappedPosition(Vector3 worldPosition, Vector2Int obstacleSize)
    {
        // Get the center node for the obstacle's starting point
        Node centerNode = GetNodeFromWorldPosition(worldPosition);
        if (centerNode == null)
        {
            Debug.LogWarning("Attempted to snap outside grid bounds.");
            return worldPosition;
        }

        int halfWidth = (obstacleSize.x - 1) / 2;
        int halfHeight = (obstacleSize.y - 1) / 2;

        int targetX = Mathf.Clamp(centerNode.gridX - halfWidth, 0, gridSizeX - obstacleSize.x);
        int targetY = Mathf.Clamp(centerNode.gridY - halfHeight, 0, gridSizeY - obstacleSize.y);

        Node targetNode = nodeGrid[targetX, targetY];

        Vector3 offset = new Vector3(
            (obstacleSize.x - 1) * nodeDiameter / 2,
            (obstacleSize.y - 1) * nodeDiameter / 2,
            0);

        return targetNode.worldPosition + offset;
    }

    public void OnDrawGizmos()
    {
        // Draw grid bounds in the editor for visualization
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 1));

        if (nodeGrid != null && showGizmos)
        {
            foreach (Node n in nodeGrid)
            {
                Color nodeColor = (n.walkable) ? new Color(0, 1, 1, 0.05f) : new Color(1, 0, 0, 0.06f); // Green for walkable, red for unwalkable, with 0.3f alpha
                Gizmos.color = nodeColor;

                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - 0.025f));
            }
        }
    }
}*/

