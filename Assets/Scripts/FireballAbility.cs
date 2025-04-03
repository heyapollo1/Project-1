using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*public class FireballAbility : PlayerAbilityBase
{
    [Header("Fireball Specials")]
    public GameObject fireballPrefab;
    public float baseProjectileSpeed = 12f;
    public float baseFireballSize = 0.8f;

    private float currentDamage;
    private float currentCooldownRate;
    //private float currentRange;
    private float currentKnockbackForce;
    private float currentCriticalHitDamage;
    private float currentProjectileSpeed;
    private float currentFireballSize;

    [HideInInspector] public ObjectPoolManager objectPool;
    private PlayerController playerController;

    public override void Initialize()
    {
        Debug.Log("Initializing Fireball Ability...");

        abilityData.playerStats = AttributeManager.Instance;
        abilityData.player = GameObject.FindWithTag("Player").transform;
        abilityData.abilityPrefab = Resources.Load<GameObject>("PlayerAbilities/FireballAbility");
        abilityData.enemyLayer = LayerMask.GetMask("Enemy");
        playerController = abilityData.player.GetComponent<PlayerController>();
        fireballPrefab = Resources.Load<GameObject>("Prefabs/FireballPrefab");
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
        //currentRange = abilityData.baseRange;
        currentKnockbackForce = abilityData.baseKnockbackForce;
        currentCriticalHitDamage = AttributeManager.Instance.CalculateCriticalHitDamage(currentDamage);
        currentFireballSize = baseFireballSize;
        currentProjectileSpeed = baseProjectileSpeed;
    }

    public override bool CanUseAbility()
    {
        return IsAbilityReady();
    }

    public override void UseAbility()
    {
        if (isOnCooldown) return;

        Vector2 aimDirection = playerController.GetAimDirection();
        Debug.Log("Fireball Size: " + currentFireballSize);
        GameObject fireball = objectPool.GetFromPool("Fireball", playerController.castLocation.position, Quaternion.identity);
        fireball.transform.localScale = new Vector2(currentFireballSize, currentFireballSize);
        Fireball fireballScript = fireball.GetComponent<Fireball>();
        fireballScript.Initialize(aimDirection);

        if (abilityData.playerStats.IsCriticalHit())
        {
            fireballScript.SetStats(currentDamage, currentKnockbackForce, currentProjectileSpeed, fireball.transform.localScale);
        }
        else
        {
            fireballScript.SetStats(currentCriticalHitDamage, currentKnockbackForce, currentProjectileSpeed, fireball.transform.localScale, true);
        }

        AbilityReset();
    }

    private void UpdateAbilityStats()
    {
        currentDamage = abilityData.playerStats.GetStatValue(StatType.Damage, abilityData.baseDamage);
        //currentRange = abilityData.playerStats.GetStatValue(StatType.Range, abilityData.baseRange);
        currentCooldownRate = abilityData.playerStats.GetStatValue(StatType.CooldownRate, abilityData.baseCooldownRate);
        currentCriticalHitDamage = AttributeManager.Instance.CalculateCriticalHitDamage(currentDamage);
        Debug.Log("Current CDR: " + currentCooldownRate + ". My base CDR:" + abilityData.baseCooldownRate);
    }

    public override void AddAbilitySpecificUpgrades()
    {
    }

    public override void AbilityReset()
    {
        StartCooldown(currentCooldownRate);
    }
}*/