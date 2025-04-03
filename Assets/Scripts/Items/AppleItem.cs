using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppleItem : BaseItem
{
    private static readonly List<TagType> ClassTags = new()
    {
        TagType.Food
    };
    
    private static readonly Dictionary<Rarity, List<StatModifier>> rarityModifiers = new()
    {
        { Rarity.Common, new List<StatModifier> { new (StatType.MaxHealth, 25f) } },
        { Rarity.Rare, new List<StatModifier> { new (StatType.MaxHealth, 50f) } },
        { Rarity.Epic, new List<StatModifier> { new (StatType.MaxHealth, 75f) } },
        { Rarity.Legendary, new List<StatModifier> { new (StatType.MaxHealth, 100f) } }
    };

    public AppleItem(Sprite appleIcon, Rarity rarity) : base(
        "Apple", appleIcon, 50, ItemType.Simple, rarity,
        rarityModifiers[rarity], ClassTags)
    {
        if (modifiers == null || modifiers.Count == 0)
        {
            Debug.LogError($"Modifiers list is null or empty for {itemName} at rarity {rarity}!");
        }
        
        UpdateDescription();
    }
    
    public override void Upgrade()
    {
        if (currentRarity < Rarity.Legendary)
        {
            currentRarity++;
            value = Mathf.RoundToInt(value * GetRarityMultiplier());
            modifiers = rarityModifiers[currentRarity];
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
                nextModifier = nextMods[0];
                return true;
            }
        }

        nextModifier = null;
        return false;
    }
    
    public override string UpdateDescription(bool showUpgrade = false)
    {
        float currentBonus = modifiers[0].flatBonus;
        string upgradePart = "";

        if (showUpgrade && TryGetNextUpgradeModifier(out var next))
        {
            upgradePart = $" >> <color=#00FF00>{next.flatBonus}%</color>";
        }
        return description = $"Increase max HP by {currentBonus}%{upgradePart}.";
    }
}
