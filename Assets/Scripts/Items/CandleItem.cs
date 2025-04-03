using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandleItem : BaseItem
{
    private static readonly Dictionary<Rarity, List<StatModifier>> rarityModifiers = new()
    {
        { Rarity.Rare, new List<StatModifier> { new (StatType.BurnChance, 20f) } },
        { Rarity.Epic, new List<StatModifier> { new (StatType.BurnChance, 40f) } },
        { Rarity.Legendary, new List<StatModifier> { new (StatType.BurnChance, 60f) } }
    };
    
    public CandleItem(Sprite candleIcon, Rarity rarity) : base(
        "Candle", candleIcon, 50, ItemType.Simple, rarity, 
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
                Debug.LogError($"No modifiers found for upgraded Candle rarity {currentRarity}.");
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

        return description = $"{(currentBonus):0}%{upgradePart} to inflict Burn.";
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

/*public class CandleItem : BaseItem
{
    private static readonly Dictionary<Rarity, float> GetBurnChance = new()
    {
        { Rarity.Rare, 20f },
        { Rarity.Epic, 40f },
        { Rarity.Legendary, 60f }
    };
    
    private float burnChance;

    public CandleItem(Sprite candleIcon, Rarity rarity) 
        : base("Candle", candleIcon, 50, ItemType.Combat, rarity, null, null,
            new CandleEffect(GetBurnChance[rarity]))
    {
        burnChance = GetBurnChance[rarity];
        UpdateDescription();
    }
    
    public override void Upgrade()
    {
        if (currentRarity < Rarity.Legendary)
        {
            currentRarity++;
            value = Mathf.RoundToInt(value * GetRarityMultiplier()); 
            burnChance = GetBurnChance[currentRarity];
            combatEffect = new CandleEffect(burnChance); 
            UpdateDescription(); 
        }
    }
    
    public bool TryGetNextUpgradeModifier(out float nextBurnChance)
    {
        if (currentRarity < Rarity.Legendary)
        {
            Rarity nextRarity = currentRarity + 1;
            if (GetBurnChance.TryGetValue(nextRarity, out nextBurnChance))
                return true;
        }

        nextBurnChance = 0f;
        return false;
    }
    
    public override string UpdateDescription(bool showUpgrade = false)
    {
        string upgradePart = "";

        if (showUpgrade && TryGetNextUpgradeModifier(out float nextChance))
        {
            upgradePart = $" >> <color=#00FF00>{nextChance * 100}%</color>";
        }

        return description = $"{(burnChance):0}%{upgradePart} to inflict Burn.";
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
}*/
