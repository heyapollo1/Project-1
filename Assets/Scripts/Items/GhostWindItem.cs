using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostWindItem : BaseItem
{
    private static readonly Dictionary<Rarity, List<StatModifier>> rarityModifiers = new()
    {
        { Rarity.Common, new List<StatModifier> { new (StatType.MovementSpeed, 0f, 15f) } },
        { Rarity.Rare, new List<StatModifier> { new (StatType.MovementSpeed, 0f, 20f) } },
        { Rarity.Epic, new List<StatModifier> { new (StatType.MovementSpeed, 0f, 25f) } },
        { Rarity.Legendary, new List<StatModifier> { new (StatType.MovementSpeed, 0f, 30f) } }
    };

    public GhostWindItem(Sprite ghostWindIcon, Rarity rarity) : base(
        "Ghost Wind", ghostWindIcon, 50, ItemType.Simple, rarity, 
        rarityModifiers[rarity])
    {
        UpdateDescription();
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
            }
            else
            {
                Debug.LogError($"No modifiers found for upgraded GhostWindItem rarity {currentRarity}.");
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
        return description = $"Increase movement speed by {currentBonus}%{upgradePart}.";
    }
}