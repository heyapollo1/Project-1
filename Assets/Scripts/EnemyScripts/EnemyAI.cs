using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DeathType
{
    Standard,
    Instant,
    Boss,
    Delete
}

public class EnemyAI : MonoBehaviour
{
    public EnemyAbilityBase currentAbility { get; private set; }
    public DeathType deathType;

    public IEnemyState currentState;
    public IEnemyState idleState; 
    public IEnemyState movingState;  
    public IEnemyState roamingState;
    public IEnemyState deathState;
    public IEnemyState stunnedState;
    public IEnemyState spawnState;
    public IEnemyState evadingState;

    [Header("Base Stats")]
    public float baseHealth;
    public float baseDamage;
    public float baseMoveSpeed;
    [HideInInspector] public float currentHealth;
    [HideInInspector] public float currentDamage;
    [HideInInspector] public float currentMoveSpeed;

    [Header("References")]
    public EnemyAbilityManager abilityManager;
    public EnemyHealthManager healthManager;
    public SceneGrid grid;
    public Animator animator;  
    public Transform player;
    public Rigidbody2D rb;
    public Pathfinding pathfinding;
    public SpriteRenderer enemySprite;
    public EnemyMovement enemyMovement;
    public Canvas damageCanvas;
    public GameObject damageNumberPrefab;
    public CapsuleCollider2D physicalCollider;
    public CircleCollider2D triggerCollider;
    public GameObject shadowPrefab;
    public GameObject shadowInstance;
    public FlowFieldManager flowFieldManager;
    public FlowField flowField;
    public Vector2 shadowOffset = new(0, -0.8f);
    public Vector2 shadowScale = new(6, 3);
    public LayerMask obstacleLayerMask;
    public Vector2 directionToPlayer;
    public Vector2 startPosition;

    [Header("Behaviour Settings")]
    [SerializeField] public float alertDetectionRange = 10f;
    [SerializeField] public float minRangeToPlayer = 1f;
    [SerializeField] public float effectiveRangeToPlayer = 0f;
    [SerializeField] public float orbitRange = 5f;
    [SerializeField] public bool isInitialized = false;
    [SerializeField] public bool isAlerted = false;
    [SerializeField] public bool overrideFacing = false;
    [SerializeField] public bool isPerformingAbility = false;
    [SerializeField] public bool isDead = false;
    [SerializeField] public bool isDisabled = false;
    [SerializeField] public bool isStunned = false;

    private void Awake()
    {
        idleState = new IdleState();
        movingState = new MovingState();
        roamingState = new RoamingState();
        deathState = new DeathState();
        stunnedState = new StunnedState();
        spawnState = new SpawnState();
        evadingState = new EvadingState(); 
        //EventManager.Instance.StartListening("GameStarted", SceneInitialize);
    }

    private void OnDisable()
    {
        //EventManager.Instance.StopListening("GameStarted", SceneInitialize);
    }

    private void SceneInitialize()
    {
        if (!isInitialized)
        {
            Debug.LogWarning($"Built in initializing system triggered");
            ActivateFromPool(transform.position);
        }
    }

    public void Initialize()
    {
        Debug.LogWarning($"Initializing enemy {gameObject.name} after systems ready...");
        isInitialized = true;

        InitializeComponents();
        InitializeShadow();
        InitializeStatsAndAbilities();
        InitializeMovement();
        InitializeDrops();
    }

    protected virtual void InitializeComponents()
    {
        deathType = DeathType.Standard;
        player = GameObject.FindWithTag("Player").transform;
        damageNumberPrefab = Resources.Load<GameObject>("Prefabs/DamageNumbers");
        damageCanvas = GameObject.FindWithTag("StatCanvas").GetComponent<Canvas>();
        abilityManager = GetComponent<EnemyAbilityManager>();
        healthManager = GetComponent<EnemyHealthManager>();
        enemyMovement = GetComponent<EnemyMovement>();
        rb = GetComponent<Rigidbody2D>();
        physicalCollider = GetComponent<CapsuleCollider2D>();
        triggerCollider = GetComponent<CircleCollider2D>();
        animator = GetComponent<Animator>();  
        pathfinding = GetComponent<Pathfinding>();
        enemySprite = GetComponent<SpriteRenderer>();
        enemySprite.material = new Material(enemySprite.material);
        obstacleLayerMask = LayerMask.GetMask("Wall", "Low Wall");
        grid = GridManager.Instance.GetActiveGrid();
        flowFieldManager = FindObjectOfType<FlowFieldManager>();
        flowField = flowFieldManager.GetActiveFlowField();
        shadowPrefab = Resources.Load<GameObject>("Prefabs/Shadow");
        shadowInstance = Instantiate(shadowPrefab, transform);
        startPosition = transform.position;
    }

    protected virtual void InitializeShadow()
    {
        Debug.LogWarning($"Initializing enemy shadow");
        if (shadowPrefab != null && shadowInstance != null)
        {
            shadowInstance.SetActive(true);
            shadowInstance.transform.localPosition = shadowOffset;
            shadowInstance.transform.localScale = shadowScale;
        }
    }

    protected virtual void InitializeStatsAndAbilities()
    {
        currentHealth = baseHealth;
        currentDamage = baseDamage;
        currentMoveSpeed = baseMoveSpeed;
    }

