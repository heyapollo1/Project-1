using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum StatType
{
    Damage,
    MaxHealth,
    CooldownRate,
    Range,
    MovementSpeed,
    CriticalHitChance,
    CriticalHitDamage,
    KnockbackForce,
    AreaSize,
    BurstFire,
    DodgeChance,
    Armour,
    BleedChance,
    BurnChance,
    StunChance,
}

public class AttributeManager : MonoBehaviour
{
    public static AttributeManager Instance;
    
    public float baseMaxHealth = 100f;
    public float baseMoveSpeed = 4f;
    public float baseDamage = 0f;
    public float baseCooldownRate = 0f;
    public float baseRange = 0f;
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
    [HideInInspector] public float currentCooldownRate;
    [HideInInspector] public float currentRange;
    [HideInInspector] public float currentCriticalHitChance;
    [HideInInspector] public float currentCriticalHitDamage;
    [HideInInspector] public float currentKnockbackForce;
    [HideInInspector] public float currentDodgeChance;
    [HideInInspector] public float currentArmour;
    [HideInInspector] public float currentBleedChance;
    [HideInInspector] public float currentBurnChance;
    [HideInInspector] public float currentStunChance;


    public StatCollection Stats { get; } = new();
    private float minCooldownValue = 0.1f;
    
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
                    Stats.flatBonuses[statType] = 0f;
                    Stats.percentBonuses[statType] = 0f;
                }
                SetBaseStats();
                RecalculateStats();
            }
        }
    }

    public void SetBaseStats()
    {
        currentDamage = baseDamage;
        currentMaxHealth = baseMaxHealth;
        currentMoveSpeed = baseMoveSpeed;
        currentCooldownRate = baseCooldownRate;
        currentRange = baseRange;
        currentCriticalHitChance = baseCriticalHitChance;
        currentCriticalHitDamage = baseCriticalHitDamage;
        currentKnockbackForce = baseKnockbackForce;
        currentDodgeChance = baseDodgeChance;
        currentArmour = baseArmour;
        currentBleedChance = baseBleedChance;
        currentBurnChance = baseBurnChance;
        currentStunChance = baseStunChance;
    }
    
    public float GetBaseStat(StatType statType)
    {
        return statType switch
        {
            StatType.Damage => baseDamage,
            StatType.MaxHealth => baseMaxHealth,
            StatType.MovementSpeed => baseMoveSpeed,
            StatType.CooldownRate => baseCooldownRate,
            StatType.Range => baseRange,
            StatType.CriticalHitChance => baseCriticalHitChance,
            StatType.CriticalHitDamage => baseCriticalHitDamage,
            StatType.KnockbackForce => baseKnockbackForce,
            StatType.DodgeChance => baseDodgeChance,
            StatType.Armour => baseArmour,
            StatType.BleedChance => baseBleedChance,
            StatType.BurnChance => baseBurnChance,
            StatType.StunChance => baseStunChance,
            _ => 0f
        };
    }
    
    public float GetStatValue(StatType statType)
    {
        float baseVal = GetBaseStat(statType);
        float flat = Stats.flatBonuses.GetValueOrDefault(statType, 0f);
        float percent = Stats.percentBonuses.GetValueOrDefault(statType, 0f);
        return (baseVal + flat) * (1 + percent / 100f);
    }
    
    public void ApplyModifier(StatModifier modifier)
    {
        Debug.Log("Applying modifier: " + modifier.ToString());
        Stats.Apply(modifier);
        RecalculateStats();
    }

    public void RemoveModifier(StatModifier modifier)
    {
        Stats.Remove(modifier);
        RecalculateStats();
    }

    public void ResetAllStats()
    {
        foreach (StatType statType in Enum.GetValues(typeof(StatType)))
        {
            Stats.flatBonuses[statType] = 0f;
            Stats.percentBonuses[statType] = 0f;
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
        
        return triggers;
    }
    
    public bool ShouldTrigger(float chance)
    {
        return chance > 0f && Random.Range(0f, 100f) <= chance;
    }

    public float CalculateCriticalHitDamage(float baseDamage, float bonusCriticalHitDamage)
    {
        float critMultiplier = 1.5f + (bonusCriticalHitDamage / 100f);
        float finalCriticalHitValue = baseDamage * critMultiplier;
        return Mathf.Ceil(finalCriticalHitValue);
    }

    public float ArmourMitigation(float damage, float armour)
    {
        float damageReduction = (armour * 100) / (armour + 100);
        float reducedDamage = damage * (1 - (damageReduction / 100));
        return Mathf.Ceil(reducedDamage);
    }

    public void RecalculateStats()
    {
        currentDamage = Stats.Get(StatType.Damage, baseDamage);
        currentMaxHealth = Stats.Get(StatType.MaxHealth, baseMaxHealth);
        currentMoveSpeed = Stats.Get(StatType.MovementSpeed, baseMoveSpeed);
        currentCriticalHitChance = Stats.Get(StatType.CriticalHitChance, baseCriticalHitChance);
        currentCriticalHitDamage = Stats.Get(StatType.CriticalHitDamage, baseCriticalHitDamage);
        currentKnockbackForce = Stats.Get(StatType.KnockbackForce, baseKnockbackForce);
        currentDodgeChance = Stats.Get(StatType.DodgeChance, baseDodgeChance);
        currentArmour = Stats.Get(StatType.Armour, baseArmour);
        currentBleedChance = Stats.Get(StatType.BleedChance, baseBleedChance);
        currentBurnChance = Stats.Get(StatType.BurnChance, baseBurnChance);
        currentStunChance = Stats.Get(StatType.StunChance, baseStunChance);
        
        EventManager.Instance.TriggerEvent("StatsChanged");
    }
}