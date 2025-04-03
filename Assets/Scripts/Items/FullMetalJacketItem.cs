using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullMetalJacketItem : BaseItem
{
    private float damageMultiplier;

    public FullMetalJacketItem(Sprite fullMetalJacketIcon, Rarity rarity) 
        : base("Full Metal Jacket", fullMetalJacketIcon, 50, ItemType.Combat, rarity, null, null, 
            new FullMetalJacketEffect(GetDamageMultiplier[rarity]))
    {
        damageMultiplier = GetDamageMultiplier[rarity];
        UpdateDescription();
    }
    
    private static readonly Dictionary<Rarity, float> GetDamageMultiplier = new()
    {
        { Rarity.Common, 0.5f },
        { Rarity.Rare, 1.0f },
        { Rarity.Epic, 1.5f },
        { Rarity.Legendary, 2.0f }
    };
    
    public bool TryGetNextUpgradeModifier(out float nextDamageMultiplier)
    {
        if (currentRarity < Rarity.Legendary)
        {
            Rarity nextRarity = currentRarity + 1;
            if (GetDamageMultiplier.TryGetValue(nextRarity, out nextDamageMultiplier))
                return true;
        }

        nextDamageMultiplier = 0f;
        return false;
    }
    
    public override string UpdateDescription(bool showUpgrade = false)
    {
        string upgradePart = "";

        if (showUpgrade && TryGetNextUpgradeModifier(out float nextChance))
        {
            upgradePart = $" >> <color=#00FF00>{nextChance * 100}%</color>";
        }
        
        return description = $"Deal +{damageMultiplier * 100}%{upgradePart} bonus damage against enemies above 90% health.";
    }
    
    public override void Upgrade()
    {
        if (currentRarity < Rarity.Legendary)
        {
            currentRarity++;
            value = Mathf.RoundToInt(value * GetRarityMultiplier()); 
            damageMultiplier = GetDamageMultiplier[currentRarity];
            combatEffect = new FullMetalJacketEffect(damageMultiplier); 
            UpdateDescription(); 
        }
    }
}