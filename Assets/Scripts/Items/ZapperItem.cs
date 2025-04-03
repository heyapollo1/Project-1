using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*public class ZapperItem : BaseItem
{
    private float healAmount;
    private PotionEffect potionEffect;

    private static readonly Dictionary<Rarity, float> GetHealAmount = new()
    {
        { Rarity.Common, 5f },
        { Rarity.Rare, 10f },
        { Rarity.Epic, 15f },
        { Rarity.Legendary, 20f }
    };
    
    private static readonly List<TagType> ClassTags = new()
    {
        TagType.Junk
    };
    
    public ZapperItem(Sprite icon, Rarity rarity)
        : base("Zapper", icon, 100, ItemType.Combat, rarity, null,
            new List<TagType> { TagType.Tool }, null,
            new ZapperEffect(20f, 8f, 10f, 3f, interval: 3f, hasChain: true))
    {
        UpdateDescription();
    }
    
    public ZapperItem(Sprite zapperIcon, Rarity rarity) : base(
        "Zapper", zapperIcon, 50, ItemType.Simple, rarity, null, ClassTags, null, new ZapperEffect(GetHealAmount[rarity]))
    {
        healAmount = GetHealAmount[rarity];
        potionEffect = (ZapperEffect)passiveEffect;
        UpdateDescription();
    }

    public override void Upgrade()
    {
        if (currentRarity < Rarity.Legendary)
        {
            currentRarity++;
            value = Mathf.RoundToInt(value * GetRarityMultiplier());
            healAmount = GetHealAmount[currentRarity];
            potionEffect.IncreaseHealAmount(healAmount); // Incremental scaling
            UpdateDescription();
        }
    }
    
    public bool TryGetNextUpgradeModifier(out float nextHealAmount)
    {
        if (currentRarity < Rarity.Legendary)
        {
            Rarity nextRarity = currentRarity + 1;
            if (GetHealAmount.TryGetValue(nextRarity, out nextHealAmount))
                return true;
        }
        nextHealAmount = 0f;
        return false;
    }
    
    public override string UpdateDescription(bool showUpgrade = false)
    {
        string upgradePart = "";

        if (showUpgrade && TryGetNextUpgradeModifier(out float nextChance))
        {
            upgradePart = $" >> <color=#00FF00>{nextChance * 100}%</color>";
        }
        return description = $"Regenerates {(healAmount * 100):0}%{upgradePart} HP every 5 seconds.";
    }

    protected override void UpdateDescription()
    {
        description = $"Shoots lightning at the nearest enemy every {3f} seconds, chaining up to 3 targets.";
    }
}*/
