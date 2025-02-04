
/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearAIBackup : EnemyBase
{
    [Header("Charge Settings")]
    public int chargeDamage;
    public float chargeDetectRange;//can be hardcoded after testing
    public float chargeSpeed;
    public float chargeCooldown;
    public float chargePreparation;//can be hardcoded after testing
    public float chargeDuration;//can be hardcoded after testing
    public float chargeKnockbackForce;

    [Header("Bite Settings")]
    public int biteDamage;
    public float biteDetectRange;//can be hardcoded after testing
    public float biteAttackRadius;
    public float bitePrepTime; //can be hardcoded after testing
    //public float biteDashDistance; //can be hardcoded after testing
    public float biteCooldown;
    public float biteKnockbackForce = 0.2f;

    private float biteCooldownTimer = 0f; // Timer for the bite cooldown
    private float chargeCooldownTimer = 0f;
    private float chargePreparationTimer = 0f;
    private float chargeTimeCounter = 0f;
    private float endChargeTimer = 0.5f;
    private bool isCharging = false;
    private bool targetLocked = false;
    private bool chargeEnded = false;
    private bool chargePreparationTriggered = false;
    private float transitionTimer = 0.2f;
    private float endChargeTime;
    private Vector2 chargeTargetPosition;
    private float chargeCrashTimer;

    private EnemyMovement movement;
    // Start is called before the first frame update

    protected override void Start()
    {
        base.Start();
        movement = GetComponent<EnemyMovement>();
        //movement.Initialize(player, pathfinding, rb);  // Initialize movement with player, pathfinding, and rigidbody
        //lastPosition = rb.position;  // Initialize last position to the starting position

        // Check if movement is found
        if (movement != null)
        {
            movement.Initialize(player, pathfinding, rb);  // Initialize movement with player, pathfinding, and rigidbody
            Debug.Log("EnemyMovement component initialized.");
        }
        else
        {
            Debug.LogError("EnemyMovement component not found!");
        }
    }


    protected override void StateHandler()
    {
        HandleCooldown(ref chargeCooldownTimer);
        HandleCooldown(ref biteCooldownTimer);

        switch (currentState)
        {
            case EnemyState.Chasing:
                HandleChasing();
                break;

            case EnemyState.Charging:
                HandleCharging();
                break;

            case EnemyState.Biting:
                HandleBiting();
                break;

            case EnemyState.Roaming:
                HandleRoaming();  // Handle roaming only in the roaming state
                break;

            case EnemyState.Idle:
                // Ensure no movement during idle
                rb.velocity = Vector2.zero;
                animator.SetBool("isWalking", false);
                break;

            case EnemyState.Transitioning:
                HandleTransitioning();
                break;

            case EnemyState.EndCharge:
                HandleEndCharge();
                break;
        }
    }

    void HandleChasing()
    {
        float distance = Vector2.Distance(transform.position, player.position);

        // Transition to charging if close enough and cooldown is ready
        if (distance <= chargeDetectRange && chargeCooldownTimer <= 0f && !isCharging && currentState == EnemyState.Chasing)
        {
            chargePreparationTimer = chargePreparation;
            chargeEnded = false;
            currentState = EnemyState.Charging;
        }
        // Transition to biting if within bite range and cooldown is ready
        else if (distance <= biteDetectRange && biteCooldownTimer <= 0f && !isAttacking && currentState == EnemyState.Chasing)
        {
            currentState = EnemyState.Biting;
        }
        else if (distance > 1.25 && !isInKnockback)
        {
            movement.Move();
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }
    /*void HandleChasing()
    {
        float distance = Vector2.Distance(transform.position, player.position);

        // Transition to charging if close enough and cooldown is ready
        if (distance <= chargeDetectRange && chargeCooldownTimer <= 0f && !isCharging && currentState == EnemyState.Chasing)
        {
            chargePreparationTimer = chargePreparation;
            chargeEnded = false;
            currentState = EnemyState.Charging;
        }
        // Transition to biting if within bite range and cooldown is ready
        else if (distance <= biteDetectRange && biteCooldownTimer <= 0f && !isAttacking && currentState == EnemyState.Chasing)
        {
            currentState = EnemyState.Biting;
        }
        else if (distance > 1.25 && !isInKnockback)  // Keep distance during chasing
        {
            // Path update and following logic
            if (pathUpdateTimer <= 0f && target != null)
            {
                path = pathfinding.FindPath(transform.position, player.position);
                if (path != null && path.Count > 0)
                {
                    currentPathIndex = 0;  // Reset to start following the new path
                    pathUpdateTimer = pathUpdateCooldown;  // Reset timer for the next update
                    //Debug.Log("Path updated towards player.");
                }
                else
                {
                    //Debug.Log("Pathfinding failed.");
                    StartCoroutine(HandlePathfindingFailure());  // Softly handle the failure
                    return;  // Exit early if the path is invalid
                }
            }
            // Follow the path only if a valid path exists
            if (path != null && path.Count > 0 && currentPathIndex < path.Count)
            {
                FollowPath();
            }
            else
            {
                Debug.LogWarning("Invalid path or currentPathIndex is out of bounds.");
                return;  // Exit if path is invalid
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
            //Debug.Log("Stopping movement, close enough to player.");
        }
    }

    private void HandleBiting()
    {
        StartCoroutine(Bite());
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate(); // Call base for shared behavior if needed


        if (isCharging && targetLocked && !chargeEnded)
        {
            RaycastHit2D hit = Physics2D.BoxCast(rb.position, new Vector2(0.1f, 0.1f), 1f, chargeTargetPosition, 0.1f, LayerMask.GetMask("Wall"));
            if (chargeTimeCounter > 0 && hit.collider == null)
            {
                //Debug.Log("Charging in progress... Time left: " + chargeTimeCounter);

                // Start the charging phase
                rb.MovePosition(rb.position + chargeTargetPosition * chargeSpeed * Time.fixedDeltaTime);

                chargeTimeCounter -= Time.fixedDeltaTime;
            }

            /*else if (chargeTimeCounter > 0 && hit.collider != null)
            {
                Debug.Log("Wall Hit! Reflecting..");
                // Reflect the direction and reduce knockback force
                Vector2 collisionNormal = hit.normal;
                chargeTargetPosition = Vector2.Reflect(chargeTargetPosition, collisionNormal);
                ChargeCrash();
            }
            else
            {
                Debug.Log("Charge Ended");
                currentState = EnemyState.EndCharge;
            }

        }
    }

    public void HandleCharging()
    {
        //charge preparation starts only ONCE
        if (!isCharging && !chargePreparationTriggered)
        {
            //Debug.Log("Charge Preparing..");
            animator.SetTrigger("ChargePreparation");
            chargePreparationTriggered = true;
        }

        if (!isCharging && chargePreparationTimer > 0f)
        {
            chargePreparationTimer -= Time.deltaTime;

            Vector2 directionToPlayer = (player.position - transform.position).normalized;
            FaceDirection(directionToPlayer);

            if (chargePreparationTimer <= 0f)
            {
                //Debug.Log("Charge Preparation complete, starting charge");
                chargePreparationTriggered = false;
                chargeTargetPosition = ((Vector2)player.position - rb.position).normalized; // Lock the player's position
                targetLocked = true;  // Prevent further updates
                //chargePreparationTriggered = false;
                isCharging = true;
                chargeTimeCounter = chargeDuration;
                animator.SetBool("isCharging", true);
            }
        }
    }

    public void HandleEndCharge()
    {
        if (isCharging && !chargeEnded)
        {
            Debug.Log("Ending Charge." + endChargeTimer);
            //Debug.Log("Charge Ending, playing animation..");
            animator.SetTrigger("EndCharge");
            animator.ResetTrigger("ChargePreparation");
            animator.SetBool("isCharging", false);
            rb.velocity = Vector2.zero;
            endChargeTime = endChargeTimer;
            isCharging = false;
            targetLocked = false;
            chargeEnded = true;
        }

        if (endChargeTime > 0 && !isCharging)
        {
            endChargeTime -= Time.deltaTime;
            if (endChargeTime <= 0)
            {
                Debug.Log("Transitioning from non wall crash charge..");
                animator.ResetTrigger("EndCharge");
                chargeCooldownTimer = chargeCooldown;
                //Debug.Log("Simulated EndCharge animation finished, transitioning to next state.");
                currentState = EnemyState.Transitioning;
            }
        }
    }
    public void ChargeCrash()
    {
        float timer = 0f;

        if (isCharging && !chargeEnded && chargeCrashTimer > timer)
        {
            Debug.Log("Knocking back from wall..");
            //Debug.Log("Charge Ending, playing animation..");
            animator.SetTrigger("ChargeCrash");
            animator.ResetTrigger("ChargePreparation");
            animator.SetBool("isCharging", false);
            //rb.velocity = Vector2.zero;
            timer += Time.deltaTime;
            speed *= 0.4f;
        }
        else
        {
            Debug.Log("Done Reflecting, Transitioning now..");
            rb.velocity = Vector2.zero;
            endChargeTime = endChargeTimer;
            isCharging = false;
            targetLocked = false;
            chargeEnded = true;
            currentState = EnemyState.EndCharge;
        }
    }

    IEnumerator Bite()
    {
        isAttacking = true;
        float originalSpeed = speed;
        speed *= 1.5f; //speed up before attack

        animator.SetTrigger("BitePreparation");
        float biteAnimationTime = bitePrepTime;
        while (biteAnimationTime > 0f)
        {
            //Vector2 direction = (player.position - transform.position).normalized;
            transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
            biteAnimationTime -= Time.deltaTime;
            yield return null;
        }

        animator.SetTrigger("Bite");
        speed = originalSpeed;

        Vector2 biteDirection = (player.position - transform.position).normalized;
        Vector2 bitePosition = rb.position + biteDirection * biteAttackRadius;
        yield return new WaitForSeconds(0.4f); // wait fot bite attack to finish

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(bitePosition, biteAttackRadius);
        foreach (Collider2D collider in hitColliders)
        {
            if (collider.CompareTag("Player"))
            {
                HealthManager healthManager = collider.GetComponent<HealthManager>();
                if (healthManager != null)
                {
                    healthManager.TakeDamage(biteDamage, biteDirection, biteKnockbackForce, KnockbackType.Light); ;
                }
            }
            // Check if the hit collider is a breakable object
            else if (collider.CompareTag("BreakableObstacle"))
            {
                Rock rock = collider.GetComponent<Rock>();
                if (rock != null)
                {
                    // Apply damage to the breakable object
                    rock.TakeDamage(biteDamage);
                }
            }
        }

        animator.ResetTrigger("BitePreparation");
        animator.ResetTrigger("Bite");
        animator.SetBool("isWalking", false);
        animator.SetBool("isCharging", false);

        biteCooldownTimer = biteCooldown;
        currentState = EnemyState.Transitioning; // Move back to chasing after the bite
        isAttacking = false;
    }

    private void HandleTransitioning()
    {
        // Wait for the transition/rest period to finish
        transitionTimer -= Time.deltaTime;
        if (transitionTimer <= 0f)
        {
            currentState = EnemyState.Chasing;  // After transition, go back to chasing
            transitionTimer = 0.2f;  // Reset timer for future transitions
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isCharging)
        {
            if (collision.CompareTag("Player"))
            {
                Debug.Log("Player hit");

                HealthManager healthManager = collision.GetComponent<HealthManager>();
                if (healthManager != null)
                {
                    // Calculate knockback direction (away from the enemy)
                    Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;

                    healthManager.TakeDamage(chargeDamage, knockbackDirection, chargeKnockbackForce, KnockbackType.Heavy);
                    currentState = EnemyState.EndCharge;
                }
            }

            if (collision.CompareTag("BreakableObstacle"))
            {
                Debug.Log("Rock hit");
                Rock rock = collision.GetComponent<Rock>();
                if (rock != null)
                {
                    rock.TakeDamage(chargeDamage + 50);
                }
            }
        }
    }
}*/
