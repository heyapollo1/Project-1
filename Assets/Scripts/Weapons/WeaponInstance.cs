using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponInstance
{
    public WeaponData weaponData;  
    public WeaponBase weaponBase;
    public ItemUI assignedUI = null;
    public Rarity rarity;

    public string uniqueID;
    public string weaponTitle;
    public string weaponDescription;
    public Sprite weaponIcon;
    public GameObject weaponPrefab;
    public TagType mainTag;
    public int value;
    
    [HideInInspector] public List<TagType> classTags = new List<TagType>();
    [HideInInspector] public Dictionary<string, float> statTags = new Dictionary<string, float>();
    
    public float baseDamage;
    public float baseCooldownRate;
    public float baseRange;
    public float baseKnockbackForce;
    public float baseCriticalHitChance;
    public float baseCriticalHitDamage;
    public ICombatEffect playerCombatEffect = null;
    public List<WeaponUpgradeEffect> upgradeEffects = new List<WeaponUpgradeEffect>();
    public StatCollection Stats { get; private set; }
    //public List<ICombatEffect> playerCombatEffect = new List<ICombatEffect>();

    public WeaponInstance(WeaponData baseData)
    {
        weaponData = baseData;
        rarity = baseData.rarity;
        
        weaponTitle = baseData.weaponTitle;
        uniqueID = $"{weaponTitle}_{Guid.NewGuid()}";
        weaponDescription = baseData.weaponDescription;
        weaponIcon = baseData.weaponIcon;
        weaponPrefab = baseData.weaponPrefab;
        value = baseData.price;
        
        baseDamage = baseData.baseDamage;
        baseCooldownRate = baseData.baseCooldownRate;
        baseRange = baseData.baseRange;
        baseKnockbackForce = baseData.baseKnockbackForce;
        baseCriticalHitChance = baseData.baseCriticalHitChance;
        baseCriticalHitDamage = baseData.baseCriticalHitDamage;
        upgradeEffects = baseData.rarityUpgrades;
        mainTag = TagType.Weapon;
        
        statTags.Add("DMG", Mathf.RoundToInt(baseDamage));
        statTags.Add("SPD", Mathf.Round(baseCooldownRate * 10) / 10f);
        statTags.Add("RNG", Mathf.RoundToInt(baseRange));
        
        Stats = new StatCollection();
        
        foreach (TagType tag in baseData.associatedTags)
        {
            if (TagDatabase.ClassTags.ContainsKey(tag)) 
            {
                classTags.Add(tag);
            }
        }
        
        ApplyRarityEffects(baseData.rarityUpgrades, false);
        Debug.Log($"Weapon: {weaponTitle} ({rarity}) [ID: {uniqueID}]");
    }
    
    public void UpgradeWeaponInstance()
    {
        if (rarity < Rarity.Legendary)
        {
            rarity++;
            Debug.Log($"Upgraded Weapon to: {rarity}");
        }
        ApplyRarityEffects(upgradeEffects, true);
    }

    public void AssignUI(ItemUI ui)
    {
        assignedUI = ui;
    }
    
    private void ApplyRarityEffects(List<WeaponUpgradeEffect> possibleEffects, bool playerOwned)
    {
        foreach (var effect in possibleEffects)
        {
            if (rarity == effect.rarityTier)
            {
                baseDamage += effect.additionalDamage;
                baseCooldownRate += effect.cooldownReduction;
                baseRange += effect.rangeIncrease;
            }
        }
        
        if (playerCombatEffect != null)
        {
            PlayerCombat.Instance.RegisterCombatEffect(playerCombatEffect);
        }
        
        if (playerOwned && weaponBase != null)
        {
            weaponBase.weaponInstance = this;
            weaponBase.UpgradeWeapon();
            Debug.Log($"Applied upgrade to weaponBase: {weaponBase.name}");
        }
    }
    
    public float GetFinalStat(StatType statType)
    {
        float modifierBonus = AttributeManager.Instance.Stats.Get(statType, 0f);
        return Stats.Get(statType, GetBaseStat(statType)) + modifierBonus;
    }

    private float GetBaseStat(StatType statType)
    {
        return statType switch
        {
            StatType.Damage => baseDamage,
            StatType.CooldownRate => baseCooldownRate,
            StatType.Range => baseRange,
            StatType.KnockbackForce => baseKnockbackForce,
            StatType.CriticalHitChance => baseCriticalHitChance,
            StatType.CriticalHitDamage => baseCriticalHitDamage,
            _ => 0f
        };
    }
    
    public void UpdateStatTags(float newDamage, float newCooldown, float newRange)
    {
        //Debug.Log($"Updating Stat Tags, damage: {newDamage}, CDR: {newCooldown}, RNG: {newRange}");
        statTags["DMG"] = Mathf.RoundToInt(newDamage); 
        statTags["SPD"] = Mathf.Round(newCooldown * 10) / 10f; // Rounds to one decimal place
        statTags["RNG"] = Mathf.RoundToInt(newRange);
    }
    
    public void WeaponDiscarded()
    {
        //loadoutIndex = -1;
        //slotIndex = -1;
        //location = ItemLocation.World;
    }
}
