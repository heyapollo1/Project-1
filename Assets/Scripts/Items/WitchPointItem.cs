using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WitchPointItem : BaseItem
{
    private static readonly List<TagType> ClassTags = new()
    {
        TagType.Junk
    };
    
    private float executeThreshold;

    public WitchPointItem(Sprite witchPointIcon, Rarity rarity) 
        : base("Witch Point", witchPointIcon, 50, ItemType.Inventory, rarity, null, ClassTags,
            new WitchPointEffect(0.20f))
    {
        executeThreshold = 0.20f;
        UpdateDescription();
    }
    
    public override string UpdateDescription()
    {
        return description = $"Instantly kills enemies under {executeThreshold}% HP.";
    }
    
    public override void Apply()
    {
        base.Apply();

        EventManager.Instance.TriggerEvent("FXPoolUpdate", "WitchPointFX", 10);
        Debug.Log("WitchPointFX added to FX pool.");
    }
    
    public override void Remove()
    {
        base.Remove();
        FXManager.Instance.RemovePool("WitchPointFX");
        Debug.Log("WitchPointFX removed from FX pool.");
    }
}

/*public class WitchPointItem : BaseItem
   {
       private static readonly List<TagType> ClassTags = new()
       {
           TagType.Junk
       };
       
       private static readonly Dictionary<Rarity, float> GetExecuteThreshold = new()
       {
           { Rarity.Rare, 0.15f },
           { Rarity.Epic, 0.20f },
           { Rarity.Legendary, 0.25f }
       };
       
       private float executeThreshold;
   
       public WitchPointItem(Sprite witchPointIcon, Rarity rarity) 
           : base("Witch Point", witchPointIcon, 50, ItemType.Inventory, rarity, null, ClassTags,
               new WitchPointEffect(GetExecuteThreshold[rarity]))
       {
           executeThreshold = GetExecuteThreshold[rarity];
           UpdateDescription();
       }
       
       public override void Upgrade()
       {
           if (currentRarity < Rarity.Legendary)
           {
               currentRarity++;
               value = Mathf.RoundToInt(value * GetRarityMultiplier()); 
               executeThreshold = GetExecuteThreshold[currentRarity];
               combatEffect = new WitchPointEffect(executeThreshold); 
               UpdateDescription(); 
           }
       }
       
       public bool TryGetNextUpgradeModifier(out float nextExecuteThreshold)
       {
           if (currentRarity < Rarity.Legendary)
           {
               Rarity nextRarity = currentRarity + 1;
               if (GetExecuteThreshold.TryGetValue(nextRarity, out nextExecuteThreshold))
                   return true;
           }
   
           nextExecuteThreshold = 0f;
           return false;
       }
       
       public override string UpdateDescription(bool showUpgrade = false)
       {
           string upgradePart = "";
   
           if (showUpgrade && TryGetNextUpgradeModifier(out float nextChance))
           {
               upgradePart = $" >> <color=#00FF00>{nextChance * 100}%</color>";
           }
           return description = $"Instantly kills enemies under {(executeThreshold * 100):0}%{upgradePart} HP.";
       }
       
       public override void Apply(AttributeManager playerStats)
       {
           base.Apply(playerStats);
   
           EventManager.Instance.TriggerEvent("FXPoolUpdate", "WitchPointFX", 10);
           Debug.Log("WitchPointFX added to FX pool.");
       }
       
       public override void Remove(AttributeManager playerStats)
       {
           base.Remove(playerStats);
           FXManager.Instance.RemovePool("WitchPointFX");
           Debug.Log("WitchPointFX removed from FX pool.");
       }
   }*/