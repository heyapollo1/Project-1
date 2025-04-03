using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*public class BombAbility : PlayerAbilityBase
{
    [Header("Bomb Specials")]
    //public GameObject AoEIndicatorPrefab;
    public float baseAreaSize = 5f;
    public bool hasBigBoom = false;

    private float currentDamage;
    private float currentCooldownRate;
    private float currentRange;
    private float currentKnockbackForce;
    private float currentCriticalHitDamage;
    private float currentAreaSize;

    private ObjectPoolManager objectPool;
    //private GameObject bombProjectile;
    private GameObject AoEIndicatorPrefab;
    private GameObject bombExplosionPrefab;
    private GameObject activeAoEIndicator;

    private bool IsFiring = false;
    private Vector2 explosionPosition;

    public override void Initialize()
    {
        Debug.Log("Initializing Bomb Ability...");

        abilityData.playerStats = AttributeManager.Instance;
        abilityData.player = GameObject.FindWithTag("Player").transform;
        abilityData.abilityPrefab = Resources.Load<GameObject>("PlayerAbilities/BombAbility");
        abilityData.enemyDetector = abilityData.player.GetComponent<EnemyDetector>();
        abilityData.enemyLayer = LayerMask.GetMask("Enemy");

        bombExplosionPrefab = Resources.Load<GameObject>("Prefabs/BombExplosion");
        AoEIndicatorPrefab = Resources.Load<GameObject>("Prefabs/BombAoEIndicator");
        //bombProjectile = Resources.Load<GameObject>("Prefabs/BombProjectile");
        objectPool = FindObjectOfType<ObjectPoolManager>();

        EventManager.Instance.StartListening("StatsChanged", UpdateAbilityStats);
        SetBaseStats();
        UpdateAbilityStats();
    }

    private void OnDestroy()
    {
        EventManager.Instance.StopListening("StatsChanged", UpdateAbilityStats);
    }

    private void SetBaseStats()
    {
        currentDamage = abilityData.baseDamage;
        currentCooldownRate = abilityData.baseCooldownRate;
        currentRange = abilityData.baseRange;
        currentKnockbackForce = abilityData.baseKnockbackForce;
        //currentCriticalHitDamage = AttributeManager.Instance.CalculateCriticalHitDamage(currentDamage);
        currentAreaSize = baseAreaSize;
    }

    public override bool CanUseAbility()
    {
        GameObject nearestEnemy = abilityData.enemyDetector.FindNearestEnemyInRange(currentRange);
        return nearestEnemy != null && IsAbilityReady() && !IsFiring;  // Ability can be used if there's a valid target and it's not on cooldown
    }

    public override void UseAbility()
    {
        if (isOnCooldown) return;

        GameObject nearestEnemy = abilityData.enemyDetector.FindNearestEnemyInRange(currentRange);

        if (nearestEnemy != null && !IsFiring)
        {
            IsFiring = true;
            Vector2 targetPosition = nearestEnemy.transform.position;
            LaunchBomb(targetPosition);
            AbilityReset();
        }
        else
        {
            return;
        }
    }

    private void LaunchBomb(Vector2 targetPosition)
    {
        GameObject bomb = objectPool.GetFromPool("Bomb", abilityData.player.position, Quaternion.identity);
        activeAoEIndicator = Instantiate(AoEIndicatorPrefab, targetPosition, Quaternion.identity);

        if (activeAoEIndicator != null)
        {
            UpdateAoEIndicatorScale();  // Apply the correct scaling
        }
        else
        {
            Debug.LogError("AoE Indicator instantiation failed!");
        }

        StartCoroutine(ArcMove(bomb, targetPosition, 1.0f));  // 1 second to reach target
    }

    private IEnumerator ArcMove(GameObject bomb, Vector2 targetPosition, float duration)
    {
        Vector2 startPosition = bomb.transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            Vector2 currentPosition = Vector2.Lerp(startPosition, targetPosition, t);
            float arcHeight = Mathf.Sin(Mathf.PI * t) * 2f; // Simulate arc height
            bomb.transform.position = new Vector2(currentPosition.x, currentPosition.y + arcHeight);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        bomb.transform.position = targetPosition; // Ensure it lands exactly on target
        Explode(bomb, targetPosition);
    }

    private void Explode(GameObject bomb, Vector2 targetPosition)
    {
        // Instantiate and play the explosion effect
        GameObject explosionEffect = Instantiate(bombExplosionPrefab, targetPosition, Quaternion.identity);
        StartCoroutine(PlayAndDestroyEffect(explosionEffect));

        // Set the explosion position for the gizmo
        explosionPosition = targetPosition;

        // Get the AoE size from the central GetAoESize method
        Vector2 capsuleSize = GetAoESize();

        // Use OverlapCapsuleAll to detect enemies within the capsule-shaped area
        CapsuleDirection2D direction = CapsuleDirection2D.Horizontal;  // Adjust for horizontal capsule
        Collider2D[] enemiesInCapsule = Physics2D.OverlapCapsuleAll(targetPosition, capsuleSize, direction, 0f, LayerMask.GetMask("Enemy"));

        // Handle detected enemies
        HandleEnemiesDetected(new List<Collider2D>(enemiesInCapsule), targetPosition);

        bomb.SetActive(false);  // Return bomb to the pool

        if (activeAoEIndicator != null)
        {
            Destroy(activeAoEIndicator);  // Clean up the AoE indicator
        }
    }

    private IEnumerator PlayAndDestroyEffect(GameObject explosionEffect)
    {
        ParticleSystem ps = explosionEffect.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            yield return new WaitForSeconds(ps.main.duration); // Wait for effect to finish
        }
        Destroy(explosionEffect); // Destroy effect after playing
    }

    private void HandleEnemiesDetected(List<Collider2D> detectedEnemies, Vector3 explosionOrigin)
    {
        foreach (Collider2D enemy in detectedEnemies)
        {
            if (enemy.TryGetComponent(out EnemyHealthManager enemyHealth))
            {
                float finalDamage = abilityData.playerStats.IsCriticalHit(currentCriticalHitDamage)
                    ? currentDamage * currentCriticalHitDamage
                    : currentDamage;

                Vector2 knockbackDirection = (enemy.transform.position - explosionOrigin).normalized;

                enemyHealth.TakeDamage(finalDamage, knockbackDirection, currentKnockbackForce);
            }
            else
            {
                Debug.LogWarning($"No EnemyHealth component found on {enemy.name}");
            }
        }
    }

    private Vector2 GetAoESize()
    {
        float dampingFactor = 5f;
        float width = (currentAreaSize / dampingFactor) * 2.0f;   // Full width (diameter)
        float height = (currentAreaSize / dampingFactor) * 1.25f;  // Adjusted height for visual scale
        return new Vector2(width, height);
    }

    private void UpdateAoEIndicatorScale()
    {
        if (activeAoEIndicator != null)
        {
            // Get the AoE size for the collider/gizmo
            Vector2 size = GetAoESize();

            // Get the sprite renderer to determine its native size
            SpriteRenderer spriteRenderer = activeAoEIndicator.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                // Calculate the native size of the sprite in world units
                float spriteWidth = spriteRenderer.sprite.bounds.size.x;
                float spriteHeight = spriteRenderer.sprite.bounds.size.y;

                // Calculate the scale factor needed to match the AoE size with the sprite size
                float scaleX = size.x / spriteWidth;
                float scaleY = size.y / spriteHeight;

                // Apply the calculated scale to the sprite's transform
                activeAoEIndicator.transform.localScale = new Vector3(scaleX, scaleY, 1f);
            }
            else
            {
                Debug.LogWarning("No SpriteRenderer found on the AoE indicator.");
            }
        }
    }

    private void UpdateAbilityStats()
    {
        currentDamage = abilityData.playerStats.GetStatValue(StatType.Damage, abilityData.baseDamage);
        currentRange = abilityData.playerStats.GetStatValue(StatType.Range, abilityData.baseRange);
        currentCooldownRate = abilityData.playerStats.GetStatValue(StatType.CooldownRate, abilityData.baseCooldownRate);
        currentCriticalHitDamage = AttributeManager.Instance.CalculateCriticalHitDamage(currentDamage);
        currentAreaSize = abilityData.playerStats.GetStatValue(StatType.AreaSize, baseAreaSize);
        if (activeAoEIndicator != null)
        {
            UpdateAoEIndicatorScale();
        }
        Debug.Log("Current CDR: " + currentCooldownRate + ". My base CDR:" + abilityData.baseCooldownRate);
        Debug.Log("Current Area: " + currentAreaSize + ". My base CDR:" + baseAreaSize);
    }

    public override void AbilityReset()
    {
        IsFiring = false;
        StartCooldown(currentCooldownRate);
    }

    public override void AddAbilitySpecificUpgrades()
    {
        Sprite areaSizeIcon = Resources.Load<Sprite>("Icons/areaSizeIcon");

        AreaSizeUpgrade areaSizeUpgrade = new AreaSizeUpgrade(areaSizeIcon);
        UpgradeDatabase.Instance.AddNewUpgrade(areaSizeUpgrade);

        Debug.Log("Adding Bomb-specific upgrades...");
    }

    public void UnlockBigBoom()
    {
        hasBigBoom = true;
        Debug.Log("Big Boom upgrade unlocked!");
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying || explosionPosition == Vector2.zero) return;

        Gizmos.color = Color.green;

        // Get the exact same size from the GetAoESize method
        Vector2 capsuleSize = GetAoESize();

        Vector3 gizmoCenter = new Vector3(explosionPosition.x, explosionPosition.y, 0);

        // Calculate the radius for the round parts of the capsule (half of the height, horizontal direction)
        float capsuleRadius = capsuleSize.y / 2.0f;

        // Calculate the straight segment width
        float straightWidth = capsuleSize.x - (capsuleRadius * 2.0f);  // Width of the straight section between the two circles

        // Determine the center points for the left and right circles (capsule ends)
        Vector3 leftCircleCenter = gizmoCenter + Vector3.left * (straightWidth / 2.0f);
        Vector3 rightCircleCenter = gizmoCenter + Vector3.right * (straightWidth / 2.0f);

        // Draw the round ends of the capsule
        Gizmos.DrawWireSphere(leftCircleCenter, capsuleRadius);
        Gizmos.DrawWireSphere(rightCircleCenter, capsuleRadius);

        // Draw the straight sides connecting the two circle ends
        Gizmos.DrawLine(new Vector3(leftCircleCenter.x + capsuleRadius, leftCircleCenter.y, 0),
                        new Vector3(rightCircleCenter.x - capsuleRadius, rightCircleCenter.y, 0));

        Gizmos.DrawLine(new Vector3(leftCircleCenter.x + capsuleRadius, leftCircleCenter.y - capsuleRadius, 0),
                        new Vector3(rightCircleCenter.x - capsuleRadius, rightCircleCenter.y - capsuleRadius, 0));

        Gizmos.DrawLine(new Vector3(leftCircleCenter.x + capsuleRadius, leftCircleCenter.y + capsuleRadius, 0),
                        new Vector3(rightCircleCenter.x - capsuleRadius, rightCircleCenter.y + capsuleRadius, 0));
    }
}*/