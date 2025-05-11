using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionItem : BaseItem
{
    private float healAmount;
    private PotionEffect potionEffect;
    
    private static readonly List<TagType> ClassTags = new()
    {
        TagType.Junk
    };
    
    public PotionItem(Sprite potionIcon, Rarity rarity) : base(
        "Potion", potionIcon, 50, ItemType.Inventory, rarity, null, ClassTags, null, new PotionEffect(10f))
    {
        healAmount = 10f;
        potionEffect = (PotionEffect)passiveEffect;
        UpdateDescription();
    }
    
    public override string UpdateDescription()
    {
        return description = $"Regenerates {(healAmount * 100):0}% HP every 5 seconds.";
    }
    
}

/*public class PotionItem : BaseItem
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
       
       public PotionItem(Sprite potionIcon, Rarity rarity) : base(
           "Potion", potionIcon, 50, ItemType.Inventory, rarity, null, ClassTags, null, new PotionEffect(GetHealAmount[rarity]))
       {
           healAmount = GetHealAmount[rarity];
           potionEffect = (PotionEffect)passiveEffect;
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
       
   }*/