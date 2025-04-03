using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*public class FireBreathAbility : PlayerAbilityBase
{
    [Header("Fire Breath Specials")]
    public GameObject fireBreathFX;
    private ParticleSystem fireBreathParticles;
    public float baseFireBreathAngle = 10f;
    public float fireBreathDuration = 0.25f;
    private float visualFireBreathAngle;

    private float currentDamage;
    private float currentCooldownRate;
    private float currentRange;
    private float currentKnockbackForce;
    private float currentCriticalHitDamage;
    private float currentFireBreathAngle;
    
    private PlayerController playerController;

    private bool IsFiring = false;
    private HashSet<Collider2D> hitTargets = new HashSet<Collider2D>();

    public override void Initialize()
    {
        Debug.Log("Initializing FireBreath Ability...");
        
        abilityData.playerStats = AttributeManager.Instance;
        abilityData.player = GameObject.FindWithTag("Player").transform;
        abilityData.abilityPrefab = Resources.Load<GameObject>("PlayerAbilities/FireBreathAbility");
        abilityData.enemyLayer = LayerMask.GetMask("Enemy", "LowWall");
        playerController = abilityData.player.GetComponent<PlayerController>();
        fireBreathFX = Resources.Load<GameObject>("FX/FireBreathFX");
        fireBreathParticles = fireBreathFX.GetComponent<ParticleSystem>();

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
        currentCriticalHitDamage = AttributeManager.Instance.CalculateCriticalHitDamage(currentDamage);
        currentFireBreathAngle = baseFireBreathAngle;
        visualFireBreathAngle = (currentFireBreathAngle/5);
        var shape = fireBreathParticles.shape;
        shape.angle = visualFireBreathAngle;

        EventManager.Instance.TriggerEvent("FXPoolUpdate", fireBreathFX.name, 3);
        EventManager.Instance.StartListening("StatsChanged", UpdateAbilityStats);
    }

    public override bool CanUseAbility()
    {
        return IsAbilityReady();
    }

    public override void UseAbility()
    {
        if (isOnCooldown) return;

        if (!IsFiring)
        {
            Debug.Log("Casting Fire");
            IsFiring = true;
            hitTargets.Clear();

            fireBreathFX.transform.SetParent(abilityData.player);
            fireBreathParticles = fireBreathFX.GetComponent<ParticleSystem>();
            var shape = fireBreathParticles.shape;
            shape.angle = visualFireBreathAngle;
            AudioManager.TriggerSound("Ability_FireBreath_Cast", transform.position);
            fireBreathFX = FXManager.Instance.PlayFX("FireBreathFX", playerController.castLocation.position);

            //fireBreathFX.transform.SetParent(abilityData.player);
            //fireBreathFX = FXManager.Instance.PlayFX("FireBreathFX", playerController.firePointTransform.position);
            // Initialize the visual fire breath with a set angle
            //FireBreath fireBreathScript = fireBreathFX.GetComponent<FireBreath>();
            //fireBreathScript.Initialize(visualFireBreathAngle);

            StartCoroutine(ActivateFireBreath());
        }
    }

    private IEnumerator ActivateFireBreath()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fireBreathDuration)
        {
            Vector2 abilityPosition = abilityData.player.position;
            Vector2 aimDirection = playerController.GetAimDirection();
            float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
            fireBreathFX.transform.rotation = Quaternion.Euler(0, 0, angle);

            // Visualize OverlapCircle
            DrawWireCircle(abilityPosition, currentRange, Color.red);
            float halfAngle = currentFireBreathAngle;

            // Calculate left and right boundary directions for cone
            Vector2 leftBoundary = Quaternion.Euler(0, 0, -halfAngle) * aimDirection;
            Vector2 rightBoundary = Quaternion.Euler(0, 0, halfAngle) * aimDirection;

            // Draw lines for cone boundaries
            Debug.DrawLine(abilityData.player.position, abilityPosition + (leftBoundary * currentRange), Color.blue, 0.5f);
            Debug.DrawLine(abilityData.player.position, abilityPosition + (rightBoundary * currentRange), Color.blue, 0.5f);

            DetectAndDamageEnemies(aimDirection);

            elapsedTime += Time.deltaTime;
            yield return null; // Wait until the next frame
        }

        hitTargets.Clear();
        AbilityReset();
    }

    private void DetectAndDamageEnemies(Vector2 aimDirection)
    {
        Vector2 abilityPosition = abilityData.player.position;
        float halfAngle = currentFireBreathAngle;

        bool willInflictBurn = AttributeManager.Instance.AttemptToApplyStatusEffect(StatType.BurnChance);
        bool isCriticalHit = abilityData.playerStats.IsCriticalHit();
        float finalDamage = isCriticalHit ? currentCriticalHitDamage : currentDamage;

        Collider2D[] collisions = Physics2D.OverlapCircleAll(abilityPosition, currentRange, abilityData.enemyLayer);

        foreach (Collider2D collision in collisions)
        {
            if (hitTargets.Contains(collision)) continue;

            Vector2 toTarget = (collision.transform.position - (Vector3)abilityPosition).normalized;
            float angleToTarget = Vector2.Angle(aimDirection, toTarget);

            if (angleToTarget <= halfAngle)
            {
                if (collision.CompareTag("Enemy") && collision.TryGetComponent(out EnemyHealthManager enemyHealth))
                {
                    enemyHealth.TakeDamage(finalDamage, toTarget, currentKnockbackForce, DamageSource.Player, isCriticalHit);
                    if (willInflictBurn)
                    {
                        StatusEffectManager.Instance.ApplyBurnEffect(collision.gameObject, 5f, AttributeManager.Instance.currentDamage / 2);
                    }
                    hitTargets.Add(collision);
                }

                if (collision.CompareTag("BreakableObstacle") && collision.TryGetComponent(out ObjectHealthManager obstacleHealth))
                {
                    obstacleHealth.TakeDamage(finalDamage, isCriticalHit);
                    hitTargets.Add(collision);
                }
            }
        }
    }

    private void UpdateAbilityStats()
    {
        currentDamage = abilityData.playerStats.GetStatValue(StatType.Damage, abilityData.baseDamage);
        currentRange = abilityData.playerStats.GetStatValue(StatType.Range, abilityData.baseRange);
        currentCooldownRate = abilityData.playerStats.GetStatValue(StatType.CooldownRate, abilityData.baseCooldownRate);
        currentCriticalHitDamage = abilityData.playerStats.GetStatValue(StatType.CriticalHitDamage, AttributeManager.Instance.CalculateCriticalHitDamage(currentDamage));
        currentFireBreathAngle = abilityData.playerStats.GetStatValue(StatType.Scorch, baseFireBreathAngle);
        visualFireBreathAngle = (currentFireBreathAngle / 5);
        var shape = fireBreathParticles.shape;
        shape.angle = visualFireBreathAngle;
    }

    public override void AddAbilitySpecificUpgrades()
    {
        Sprite scorchIcon = Resources.Load<Sprite>("Icons/ScorchUpgrade");

        ScorchUpgrade scorchUpgrade = new ScorchUpgrade(scorchIcon);
        UpgradeDatabase.Instance.AddNewUpgrade(scorchUpgrade);

        Debug.Log("Firebreath-specific upgrades added to the upgrade pool.");
    }

    public override void AbilityReset()
    {
        IsFiring = false;
        StartCooldown(currentCooldownRate);
    }

    private void DrawWireCircle(Vector2 center, float radius, Color color, int segments = 30)
    {
        float angleStep = 360f / segments;
        Vector2 previousPoint = center + new Vector2(Mathf.Cos(0), Mathf.Sin(0)) * radius;

        for (int i = 1; i <= segments; i++)
        {
            float angle = angleStep * i * Mathf.Deg2Rad;
            Vector2 nextPoint = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            Debug.DrawLine(previousPoint, nextPoint, color, 0.1f);
            previousPoint = nextPoint;
        }
    }
}*/