    protected virtual void InitializeDrops()
    {
        DeathEffectManager.Instance.RegisterOnDeathEffect(gameObject, new DropHealingOrb(1f));
        DeathEffectManager.Instance.RegisterOnDeathEffect(gameObject, new DropGold(1f));
    }

    protected void InitializeMovement()
    {
        enemyMovement.Initialize(pathfinding, rb, player, obstacleLayerMask, flowField, animator);
    }

    public void DisableEnemy()
    {
        isDisabled = true;
        InterruptCurrentAbility();
        if (abilityManager != null) abilityManager.enabled = false;
        //EnemyManager.Instance.UnregisterEnemy(gameObject);
        TransitionToState(idleState);
    }

    public void RemoveComponentsOnDeath()
    {
        isDead = true;
        if (physicalCollider != null) physicalCollider.enabled = false;
        if (triggerCollider != null) triggerCollider.enabled = false;
        if (abilityManager != null) abilityManager.enabled = false;
        if (pathfinding != null) pathfinding.enabled = false;
        if (shadowInstance != null) shadowInstance.SetActive(false);
        healthManager.RemoveAllFX();
        healthManager.RemoveAllStatusEffects();
        DeathEffectManager.Instance.ClearDeathEffects(gameObject);
        EnemyManager.Instance.UnregisterEnemy(gameObject);
    }

    public void ActivateFromPool(Vector3 spawnPosition)
    {
        Debug.Log($"Activating enemy {gameObject.name} at position {spawnPosition}");
        if (!isInitialized)
        {
            Debug.LogWarning($"Pool Spawn initialization");
            Initialize();
        }
        else
        {
            isDead = false;
            isAlerted = false;
            isStunned = false;
            isDisabled = false;
            overrideFacing = false;
            isPerformingAbility = false;

            if (physicalCollider != null) physicalCollider.enabled = true;
            if (triggerCollider != null) triggerCollider.enabled = true;
            if (pathfinding != null) pathfinding.enabled = true;

            InitializeShadow();
            InitializeMovement();
            InitializeDrops();
        }
        
        healthManager.ResetDeathState();
        EnemyManager.Instance.RegisterEnemy(gameObject);
        transform.position = spawnPosition;
        gameObject.SetActive(true);

        TransitionToState(spawnState);
    }

    protected virtual void Update()
    {
        if ((isDead) || (!isInitialized)) return;

        currentState?.UpdateState(this);  

        directionToPlayer = (player.position - rb.transform.position).normalized;

        if (isAlerted && !overrideFacing && !isStunned)
        {
            FaceDirection(directionToPlayer);
        }

        /*if (IsStuckInUnwalkableNode())
        {
            ApplyStuckNudge();
        }*/

    }

    public void TransitionToState(IEnemyState newState)
    {
        if (isDead) return;

        currentState?.ExitState(this);
        currentState = newState;
        currentState?.EnterState(this);

    }

    public void ApplyDifficultyModifiers(float globalHealthMultiplier, float globalDamageMultiplier)
    {
        currentHealth = baseHealth * globalHealthMultiplier;
        currentDamage = baseDamage * globalDamageMultiplier;
    }

    public bool IsPlayerInSightRange()
    {
        return Vector2.Distance(transform.position, player.position) <= alertDetectionRange;
    }

    public bool IsPlayerAtEffectiveDistance()
    {
        return Vector2.Distance(transform.position, player.position) <= effectiveRangeToPlayer;
    }

    public bool IsPlayerTooClose()
    {
        return Vector2.Distance(transform.position, player.position) <= minRangeToPlayer;
    }

    public bool TryUseSpecialAbility()
    {
        EnemyAbilityBase availableAbility = abilityManager.GetAvailableAbility(this);

        if (availableAbility != null)
        {
            TransitionToState(new AbilityState(availableAbility));

            return true;
        }
        return false;
    }

    public void SetCurrentAbility(EnemyAbilityBase ability)
    {
        currentAbility = ability;
    }

    public void InterruptCurrentAbility()
    {
        Debug.Log("Attempt to interrupt: " + currentAbility);
        if (currentAbility != null)
        {
            Debug.Log("Interrupting ability: " + currentAbility);
            currentAbility.InterruptAbility(this);
            currentAbility = null;
        }
    }
    
    public void FaceDirection(Vector2 direction)
    {
        direction.Normalize();
        // Get the angle relative to the right (0 degrees)
        float angle = Vector2.SignedAngle(Vector2.right, direction);

        // Determine the direction index (0 = Left, 1 = Right, 2 = Up-Right, 3 = Up, 4 = Down, 5 = Up-Left)
        int directionIndex = GetDirectionIndex(angle);

        animator.SetFloat("Direction", directionIndex);

        bool shouldFlipX = (directionIndex == 0 || directionIndex == 5);
        if (enemySprite.flipX != shouldFlipX)
        {
            enemySprite.flipX = shouldFlipX;
        }
    }

    int GetDirectionIndex(float angle)
    {
        if (angle >= -67.5f && angle < 22.5f) return 1; // Down-Right
        if (angle >= 22.5f && angle < 67.5f) return 2; // Up-Right
        if (angle >= 67.5f && angle < 112.5f) return 3; // Up
        if (angle >= 112.5f && angle < 157.5f) return 5; // Up-Left
        if (angle >= -112.5f && angle < -67.5f) return 4; // Down
        return 0; // Down-Left
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, minRangeToPlayer);
        Gizmos.DrawWireSphere(transform.position, 1.15f);
    }
}