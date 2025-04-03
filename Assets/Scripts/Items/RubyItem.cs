using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Utils;
using System;

public class RubyItem : BaseItem
{
    private static readonly List<TagType> ClassTags = new()
    {
        TagType.Junk
    };
            
    private static readonly Dictionary<Rarity, List<StatModifier>> rarityModifiers = new()
    {
        { Rarity.Common, new List<StatModifier> { new (StatType.Damage, 0f, 50f) } },
        { Rarity.Rare, new List<StatModifier> { new (StatType.Damage, 0f, 75f) } },
        { Rarity.Epic, new List<StatModifier> { new (StatType.Damage, 0f, 100f) } },
        { Rarity.Legendary, new List<StatModifier> { new (StatType.Damage, 0f, 125f) } }
    };
    
    private float damageMultiplier;
    
    public RubyItem(Sprite rubyIcon, Rarity rarity) 
        : base("Ruby Item", rubyIcon, 50, ItemType.Combat, rarity, rarityModifiers[rarity], ClassTags,
            new RubyItemEffect(ModifierUtils.GetPercentBonus(rarityModifiers[rarity], StatType.Damage)))
    {
        damageMultiplier = ModifierUtils.GetPercentBonus(rarityModifiers[rarity], StatType.Damage);
        UpdateDescription();
    }
    
    private static float GetBonusDamage(List<StatModifier> modifiers)
    {
        return modifiers.Find(m => m.statType == StatType.Damage)?.percentBonus ?? 0f;
    }
    
    public override void Upgrade()
    {
        if (currentRarity < Rarity.Legendary)
        {
            currentRarity++;
            value = Mathf.RoundToInt(value * GetRarityMultiplier());
            
            if (rarityModifiers.ContainsKey(currentRarity))
            {
                modifiers = rarityModifiers[currentRarity];
                damageMultiplier = ModifierUtils.GetPercentBonus(rarityModifiers[currentRarity], StatType.Damage);
                combatEffect = new RubyItemEffect(GetBonusDamage(modifiers));
            }
            else
            {
                Debug.LogError($"No modifiers found for upgraded RubyItem rarity {currentRarity}.");
            }
            UpdateDescription();
        }
    }
    
    public bool TryGetNextUpgradeModifier(out StatModifier nextModifier)
    {
        if (currentRarity < Rarity.Legendary)
        {
            Rarity nextRarity = currentRarity + 1;
            if (rarityModifiers.TryGetValue(nextRarity, out var nextMods))
            {
                nextModifier = nextMods.Find(m => m.statType == StatType.Damage);
                return true;
            }
        }
        nextModifier = null;
        return false;
    }
    
    public override string UpdateDescription(bool showUpgrade = false)
    {
        float currentBonus = ModifierUtils.GetPercentBonus(modifiers, StatType.Damage);
        string upgradePart = "";
        
        if (showUpgrade && TryGetNextUpgradeModifier(out var next))
        {
            upgradePart = $" >> <color=#00FF00>{next.percentBonus}%</color>";
        }
        return description = $"Deal {currentBonus}%{upgradePart} bonus damage when under half health.";
    }
}