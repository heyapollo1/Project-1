using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum StatType
{
    Damage,
    MaxHealth,
    CooldownRate,
    MovementSpeed,
    CriticalHitChance,
    CriticalHitDamage,
    KnockbackForce,
    Range,
    AreaSize,
    BurstFire,
    DodgeChance,
    Armour,
    BleedChance,
    BurnChance,
    StunChance,
    Whirlwind,
    CleaveSize,
    Scorch,
    Zap_ChainLightning
}

public class AttributeManager : MonoBehaviour
{
    public static AttributeManager Instance;

    private Dictionary<StatType, float> flatBonuses = new Dictionary<StatType, float>();
    private Dictionary<StatType, float> percentBonuses = new Dictionary<StatType, float>();

    private float minCooldownValue = 0.1f;

    public float baseMaxHealth = 100f;
    public float baseMoveSpeed = 4f;
    public float baseDamage = 0f;
    public float baseCriticalHitChance = 0f;
    public float baseCriticalHitDamage = 0f;
    public float baseKnockbackForce = 0f;
    public float baseDodgeChance = 0f;
    public float baseArmour = 0f;
    public float baseBleedChance = 0f;
    public float baseBurnChance = 0f;
    public float baseStunChance = 0f;

    [HideInInspector] public float currentDamage;
    [HideInInspector] public float currentMaxHealth;
    [HideInInspector] public float currentMoveSpeed;
    [HideInInspector] public float currentCriticalHitChance;
    [HideInInspector] public float currentCriticalHitDamage;
    [HideInInspector] public float currentKnockbackForce;
    [HideInInspector] public float currentDodgeChance;
    [HideInInspector] public float currentArmour;
    [HideInInspector] public float currentBleedChance;
    [HideInInspector] public float currentBurnChance;
    [HideInInspector] public float currentStunChance;

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
        
        EventManager.Instance.StartListening("SceneLoaded", OnSceneLoaded);
    }

    private void OnDestroy()
    {
        EventManager.Instance.StopListening("SceneLoaded", OnSceneLoaded);
    }
    
    private void OnSceneLoaded(string scene)
    {
        if (scene == "GameScene")
        {
            GameData gameData = SaveManager.LoadGame();
            EventManager.Instance.TriggerEvent("PlayerStatsReady");
            if (gameData.isNewGame) 
            {
                foreach (StatType statType in Enum.GetValues(typeof(StatType)))
                {
                    flatBonuses[statType] = 0f;
                    percentBonuses[statType] = 0f;
                }
                SetBaseStats();
                UpdateAbilityStats();
            }
        }
    }

    public void SetBaseStats()
    {
        currentDamage = baseDamage;
        currentMaxHealth = baseMaxHealth;
        currentMoveSpeed = baseMoveSpeed;
        currentCriticalHitChance = baseCriticalHitChance;
        currentCriticalHitDamage = baseCriticalHitDamage;
        currentKnockbackForce = baseKnockbackForce;
        currentDodgeChance = baseDodgeChance;
        currentArmour = baseArmour;
        currentBleedChance = baseBleedChance;
        currentBurnChance = baseBurnChance;
        currentStunChance = baseStunChance;
    }

    private void UpdateAbilityStats()
    {
        currentDamage = GetStatValue(StatType.Damage, baseDamage);
        currentMaxHealth = GetStatValue(StatType.MaxHealth, baseMaxHealth);
        currentMoveSpeed = GetStatValue(StatType.MovementSpeed, baseMoveSpeed);
        currentCriticalHitChance = GetStatValue(StatType.CriticalHitChance, baseCriticalHitChance);
        currentCriticalHitDamage = GetStatValue(StatType.CriticalHitDamage, baseCriticalHitDamage);
        currentKnockbackForce = GetStatValue(StatType.KnockbackForce, baseKnockbackForce);
        currentDodgeChance = GetStatValue(StatType.DodgeChance, baseDodgeChance);
        currentArmour = GetStatValue(StatType.Armour, baseArmour);
        currentBleedChance = GetStatValue(StatType.BleedChance, baseBleedChance);
        currentBurnChance = GetStatValue(StatType.BleedChance, baseBurnChance);
        currentStunChance = GetStatValue(StatType.BleedChance, baseStunChance);
        Debug.Log($"Applying burn chance modifier: {currentBurnChance}");
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
        
        return (baseValue + flatBonus) * (1 + percentBonus / 100f);
    }
    
    public void ApplyModifier(StatModifier modifier)
    {
        Debug.Log("Applying modifier: " + modifier.ToString());
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
    
    public Dictionary<StatType, float> GetActiveStatusEffectTriggers()
    {
        Dictionary<StatType, float>  triggers = new();

        float bleedChance = currentBleedChance;
        if (bleedChance > 0)
            triggers.Add(StatType.BleedChance, currentBleedChance);
        
        float burnChance = currentBurnChance;
        if (bleedChance > 0)
            triggers.Add(StatType.BurnChance, currentBurnChance);
        
        /*float burnChance = currentBurnChance;
        if (burnChance > 0)
            triggers.Add(new StatusEffectTrigger(StatType.BurnChance, burnChance));
        
        float stunChance = currentStunChance;
        if (burnChance > 0)
            triggers.Add(new StatusEffectTrigger(StatType.StunChance, stunChance));*/
        
        return triggers;
    }
    
    public bool ShouldTrigger(float chance)
    {
        return chance > 0f && Random.Range(0f, 100f) <= chance;
    }
    
    public bool IsCriticalHit(float criticalHitChance)
    {
        float roll = UnityEngine.Random.Range(0f, 100f);

        return roll <= criticalHitChance;
    }

    public float CalculateCriticalHitDamage(float baseDamage, float bonusCriticalHitDamage)
    {
        float critMultiplier = 1.5f + (bonusCriticalHitDamage / 100f);
        float finalCriticalHitValue = baseDamage * critMultiplier;
        return Mathf.Ceil(finalCriticalHitValue);
    }
    
    public float GetStatusEffectChance(StatType statusType)
    {
        float baseChance = GetStatValue(statusType, 0f);
        if (baseChance <= 0) return 0;
        return baseChance;
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
    
    public Dictionary<StatType, float> GetFlatBonuses()
    {
        return new Dictionary<StatType, float>(flatBonuses);
    }
    
    public Dictionary<StatType, float> GetPercentBonuses()
    {
        return new Dictionary<StatType, float>(percentBonuses);
    }

}