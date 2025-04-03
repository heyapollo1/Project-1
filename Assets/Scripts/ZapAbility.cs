using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*public class ZapAbility : PlayerAbilityBase
{
    
    [Header("Zap Specials")]
    public float baseChainAmount = 0;
    public bool hasChainLightning = false;
    public bool canChainToSameEnemy = false;

    private float currentDamage;
    private float currentCooldownRate;
    private float currentRange;
    private float currentKnockbackForce;
    private float currentCriticalHitDamage;
    private float currentChainAmount;

    private ChainLightning chainLightning;
    private GameObject lightningProjectile;
    private LineRenderer lightning;
    private ObjectPoolManager objectPool;

    private bool IsFiring = false;
    private float lifetime = 0.1f;

    public override void Initialize()
    {
        Debug.Log("Initializing Zap Ability...");

        abilityData.playerStats = AttributeManager.Instance;
        abilityData.player = GameObject.FindWithTag("Player").transform;
        abilityData.abilityPrefab = Resources.Load<GameObject>("PlayerAbilities/ZapAbility");
        abilityData.enemyDetector = abilityData.player.GetComponent<EnemyDetector>();

        chainLightning = FindObjectOfType<ChainLightning>();
        objectPool = FindObjectOfType<ObjectPoolManager>();
        lightningProjectile = Resources.Load<GameObject>("Prefabs/LightningProjectile");

        EventManager.Instance.TriggerEvent("ObjectPoolUpdate", "LightningProjectile", 10);
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
        currentCriticalHitDamage = AttributeManager.Instance.CalculateCriticalHitDamage(currentDamage);
        currentChainAmount = baseChainAmount;
    }

    public override bool CanUseAbility()
    {
        return IsAbilityReady() && !IsFiring;
    }

    public override void UseAbility()
    {
        GameObject nearestEnemy = abilityData.enemyDetector.FindNearestEnemyInRange(currentRange);

        if (nearestEnemy == null && !IsFiring) return;

        IsFiring = true;
        AudioManager.TriggerSound("Ability_Zap_Cast", transform.position);
        DealDamage(nearestEnemy, abilityData.player.transform.position);
        DrawLightning(abilityData.player.position, nearestEnemy.transform.position);
        if (hasChainLightning)
        {
            chainLightning.StartChain(nearestEnemy.transform, DealDamage, DrawLightning, lightningProjectile, currentChainAmount);
        }

        AbilityReset();
    }

    private void DealDamage(GameObject enemy, Vector3 originPosition)
    {
        if (enemy == null)
        {
            Debug.LogWarning("Enemy is null. Skipping damage.");
            return;
        }

        if (enemy.TryGetComponent(out EnemyHealthManager enemyHealth))
        {
            bool isCritical = abilityData.playerStats.IsCriticalHit();//to pass critical bool for health manager and determine damage colour/size
            float finalDamage = isCritical ? currentCriticalHitDamage : currentDamage;
            Vector2 knockbackDirection = (enemy.transform.position - originPosition).normalized;

            enemyHealth.TakeDamage(finalDamage, knockbackDirection, currentKnockbackForce, DamageSource.Player, isCritical);
        }
        else
        {
            Debug.LogWarning($"No EnemyHealth component found on {enemy.name}");
        }
    }

    public void DrawLightning(Vector3 start, Vector3 end)
    {
        lightningProjectile = objectPool.GetFromPool("LightningProjectile", abilityData.player.position, Quaternion.identity);
        lightning = lightningProjectile.GetComponent<LineRenderer>();

        int segments = 8;
        lightning.positionCount = segments;  // Use 5 points in the line for jagged lightning

        for (int i = 0; i < segments; i++)
        {
            float t = i / (float)(segments - 1);  // Fraction along line
            Vector3 point = Vector3.Lerp(start, end, t);

            // Add random offset to create jagged effect
            point.x += Random.Range(-0.2f, 0.2f);
            point.y += Random.Range(-0.2f, 0.2f);

            lightning.SetPosition(i, point);
        }

        StartCoroutine(EndLightning(lightningProjectile, lifetime));
    }

    private IEnumerator EndLightning(GameObject lightningObject, float delay)
    {
        LineRenderer lightning = lightningObject.GetComponent<LineRenderer>();
        float elapsed = 0f;

        while (elapsed < lifetime)
        {
            // Randomly adjust the width to simulate flickering
            float width = Random.Range(0.02f, 0.06f);
            lightning.startWidth = width;
            lightning.endWidth = width;

            elapsed += Time.deltaTime;
            yield return null;
        }

        ObjectPoolManager.Instance.ReturnToPool(lightningObject);
    }

    private void UpdateAbilityStats()
    {
        currentDamage = abilityData.playerStats.GetStatValue(StatType.Damage, abilityData.baseDamage);
        currentRange = abilityData.playerStats.GetStatValue(StatType.Range, abilityData.baseRange);
        currentCooldownRate = abilityData.playerStats.GetStatValue(StatType.CooldownRate, abilityData.baseCooldownRate);
        currentCriticalHitDamage = abilityData.playerStats.GetStatValue(StatType.CriticalHitDamage, AttributeManager.Instance.CalculateCriticalHitDamage(currentDamage));
        currentChainAmount = abilityData.playerStats.GetStatValue(StatType.Zap_ChainLightning, baseChainAmount);
    }

    public override void AbilityReset()
    {
        IsFiring = false;
        StartCooldown(currentCooldownRate);
    }

    public override void AddAbilitySpecificUpgrades()
    {
        Sprite zapChainLightningIcon = Resources.Load<Sprite>("Icons/Zap_ChainLightningUpgrade");

        Zap_ChainLightning zap_ChainLightningUpgrade = new Zap_ChainLightning(zapChainLightningIcon);
        UpgradeDatabase.Instance.AddNewUpgrade(zap_ChainLightningUpgrade);

        Debug.Log("Zap-specific upgrades added to the upgrade pool.");
    }

    public void UnlockChainLightning()
    {
        hasChainLightning = true;
        Debug.Log("Chain Lightning upgrade unlocked!");
    }

    public void UnlockMultiChainUpgrade()
    {
        canChainToSameEnemy = true;
        Debug.Log("Multi-chain upgrade unlocked! Enemies can now be chained to multiple times.");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;  // Set the color of the gizmo
        Gizmos.DrawWireSphere(transform.position, currentRange);  // Draw a wireframe circle with baseRange as the radius
    }
}*/
