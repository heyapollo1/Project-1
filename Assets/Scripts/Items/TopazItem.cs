using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopazItem : BaseItem
{
    private static readonly List<TagType> ClassTags = new()
    {
        TagType.Junk
    };
    
    private static readonly Dictionary<Rarity, List<StatModifier>> rarityModifiers = new()
    {
        { Rarity.Epic, new List<StatModifier> { new (StatType.BurnChance, 10f) } },
        { Rarity.Legendary, new List<StatModifier> { new (StatType.BurnChance, 20f) } }
    };
    
    private float stunChance = 20f;
    
    public TopazItem(Sprite topazIcon, Rarity rarity) : base(
        "Topaz", topazIcon, 50, ItemType.Simple, rarity, 
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
                Debug.LogError($"No modifiers found for upgraded Topaz rarity {currentRarity}.");
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

        return description = $"{(currentBonus):0}%{upgradePart} to inflict Stun.";
    }
    
    public override void Apply(AttributeManager playerStats)
    {
        base.Apply(playerStats);

        EventManager.Instance.TriggerEvent("FXPoolUpdate", "BurningFX", 10);
        Debug.Log("BurningFX added to FX pool.");
    }
    
    public override void Remove(AttributeManager playerStats)
    {
        base.Remove(playerStats);
        FXManager.Instance.RemovePool("BurningFX");
        Debug.Log("BurningFX removed from FX pool.");
    }
}
