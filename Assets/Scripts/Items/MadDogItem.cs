using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MadDogItem : BaseItem
{
    private static readonly List<TagType> ClassTags = new()
    {
        TagType.Junk
    };
    
    private static readonly Dictionary<Rarity, List<StatModifier>> rarityModifiers = new()
    {
        { Rarity.Rare, new List<StatModifier> { new (StatType.BurnChance, 20f) } },
        { Rarity.Epic, new List<StatModifier> { new (StatType.BurnChance, 40f) } },
        { Rarity.Legendary, new List<StatModifier> { new (StatType.BurnChance, 60f) } }
    };
    
    public MadDogItem(Sprite madDogIcon, Rarity rarity) : base(
        "Mad Dog", madDogIcon, 50, ItemType.Simple, rarity, 
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
        return description = $"{(currentBonus):0}%{upgradePart} to inflict Bleed.";
    }
    
    public override void Apply(AttributeManager playerStats)
    {
        base.Apply(playerStats);
        
        EventManager.Instance.TriggerEvent("FXPoolUpdate", "BleedingFX", 10);
        Debug.Log("BleedingFX added to FX pool.");
    }
    
    public override void Remove(AttributeManager playerStats)
    {
        base.Remove(playerStats);
        FXManager.Instance.RemovePool("BleedingFX");
        Debug.Log("BleedingFX removed from FX pool.");
    }
}
/*public class MadDogItem : BaseItem
{
    private static readonly List<TagType> ClassTags = new()
    {
        TagType.Junk
    };
    
    private static readonly Dictionary<Rarity, float> GetBleedChance = new()
    {
        { Rarity.Rare, 0.1f },
        { Rarity.Epic, 0.15f },
        { Rarity.Legendary, 0.2f }
    };
    
    private float bleedChance;

    public MadDogItem(Sprite madDogIcon, Rarity rarity) 
        : base("Mad Dog", madDogIcon, 50, ItemType.Combat, rarity, null, ClassTags,
            new MadDogEffect(GetBleedChance[rarity]))
    {
        bleedChance = GetBleedChance[rarity];
        UpdateDescription();
    }
    
    public override void Upgrade()
    {
        if (currentRarity < Rarity.Legendary)
        {
            currentRarity++;
            value = Mathf.RoundToInt(value * GetRarityMultiplier()); 
            bleedChance = GetBleedChance[currentRarity];
            combatEffect = new MadDogEffect(bleedChance); 
            UpdateDescription(); 
        }
    }
    
    public bool TryGetNextUpgradeModifier(out float nextBleedChance)
    {
        if (currentRarity < Rarity.Legendary)
        {
            Rarity nextRarity = currentRarity + 1;
            if (GetBleedChance.TryGetValue(nextRarity, out nextBleedChance))
                return true;
        }

        nextBleedChance = 0f;
        return false;
    }
    
    public override string UpdateDescription(bool showUpgrade = false)
    {
        string upgradePart = "";

        if (showUpgrade && TryGetNextUpgradeModifier(out float nextChance))
        {
            upgradePart = $" >> <color=#00FF00>{nextChance * 100}%</color>";
        }
        
        return description = $"{(bleedChance * 100)}%{upgradePart} to inflict Bleed.";
    }
    
    public override void Apply(AttributeManager playerStats)
    {
        base.Apply(playerStats);
        
        EventManager.Instance.TriggerEvent("FXPoolUpdate", "BleedingFX", 10);
        Debug.Log("BleedingFX added to FX pool.");
    }
    
    public override void Remove(AttributeManager playerStats)
    {
        base.Remove(playerStats);
        FXManager.Instance.RemovePool("BleedingFX");
        Debug.Log("BleedingFX removed from FX pool.");
    }
}*/