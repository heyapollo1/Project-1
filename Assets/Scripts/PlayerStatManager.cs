using System;
using System.Collections.Generic;
using UnityEngine;

public enum StatType
{
    Damage,
    MaxHealth,
    CooldownRate,
    MovementSpeed,
    CriticalHitChance,
    CriticalHitDamage,
    Range,
    AreaSize,
    BurstFire,
    DodgeChance,
    Armour,
    BleedChance,
    BleedDamage,
    BurnChance,
    BurnDamage,
    PickupRadius,
    Whirlwind,
    CleaveSize,
    Scorch,
    Zap_ChainLightning
}

public class PlayerStatManager : MonoBehaviour
{
    public static PlayerStatManager Instance;

    private Dictionary<StatType, float> flatBonuses = new Dictionary<StatType, float>();
    private Dictionary<StatType, float> percentBonuses = new Dictionary<StatType, float>();

    private float minCooldownValue = 0.1f;

    public float baseDamage = 10f;
    public float baseMaxHealth = 100f;
    public float baseMoveSpeed = 4f;
    public float baseCriticalHitChance = 0f;
    public float baseDodgeChance = 0f;
    public float baseArmour = 0f;
    public float baseBleedChance = 0f;
    public float baseBurnChance = 0f;

    [HideInInspector] public float currentDamage;
    [HideInInspector] public float currentMaxHealth;
    [HideInInspector] public float currentMoveSpeed;
    [HideInInspector] public float currentCriticalHitChance;
    [HideInInspector] public float currentCriticalHitDamage;
    [HideInInspector] public float currentDodgeChance;
    [HideInInspector] public float currentArmour;
    [HideInInspector] public float currentBleedChance;
    [HideInInspector] public float currentBurnChance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        foreach (StatType statType in Enum.GetValues(typeof(StatType)))
        {
            flatBonuses[statType] = 0f;
            percentBonuses[statType] = 0f;
        }

        SetBaseStats();
        UpdateAbilityStats();
        EventManager.Instance.TriggerEvent("PlayerStatsReady");
    }

    public void SetBaseStats()
    {
        currentDamage = baseDamage;
        currentMaxHealth = baseMaxHealth;
        currentMoveSpeed = baseMoveSpeed;
        currentCriticalHitChance = baseCriticalHitChance;
        currentDodgeChance = baseDodgeChance;
        currentArmour = baseArmour;
        currentBleedChance = baseBleedChance;
        currentBurnChance = baseBurnChance;
    }

    private void UpdateAbilityStats()
    {
        currentDamage = GetStatValue(StatType.Damage, baseDamage);
        currentMaxHealth = GetStatValue(StatType.MaxHealth, baseMaxHealth);
        currentMoveSpeed = GetStatValue(StatType.MovementSpeed, baseMoveSpeed);
        currentCriticalHitChance = GetStatValue(StatType.CriticalHitChance, baseCriticalHitChance);
        currentDodgeChance = GetStatValue(StatType.DodgeChance, baseDodgeChance);
        currentArmour = GetStatValue(StatType.Armour, baseArmour);
        currentBleedChance = GetStatValue(StatType.BleedChance, baseBleedChance);
        currentBurnChance = GetStatValue(StatType.BurnChance, baseBurnChance);
    }

    public void ApplyModifier(StatModifier modifier)
    {
        flatBonuses[modifier.statType] += modifier.flatBonus;
        percentBonuses[modifier.statType] += modifier.percentBonus;

        RecalculateStats();
    }

    public void RemoveModifier(StatModifier modifier)
    {
        flatBonuses[modifier.statType] -= modifier.flatBonus;
        percentBonuses[modifier.statType] -= modifier.percentBonus;

        RecalculateStats();
    }

    public void ResetAllStats()
    {
        foreach (StatType statType in Enum.GetValues(typeof(StatType)))
        {
            flatBonuses[statType] = 0f;
            percentBonuses[statType] = 0f;
        }

        SetBaseStats();
        RecalculateStats();
    }

    public bool IsCriticalHit()
    {
        float roll = UnityEngine.Random.Range(0f, 100f);

        return roll <= currentCriticalHitChance;
    }

    public float CalculateCriticalHitDamage(float baseDamage)
    {
        float critBonus = GetStatValue(StatType.CriticalHitDamage, 0f);
        float critMultiplier = 1.5f + (critBonus / 100f);
        float finalCriticalHitValue = baseDamage * critMultiplier;
        return Mathf.Ceil(finalCriticalHitValue);
    }

    public bool AttemptToApplyStatusEffect(StatType statusEffect)
    {
        float roll = UnityEngine.Random.Range(0f, 100f);
        float chanceToApply;

        switch (statusEffect)
        {
            case StatType.BleedChance:
                chanceToApply = currentBleedChance;
                break;
            case StatType.BurnChance:
                chanceToApply = currentBurnChance;
                break;
            default:
                Debug.LogWarning("Unknown status effect type: " + statusEffect);
                return false;
        }
        return roll <= chanceToApply;
    }

    public float ArmourMitigation(float damage, float armour)
    {
        float damageReduction = (armour * 100) / (armour + 100);
        float reducedDamage = damage * (1 - (damageReduction / 100));
        return Mathf.Ceil(reducedDamage);
    }

    public bool IsDodgeSuccessful(float dodgeChance)
    {
        float roll = UnityEngine.Random.value * 100;
        return roll < dodgeChance; 
    }

    private void RecalculateStats()
    {
        Debug.Log($"Invoking stat change!");
        UpdateAbilityStats();
        EventManager.Instance.TriggerEvent("StatsChanged");
    }

    public float GetStatValue(StatType statType, float baseValue)
    {
        float flatBonus = flatBonuses[statType];
        float percentBonus = percentBonuses[statType];

        if (statType == StatType.CooldownRate)
        {
            float baseCooldown = baseValue + flatBonus;

            // Apply additive scaling up to 50% total reduction
            if (percentBonus <= 50f)
            {
                // Simple additive reduction
                float cooldown = baseCooldown * (1 - percentBonus / 100f);
                return Mathf.Max(cooldown, minCooldownValue);  // Clamp to min cooldown
            }
            else
            {
                // First apply the flat 50% reduction
                float cooldown = baseCooldown * 0.5f;  // 50% reduction

                // Apply the remaining reduction multiplicatively
                float extraReduction = percentBonus - 50f;
                float cooldownMultiplier = Mathf.Pow(0.80f, extraReduction / 20f);  // Multiplicative for further reductions
                return Mathf.Max(cooldown * cooldownMultiplier, minCooldownValue);  // Clamp to min cooldown
            }
        }

        // Default case for other stat types
        return (baseValue + flatBonus) * (1 + percentBonus / 100f);
    }
}