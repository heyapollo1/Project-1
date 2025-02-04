/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemyMovement : EnemyMovement
{  
    public override void Move()
    {
        float distanceToPlayer = Vector2.Distance(rb.position, target.position);

        if (distanceToPlayer < fleeRange)
        {
            FleeFromPlayer();
        }
        else
        {
            base.Move();
        }
    }

    private void FleeFromPlayer()
    {
        // Calculate direction away from the player
        Vector2 fleeDirection = (rb.position - (Vector2)target.position).normalized;

        // Find a point to move towards, away from the player, using A* pathfinding
        Vector2 fleeTarget = rb.position + fleeDirection * fleeRange;

        // Use pathfinding to calculate a path to the flee target
        path = pathfinding.FindPath(rb.position, fleeTarget);

        if (path != null && path.Count > 0)
        {
            currentPathIndex = 0;  // Start following the flee path
            FollowPath();
        }
    }
}*/