using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoamingState : IEnemyState
{
    private float patrolRadius = 2f; // Radius for random roaming
    private float roamDurationMin = 0.2f; // Minimum time between changing directions while idle
    private float roamDurationMax = 1.0f; // Maximum time between changing directions while idle
    private Vector2 roamDirection;
    private float roamTimer;

    public void EnterState(EnemyAI enemy)
    {
        roamDirection = Random.insideUnitCircle.normalized;
        Roam(enemy);

        roamTimer = Random.Range(roamDurationMin, roamDurationMax);
        enemy.animator.SetBool("isWalking", true);
    }

    public void UpdateState(EnemyAI enemy)
    {
        roamTimer -= Time.deltaTime;

        if (enemy.IsPlayerInSightRange())
        {
            enemy.isAlerted = true;
            enemy.TransitionToState(enemy.idleState);
        }
        else if (roamTimer <= 0)
        {
            enemy.TransitionToState(enemy.idleState);
        }
    }
    void Roam(EnemyAI enemy)
    {
        Vector2 roamMovePosition = enemy.rb.position + roamDirection * (enemy.currentMoveSpeed * 0.5f) * Time.deltaTime;

        // Perform a raycast in the direction of movement to detect obstacles
        RaycastHit2D hit = Physics2D.Raycast(enemy.rb.position, roamDirection, patrolRadius, enemy.obstacleLayerMask);

        // Check if the new position is within the patrol radius from the start position
        if (hit.collider != null)
        {
            Debug.Log("Obstacle detected during roaming. Choosing a new direction.");
            ChooseNewRoamDirection();
        }
        else
        {
            if (Vector2.Distance(roamMovePosition, enemy.startPosition) <= patrolRadius)
            {
                //rb.MovePosition(newPosition);
                enemy.rb.MovePosition(roamMovePosition);
                enemy.FaceDirection(roamDirection);

            }
            else
            {
                ChooseNewRoamDirection();
            }
        }
    }

    void ChooseNewRoamDirection()
    {
        roamDirection = Random.insideUnitCircle.normalized;
    }

    public void ExitState(EnemyAI enemy)
    {
    }
}
