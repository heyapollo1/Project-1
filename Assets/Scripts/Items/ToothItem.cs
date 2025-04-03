using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToothItem : BaseItem
{
    private static readonly List<TagType> ClassTags = new()
    {
        TagType.Junk
    };
    
    private static readonly Dictionary<Rarity, float> GetDamageBonus = new()
    {
        { Rarity.Epic, 0.5f },
        { Rarity.Legendary, 1.0f }
    };
    
    private float damageBonus;

    public ToothItem(Sprite toothIcon, Rarity rarity) 
        : base("Tooth", toothIcon, 50, ItemType.Combat, rarity, null, ClassTags,
            new ToothEffect(GetDamageBonus[rarity]))
    {
        damageBonus = GetDamageBonus[rarity];
        UpdateDescription();
    }
    
    public override void Upgrade()
    {
        if (currentRarity < Rarity.Legendary)
        {
            currentRarity++;
            value = Mathf.RoundToInt(value * GetRarityMultiplier()); 
            damageBonus = GetDamageBonus[currentRarity];
            combatEffect = new ToothEffect(damageBonus); 
            UpdateDescription(); 
        }
    }
    
    public bool TryGetNextUpgradeModifier(out float nextDamageBonus)
    {
        if (currentRarity < Rarity.Legendary)
        {
            Rarity nextRarity = currentRarity + 1;
            if (GetDamageBonus.TryGetValue(nextRarity, out nextDamageBonus))
                return true;
        }

        nextDamageBonus = 0f;
        return false;
    }
    
    public override string UpdateDescription(bool showUpgrade = false)
    {
        string upgradePart = "";

        if (showUpgrade && TryGetNextUpgradeModifier(out float nextChance))
        {
            upgradePart = $" >> <color=#00FF00>{nextChance * 100}%</color>";
        }
        
        return description = $"Increase damage against Bleeding enemies by {(damageBonus * 100):0}%{upgradePart}.";
    }
}