using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*public class CleaveAbility : PlayerAbilityBase
{
    [Header("Cleave Specials")]
    public float baseCleaveSize;
    public float baseSpinAmount;

    [HideInInspector] public float currentDamage;
    [HideInInspector] public float currentCooldownRate;
    [HideInInspector] public float currentRange;
    [HideInInspector] public float currentKnockbackForce;
    [HideInInspector] public float currentCriticalHitChance;
    [HideInInspector] public float currentCriticalHitDamage;
    [HideInInspector] public float currentCleaveSize;
    [HideInInspector] public float currentSpinAmount;

    private GameObject cleavePrefab;
    private ObjectPoolManager objectPool;

    private bool IsActive = false;

    public override void Initialize()
    {
        Debug.Log("Initializing Cleave Ability...");

        abilityData.playerStats = AttributeManager.Instance;
        abilityData.player = GameObject.FindWithTag("Player").transform;
        abilityData.abilityPrefab = Resources.Load<GameObject>("PlayerAbilities/CleaveAbility");
        abilityData.enemyLayer = LayerMask.GetMask("Enemy");
        cleavePrefab = Resources.Load<GameObject>("Prefabs/CleavePrefab");
        objectPool = FindObjectOfType<ObjectPoolManager>();

        EventManager.Instance.TriggerEvent("ObjectPoolUpdate", "CleavePrefab", 3);
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
        currentCleaveSize = baseCleaveSize;
        currentSpinAmount = baseSpinAmount;
    }

    public override bool CanUseAbility()
    {
        return IsAbilityReady() && !IsActive;
    }

    public override void UseAbility()
    {
        if (isOnCooldown) return;
        IsActive = true;
        
        cleavePrefab = objectPool.GetFromPool("CleavePrefab", abilityData.player.position, Quaternion.identity);
        cleavePrefab.transform.localScale = new Vector2(currentCleaveSize, currentCleaveSize);
        cleavePrefab.transform.SetParent(abilityData.player);

        var cleaveEffect = cleavePrefab.GetComponent<CleaveEffect>();

        cleaveEffect.Initialize(abilityData.enemyLayer);
        cleaveEffect.SetStats(currentDamage, currentKnockbackForce, currentCriticalHitChance, currentCriticalHitDamage, currentSpinAmount, cleavePrefab.transform.localScale, abilityData.player.transform);

        AbilityReset();
    }

    private void UpdateAbilityStats()
    {
        currentDamage = abilityData.playerStats.GetStatValue(StatType.Damage, abilityData.baseDamage);
        currentRange = abilityData.playerStats.GetStatValue(StatType.Range, abilityData.baseRange);
        currentCooldownRate = abilityData.playerStats.GetStatValue(StatType.CooldownRate, abilityData.baseCooldownRate);
        currentCriticalHitDamage = abilityData.playerStats.GetStatValue(StatType.CriticalHitDamage, AttributeManager.Instance.CalculateCriticalHitDamage(currentDamage));
        currentSpinAmount = abilityData.playerStats.GetStatValue(StatType.Whirlwind, baseSpinAmount);
        currentCleaveSize = abilityData.playerStats.GetStatValue(StatType.CleaveSize, baseCleaveSize);
    }

    public override void AbilityReset()
    {
        IsActive = false;

        StartCooldown(currentCooldownRate); 
    }

    public override void AddAbilitySpecificUpgrades()
    {
        Sprite whirlwindIcon = Resources.Load<Sprite>("Icons/WhirlwindIcon");

        WhirlwindUpgrade whirlwindUpgrade = new WhirlwindUpgrade(whirlwindIcon);
        UpgradeDatabase.Instance.AddNewUpgrade(whirlwindUpgrade);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue; 
        Gizmos.DrawWireSphere(transform.position, currentRange); 
    }
}*/

