using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeItem : BaseItem
{
    private static readonly Dictionary<Rarity, List<StatModifier>> rarityModifiers = new()
    {
        { Rarity.Common, new List<StatModifier> { new (StatType.CriticalHitDamage, 20f) } },
        { Rarity.Rare, new List<StatModifier> { new (StatType.CriticalHitDamage, 30f) } },
        { Rarity.Epic, new List<StatModifier> { new (StatType.CriticalHitDamage, 40f) } },
        { Rarity.Legendary, new List<StatModifier> { new (StatType.CriticalHitDamage, 50f) } }
    };
    
    public AxeItem(Sprite axeIcon, Rarity rarity) : base(
        "Axe", axeIcon, 50, ItemType.Simple, rarity,
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
        return description = $"Increase Critical hit damage by {currentBonus}%{upgradePart}.";
    }
}
