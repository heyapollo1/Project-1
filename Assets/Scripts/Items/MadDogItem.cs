using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MadDogItem : BaseItem
{
    private static readonly List<TagType> ClassTags = new()
    {
        TagType.Junk
    };

    private float bleedChance;
    
    public MadDogItem(Sprite madDogIcon, Rarity rarity) : base(
        "Mad Dog", madDogIcon, 50, ItemType.Inventory, rarity, 
        new List<StatModifier> { new (StatType.BleedChance, 10f)})
    {
        bleedChance = 10f;
        UpdateDescription();
    }
    
    public override string UpdateDescription()
    {
        return description = $"{(bleedChance)}% to inflict Bleed.";
    }
    
    public override void Apply()
    {
        base.Apply();
        
        EventManager.Instance.TriggerEvent("FXPoolUpdate", "BleedingFX", 10);
        Debug.Log("BleedingFX added to FX pool.");
    }
    
    public override void Remove()
    {
        base.Remove();
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