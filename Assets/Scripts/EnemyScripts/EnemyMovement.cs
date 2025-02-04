using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float avoidRadius = 1f;
    public float avoidForce = 10f;
    public float minimumSeparationDistance = 1f;
    public float detailedPathfindingDistance = 3f; // Distance to switch to A* pathfinding
    public float repulsionForce = 0.5f;
    public float wallRepulsionRadius = 0.6f;
    public float wallDetectionRadius = 1.5f;

    private FlowField flowField;
    private EnemyAI enemy;
    private Pathfinding pathfinding;
    private Rigidbody2D rb;
    private Transform player;
    private Animator animator;
    private List<Node> path = new List<Node>(); // Shared move path for all enemy types

    private LayerMask obstacleLayer;
    private SceneGrid grid;
    public int currentPathIndex = 0;
    private float pathUpdateCooldown = 0.25f;
    private float pathUpdateTimer = 0f;
    private float offsetUpdateCooldown = 1f;
    private float offsetTimer;
    private Vector2 simulatedVelocity;
    //rivate float footstepTimer = 0f;
    public float footstepInterval = 0.15f;

    protected bool isActive;
    protected bool isEvading;

    public void Initialize(Pathfinding pathfindingSystem, Rigidbody2D rbComponent, Transform playerTarget, LayerMask obstacleLayerMask, FlowField flowFieldGrid, Animator animator)
    {
        pathfinding = pathfindingSystem;
        rb = rbComponent;
        player = playerTarget;
        obstacleLayer = obstacleLayerMask;
        flowField = flowFieldGrid;

        grid = FindObjectOfType<SceneGrid>();
        enemy = GetComponent<EnemyAI>();
    }

    public void StartMovement()
    {
        isActive = true;
        pathUpdateTimer = 0f;  // Force path update immediately
    }

    public void StartEvading()
    {
        isEvading = true;
        isActive = true;
        pathUpdateTimer = 0f;  // Force path update immediately
    }

    public void StopMovement()
    {
        isEvading = false;
        isActive = false;
        rb.velocity = Vector2.zero;
    }

    private void Update()
    {
        if (!isActive) return;

        if (IsStuckInUnwalkableNode())
        {
            ApplyStuckNudge();
            return;
        }

        pathUpdateTimer -= Time.deltaTime;
        offsetTimer -= Time.deltaTime;

        if (pathUpdateTimer <= 0f)
        {
            if (!isEvading)
            {
                UpdatePathToTarget();
            }
            else
            {
                UpdatePathAwayFromPlayer();
            }

            pathUpdateTimer = pathUpdateCooldown;
        }

        //ApplyWallRepulsion();
        Move();
    }

    public virtual void Move()
    {
        if (path == null || path.Count == 0)
        {
            UpdatePathToTarget(); // Trigger path recalculation
            return;
        }

        if (DetectNearbyWalls() || IsCloseToTarget())
        {
            FollowAStarPath();
        }
        else
        {
            FollowFlowField();
        }
    }

    private bool DetectNearbyWalls()
    {
        Collider2D[] nearbyWalls = Physics2D.OverlapCircleAll(transform.position, wallDetectionRadius, obstacleLayer);
        return nearbyWalls.Length > 0;
    }

    private bool IsCrowded()
    {
        var nearbyEnemies = EnemyManager.Instance.GetEnemiesInRange(enemy.transform.position, 3f, gameObject);
        return nearbyEnemies.Count > 5;
    }

    private bool IsCloseToTarget()
    {
        return Vector2.Distance(transform.position, player.position) <= detailedPathfindingDistance;
    }

    private void FollowFlowField()
    {
        if (grid == null)
        {
            Debug.LogWarning("gridnotfound");
            return;
        }
        Vector2 flowDirection = flowField.GetFlowDirection(transform.position);
        Vector2 avoidanceDirection = CalculateAvoidance();

        Vector2 combinedDirection = flowDirection + avoidanceDirection;

        Vector2 smoothedDirection = Vector2.Lerp(rb.velocity.normalized, combinedDirection.normalized, 0.2f);
        Vector2 finalDirection = smoothedDirection.normalized * enemy.currentMoveSpeed;

        simulatedVelocity = finalDirection;

        rb.MovePosition(rb.position + simulatedVelocity * Time.deltaTime);
    }

    private void FollowAStarPath()
    {
        if (path == null || currentPathIndex >= path.Count)
        {
            //Debug.LogWarning("Path invalid or complete, recalculating...");
            UpdatePathToTarget();
            return;
        }

        Vector3 targetPosition = path[currentPathIndex].worldPosition;
        Vector2 pathDirection = (targetPosition - transform.position).normalized;
        Vector2 avoidanceDirection = CalculateAvoidance();

        // Combine path direction and avoidance forces
        Vector2 combinedDirection = pathDirection + avoidanceDirection;

        // Smooth the transition to the new direction
        Vector2 currentVelocity = rb.velocity;
        Vector2 smoothedDirection = Vector2.Lerp(currentVelocity, combinedDirection, 0.1f);

        // Normalize the smoothed direction and apply the correct speed
        Vector2 finalDirection = smoothedDirection.normalized * enemy.currentMoveSpeed;

        simulatedVelocity = finalDirection;
        // Apply movement
        rb.MovePosition(rb.position + simulatedVelocity * Time.deltaTime);

        // Advance to the next waypoint if close enough
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            currentPathIndex++;
        }
    }

    protected void UpdatePathToTarget()
    {
        if (pathfinding == null) return;

        if (IsCrowded() && IsCloseToTarget() && offsetTimer <= 0)
        {
            //Debug.LogWarning("offset target");
            Vector3 targetPosition = CalculateAdaptiveOffsetTarget();
            path = pathfinding.FindPath(rb.position, targetPosition);
            offsetTimer = offsetUpdateCooldown;
            currentPathIndex = 0;
        }
        else
        {
            //Debug.LogWarning("direct target");
            path = pathfinding.FindPath(rb.position, player.position);
            currentPathIndex = 0;
        }
    }

    private Vector3 CalculateAdaptiveOffsetTarget()
    {
        Vector3 toPlayer = (player.position - transform.position).normalized;
        float[] anglesToCheck = { -45f, 45f, -90f, 90f};

        foreach (float angle in anglesToCheck)
        {
            Vector3 offsetDirection = Quaternion.Euler(0, 0, angle) * toPlayer;
            float offsetDistance = Mathf.Clamp(Vector3.Distance(transform.position, player.position) - 1f, 1f, 2f);

            Vector3 potentialTarget = player.position + offsetDirection * offsetDistance;
            Node targetNode = grid.GetNodeFromWorldPosition(potentialTarget);
            if (targetNode != null && targetNode.walkable)
            {
                return potentialTarget;
            }
        }
        return player.position;
    }

    protected Vector2 CalculateAvoidance()
    {
        Vector2 separation = Vector2.zero;
        Vector2 alignment = Vector2.zero;
        Vector2 cohesion = Vector2.zero;
        int neighborCount = 0;

        // Get nearby enemies
        List<GameObject> enemiesNearby = EnemyManager.Instance.GetEnemiesInRange(transform.position, avoidRadius, gameObject);

        foreach (var enemy in enemiesNearby)
        {
            if (enemy == null || enemy == gameObject) continue;

            Vector2 directionAway = (transform.position - enemy.transform.position);
            float distance = directionAway.magnitude;

            if (distance > 0)
            {
                // Separation: Add clamped force based on distance
                float separationStrength = Mathf.Clamp(avoidForce / Mathf.Pow(distance, 2), 0.1f, 3f);
                separation += directionAway.normalized * separationStrength;
            }

            alignment += enemy.GetComponent<EnemyMovement>().GetCurrentDirection();
            neighborCount++;
        }

        if (neighborCount > 0)
        {
            // Average alignment direction
            alignment = (alignment / neighborCount).normalized;

            // Cohesion: Move toward the group's center of mass
            cohesion = ((Vector2)player.position - (Vector2)transform.position).normalized;
        }

        // Adjust weights dynamically based on crowd density
        float separationWeight = 0.55f;
        float alignmentWeight = 0.5f;
        float cohesionWeight = 0.3f;

        // Final avoidance vector without normalization
        Vector2 finalAvoidance = (separation * separationWeight + alignment * alignmentWeight + cohesion * cohesionWeight);

        return finalAvoidance;
    }

    public Vector2 GetCurrentDirection()
    {
        return rb.velocity.normalized; // Use the current velocity as the enemy's movement direction
    }

    protected void UpdatePathAwayFromPlayer()
    {
        float fleeDistance = 5f; // Base flee distance
        Vector2 directionAwayFromPlayer = (rb.position - (Vector2)player.position).normalized;
        Vector2 fleePosition;
        bool pathFound = false;

        // Incrementally increase flee distance until a valid path is found or max attempts reached
        for (int i = 1; i <= 5; i++)
        {
            fleePosition = rb.position + directionAwayFromPlayer * (fleeDistance * i);

            path = pathfinding.FindPath(rb.position, fleePosition);
            if (path != null && path.Count > 0)
            {
                currentPathIndex = 0;
                pathUpdateTimer = pathUpdateCooldown;
                pathFound = true;
                break;
            }
        }
        // If no path found in direct opposite direction, try alternative angles
        if (!pathFound)
        {
            float[] angles = { 45f, -45f, 90f, -90f}; // Adjust angles to test as needed

            foreach (float angle in angles)
            {
                Vector2 escapeDirection = Quaternion.Euler(0, 0, angle) * directionAwayFromPlayer;
                fleePosition = rb.position + escapeDirection * fleeDistance;

                path = pathfinding.FindPath(rb.position, fleePosition);
                if (path != null && path.Count > 0)
                {
                    currentPathIndex = 0;
                    pathUpdateTimer = pathUpdateCooldown;
                    pathFound = true;
                    break;
                }
            }
        }

        if (!pathFound)
        {
            StopMovement();
            Debug.Log("No efficient path found for fleeing.");
        }
    }

    private bool IsStuckInUnwalkableNode()
    {
        Node currentNode = grid.GetNodeFromWorldPosition(transform.position);
        return currentNode != null && !currentNode.walkable;
    }

    private void ApplyStuckNudge()
    {
        Node currentNode = grid.GetNodeFromWorldPosition(transform.position);
        Node closestWalkableNode = FindClosestWalkableNode(currentNode);

        if (closestWalkableNode != null)
        {
            Vector2 directionToWalkable = (closestWalkableNode.worldPosition - transform.position).normalized;
            transform.position += (Vector3)directionToWalkable * 0.3f;
            Debug.Log("Nudge applied towards nearest walkable node.");
        }
        else
        {
            Debug.LogWarning("No walkable node found nearby.");
        }
    }

    private Node FindClosestWalkableNode(Node originNode)
    {
        Node closestNode = null;
        float closestDistance = Mathf.Infinity;

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                int checkX = originNode.gridX + x;
                int checkY = originNode.gridY + y;

                if (checkX >= 0 && checkX < grid.gridNodeAmountX && checkY >= 0 && checkY < grid.gridNodeAmountY)
                {
                    Node neighbor = grid.nodeGrid[checkX, checkY];
                    if (neighbor.walkable)
                    {
                        float distance = Vector3.Distance(originNode.worldPosition, neighbor.worldPosition);
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestNode = neighbor;
                        }
                    }
                }
            }
        }

        return closestNode;
    }
}
