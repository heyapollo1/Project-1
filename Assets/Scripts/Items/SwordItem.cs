using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordItem : BaseItem
{
    private static readonly Dictionary<Rarity, List<StatModifier>> rarityModifiers = new()
    {
        { Rarity.Common, new List<StatModifier> { new (StatType.Damage, 5f) } },
        { Rarity.Rare, new List<StatModifier> { new (StatType.Damage, 10f) } },
        { Rarity.Epic, new List<StatModifier> { new (StatType.Damage, 15f) } },
        { Rarity.Legendary, new List<StatModifier> { new (StatType.Damage, 20f) } }
    };
    
    public SwordItem(Sprite swordIcon, Rarity rarity) : base(
        "Sword", swordIcon, 50, ItemType.Simple, rarity,
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

        nextModifier = default;
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

        return  description = $"Increase damage by {currentBonus}%{upgradePart}.";
    }
}