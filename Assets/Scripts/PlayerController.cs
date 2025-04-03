using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    
    public Rigidbody2D rb;
    public Animator animator;
    public Transform castLocation;
    public SpriteRenderer playerSprite;
    public KnockbackManager knockbackManager;
    public AttributeManager playerAttributes;
    public DodgeManager dodgeManager;
    public WeaponManager weaponManager;
    public BoxCollider2D playerCollider;
    public LayerMask defaultLayer;

    public Vector2 aimDirection { get; private set; }  // Expose aimDirection
    public float footstepInterval = 0.15f;
    
    [HideInInspector] public Vector2 moveDirection;
    private Vector2 mousePosition;
    private Vector3 shadowOffset;

    private Camera sceneCamera;
    private float footstepTimer = 0f;
    private float directionIndex;
    private Transform defaultSpawnPoint; //New game spawn
    
    [HideInInspector] public bool isDead = false;
    [HideInInspector] public bool isDisabled = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        defaultLayer = LayerMask.NameToLayer("Player");
    }

    void OnEnable()
    {
        EventManager.Instance.StartListening("SceneLoaded", OnSceneLoaded);
        EventManager.Instance.StartListening("SceneUnloaded", OnSceneUnloaded);
        EventManager.Instance.StartListening("KillPlayer", KillPlayer);
        EventManager.Instance.StartListening("PlayerRevived", RevivePlayer);
    }

    void OnDestroy()
    {
        ResetPlayer();
        EventManager.Instance.StopListening("SceneLoaded", OnSceneLoaded);
        EventManager.Instance.StopListening("SceneUnloaded", OnSceneUnloaded);
        EventManager.Instance.StopListening("KillPlayer", KillPlayer);
        EventManager.Instance.StopListening("PlayerRevived", RevivePlayer);
    }

    public void OnSceneLoaded(string scene)
    {
        if (scene == "GameScene")
        {
            sceneCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
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
    
    public bool IsPlayerDead() => isDead;
    public bool IsPlayerDisabled() => isDisabled;

    void Update()
    {
        if (isDead || isDisabled || dodgeManager.IsPlayerDodging() || GameStateManager.Instance.CurrentState != GameState.Playing) return;

        mousePosition = sceneCamera.ScreenToWorldPoint(Input.mousePosition);
        aimDirection = (mousePosition - rb.position).normalized;
            
        if (weaponManager.activeLeftWeapon != null)
        {
            weaponManager.activeLeftWeapon.weaponBase.WeaponAiming(aimDirection);
        }
            
        if (weaponManager.activeRightWeapon != null)
        {
            weaponManager.activeRightWeapon.weaponBase.WeaponAiming(aimDirection);
        }
        
        ProcessInputs();
        Animate();
    }

    private void FixedUpdate()
    {
        if (isDead || isDisabled || dodgeManager.IsPlayerDodging() || GameStateManager.Instance.CurrentState != GameState.Playing) return;
        Move();
    }

    void ProcessInputs()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector2(moveX, moveY).normalized;
    }

    void Move()
    {
        if (!knockbackManager.IsInKnockback())
        {
            rb.velocity = new Vector2(moveDirection.x * playerAttributes.currentMoveSpeed, moveDirection.y * playerAttributes.currentMoveSpeed);

            if (moveDirection.magnitude > 0)
            {
                footstepTimer -= Time.deltaTime;
                if (footstepTimer <= 0)
                {
                    AudioManager.TriggerSound("Player_Footsteps", transform.position, 0.4f);
                    footstepTimer = footstepInterval;
                }
            }
            else
            {
                footstepTimer = 0f;
            }
        }
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
        
        directionToAnimate = aimDirection;
        float angle = Mathf.Atan2(directionToAnimate.y, directionToAnimate.x) * Mathf.Rad2Deg;
        directionIndex = GetDirectionIndex(angle);  // Convert angle to direction index
        animator.SetFloat("Direction", directionIndex);
        playerSprite.flipX = (directionToAnimate.x < 0); // flip player if aiming left
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

    public Vector2 GetAimDirection()
    {
        return aimDirection;
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

    private void KillPlayer()
    {
        if (isDead) return;
        playerAttributes.currentMoveSpeed = 0;
        animator.SetTrigger("Death");
        rb.drag = 5f;
        isDead = true;
    }

    private void RevivePlayer()
    {
        if (!isDead) return;
        Debug.LogWarning("Player revived");
        playerAttributes.currentMoveSpeed = playerAttributes.GetStatValue(StatType.MovementSpeed, playerAttributes.baseMoveSpeed);
        animator.SetTrigger("Revive");
        rb.drag = 0f;
        isDead = false;
    }
    
    public void DisableControls()
    {
        rb.velocity = Vector2.zero;
        rb.drag = 5f;
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
        isDead = false;
        isDisabled = false;
        rb.drag = 0f;
        playerAttributes.ResetAllStats();
        PlayerAbilityManager.Instance.ResetAbilities();
        WeaponManager.Instance.ResetWeaponLoadouts();
        UpgradeDatabase.Instance.ResetUpgrades();
        ItemDatabase.Instance.ResetItems();
        PlayerCombat.Instance.ResetPlayerCombatEffects();
        AudioManager.Instance.ResetAudio();
        GameManager.Instance.ResetDependencies();
    }
}