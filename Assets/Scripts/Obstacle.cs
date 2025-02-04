using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Obstacle : MonoBehaviour
{
    public Vector2Int obstacleSize; // Size in grid cell
    public List<Node> previouslyOccupiedNodes = new List<Node>(); // Track previous nodes
    private Vector3 previousPosition;
    private bool isInitialized = false;

    private void Awake()
    {
        if (Application.isPlaying)
        {
            EventManager.Instance.StartListening("GridReady", InitializeObstacle);
        }
    }

    private void OnDestroy()
    {
        if (Application.isPlaying && isInitialized && EventManager.Instance != null)
        {
            EventManager.Instance.StopListening("GridReady", InitializeObstacle);
        }
        //UpdateWalkability(transform.position, true);
    }

    private void OnEnable()
    {
        if (!Application.isPlaying && isInitialized)
        {
            isInitialized = false;
        }
    }

    public void InitializeObstacle()
    {
        //Debug.Log($"Initializing obstacle: {name}");
        //AlignToGrid();
        UpdateWalkability(transform.position, false);
    }

    private void Update()
    {
        if (isInitialized)
        {
            if (transform.position != previousPosition)
            {
                AlignToGrid();
                UpdateWalkability(transform.position, false);
                previousPosition = transform.position;
            }
        }
    }

    public void AlignToGrid()
    {
        Vector3 snappedPosition = GridManager.Instance.GetSnappedPosition(transform.position, obstacleSize);
        UpdateWalkability(snappedPosition, false);
        transform.position = snappedPosition;
        isInitialized = true;
    }

    private bool AreNodesWalkable(Vector3 obstaclePosition)
    {
        float nodeDiameter = GridManager.Instance.activeGrid.nodeDiameter;

        Vector3 bottomLeftPosition = obstaclePosition - new Vector3(
            (obstacleSize.x - 1) * nodeDiameter / 2,
            (obstacleSize.y - 1) * nodeDiameter / 2,
            0);

        for (int x = 0; x < obstacleSize.x; x++)
        {
            for (int y = 0; y < obstacleSize.y; y++)
            {
                Vector3 nodePosition = bottomLeftPosition + new Vector3(x * nodeDiameter, y * nodeDiameter, 0);
                Node node = GridManager.Instance.activeGrid.GetNodeFromWorldPosition(nodePosition);

                if (node == null || !node.walkable)
                {
                    return false; // Found unwalkable node
                }
            }
        }
        return true;
    }

    public void UpdateWalkability(Vector3 obstaclePosition, bool nodeWalkability)
    {
        float nodeDiameter = GridManager.Instance.activeGrid.nodeDiameter;

        if (GridManager.Instance == null || GridManager.Instance.activeGrid == null)
        {
            Debug.LogWarning($"{name}: GridManager or grid is not ready.");
            return;
        }
        List<Node> currentNodes = new List<Node>();

        Vector3 bottomLeftPosition = obstaclePosition - new Vector3(
            (obstacleSize.x - 1) * nodeDiameter / 2,
            (obstacleSize.y - 1) * nodeDiameter / 2,
            0);

        for (int x = 0; x < obstacleSize.x; x++)
        {
            for (int y = 0; y < obstacleSize.y; y++)
            {
                Vector3 nodePosition = bottomLeftPosition + new Vector3(x * nodeDiameter, y * nodeDiameter, 0);
                Node node = GridManager.Instance.activeGrid.GetNodeFromWorldPosition(nodePosition);

                if (node != null)
                {
                    currentNodes.Add(node); // Track currently occupied nodes
                    node.walkable = nodeWalkability; // Mark current node as nwsalkabl
                    //Debug.Log($"Marking node at {node.worldPosition}sdas {(nodeWalkability ? "walkable" : "unwalkable")}.");
                }
                else
                {
                    //Debug.LogWarning($"No node found at {nodePosition} for obstacle {name}.");
                }
            }
        }

        foreach (Node node in previouslyOccupiedNodes)
        {
            if (node != null && !currentNodes.Contains(node))
            {
                node.walkable = !nodeWalkability; // Reset to walkable only if not currently occupied
                //Debug.Log($"Marking previous node at {node.worldPosition} as {(!nodeWalkability ? "walkable" : "unwalkable")}.");
            }
        }

        if (!isInitialized)
        {
            isInitialized = true;
            previousPosition = transform.position;
        }

        previouslyOccupiedNodes = currentNodes;
    }
}