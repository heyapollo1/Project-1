/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    [Header("General Settings")]
    public Transform player;
    public float speed;
    public float roamSpeed = 0.5f;
    public float playerMovementThreshold = 1.0f;  // Set a threshold value for how far the player needs to move before updating the path
    public LayerMask obstacleLayerMask;

    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("FOV Settings")]
    public float sightRange = 10f;
    public float fieldOfView = 90f;
    public float alertDuration = 1f;
    public GameObject alertFXPrefab;

    [Header("Roaming Settings")]
    //public bool canRoam = true; // Flag to enable or disable roaming
    public float patrolRadius = 2f; // Radius for random roaming
    public float idleRoamDelayMin = 0.2f; // Minimum time between changing directions while idle
    public float idleRoamDelayMax = 1.0f; // Maximum time between changing directions while idle
    public float idleAfterRoamMin = 3.0f; // Minimum time spent idling after roaming
    public float idleAfterRoamMax = 5.0f; // Maximum time spent idling after roaming

    [Header("Knockback Settings")]
    public float knockbackDuration = 0.25f;
    public float knockbackCooldown = 1.0f;
    public float whiteFlashCooldown = 0.25f;
    public float blendingFactor = 0.5f;
    public float flashTime = 0.25f;
    private float currentKnockbackForce;

    [Header("Shadow Settings")]
    public Transform shadowTransform;  // Reference to the shadow GameObject
    public float shadowNormalHeight = 0.15f;  // Default shadow height
    private Vector3 shadowOffset;

    protected SpriteRenderer spriteRenderer;
    protected Material _material;

    protected KnockbackType currentKnockbackType = KnockbackType.Light; // Default to the lowest tier
    protected bool isInKnockback = false; // To track if the enemy is currently in knockback
    protected float knockbackCooldownTimer;
    protected float whiteFlashCooldownTimer;
    protected Color _flashColor = Color.white;
    protected Vector2 knockbackDirection;
    public float airHeight = 1f; // shadow offseyt while enemy is "in the air".

    //public LayerMask AlertTarget;
    protected Vector2 roamDirection;
    protected Vector2 lastPosition;
    protected float idleRoamTimer;

    protected Rigidbody2D rb;
    protected Animator animator;
    protected Collider2D enemyCollider;
    protected Pathfinding pathfinding;
    protected bool isAttacking = false;
    protected float alertTimer;
    protected Vector2 startPosition;

    protected bool isDead = false; // Check if the character is dead

    protected enum EnemyState { Idle, Roaming, Alert, Chasing, Biting, Charging, Shooting, Melee, Transitioning, EndCharge }
    protected EnemyState currentState = EnemyState.Idle;

    protected bool alerted = false;

    protected Grid grid;

    /*public int CurrentPathIndex
    {
        get { return currentPathIndex; }
    }*/

    /*protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        startPosition = transform.position;
    }

    protected virtual void Start()
    {
        grid = FindObjectOfType<Grid>();
        enemyCollider = GetComponent<Collider2D>();
        pathfinding = FindObjectOfType<Pathfinding>();
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        _material = Instantiate(spriteRenderer.material);
        spriteRenderer.material = _material;
        //_material.SetFloat(flashAmount, 0f);
        shadowOffset = shadowTransform.position;  // Save the original shadow position

        rb.useFullKinematicContacts = true;
        rb.freezeRotation = true;
    }

    protected virtual void FixedUpdate()
    {
        // Placeholder for shared FixedUpdate behavior if any
    }

    protected virtual void Update()
    {
      // Update the knockback cooldown timer
        if (knockbackCooldownTimer > 0)
        {
            knockbackCooldownTimer -= Time.deltaTime;
        }

        // Update the white flash cooldown timer independently
        if (whiteFlashCooldownTimer > 0)
        {
            whiteFlashCooldownTimer -= Time.deltaTime;
        }

        if (!isAttacking)
        {
            LineOfSightCheck();
            StateHandler();
            if (alerted == false)
            {
                HandleRoaming(); // Handle roaming and idling in a loop
            }
        }
    }

    protected abstract void StateHandler();

    //protected virtual Vector2 GetFacingDirection()
    //{
    //return transform.right;
    //}

    #region Roaming and Alert
    protected bool LineOfSightCheck()
    {
        if (alerted) return false;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= sightRange)
        {
            Vector2 directionToPlayer = (player.position - transform.position).normalized;

            // Use the obstacleLayer in the raycast
            RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, sightRange, obstacleLayerMask);

            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                TriggerAlert();
                return true;
            }
        }
        return false;
    }

    public virtual void TriggerAlert()
    {
        if (alerted) return;

        alerted = true;
        currentState = EnemyState.Chasing;
        //alertTimer = alertDuration;

        // Instantiate alert FX and start coroutine to handle its lifetime
        if (alertFXPrefab != null)
        {
            GameObject alertFX = Instantiate(alertFXPrefab, transform.position + Vector3.up, Quaternion.identity, transform);
            StartCoroutine(HandleAlertFX(alertFX));
        }

        AlertNearbyEnemies();
    }

    private IEnumerator HandleAlertFX(GameObject alertFX)
    {
        // Wait for the alert duration
        yield return new WaitForSeconds(0.5f);

        // Destroy the alert FX after the alert duration ends
        if (alertFX != null)
        {
            Destroy(alertFX);
        }
    }

    /*protected void HandleAlert()
 {
     alertTimer -= Time.deltaTime;
     if (alertTimer <= 0f)
     {
         currentState = EnemyState.Chasing; // Transition to chasing after alert duration
     }
}*/

    /*protected void AlertNearbyEnemies()
    {
        float alertRadius = sightRange; // Or any other appropriate value
        Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(transform.position, alertRadius);

        foreach (Collider2D collider in nearbyEnemies)
        {
            if (collider.gameObject != gameObject && collider.CompareTag("Enemy"))
            {
                EnemyBase enemy = collider.GetComponent<EnemyBase>();
                if (enemy != null && !enemy.alerted)  // Check if the nearby enemy is not already alerted
                {
                    enemy.TriggerAlert();
                }
            }
        }
    }

    protected void HandleRoaming()
    {
        // Handle the roaming movement if currently in the roaming state
        if (currentState == EnemyState.Roaming)
        {
            Roam();

            idleRoamTimer -= Time.deltaTime;

            // Check if it's time to stop roaming and start idling
            if (idleRoamTimer <= 0f)
            {
                // Switch to idle state
                currentState = EnemyState.Idle;
                SetIdleTimer(); // Set the idle timer for how long to stay idle
            }
        }
        // Handle the idle state
        else if (currentState == EnemyState.Idle)
        {
            idleRoamTimer -= Time.deltaTime;

            // Check if it's time to stop idling and start roaming again
            if (idleRoamTimer <= 0f)
            {
                ChooseNewRoamDirection(); // Pick a new direction to roam
                currentState = EnemyState.Roaming;
            }
        }
    }

    protected void SetIdleTimer()
    {
        // Set the timer for how long the enemy will idle
        idleRoamTimer = Random.Range(idleAfterRoamMin, idleAfterRoamMax);
    }

    protected void ChooseNewRoamDirection()
    {
        // Pick a new random direction but ensure it stays within the patrol radius
        roamDirection = Random.insideUnitCircle.normalized;

        // Set the timer for how long to roam before going idle
        idleRoamTimer = Random.Range(idleRoamDelayMin, idleRoamDelayMax);

        // Set the state to roaming
        currentState = EnemyState.Roaming;
    }

    protected void Roam()
    {
        // Calculate the potential new position
        Vector2 newPosition = rb.position + roamDirection * roamSpeed * Time.deltaTime;

        // Perform a raycast in the direction of movement to detect obstacles
        RaycastHit2D hit = Physics2D.Raycast(transform.position, roamDirection, patrolRadius, obstacleLayerMask);

        // Check if the new position is within the patrol radius from the start position
        if (hit.collider != null)
        {
            Debug.Log("Obstacle detected during roaming. Choosing a new direction.");
            ChooseNewRoamDirection();
        }
        else
        {
            // If no obstacle, move towards the new position
            if (Vector3.Distance(newPosition, startPosition) <= patrolRadius)
            {
                //rb.MovePosition(newPosition);
                rb.MovePosition(newPosition);
                FaceDirection(roamDirection);

                float roamVelocityThreshold = 0.005f;

                // Calculate velocity manually based on the position change for animation purposes
                Vector2 currentVelocity = (rb.position - lastPosition) / Time.deltaTime;
                lastPosition = newPosition;  // Update last position

                // Trigger the walking animation if moving
                if (currentVelocity.magnitude > roamVelocityThreshold)
                {
                    //Debug.Log("Setting isWalking to TRUE.");
                    animator.SetBool("isWalking", true);
                }
                else
                {
                    //Debug.Log("Setting isWalking to FALSE.");
                    animator.SetBool("isWalking", false);
                }
            }
            else
            {
                // If the enemy goes out of the patrol radius, choose a new direction
                ChooseNewRoamDirection();
            }
        }
    }
    #endregion

    public void FaceDirection(Vector2 direction)
    {
        // Only handle left and right movement based on the X direction
        if (Mathf.Abs(direction.x) > 0.01f)  // Ignore very small values to avoid jitter
        {
            // Check if moving right (positive X) or left (negative X)
            if (direction.x > 0)
            {
                // Face right by setting scale to positive
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else
            {
                // Face left by setting scale to negative (flip the sprite horizontally)
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
    }

    #region Take Damage
    // HealthManager will trigger these events
    public void TakeDamage(float amount, Vector2 knockbackDirection, float knockbackForce, KnockbackType knockbackType)
    {
        //Debug.Log("Enemy Hit!");
        currentHealth -= amount;

        if (!alerted)
        {
            TriggerAlert();
        }

        if (!IsInImmuneState(knockbackType))
        {
            Debug.Log("Not immune, checking knockback" + knockbackCooldownTimer);
            ApplyKnockback(knockbackDirection, knockbackForce, knockbackType);
        }
        else
        {
            Debug.Log("Is immune, Timer:" + knockbackCooldownTimer);
        }

        // Trigger the appropriate white flash effect
        if (whiteFlashCooldownTimer <= 0f)
        {
            TriggerWhiteFlash(knockbackType);
        }

        // Trigger the hurt animation if not in high-tier knockback
        if (animator != null)
        {
            animator.SetTrigger("TakeDamage");
        }

        // Handle the death scenario
        if (currentHealth <= 0)
        {
            Die();
            return;
        }
    }

    private void ApplyKnockback(Vector2 direction, float force, KnockbackType knockbackType)
    {
        // Ensure knockback direction is normalized
        knockbackDirection = direction.normalized;
        Debug.Log("Knockback direction: " + knockbackDirection + " with force: " + force);

        if ((!isInKnockback && knockbackCooldownTimer <= 0) || (knockbackType > currentKnockbackType && isInKnockback))
        {
            //Debug.Log("KnockingBack Prep");
            currentKnockbackType = knockbackType;  // Update to the higher knockback type
            if (knockbackType != KnockbackType.Heavy)
            {
                StartCoroutine(SmallKnockback(knockbackDirection, force, knockbackType));
            }
            else
            {
                //Debug.Log("Big One Coming");
                //StartCoroutine(BigKnockback(knockbackDirection, force, knockbackType));
            }
        }
    }

    /*private IEnumerator BigKnockback(Vector2 knockbackDirection, float force, KnockbackType knockbackType)
    {
        Debug.Log("Starting Knockback - Direction: " + knockbackDirection + ", Force: " + force);

        // Ensure the enemy is set to dynamic
        rb.bodyType = RigidbodyType2D.Dynamic;

        // Reset velocity to avoid compounding previous movement
        rb.velocity = Vector2.zero;
        pathfinding.enabled = false;

        // Normalize the knockback direction to ensure consistent force in both X and Y directions
        knockbackDirection = knockbackDirection.normalized;

        // Apply knockback force directly to the Rigidbody2D
        Vector2 appliedForce = knockbackDirection * force;
        rb.AddForce(appliedForce, ForceMode2D.Impulse); // Knockback applied to both X and Y

        // Debug to confirm force applied
        Debug.Log("Applied Force: " + appliedForce);

        // Variables for simulating the knock-up (arc movement)
        float timer = 0f;
        float knockUpHeight = 1f;  // Adjust this value to control the peak height of the arc
        float knockbackDuration = 0.5f;  // Control the overall knockback duration

        // Store the initial local position of the sprite for arc calculations
        Vector3 originalLocalPosition = spriteRenderer.transform.localPosition;

        // Main loop for knockback duration
        while (timer < knockbackDuration)
        {
            // Calculate how far along the knockback process we are (0 to 1)
            float progress = timer / knockbackDuration;

            // Apply the arc visually while the Rigidbody2D handles physical movement
            float verticalOffset = Mathf.Sin(progress * Mathf.PI) * knockUpHeight;
            spriteRenderer.transform.localPosition = new Vector3(
                originalLocalPosition.x,
                originalLocalPosition.y + verticalOffset,
                0
            );

            // Keep the shadow grounded
            shadowTransform.position = new Vector3(rb.position.x, rb.position.y + shadowNormalHeight, 0);

            // Debug to track Rigidbody2D movement (Y-axis should now move)
            Debug.Log("Rigidbody Position: " + rb.position);

            timer += Time.deltaTime;
            yield return null;
        }

        Debug.Log("Knockback finished");

        // Reset the enemy back to kinematic after knockback
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.velocity = Vector2.zero;  // Ensure the enemy stops moving after knockback
        isInKnockback = false;
        knockbackCooldownTimer = knockbackCooldown;  // Reset knockback cooldown

        // Re-enable pathfinding after knockback
        pathfinding.enabled = true;
    }

    /*private IEnumerator BigKnockback(Vector2 knockbackDirection, float force, KnockbackType knockbackType)
    {
        //enemyCollider.enabled = false;
        Debug.Log("KnockingBack" + knockbackDirection);
        isInKnockback = true;
        float timer = 0f;
        pathfinding.enabled = false;
        currentKnockbackForce = force;
        //currentKnockbackForce = Mathf.Min(force, 10f);
        knockbackDirection = knockbackDirection.normalized;
        //Vector2 currentDirection = initialDirection.normalized;
        float reboundDamping = 0f;  // Damping factor to slow down after each rebound
        animator.SetTrigger("ExplosiveKnockback");  // Trigger explosive knockback animation

        // New: Rebound cooldown timer to prevent rapid successions
        float reboundCooldown = 0f;

        // Calculate the total knockback distance
        Vector2 knockbackEndPosition = rb.position + knockbackDirection * currentKnockbackForce;
        Vector2 startPosition = rb.position;
        bool hasRebounded = false;  // Track if a rebound has happened

        // Track air height offset so that it doesn't reset after a rebound
        float currentAirHeightOffset = 0f;
        //rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;  // Reset back to discrete mode
        //rb.AddForce(knockbackDirection * currentKnockbackForce, ForceMode2D.Impulse);
        // Enable continuous collision detection mode for smoother movement

        RaycastHit2D wallHit = Physics2D.Raycast(rb.position, knockbackDirection, force, LayerMask.GetMask("Wall"));

        if (wallHit.collider != null)
        {
            // Get the distance to the wall from the current position
            float distanceToWall = Vector2.Distance(rb.position, wallHit.point);
            Debug.Log("Wall detected ahead! Distance: " + distanceToWall);
        }

        while (timer < knockbackDuration)
        {
            // Calculate the interpolation factor (0 to 1)
            float currentKnockback = timer / knockbackDuration;

            if (wallHit.collider != null)
            {
                float currentDistanceToWall = Vector2.Distance(rb.position, wallHit.point);
                if (currentDistanceToWall < 0.6f)  // Threshold for triggering the rebound
                {
                    Debug.Log("Too close to the wall! Rebounding...");
                    knockbackDirection = Vector2.Reflect(knockbackDirection, wallHit.normal);  // Reflect the knockback direction
                    knockbackEndPosition = rb.position + knockbackDirection * currentKnockbackForce * (1 - currentKnockback);  // Update knockback end position
                    currentKnockbackForce *= reboundDamping;  // Apply damping after rebound
                    hasRebounded = true;
                    reboundCooldown = 0.1f;
                }
            }

            /*if (reboundCooldown <= 0f)  // Only check for collision if cooldown is over
            {
                Vector2 newDirection = HandleWallCollisionsDuringExplosion(knockbackDirection, ref currentKnockbackForce, reboundDamping);
                if (!IsTooCloseToWall(0.5f))
                {
                    if (newDirection != knockbackDirection)
                    {
                        Debug.Log("Recalculate.. New Direction =" + newDirection);
                        knockbackDirection = newDirection;  // Update the knockback direction after rebound
                        knockbackEndPosition = rb.position + knockbackDirection * currentKnockbackForce * (1 - currentKnockback);  // Recalculate end position
                        hasRebounded = true;
                        reboundCooldown = 0.1f;  // Set cooldown after a rebound
                    }
                    else
                    {
                        rb.MovePosition(rb.position + newDirection * 0.3f);
                    }
                }
            }

            // Calculate the grounded position for the shadow (without arc height)
            Vector2 groundedPosition = Vector2.Lerp(startPosition, knockbackEndPosition, currentKnockback);

            // Update the shadow position (stay grounded)
            shadowTransform.position = new Vector3(groundedPosition.x, groundedPosition.y + shadowNormalHeight, 0);

            // Smoothly calculate air height offset after the rebound
            currentAirHeightOffset = hasRebounded
                ? Mathf.Lerp(currentAirHeightOffset, 0f, timer / knockbackDuration)
                : Mathf.Sin(currentKnockback * Mathf.PI) * airHeight;

            // Move horizontally
            Vector2 currentPosition = hasRebounded
                ? Vector2.Lerp(rb.position, knockbackEndPosition, 0.1f)
                : Vector2.Lerp(startPosition, knockbackEndPosition, currentKnockback);


            rb.MovePosition(currentPosition);  // Move horizontally
            // Keep the shadow grounded at the correct position (without Y-offset)
            //shadowTransform.position = new Vector3(rb.position.x, rb.position.y, 0) + new Vector3(0, shadowNormalHeight, 0);
            
            spriteRenderer.transform.localPosition = new Vector3(currentPosition.x, currentPosition.y + currentAirHeightOffset, 0);  // Simulate vertical arc

            // Adjust shadow position (independent of Y-axis movement)
            //shadowOffset = new Vector3(currentPosition.x, shadowOffset.y, shadowTransform.position.z);
            //Debug.Log("Shadow Position: " + shadowTransform.position);
            reboundCooldown -= Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }
        Debug.Log("Finish");
        HandlePostKnockbackState();
        pathfinding.enabled = true;
        // Re-enable pathfinding after explosive knockback ends
        //rb.isKinematic = true;
        isInKnockback = false;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Discrete;  // Reset back to discrete mode
        knockbackCooldownTimer = knockbackCooldown;  // Reset knockback cooldown
        //enemyCollider.enabled = true;
    }*/

    /*private IEnumerator SmallKnockback(Vector2 knockbackDirection, float force, KnockbackType knockbackType)
    {
        isInKnockback = true;
        float timer = 0f;
        currentKnockbackForce = force;  // Track remaining knockback force
        Vector2 currentDirection = knockbackDirection.normalized;
        //Vector2 pathfindingVelocity = new Vector2(speed, speed);  // Simulate pathfinding velocity

        while (timer < knockbackDuration)
        {
            // Check for wall collisions and handle rebounds
            currentDirection = HandleWallCollision(currentDirection);

            // Apply knockback movement
            float knockbackInfluence = Mathf.Lerp(1, 0, timer / knockbackDuration);
            Vector2 knockbackMovement = currentDirection * currentKnockbackForce * knockbackInfluence * Time.deltaTime;

            rb.MovePosition(rb.position + knockbackMovement);  // Apply the movement

            timer += Time.deltaTime;
            yield return null;
        }

        // Re-enable pathfinding after explosive knockback ends
        isInKnockback = false;
        knockbackCooldownTimer = knockbackCooldown;  // Reset knockback cooldown

    }

    private Vector2 HandleWallCollision(Vector2 currentDirection)
    {
        // Use IsCollidingWithWall for the wall collision check
        if (IsCollidingWithWall(currentDirection))
        {
            // Perform the BoxCast again to get the collision normal for reflection
            RaycastHit2D hit = Physics2D.BoxCast(rb.position, new Vector2(0.1f, 0.1f), 0f, currentDirection, 0.1f, LayerMask.GetMask("Wall"));
            if (hit.collider != null)
            {
                // Reflect the direction and reduce knockback force
                Vector2 collisionNormal = hit.normal;
                currentDirection = Vector2.Reflect(currentDirection, collisionNormal);
                currentKnockbackForce *= 0.4f;

                Debug.Log("Rebounded! New direction: " + currentDirection + " Remaining force: " + currentKnockbackForce);
            }
        }
        return currentDirection;
    }

    private bool IsCollidingWithWall(Vector2 direction)
    {
        // Cast a small box in the knockback direction to check if the enemy is about to hit a wall
        RaycastHit2D hit = Physics2D.BoxCast(rb.position, new Vector2(0.1f, 0.1f), 0f, direction, 0.1f, LayerMask.GetMask("Wall"));
        return hit.collider != null;
    }

    // This method checks if the enemy is too close to the wall
    private bool IsTooCloseToWall(float safeDistance)
    {
        Debug.Log("Too close, nudging");
        RaycastHit2D hit = Physics2D.Raycast(rb.position, knockbackDirection, safeDistance, LayerMask.GetMask("Wall"));
        return hit.collider != null;  // Returns true if there is a wall within the safe distance
    }

    private Vector2 HandleWallCollisionsDuringExplosion(Vector2 currentDirection, ref float currentKnockbackForce, float dampingFactor)
    {
        //if (IsCollidingWithWall(currentDirection))
        //{
        // Perform a BoxCast to detect collisions with walls
        RaycastHit2D hit = Physics2D.BoxCast(rb.position, new Vector2(1f, 1f), 0f, currentDirection, 1.2f, LayerMask.GetMask("Wall"));
        if (hit.collider != null)
        {
            Vector2 collisionNormal = hit.normal;

            // Reflect the knockback direction based on the wall's normal
            currentDirection = Vector2.Reflect(currentDirection, collisionNormal);


            currentKnockbackForce *= dampingFactor;

            Debug.Log("Rebounded off wall. New direction: " + currentDirection + " Remaining force: " + currentKnockbackForce);
        }
        //}
        return currentDirection;  // Return the updated direction (if reflected)
    }

    private void HandlePostKnockbackState()
    {
        // Check if the enemy is stuck in a low wall
        RaycastHit2D hit = Physics2D.BoxCast(rb.position, new Vector2(0.1f, 0.1f), 0f, Vector2.zero, 0f, LayerMask.GetMask("Wall"));

        if (hit.collider != null)
        {
            // Nudge the enemy out of the low wall if they are stuck
            Vector2 nudgeDirection = hit.normal;  // Use the wall's normal to push them out
            rb.MovePosition(rb.position + nudgeDirection * 0.5f);  // Small nudge to prevent sticking

            Debug.Log("Nudged out of low wall after explosion knockback.");
        }
    }

    // Trigger white flash effect based on knockback type
    private void TriggerWhiteFlash()
    {
        Debug.Log("Flashing White!");
        StartCoroutine(FlashWhiteEffect());
    }

    // Small knockback (e.g., bullets)
    private IEnumerator FlashWhiteEffect()
    {
        _material.SetColor("_Flashcolor", _flashColor);

        float elapsedTime = 0f;
        while (elapsedTime < flashTime)
        {
            //Debug.Log("Flashing..!");
            elapsedTime += Time.deltaTime;
            float currentFlashAmount = Mathf.Lerp(1f, 0f, (elapsedTime / flashTime));
            SetFlashAmount(currentFlashAmount);
            yield return null; // Brief flash duration
        }
        whiteFlashCooldownTimer = whiteFlashCooldown;
        Debug.Log("Done, Reset Flash CD!" + whiteFlashCooldownTimer);
    }

    private void SetFlashAmount(float amount)
    {
        _material.SetFloat("_FlashAmount", amount);
    }

    // High-tier knockback (e.g., explosions)
    //private IEnumerator FlashWhiteEffectExplosion()
    //{
    //spriteRenderer.color = Color.white;
    //yield return new WaitForSeconds(0.5f); // Slightly longer flash
    //}

    public bool IsInImmuneState()
    {
        //Debug.Log("Checking Immunity..");
        if ((currentState == EnemyState.Charging || currentState == EnemyState.Biting || currentState == EnemyState.EndCharge) && knockbackType != KnockbackType.Heavy)
        {
            return true;  // Ignore lower-tier knockbacks during charging or biting
        }
        return false;
    }

    public void Die()
    {
        if (isDead) return;  // Ensure Die() only executes once
        isDead = true;  // A flag to prevent multiple deaths
        Debug.Log("Enemy is dead, triggering death animation.");
        // Disable the enemy's collider to prevent further interactions
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        // Trigger the death animation if it exists
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        else
        {
            Debug.LogError("Animator component is missing on the enemy!");
        }

        // Start the coroutine to destroy the game object after a delay
        HandleDeath();
    }

    void HandleDeath()
    {
        //currentState = EnemyState.Idle;
        Debug.Log("I die");
        Destroy(gameObject, 1f); // Delay to allow corpse to last for a bit
    }
    #endregion

    protected void HandleCooldown(ref float cooldownTimer)
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }

    protected virtual void OnDrawGizmos()
    {
        // Draw the sight range as a wire sphere
        Gizmos.color = Color.yellow;  // Set the color of the Gizmo
        Gizmos.DrawWireSphere(transform.position, sightRange);

        //Draw the sight range as a wire sphere
        Gizmos.color = Color.blue;  // Set the color of the Gizmo
        Gizmos.DrawWireSphere(transform.position, patrolRadius);
    }
}
/*public abstract class EnemyBase : MonoBehaviour
{
    [Header("General Settings")]
    public Transform player;
    public float speed;
    public float roamSpeed = 0.5f;
    public float playerMovementThreshold = 1.0f;  // Set a threshold value for how far the player needs to move before updating the path
    public LayerMask obstacleLayerMask;

    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("FOV Settings")]
    public float sightRange = 10f;
    public float fieldOfView = 90f;
    public float alertDuration = 1f;
    public GameObject alertFXPrefab;

    [Header("Roaming Settings")]
    //public bool canRoam = true; // Flag to enable or disable roaming
    public float patrolRadius = 2f; // Radius for random roaming
    public float idleRoamDelayMin = 0.2f; // Minimum time between changing directions while idle
    public float idleRoamDelayMax = 1.0f; // Maximum time between changing directions while idle
    public float idleAfterRoamMin = 3.0f; // Minimum time spent idling after roaming
    public float idleAfterRoamMax = 5.0f; // Maximum time spent idling after roaming

    [Header("Knockback Settings")]
    public float knockbackDuration = 0.25f;
    public float knockbackCooldown = 1.0f;
    public float whiteFlashCooldown = 0.25f;
    public float blendingFactor = 0.5f;
    public float flashTime = 0.25f;
    private float currentKnockbackForce;

    [Header("Shadow Settings")]
    public Transform shadowTransform;  // Reference to the shadow GameObject
    public float shadowNormalHeight = 0.15f;  // Default shadow height
    private Vector3 shadowOffset;

    protected SpriteRenderer spriteRenderer;
    protected Material _material;

    protected KnockbackType currentKnockbackType = KnockbackType.Light; // Default to the lowest tier
    protected bool isInKnockback = false; // To track if the enemy is currently in knockback
    protected float knockbackCooldownTimer;
    protected float whiteFlashCooldownTimer;
    protected Color _flashColor = Color.white;
    protected Vector2 knockbackDirection;
    public float airHeight = 1f; // shadow offseyt while enemy is "in the air".

    //public LayerMask AlertTarget;
    protected Vector2 roamDirection;
    protected Vector2 lastPosition;
    protected float idleRoamTimer;

    protected Rigidbody2D rb;
    protected Animator animator;
    protected Collider2D enemyCollider;
    protected bool isAttacking = false;
    protected float alertTimer;
    protected Vector2 startPosition;

    protected bool isDead = false; // Check if the character is dead

    protected enum EnemyState { Idle, Roaming, Alert, Chasing, Biting, Charging, Shooting, Melee, Transitioning, EndCharge }
    protected EnemyState currentState = EnemyState.Idle;

    protected bool alerted = false;

    protected Grid grid;
    protected Pathfinding pathfinding;
    protected Transform target;
    protected List<Node> path = new List<Node>(); // Store the path for this enemy
    protected int currentPathIndex = 0;
    protected float pathUpdateCooldown = 1f; // Time between path updates
    protected float pathUpdateTimer = 0f;
    public float avoidRadius = 1f;  // Radius to detect other enemies for local avoidance
    public float avoidForce = 50f;  // Force applied to avoid other enemies


    private Vector3 lastPlayerPosition;

    public int CurrentPathIndex
    {
        get { return currentPathIndex; }
    }

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        startPosition = transform.position;
    }

    protected virtual void Start()
    {
        pathfinding = FindObjectOfType<Pathfinding>();
        grid = FindObjectOfType<Grid>();
        enemyCollider = GetComponent<Collider2D>();
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        _material = Instantiate(spriteRenderer.material);
        spriteRenderer.material = _material;
        //_material.SetFloat(flashAmount, 0f);

        target = GameObject.FindGameObjectWithTag("Player").transform;
        shadowOffset = shadowTransform.position;  // Save the original shadow position

        rb.useFullKinematicContacts = true;
        lastPlayerPosition = player.position;
        rb.freezeRotation = true;
    }

    protected virtual void FixedUpdate()
    {
        // Placeholder for shared FixedUpdate behavior if any
    }

    protected virtual void Update()
    {
        pathUpdateTimer -= Time.deltaTime;

        float playerMovedDistance = Vector3.Distance(player.position, lastPlayerPosition);

        if (pathUpdateTimer <= 0f && target != null && currentState == EnemyState.Chasing && playerMovedDistance >= playerMovementThreshold)  // Example condition for when enemy chases player
        {
            path = pathfinding.FindPath(transform.position, target.position);

            if (path != null && path.Count > 0)
            {
                currentPathIndex = 0;
                lastPlayerPosition = player.position;
                //Debug.Log("Path generated with " + path.Count + " nodes.");
                pathUpdateTimer = pathUpdateCooldown;  // Reset timer for next update
                //FollowPath();  // Follow the path only if valid
            }
            else
            {
               // Debug.Log("Pathfinding failed to generate a path.");
            }
            FollowPath();  // Follow the path once it's generated
        }

        // Update the knockback cooldown timer
        if (knockbackCooldownTimer > 0)
        {
            knockbackCooldownTimer -= Time.deltaTime;
        }

        // Update the white flash cooldown timer independently
        if (whiteFlashCooldownTimer > 0)
        {
            whiteFlashCooldownTimer -= Time.deltaTime;
        }

        if (!isAttacking)
        {
            LineOfSightCheck();
            StateHandler();
            if (alerted == false)
            {
                HandleRoaming(); // Handle roaming and idling in a loop
            }
        }
    }

    protected abstract void StateHandler();

    //protected virtual Vector2 GetFacingDirection()
    //{
    //return transform.right;
    //}

    #region Enemy Pathfinding
    protected void FollowPath()
    {
        //Debug.Log("FollowPath() is being called");

        // Early exit if the path is invalid
        if (path == null || currentPathIndex < 0 || currentPathIndex >= path.Count)
        {
            Debug.LogWarning("No valid path or path index is out of bounds.");
            return;
        }

        // Get the target node in the path
        Node targetNode = path[currentPathIndex];
        Vector3 targetPosition = targetNode.worldPosition;

        //Debug.Log("Moving towards node at position: " + targetPosition);

        // Calculate direction and distance to the next node
       // Vector2 direction = (targetPosition - transform.position).normalized;

        // Calculate direction toward the next node
        Vector2 directionToNextNode = (path[currentPathIndex].worldPosition - transform.position).normalized;

        // Add local avoidance logic here
        Vector2 avoidanceDirection = CalculateAvoidance();

        // Combine pathfinding direction with avoidance direction
        Vector2 finalDirection = (directionToNextNode + avoidanceDirection).normalized;

        // Make the enemy face the direction of movement
        FaceDirection(directionToNextNode);

        float distanceToNextNode = Vector2.Distance(transform.position, targetPosition);

        // Move the enemy in the combined direction for kinematic Rigidbody2D
        Vector2 newPosition = Vector2.MoveTowards(rb.position, rb.position + finalDirection, speed * Time.fixedDeltaTime);
        rb.MovePosition(newPosition);

        // Check for obstacles with raycasting
        RaycastHit2D hit = Physics2D.Raycast(transform.position, finalDirection, Mathf.Max(distanceToNextNode, 0.5f), obstacleLayerMask);

        // Draw ray for debugging
        Debug.DrawRay(transform.position, finalDirection * distanceToNextNode, Color.red);

        // If no obstacle is detected, move towards the node
        if (hit.collider == null)
        {
            //Debug.Log("No obstacles, moving towards next node.");
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            // If the enemy is close enough to the node, move to the next node
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                currentPathIndex++;
            }
        }
        else
        {
            // Obstacle detected, so recalculate the path and reset currentPathIndex
            //Debug.Log("Obstacle detected! Recalculating path...");
            path = pathfinding.FindPath(transform.position, target.position);
            if (path == null || path.Count == 0)
            {
                Debug.LogWarning("Pathfinding failed. Starting fallback mechanism.");
                StartCoroutine(HandlePathfindingFailure());  // Softly handle the failure
            }
            currentPathIndex = 0;  // Reset to start following the new path
        }

        // Calculate velocity based on change in position for animation purposes
        Vector2 currentVelocity = (rb.position - lastPosition) / Time.deltaTime;
        lastPosition = rb.position;  // Update last position for the next frame

        // Set walking animation based on movement
        if (currentVelocity.magnitude > 0.1f)
        {
            animator.SetBool("isWalking", true);  // Trigger walking animation
        }
        else
        {
            animator.SetBool("isWalking", false);  // Trigger idle animation
        }
    }

    protected Vector2 CalculateAvoidance()
    {
        Vector2 avoidance = Vector2.zero;

        // Find all nearby enemies to avoid
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, avoidRadius, LayerMask.GetMask("Enemy"));
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject != gameObject)  // Avoid itself
            {
                // Calculate direction to move away from the other enemy
                Vector2 directionAwayFromEnemy = (transform.position - hitCollider.transform.position).normalized;

                // Add to the avoidance force
                avoidance += directionAwayFromEnemy * avoidForce;
            }
        }

        return avoidance;
    }

    protected IEnumerator HandlePathfindingFailure()
    {
        //Debug.Log("Handling pathfinding failure...");

        // Option 1: Stop and wait for a moment before retrying
        rb.velocity = Vector2.zero;  // Stop movement temporarily
        yield return new WaitForSeconds(3f);

        // Retry pathfinding after waiting
        path = pathfinding.FindPath(transform.position, target.position);

        // If pathfinding is still failing, patrol or idle as fallback
        if (path == null || path.Count == 0)
        {
            alerted = false;
            Debug.LogWarning("Pathfinding still failed. Entering fallback mode.");

            // Option 2: Patrol around the last known player position, or idle
            // Example: Roam randomly within a radius around the last known target position
            currentState = EnemyState.Idle;
        }
        else
        {
            currentPathIndex = 0;  // Reset to follow the new path if successful
        }
    }
    #endregion

    #region Roaming and Alert
    protected bool LineOfSightCheck()
    {
        if (alerted) return false;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= sightRange)
        {
            Vector2 directionToPlayer = (player.position - transform.position).normalized;

            // Use the obstacleLayer in the raycast
            RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, sightRange, obstacleLayerMask);

            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                TriggerAlert();
                return true;
            }
        }
        return false;
    }

    public virtual void TriggerAlert()
    {
        if (alerted) return;

        alerted = true;
        currentState = EnemyState.Chasing;
        //alertTimer = alertDuration;

        // Instantiate alert FX and start coroutine to handle its lifetime
        if (alertFXPrefab != null)
        {
            GameObject alertFX = Instantiate(alertFXPrefab, transform.position + Vector3.up, Quaternion.identity, transform);
            StartCoroutine(HandleAlertFX(alertFX));
        }

        AlertNearbyEnemies();
    }

    private IEnumerator HandleAlertFX(GameObject alertFX)
    {
        // Wait for the alert duration
        yield return new WaitForSeconds(0.5f);

        // Destroy the alert FX after the alert duration ends
        if (alertFX != null)
        {
            Destroy(alertFX);
        }
    }

    /*protected void HandleAlert()
 {
     alertTimer -= Time.deltaTime;
     if (alertTimer <= 0f)
     {
         currentState = EnemyState.Chasing; // Transition to chasing after alert duration
     }
}

protected void AlertNearbyEnemies()
    {
        float alertRadius = sightRange; // Or any other appropriate value
        Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(transform.position, alertRadius);

        foreach (Collider2D collider in nearbyEnemies)
        {
            if (collider.gameObject != gameObject && collider.CompareTag("Enemy"))
            {
                EnemyBase enemy = collider.GetComponent<EnemyBase>();
                if (enemy != null && !enemy.alerted)  // Check if the nearby enemy is not already alerted
                {
                    enemy.TriggerAlert();
                }
            }
        }
    }

    protected void HandleRoaming()
    {
        // Handle the roaming movement if currently in the roaming state
        if (currentState == EnemyState.Roaming)
        {
            Roam();

            idleRoamTimer -= Time.deltaTime;

            // Check if it's time to stop roaming and start idling
            if (idleRoamTimer <= 0f)
            {
                // Switch to idle state
                currentState = EnemyState.Idle;
                SetIdleTimer(); // Set the idle timer for how long to stay idle
            }
        }
        // Handle the idle state
        else if (currentState == EnemyState.Idle)
        {
            idleRoamTimer -= Time.deltaTime;

            // Check if it's time to stop idling and start roaming again
            if (idleRoamTimer <= 0f)
            {
                ChooseNewRoamDirection(); // Pick a new direction to roam
                currentState = EnemyState.Roaming;
            }
        }
    }

    protected void SetIdleTimer()
    {
        // Set the timer for how long the enemy will idle
        idleRoamTimer = Random.Range(idleAfterRoamMin, idleAfterRoamMax);
    }

    protected void ChooseNewRoamDirection()
    {
        // Pick a new random direction but ensure it stays within the patrol radius
        roamDirection = Random.insideUnitCircle.normalized;

        // Set the timer for how long to roam before going idle
        idleRoamTimer = Random.Range(idleRoamDelayMin, idleRoamDelayMax);

        // Set the state to roaming
        currentState = EnemyState.Roaming;
    }

    protected void Roam()
    {
        // Calculate the potential new position
        Vector2 newPosition = rb.position + roamDirection * roamSpeed * Time.deltaTime;

        // Perform a raycast in the direction of movement to detect obstacles
        RaycastHit2D hit = Physics2D.Raycast(transform.position, roamDirection, patrolRadius, obstacleLayerMask);

        // Check if the new position is within the patrol radius from the start position
        if (hit.collider != null)
        {
            Debug.Log("Obstacle detected during roaming. Choosing a new direction.");
            ChooseNewRoamDirection();
        }
        else
        {
            // If no obstacle, move towards the new position
            if (Vector3.Distance(newPosition, startPosition) <= patrolRadius)
            {
                //rb.MovePosition(newPosition);
                rb.MovePosition(newPosition);
                FaceDirection(roamDirection);

                float roamVelocityThreshold = 0.005f;

                // Calculate velocity manually based on the position change for animation purposes
                Vector2 currentVelocity = (rb.position - lastPosition) / Time.deltaTime;
                lastPosition = newPosition;  // Update last position

                // Trigger the walking animation if moving
                if (currentVelocity.magnitude > roamVelocityThreshold)
                {
                    //Debug.Log("Setting isWalking to TRUE.");
                    animator.SetBool("isWalking", true);
                }
                else
                {
                    //Debug.Log("Setting isWalking to FALSE.");
                    animator.SetBool("isWalking", false);
                }
            }
            else
            {
                // If the enemy goes out of the patrol radius, choose a new direction
                ChooseNewRoamDirection();
            }
        }
    }
    #endregion

    public void FaceDirection(Vector2 direction)
    {
        // Only handle left and right movement based on the X direction
        if (Mathf.Abs(direction.x) > 0.01f)  // Ignore very small values to avoid jitter
        {
            // Check if moving right (positive X) or left (negative X)
            if (direction.x > 0)
            {
                // Face right by setting scale to positive
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else
            {
                // Face left by setting scale to negative (flip the sprite horizontally)
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
    }

    #region Take Damage
    // HealthManager will trigger these events
    public void TakeDamage(float amount, Vector2 knockbackDirection, float knockbackForce, KnockbackType knockbackType)
    {
        //Debug.Log("Enemy Hit!");
        currentHealth -= amount;

        if (!alerted)
        {
            TriggerAlert();
        }

        if (!IsInImmuneState(knockbackType))
        {
            Debug.Log("Not immune, checking knockback" + knockbackCooldownTimer);
            ApplyKnockback(knockbackDirection, knockbackForce, knockbackType);
        }
        else
        {
            Debug.Log("Is immune, Timer:" + knockbackCooldownTimer);
        }

        // Trigger the appropriate white flash effect
        if (whiteFlashCooldownTimer <= 0f)
        {
            TriggerWhiteFlash(knockbackType);
        }

        // Trigger the hurt animation if not in high-tier knockback
        if (animator != null)
        {
            animator.SetTrigger("TakeDamage");
        }

        // Handle the death scenario
        if (currentHealth <= 0)
        {
            Die();
            return;
        }
    }

    private void ApplyKnockback(Vector2 direction, float force, KnockbackType knockbackType)
    {
        // Ensure knockback direction is normalized
        knockbackDirection = direction.normalized;
        Debug.Log("Knockback direction: " + knockbackDirection + " with force: " + force);

        if ((!isInKnockback && knockbackCooldownTimer <= 0)|| (knockbackType > currentKnockbackType && isInKnockback))
        {
            //Debug.Log("KnockingBack Prep");
            currentKnockbackType = knockbackType;  // Update to the higher knockback type
            if (knockbackType != KnockbackType.Heavy)
            {
                StartCoroutine(SmallKnockback(knockbackDirection, force, knockbackType));
            }
            else
            {
                //Debug.Log("Big One Coming");
                //StartCoroutine(BigKnockback(knockbackDirection, force, knockbackType));
            }
        }
    }

    /*private IEnumerator BigKnockback(Vector2 knockbackDirection, float force, KnockbackType knockbackType)
    {
        Debug.Log("Starting Knockback - Direction: " + knockbackDirection + ", Force: " + force);

        // Ensure the enemy is set to dynamic
        rb.bodyType = RigidbodyType2D.Dynamic;

        // Reset velocity to avoid compounding previous movement
        rb.velocity = Vector2.zero;
        pathfinding.enabled = false;

        // Normalize the knockback direction to ensure consistent force in both X and Y directions
        knockbackDirection = knockbackDirection.normalized;

        // Apply knockback force directly to the Rigidbody2D
        Vector2 appliedForce = knockbackDirection * force;
        rb.AddForce(appliedForce, ForceMode2D.Impulse); // Knockback applied to both X and Y

        // Debug to confirm force applied
        Debug.Log("Applied Force: " + appliedForce);

        // Variables for simulating the knock-up (arc movement)
        float timer = 0f;
        float knockUpHeight = 1f;  // Adjust this value to control the peak height of the arc
        float knockbackDuration = 0.5f;  // Control the overall knockback duration

        // Store the initial local position of the sprite for arc calculations
        Vector3 originalLocalPosition = spriteRenderer.transform.localPosition;

        // Main loop for knockback duration
        while (timer < knockbackDuration)
        {
            // Calculate how far along the knockback process we are (0 to 1)
            float progress = timer / knockbackDuration;

            // Apply the arc visually while the Rigidbody2D handles physical movement
            float verticalOffset = Mathf.Sin(progress * Mathf.PI) * knockUpHeight;
            spriteRenderer.transform.localPosition = new Vector3(
                originalLocalPosition.x,
                originalLocalPosition.y + verticalOffset,
                0
            );

            // Keep the shadow grounded
            shadowTransform.position = new Vector3(rb.position.x, rb.position.y + shadowNormalHeight, 0);

            // Debug to track Rigidbody2D movement (Y-axis should now move)
            Debug.Log("Rigidbody Position: " + rb.position);

            timer += Time.deltaTime;
            yield return null;
        }

        Debug.Log("Knockback finished");

        // Reset the enemy back to kinematic after knockback
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.velocity = Vector2.zero;  // Ensure the enemy stops moving after knockback
        isInKnockback = false;
        knockbackCooldownTimer = knockbackCooldown;  // Reset knockback cooldown

        // Re-enable pathfinding after knockback
        pathfinding.enabled = true;
    }

    /*private IEnumerator BigKnockback(Vector2 knockbackDirection, float force, KnockbackType knockbackType)
    {
        //enemyCollider.enabled = false;
        Debug.Log("KnockingBack" + knockbackDirection);
        isInKnockback = true;
        float timer = 0f;
        pathfinding.enabled = false;
        currentKnockbackForce = force;
        //currentKnockbackForce = Mathf.Min(force, 10f);
        knockbackDirection = knockbackDirection.normalized;
        //Vector2 currentDirection = initialDirection.normalized;
        float reboundDamping = 0f;  // Damping factor to slow down after each rebound
        animator.SetTrigger("ExplosiveKnockback");  // Trigger explosive knockback animation

        // New: Rebound cooldown timer to prevent rapid successions
        float reboundCooldown = 0f;

        // Calculate the total knockback distance
        Vector2 knockbackEndPosition = rb.position + knockbackDirection * currentKnockbackForce;
        Vector2 startPosition = rb.position;
        bool hasRebounded = false;  // Track if a rebound has happened

        // Track air height offset so that it doesn't reset after a rebound
        float currentAirHeightOffset = 0f;
        //rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;  // Reset back to discrete mode
        //rb.AddForce(knockbackDirection * currentKnockbackForce, ForceMode2D.Impulse);
        // Enable continuous collision detection mode for smoother movement

        RaycastHit2D wallHit = Physics2D.Raycast(rb.position, knockbackDirection, force, LayerMask.GetMask("Wall"));

        if (wallHit.collider != null)
        {
            // Get the distance to the wall from the current position
            float distanceToWall = Vector2.Distance(rb.position, wallHit.point);
            Debug.Log("Wall detected ahead! Distance: " + distanceToWall);
        }

        while (timer < knockbackDuration)
        {
            // Calculate the interpolation factor (0 to 1)
            float currentKnockback = timer / knockbackDuration;

            if (wallHit.collider != null)
            {
                float currentDistanceToWall = Vector2.Distance(rb.position, wallHit.point);
                if (currentDistanceToWall < 0.6f)  // Threshold for triggering the rebound
                {
                    Debug.Log("Too close to the wall! Rebounding...");
                    knockbackDirection = Vector2.Reflect(knockbackDirection, wallHit.normal);  // Reflect the knockback direction
                    knockbackEndPosition = rb.position + knockbackDirection * currentKnockbackForce * (1 - currentKnockback);  // Update knockback end position
                    currentKnockbackForce *= reboundDamping;  // Apply damping after rebound
                    hasRebounded = true;
                    reboundCooldown = 0.1f;
                }
            }

            /*if (reboundCooldown <= 0f)  // Only check for collision if cooldown is over
            {
                Vector2 newDirection = HandleWallCollisionsDuringExplosion(knockbackDirection, ref currentKnockbackForce, reboundDamping);
                if (!IsTooCloseToWall(0.5f))
                {
                    if (newDirection != knockbackDirection)
                    {
                        Debug.Log("Recalculate.. New Direction =" + newDirection);
                        knockbackDirection = newDirection;  // Update the knockback direction after rebound
                        knockbackEndPosition = rb.position + knockbackDirection * currentKnockbackForce * (1 - currentKnockback);  // Recalculate end position
                        hasRebounded = true;
                        reboundCooldown = 0.1f;  // Set cooldown after a rebound
                    }
                    else
                    {
                        rb.MovePosition(rb.position + newDirection * 0.3f);
                    }
                }
            }

            // Calculate the grounded position for the shadow (without arc height)
            Vector2 groundedPosition = Vector2.Lerp(startPosition, knockbackEndPosition, currentKnockback);

            // Update the shadow position (stay grounded)
            shadowTransform.position = new Vector3(groundedPosition.x, groundedPosition.y + shadowNormalHeight, 0);

            // Smoothly calculate air height offset after the rebound
            currentAirHeightOffset = hasRebounded
                ? Mathf.Lerp(currentAirHeightOffset, 0f, timer / knockbackDuration)
                : Mathf.Sin(currentKnockback * Mathf.PI) * airHeight;

            // Move horizontally
            Vector2 currentPosition = hasRebounded
                ? Vector2.Lerp(rb.position, knockbackEndPosition, 0.1f)
                : Vector2.Lerp(startPosition, knockbackEndPosition, currentKnockback);


            rb.MovePosition(currentPosition);  // Move horizontally
            // Keep the shadow grounded at the correct position (without Y-offset)
            //shadowTransform.position = new Vector3(rb.position.x, rb.position.y, 0) + new Vector3(0, shadowNormalHeight, 0);
            
            spriteRenderer.transform.localPosition = new Vector3(currentPosition.x, currentPosition.y + currentAirHeightOffset, 0);  // Simulate vertical arc

            // Adjust shadow position (independent of Y-axis movement)
            //shadowOffset = new Vector3(currentPosition.x, shadowOffset.y, shadowTransform.position.z);
            //Debug.Log("Shadow Position: " + shadowTransform.position);
            reboundCooldown -= Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }
        Debug.Log("Finish");
        HandlePostKnockbackState();
        pathfinding.enabled = true;
        // Re-enable pathfinding after explosive knockback ends
        //rb.isKinematic = true;
        isInKnockback = false;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Discrete;  // Reset back to discrete mode
        knockbackCooldownTimer = knockbackCooldown;  // Reset knockback cooldown
        //enemyCollider.enabled = true;
    }

    private IEnumerator SmallKnockback(Vector2 knockbackDirection, float force, KnockbackType knockbackType)
    {
        isInKnockback = true;
        float timer = 0f;
        currentKnockbackForce = force;  // Track remaining knockback force
        Vector2 currentDirection = knockbackDirection.normalized;
        //Vector2 pathfindingVelocity = new Vector2(speed, speed);  // Simulate pathfinding velocity

        while (timer < knockbackDuration)
        {
            // Check for wall collisions and handle rebounds
            currentDirection = HandleWallCollision(currentDirection);

            // Apply knockback movement
            float knockbackInfluence = Mathf.Lerp(1, 0, timer / knockbackDuration);
            Vector2 knockbackMovement = currentDirection * currentKnockbackForce * knockbackInfluence * Time.deltaTime;

            rb.MovePosition(rb.position + knockbackMovement);  // Apply the movement

            timer += Time.deltaTime;
            yield return null;
        }

        // Re-enable pathfinding after explosive knockback ends
        isInKnockback = false;
        knockbackCooldownTimer = knockbackCooldown;  // Reset knockback cooldown

    }

    private Vector2 HandleWallCollision(Vector2 currentDirection)
    {
        // Use IsCollidingWithWall for the wall collision check
        if (IsCollidingWithWall(currentDirection))
        {
            // Perform the BoxCast again to get the collision normal for reflection
            RaycastHit2D hit = Physics2D.BoxCast(rb.position, new Vector2(0.1f, 0.1f), 0f, currentDirection, 0.1f, LayerMask.GetMask("Wall"));
            if (hit.collider != null)
            {
                // Reflect the direction and reduce knockback force
                Vector2 collisionNormal = hit.normal;
                currentDirection = Vector2.Reflect(currentDirection, collisionNormal);
                currentKnockbackForce *= 0.4f;

                Debug.Log("Rebounded! New direction: " + currentDirection + " Remaining force: " + currentKnockbackForce);
            }
        }
        return currentDirection;
    }

    private bool IsCollidingWithWall(Vector2 direction)
    {
        // Cast a small box in the knockback direction to check if the enemy is about to hit a wall
        RaycastHit2D hit = Physics2D.BoxCast(rb.position, new Vector2(0.1f, 0.1f), 0f, direction, 0.1f, LayerMask.GetMask("Wall"));
        return hit.collider != null;
    }

    // This method checks if the enemy is too close to the wall
    private bool IsTooCloseToWall(float safeDistance)
    {
        Debug.Log("Too close, nudging");
        RaycastHit2D hit = Physics2D.Raycast(rb.position, knockbackDirection, safeDistance, LayerMask.GetMask("Wall"));
        return hit.collider != null;  // Returns true if there is a wall within the safe distance
    }

    private Vector2 HandleWallCollisionsDuringExplosion(Vector2 currentDirection, ref float currentKnockbackForce, float dampingFactor)
    {
        //if (IsCollidingWithWall(currentDirection))
        //{
            // Perform a BoxCast to detect collisions with walls
            RaycastHit2D hit = Physics2D.BoxCast(rb.position, new Vector2(1f, 1f), 0f, currentDirection, 1.2f, LayerMask.GetMask("Wall"));
            if (hit.collider != null)
            {
                Vector2 collisionNormal = hit.normal;

                // Reflect the knockback direction based on the wall's normal
                currentDirection = Vector2.Reflect(currentDirection, collisionNormal);

              
                currentKnockbackForce *= dampingFactor;

                Debug.Log("Rebounded off wall. New direction: " + currentDirection + " Remaining force: " + currentKnockbackForce);
            }
        //}
        return currentDirection;  // Return the updated direction (if reflected)
    }

    private void HandlePostKnockbackState()
    {
        // Check if the enemy is stuck in a low wall
        RaycastHit2D hit = Physics2D.BoxCast(rb.position, new Vector2(0.1f, 0.1f), 0f, Vector2.zero, 0f, LayerMask.GetMask("Wall"));

        if (hit.collider != null)
        {
            // Nudge the enemy out of the low wall if they are stuck
            Vector2 nudgeDirection = hit.normal;  // Use the wall's normal to push them out
            rb.MovePosition(rb.position + nudgeDirection * 0.5f);  // Small nudge to prevent sticking

            Debug.Log("Nudged out of low wall after explosion knockback.");
        }
    }

    // Trigger white flash effect based on knockback type
    private void TriggerWhiteFlash(KnockbackType knockbackType)
    {
        Debug.Log("Flashing White!");
        StartCoroutine(FlashWhiteEffect());
    }

    // Small knockback (e.g., bullets)
    private IEnumerator FlashWhiteEffect()
    {
        _material.SetColor("_Flashcolor", _flashColor);

        float elapsedTime = 0f;
        while(elapsedTime < flashTime)
        {
            //Debug.Log("Flashing..!");
            elapsedTime += Time.deltaTime;
            float currentFlashAmount = Mathf.Lerp(1f, 0f, (elapsedTime /flashTime));
            SetFlashAmount(currentFlashAmount);
            yield return null; // Brief flash duration
        }
        whiteFlashCooldownTimer = whiteFlashCooldown;
        Debug.Log("Done, Reset Flash CD!" + whiteFlashCooldownTimer);
    }

    private void SetFlashAmount(float amount)
    {
        _material.SetFloat("_FlashAmount", amount);
    }

    // High-tier knockback (e.g., explosions)
    //private IEnumerator FlashWhiteEffectExplosion()
    //{
        //spriteRenderer.color = Color.white;
        //yield return new WaitForSeconds(0.5f); // Slightly longer flash
    //}

    public bool IsInImmuneState(KnockbackType knockbackType)
    {
        //Debug.Log("Checking Immunity..");
        if ((currentState == EnemyState.Charging || currentState == EnemyState.Biting || currentState == EnemyState.EndCharge) && knockbackType != KnockbackType.Heavy)
        {
            return true;  // Ignore lower-tier knockbacks during charging or biting
        }
        return false;
    }

    public void Die()
    {
        if (isDead) return;  // Ensure Die() only executes once
        isDead = true;  // A flag to prevent multiple deaths
        Debug.Log("Enemy is dead, triggering death animation.");
        // Disable the enemy's collider to prevent further interactions
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        // Trigger the death animation if it exists
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        else
        {
            Debug.LogError("Animator component is missing on the enemy!");
        }

        // Start the coroutine to destroy the game object after a delay
        HandleDeath();
    }

    void HandleDeath()
    {
        //currentState = EnemyState.Idle;
        Debug.Log("I die");
        Destroy(gameObject, 1f); // Delay to allow corpse to last for a bit
    }
    #endregion

    protected void HandleCooldown(ref float cooldownTimer)
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }

    protected virtual void OnDrawGizmos()
   {
        // Draw the sight range as a wire sphere
        Gizmos.color = Color.yellow;  // Set the color of the Gizmo
        Gizmos.DrawWireSphere(transform.position, sightRange);

        //Draw the sight range as a wire sphere
        Gizmos.color = Color.blue;  // Set the color of the Gizmo
        Gizmos.DrawWireSphere(transform.position, patrolRadius);
    }
}*/

