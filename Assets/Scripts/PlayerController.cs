using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    public Camera sceneCamera;
    public Rigidbody2D rb;
    public Animator animator;
    public Transform gunTransform;
    public Transform firePointTransform;
    public SpriteRenderer playerSprite;
    //public SpriteRenderer gunSprite;
    public KnockbackManager knockbackManager;
    public PlayerStatManager playerStats;
    public WeaponManager weaponManager;
    public BoxCollider2D playerCollider;
    public CircleCollider2D pickupCollider;
    public Transform shadowTransform;
    private Transform defaultSpawnPoint;

    public Vector2 aimDirection { get; private set; }  // Expose aimDirection

    private Vector2 moveDirection;
    private Vector2 mousePosition;
    private Vector3 shadowOffset;

    public float dodgeSpeed = 12f;
    public float dodgeDuration = 0.5f;
    public float dodgeCooldown = 2f;
    private float footstepTimer = 0f;
    public float footstepInterval = 0.15f;

    //private string currentSurface = "Grass";
    private float directionIndex;
    private float dodgeCooldownTimer = 0f;
    private bool isDodging = false;
    private bool playerIsDead = false;
    private bool isInTransit = false;
    private bool isDisabled = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        //sceneCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    void OnEnable()
    {
        EventManager.Instance.StartListening("SceneLoaded", OnSceneLoaded);
        EventManager.Instance.StartListening("SceneUnloaded", OnSceneUnloaded);
        //EventManager.Instance.StartListening("PlayerTeleported", TeleportPlayer);
        //EventManager.Instance.StartListening("TravelArrival", OnPlayerArrival);
        EventManager.Instance.StartListening("HandlePlayerDeath", KillPlayer);
        EventManager.Instance.StartListening("PlayerRevived", OnPlayerRevive);
        EventManager.Instance.StartListening("TriggerDodge", Dodge);
        //EventManager.Instance.StartListening("CutsceneFinished", OnCutsceneEnd);
    }

    void OnDestroy()
    {
        ResetPlayer();
        EventManager.Instance.StopListening("SceneLoaded", OnSceneLoaded);
        EventManager.Instance.StopListening("SceneUnloaded", OnSceneUnloaded);
        //EventManager.Instance.StopListening("PlayerTeleported", TeleportPlayer);
       // EventManager.Instance.StopListening("TravelArrival", OnPlayerArrival);
        EventManager.Instance.StopListening("HandlePlayerDeath", KillPlayer);
        EventManager.Instance.StopListening("TriggerDodge", Dodge);
        EventManager.Instance.StopListening("PlayerRevived", OnPlayerRevive);
        //EventManager.Instance.StopListening("CutsceneFinished", OnCutsceneEnd);
    }

    public void OnSceneLoaded(string scene)
    {
        if (scene == "GameScene")
        {
            sceneCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            defaultSpawnPoint = GameObject.FindGameObjectWithTag("PlayerSpawnPoint")?.transform;
            transform.position = defaultSpawnPoint.position;
            PlayerIntroSequence();
        }
    }

    public void OnSceneUnloaded(string scene)
    {
        if (scene == "GameScene")
        {
            Debug.Log("reset player called");
            ResetPlayer();
        }
    }

    void PlayerIntroSequence()
    {
        isDisabled = true;
    }

    void OnCutsceneEnd(string cutscene)
    {
        Debug.Log($"{cutscene} ended");
        if (cutscene == "IntroCutscene")
        {
            isDisabled = false;
        }
    }

    void Update()
    {
        if (playerIsDead || isInTransit) return;

        if (GameStateManager.Instance.CurrentState == GameState.Playing)
        {
            mousePosition = sceneCamera.ScreenToWorldPoint(Input.mousePosition);
            aimDirection = (mousePosition - rb.position).normalized;

            if (weaponManager.equippedWeapon != null)
            {
                weaponManager.equippedWeapon.GunAiming(aimDirection);
            }

            ProcessInputs();
            Animate();

            UpdateCooldownTimer();
        }
    }

    private void FixedUpdate()
    {
        if (playerIsDead || isInTransit || isDisabled) return;

        if (GameStateManager.Instance.CurrentState == GameState.Playing)
        {
            Move();
        }
    }

    void ProcessInputs()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector2(moveX, moveY).normalized;
    }

    void Move()
    {
        if (!isDodging && (knockbackManager == null || !knockbackManager.IsInKnockback()))
        {
            //Debug.Log("movin");
            rb.velocity = new Vector2(moveDirection.x * playerStats.currentMoveSpeed, moveDirection.y * playerStats.currentMoveSpeed);

            if (moveDirection.magnitude > 0)
            {
                footstepTimer -= Time.deltaTime;
                if (footstepTimer <= 0)
                {
                    PlayFootstepSound();
                    footstepTimer = footstepInterval;
                }
            }
            else
            {
                footstepTimer = 0f; // Reset timer if not moving
            }
        }
    }

    private void PlayFootstepSound()
    {
        AudioManager.TriggerSound("Player_Footsteps", transform.position, 0.4f);
    }

    void Animate()
    {
        if (isDisabled)
        {
            animator.SetBool("IsWalking", false);
            return;
        }

        bool isWalking = moveDirection.magnitude > 0;
        animator.SetBool("IsWalking", isWalking);

        Vector2 directionToAnimate;

        if (weaponManager.equippedWeapon != null && weaponManager.IsAutoAiming())//for auto aim function
        {
            directionToAnimate = weaponManager.GetLockedAimDirection();
        }
        else
        {
            directionToAnimate = aimDirection;
        }

        // Calculate the direction the player is aim
        float angle = Mathf.Atan2(directionToAnimate.y, directionToAnimate.x) * Mathf.Rad2Deg;
        directionIndex = GetDirectionIndex(angle);  // Convert angle to direction index

        animator.SetFloat("Direction", directionIndex);

        // Flip player if aiming left
        playerSprite.flipX = (directionToAnimate.x < 0);
    }

    public int GetDirectionIndex(float angle)
    {
        if (angle >= -22.5f && angle < 22.5f) return 4;  // Right
        if (angle >= 22.5f && angle < 67.5f) return 5;   // Up-Right
        if (angle >= 67.5f && angle < 112.5f) return 6;  // Up
        if (angle >= 112.5f && angle < 157.5f) return 7; // Up-Left
        if (angle >= -67.5f && angle < -22.5f) return 3; // Down-Right
        if (angle >= -112.5f && angle < -67.5f) return 2; // Down
        if (angle >= -157.5f && angle < -112.5f) return 1; // Down-Left
        return 0; // Left
    }

    void Dodge()
    {
        if (dodgeCooldownTimer <= 0 && !playerIsDead)
        {
            AudioManager.TriggerSound("Player_Dodge", transform.position);
            StartCoroutine(Dodging());
        }
        else
        {
            return;
        }
    }

    public Vector2 GetAimDirection()
    {
        if (weaponManager.equippedWeapon != null && weaponManager.IsAutoAiming())
        {
            // locked direction from auto-aim
            return weaponManager.GetLockedAimDirection();
        }
        else
        {
            return aimDirection;
        }
    }

    void UpdateCooldownTimer()
    {
        if (dodgeCooldownTimer > 0)
        {
            dodgeCooldownTimer -= Time.deltaTime;

            if (dodgeCooldownTimer < 0)
            {
                dodgeCooldownTimer = 0;
            }
        }
    }

    IEnumerator Dodging()
    {
        isDodging = true;
        playerCollider.enabled = false;

        PlayerHealthManager.Instance.StartInvincibility(0.2f);

        Vector2 dodgeDirection = moveDirection.normalized;
        rb.velocity = dodgeDirection * dodgeSpeed;
        yield return new WaitForSeconds(0.3f);

        playerCollider.enabled = true;
        isDodging = false;
        rb.velocity = Vector2.zero;
        dodgeCooldownTimer = dodgeCooldown;
        EventManager.Instance.TriggerEvent("DodgeUsed", dodgeCooldownTimer, dodgeCooldown);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PickupItem") && !PlayerHealthManager.Instance.playerIsDead)
        {
            IPullable pullable = other.GetComponent<IPullable>();
            if (pullable != null)
            {
                pullable.StartPull(); // magnetic pull effect
            }
        }
    }

    /*private void TeleportPlayer()
    {
        if (isInTransit) return;
        isInTransit = true;
        //gunSprite.enabled = false;
        playerCollider.enabled = false;
        playerStats.currentMoveSpeed = 0;
        rb.drag = 4f;
        //animator.SetTrigger("TransitDeparture");
    }

    private void OnPlayerArrival(string destination)
    {
        if (!isInTransit) return;
        isInTransit = false;
        //gunSprite.enabled = true;
        playerCollider.enabled = true;
        playerStats.currentMoveSpeed = playerStats.GetStatValue(StatType.MovementSpeed, PlayerStatManager.Instance.baseMoveSpeed);
        rb.drag = 0f;
        //animator.SetTrigger("TransitArrival");
    }*/

    private void KillPlayer()
    {
        if (playerIsDead) return;
        //gunSprite.enabled = false;
        playerStats.currentMoveSpeed = 0;
        animator.SetTrigger("Death");
        rb.drag = 5f;
        playerIsDead = true;
    }

    private void OnPlayerRevive()
    {
        if (!playerIsDead) return;
        Debug.LogWarning("Player controller revived");
        //gunSprite.enabled = true;
        playerStats.currentMoveSpeed = playerStats.GetStatValue(StatType.MovementSpeed, PlayerStatManager.Instance.baseMoveSpeed);
        animator.SetTrigger("Revive");
        rb.drag = 0f;
        playerIsDead = false;
    }

    public void DisableControls()
    {
        rb.velocity = Vector2.zero;
        rb.drag = 5f;

        //animator.SetFloat("Speed", 0f);
        animator.Play("Idle", 0);
        isDisabled = true;
    }

    public void EnableControls()
    {
        isDisabled = false;
        rb.drag = 0f;

        animator.Play("Idle", 0);
    }

    public void ResetPlayer()
    {
        Debug.LogWarning("Player reset");
        animator.SetTrigger("Revive");
        //gunSprite.enabled = true;
        playerIsDead = false;
        isDisabled = false;
        rb.drag = 0f;
        playerStats.ResetAllStats();
        PlayerAbilityManager.Instance.ResetAbilities();
        UpgradeDatabase.Instance.ResetUpgrades();
        ItemDatabase.Instance.ResetItems();
        PlayerCombat.Instance.ResetPlayerCombatEffects();
        CurrencyManager.Instance.ResetCurrency();
        XPManager.Instance.ResetXP();
        InventoryManager.Instance.ResetInventory();
        AudioManager.Instance.ResetAudio();
        GameManager.Instance.ResetDependencies();
    }
}