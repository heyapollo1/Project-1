using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    private SceneGrid grid;

    void Awake()
    {
        UpdateCurrentGrid();
        //EventManager.Instance.StartListening("ActiveGridChanged", UpdateCurrentGrid);
    }

    private void OnDestroy()
    {
        //EventManager.Instance.StopListening("ActiveGridChanged", UpdateCurrentGrid);
    }

    private void UpdateCurrentGrid()
    {
        grid = GridManager.Instance.GetActiveGrid();
        if (grid == null)
        {
            Debug.LogError("No active grid found in GridManager.");
        }
    }

    public List<Node> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        ResetNodes();

        Node startNode = grid.GetNodeFromWorldPosition(startPos);
        Node targetNode = grid.GetNodeFromWorldPosition(targetPos);

        if (startNode == null || targetNode == null || !startNode.walkable || !targetNode.walkable)
        {
            Debug.LogWarning("Invalid start or target node.");
            return null;
        }

        List<Node> openSet = new List<Node> { startNode };
        HashSet<Node> closedSet = new HashSet<Node>();

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                //Debug.Log("Target node reached. Retracing path.");
                return RetracePath(startNode, targetNode);
            }

            foreach (Node neighbor in GetNeighbors(currentNode))
            {
                if (!neighbor.walkable || closedSet.Contains(neighbor))
                    continue;

                int newMovementCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);
                if (newMovementCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newMovementCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }
        return null;
    }

    List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse(); // Reverse the path to go from start to end

        return path;
    }

    List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                // Skip the node itself
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                // Make sure the neighboring node is within the grid boundaries
                if (checkX >= 0 && checkX < grid.gridNodeAmountX && checkY >= 0 && checkY < grid.gridNodeAmountY)
                {
                    // Handle diagonal movement
                    if (Mathf.Abs(x) == 1 && Mathf.Abs(y) == 1)
                    {
                        // Ensure both horizontal and vertical neighbors are walkable before allowing diagonal movement, so they donet walk through obstacles diaognally,
                        Node nodeHorizontal = grid.nodeGrid[node.gridX + x, node.gridY];
                        Node nodeVertical = grid.nodeGrid[node.gridX, node.gridY + y];

                        if (nodeHorizontal.walkable && nodeVertical.walkable)
                        {
                            neighbors.Add(grid.nodeGrid[checkX, checkY]);
                        }
                    }
                    else
                    {
                        // Straight movement (horizontal or vertical)
                        neighbors.Add(grid.nodeGrid[checkX, checkY]);
                    }
                }
            }
        }

        return neighbors;
    }

    public void ResetNodes()
    {
        foreach (Node node in grid.nodeGrid)
        {
            node.gCost = int.MaxValue;  // Set the gCost to a very high value (like infinity)
            node.hCost = 0;             // Reset hCost to 0
            node.parent = null;         // Clear the parent node to prevent retrace issues
        }
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (distX > distY)
            return 14 * distY + 10 * (distX - distY);
        return 14 * distX + 10 * (distY - distX);
    }
}