/*public abstract class EnemyBase : MonoBehaviour
{
    [Header("General Settings")]
    public Transform player;
    public float speed;
    public float roamSpeed = 0.5f;
    public float playerMovementThreshold = 1.0f;  // Set a threshold value for how far the player needs to move before updating the path

    public LayerMask obstacleLayerMask;

    [Header("FOV Settings")]
    public float sightRange = 10f;
    public float fieldOfView = 90f;
    public float alertDuration = 1f;
    public GameObject alertFXPrefab;

    [Header("Roaming Settings")]
    public bool canRoam = true; // Flag to enable or disable roaming
    public float patrolRadius = 2f; // Radius for random roaming
    public float idleRoamDelayMin = 0.2f; // Minimum time between changing directions while idle
    public float idleRoamDelayMax = 1.0f; // Maximum time between changing directions while idle
    public float idleAfterRoamMin = 3.0f; // Minimum time spent idling after roaming
    public float idleAfterRoamMax = 5.0f; // Maximum time spent idling after roaming

    //public LayerMask AlertTarget;
    protected Vector2 roamDirection;
    protected Vector2 lastPosition;
    protected float idleRoamTimer;

    // Knockback and alert management
    private bool isInKnockback = false;
    private bool isAlerted = false;
    protected Vector2 knockbackDirection;

    protected Rigidbody2D rb;
    protected Animator animator;
    protected float currentHealth;
    protected bool isAttacking = false;
    protected float alertTimer;
    protected Vector2 startPosition;

    protected enum EnemyState { Idle, Roaming, Alert, Chasing, Biting, Charging, Shooting, Melee, Transitioning, EndCharge }
    protected EnemyState currentState = EnemyState.Idle;

    protected bool alerted = false;
    protected HealthManager healthManager;

    protected Grid grid;
    protected Pathfinding pathfinding;
    protected Transform target;
    protected List<Node> path = new List<Node>(); // Store the path for this enemy
    protected int currentPathIndex = 0;
    protected float pathUpdateCooldown = 1f; // Time between path updates
    protected float pathUpdateTimer = 0f;

    private Vector3 lastPlayerPosition;

    public int CurrentPathIndex
    {
        get { return currentPathIndex; }
    }

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        healthManager = GetComponent<HealthManager>();
        if (healthManager != null)
        {
            healthManager.InitializeHealth(healthManager.maxHealth, animator);  // Initialize health with max health and animator
        }
        else
        {
            Debug.LogError("HealthManager component not found on " + gameObject.name);
        }
        startPosition = transform.position;
    }

    protected virtual void Start()
    {
        pathfinding = FindObjectOfType<Pathfinding>();
        grid = FindObjectOfType<Grid>();

        target = GameObject.FindGameObjectWithTag("Player").transform;

        lastPlayerPosition = player.position;
        rb.freezeRotation = true;
    }

    protected virtual void FixedUpdate()
    {
        // Placeholder for shared FixedUpdate behavior if any
    }

    protected virtual void Update()
    {
        pathUpdateTimer -= Time.deltaTime;

        if (path == null)
        {
            Debug.Log("Path is null");
        }
        else
        {
            Debug.Log("Path has " + path.Count + " nodes.");
        }

        float playerMovedDistance = Vector3.Distance(player.position, lastPlayerPosition);

        if (pathUpdateTimer <= 0f && target != null && currentState == EnemyState.Chasing && playerMovedDistance >= playerMovementThreshold)  // Example condition for when enemy chases player
        {
            path = pathfinding.FindPath(transform.position, target.position);

            if (path != null && path.Count > 0)
            {
                currentPathIndex = 0;
                lastPlayerPosition = player.position;
                Debug.Log("Path generated with " + path.Count + " nodes.");
                pathUpdateTimer = pathUpdateCooldown;  // Reset timer for next update
                FollowPath();  // Follow the path only if valid
            }
            else
            {
                Debug.Log("Pathfinding failed to generate a path.");
            }
            FollowPath();  // Follow the path once it's generated
        }

        if (healthManager.IsDead()) return; // Check if the enemy is dead


        if (!isAttacking)
        {
            LineOfSightCheck();
            StateHandler();
            if (canRoam)
            {
                HandleRoaming(); // Handle roaming and idling in a loop
            }

            if (currentState == EnemyState.Alert)
            {
                HandleAlert();  // Manage the Alert state timing
            }
        }
    }

    protected abstract void StateHandler();

    //protected virtual Vector2 GetFacingDirection()
    //{
    //return transform.right;
    //}

    protected void FollowPath()
    {
        Debug.Log("FollowPath() is being called");

        // Early exit if the path is invalid
        if (path == null || currentPathIndex < 0 || currentPathIndex >= path.Count)
        {
            Debug.LogWarning("No valid path or path index is out of bounds.");
            return;
        }

        // Get the target node in the path
        Node targetNode = path[currentPathIndex];
        Vector3 targetPosition = targetNode.worldPosition;

        Debug.Log("Moving towards node at position: " + targetPosition);

        // Calculate direction and distance to the next node
        Vector2 direction = (targetPosition - transform.position).normalized;
        float distanceToNextNode = Vector2.Distance(transform.position, targetPosition);

        // Check for obstacles with raycasting
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, Mathf.Max(distanceToNextNode, 0.5f), obstacleLayerMask);

        // Draw ray for debugging
        Debug.DrawRay(transform.position, direction * distanceToNextNode, Color.red);

        // If no obstacle is detected, move towards the node
        if (hit.collider == null)
        {
            Debug.Log("No obstacles, moving towards next node.");
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            // If the enemy is close enough to the node, move to the next node
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                currentPathIndex++;
            }
        }
        else
        {
            // Obstacle detected, so recalculate the path and reset currentPathIndex
            Debug.Log("Obstacle detected! Recalculating path...");
            path = pathfinding.FindPath(transform.position, target.position);
            if (path == null || path.Count == 0)
            {
                Debug.LogWarning("Pathfinding failed. Starting fallback mechanism.");
                StartCoroutine(HandlePathfindingFailure());  // Softly handle the failure
            }
            currentPathIndex = 0;  // Reset to start following the new path
        }
    }

    protected IEnumerator HandlePathfindingFailure()
    {
        Debug.Log("Handling pathfinding failure...");

        // Option 1: Stop and wait for a moment before retrying
        rb.velocity = Vector2.zero;  // Stop movement temporarily
        yield return new WaitForSeconds(1.5f);  // Wait for 1.5 seconds (you can adjust this)

        // Retry pathfinding after waiting
        path = pathfinding.FindPath(transform.position, target.position);

        // If pathfinding is still failing, patrol or idle as fallback
        if (path == null || path.Count == 0)
        {
            alerted = false;
            Debug.LogWarning("Pathfinding still failed. Entering fallback mode.");

            // Option 2: Patrol around the last known player position, or idle
            // Example: Roam randomly within a radius around the last known target position
            currentState = EnemyState.Roaming;
        }
        else
        {
            currentPathIndex = 0;  // Reset to follow the new path if successful
        }
    }

    // HealthManager will trigger these events
    public void ApplyKnockback(Vector2 direction, float force)
    {
        if (isInKnockback) return;  // If already in knockback, ignore further knockbacks

        isInKnockback = true;
        knockbackDirection = direction;
        StartCoroutine(PerformKnockback(force));
    }

    private IEnumerator PerformKnockback(float force)
    {
        float knockbackDuration = 0.2f;  // Arbitrary duration for knockback
        while (knockbackDuration > 0f)
        {
            rb.MovePosition(rb.position + knockbackDirection * force * Time.deltaTime);
            knockbackDuration -= Time.deltaTime;
            yield return null;
        }
        isInKnockback = false;
        EndKnockback();  // Transition back to normal behavior
    }

    // Trigger the white flash effect for damage visual feedback
    public void TriggerWhiteFlash()
    {
        StartCoroutine(WhiteFlashEffect());
    }

    // White flash visual effect
    private IEnumerator WhiteFlashEffect()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(0.05f);  // Flash duration
        spriteRenderer.color = originalColor;
    }

    // End knockback and resume normal behavior
    protected void EndKnockback()
    {
        if (!isAlerted)
        {
            // If not yet alerted, enter the Alert state first
            TriggerAlert();
        }
        else
        {
            // Resume chasing if already alerted
            currentState = EnemyState.Chasing;
        }
    }


    protected bool LineOfSightCheck()
    {
        if (alerted) return false;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= sightRange)
        {
            Vector2 directionToPlayer = (player.position - transform.position).normalized;

            // Use the obstacleLayer in the raycast
            RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, sightRange, obstacleLayerMask);

            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                TriggerAlert();
                return true;
            }
        }
        return false;
    }

    public virtual void TriggerAlert()
    {
        if (alerted) return;

        alerted = true;
        currentState = EnemyState.Alert;
        alertTimer = alertDuration;

        // Instantiate alert FX and start coroutine to handle its lifetime
        if (alertFXPrefab != null)
        {
            GameObject alertFX = Instantiate(alertFXPrefab, transform.position + Vector3.up, Quaternion.identity, transform);
            StartCoroutine(HandleAlertFX(alertFX));
        }

        AlertNearbyEnemies();
    }


    private IEnumerator HandleAlertFX(GameObject alertFX)
    {
        // Wait for the alert duration
        yield return new WaitForSeconds(alertDuration);

        // Destroy the alert FX after the alert duration ends
        if (alertFX != null)
        {
            Destroy(alertFX);
        }
    }

    protected void HandleAlert()
    {
        alertTimer -= Time.deltaTime;
        if (alertTimer <= 0f)
        {
            currentState = EnemyState.Chasing; // Transition to chasing after alert duration
        }
    }

    protected void AlertNearbyEnemies()
    {
        float alertRadius = sightRange; // Or any other appropriate value
        Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(transform.position, alertRadius);

        foreach (Collider2D collider in nearbyEnemies)
        {
            if (collider.gameObject != gameObject && collider.CompareTag("Enemy"))
            {
                EnemyBase enemy = collider.GetComponent<EnemyBase>();
                if (enemy != null && !enemy.alerted)  // Check if the nearby enemy is not already alerted
                {
                    enemy.TriggerAlert();
                }
            }
        }
    }

    protected void HandleRoaming()
    {
        // Handle the roaming movement if currently in the roaming state
        if (currentState == EnemyState.Roaming)
        {
            Roam();

            idleRoamTimer -= Time.deltaTime;

            // Check if it's time to stop roaming and start idling
            if (idleRoamTimer <= 0f)
            {
                // Switch to idle state
                currentState = EnemyState.Idle;
                SetIdleTimer(); // Set the idle timer for how long to stay idle
            }
        }
        // Handle the idle state
        else if (currentState == EnemyState.Idle)
        {
            idleRoamTimer -= Time.deltaTime;

            // Check if it's time to stop idling and start roaming again
            if (idleRoamTimer <= 0f)
            {
                ChooseNewRoamDirection(); // Pick a new direction to roam
                currentState = EnemyState.Roaming;
            }
        }
    }

    protected void SetIdleTimer()
    {
        // Set the timer for how long the enemy will idle
        idleRoamTimer = Random.Range(idleAfterRoamMin, idleAfterRoamMax);
    }

    protected void ChooseNewRoamDirection()
    {
        // Pick a new random direction but ensure it stays within the patrol radius
        roamDirection = Random.insideUnitCircle.normalized;

        // Set the timer for how long to roam before going idle
        idleRoamTimer = Random.Range(idleRoamDelayMin, idleRoamDelayMax);

        // Set the state to roaming
        currentState = EnemyState.Roaming;
    }

    protected void Roam()
    {
        // Calculate the potential new position
        Vector2 newPosition = rb.position + roamDirection * roamSpeed * Time.deltaTime;

        // Perform a raycast in the direction of movement to detect obstacles
        RaycastHit2D hit = Physics2D.Raycast(transform.position, roamDirection, patrolRadius, obstacleLayerMask);

        // Check if the new position is within the patrol radius from the start position
        if (hit.collider != null)
        {
            Debug.Log("Obstacle detected during roaming. Choosing a new direction.");
            ChooseNewRoamDirection();
        }
        else
        {
            // If no obstacle, move towards the new position
            if (Vector3.Distance(newPosition, startPosition) <= patrolRadius)
            {
                //rb.MovePosition(newPosition);
                rb.MovePosition(newPosition);
                FaceDirection(roamDirection);

                float roamVelocityThreshold = 0.005f;

                // Calculate velocity manually based on the position change for animation purposes
                Vector2 currentVelocity = (rb.position - lastPosition) / Time.deltaTime;
                lastPosition = newPosition;  // Update last position

                // Trigger the walking animation if moving
                if (currentVelocity.magnitude > roamVelocityThreshold)
                {
                    Debug.Log("Setting isWalking to TRUE.");
                    animator.SetBool("isWalking", true);
                }
                else
                {
                    Debug.Log("Setting isWalking to FALSE.");
                    animator.SetBool("isWalking", false);
                }
            }
            else
            {
                // If the enemy goes out of the patrol radius, choose a new direction
                ChooseNewRoamDirection();
            }
        }
    }

    public void FaceDirection(Vector2 direction)
    {
        // Only handle left and right movement based on the X direction
        if (Mathf.Abs(direction.x) > 0.01f)  // Ignore very small values to avoid jitter
        {
            // Check if moving right (positive X) or left (negative X)
            if (direction.x > 0)
            {
                // Face right by setting scale to positive
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else
            {
                // Face left by setting scale to negative (flip the sprite horizontally)
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
    }

    protected void HandleCooldown(ref float cooldownTimer)
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }

    public void TakeDamage(float amount, Vector2 knockbackDirection, float knockbackForce, KnockbackType knockbackType)
    {
        healthManager.TakeDamage(amount, knockbackDirection, knockbackForce, knockbackType);
        TriggerAlert();  // Alert nearby enemies or update AI
    }

    public bool IsInImmuneState(KnockbackType knockbackType)
    {
        if ((currentState == EnemyState.Charging || currentState == EnemyState.Biting) && knockbackType != KnockbackType.Explosion)
        {
            return true;  // Ignore lower-tier knockbacks during charging or biting
        }
        return false;
    }


    public void HandleDeath()
    {
        currentState = EnemyState.Idle;
        Debug.Log("I die");
        //animator.SetTrigger("Die");
        Destroy(gameObject, 1.5f); // Delay to allow death animation
    }

    protected virtual void OnDrawGizmos()
    {
        // Draw the sight range as a wire sphere
        Gizmos.color = Color.yellow;  // Set the color of the Gizmo
        Gizmos.DrawWireSphere(transform.position, sightRange);

        // Draw the sight range as a wire sphere
        Gizmos.color = Color.blue;  // Set the color of the Gizmo
        Gizmos.DrawWireSphere(transform.position, patrolRadius);
    }
}*/