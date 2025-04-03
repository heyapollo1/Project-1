using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseItem : ItemData
{
    public BaseItem(string name, Sprite icon, int value, ItemType itemType, Rarity currentRarity, 
        List<StatModifier> modifiers = null, List<TagType> classTags = null, ICombatEffect combatEffect = null, IPassiveEffect passiveEffect = null)
    {
        itemName = name;
        this.icon = icon;
        this.itemType = itemType;
        this.currentRarity = currentRarity;
        this.combatEffect = combatEffect;
        this.passiveEffect = passiveEffect;
        this.modifiers = modifiers ?? new List<StatModifier>();
        this.classTags = classTags ?? new List<TagType>();
        this.value = Mathf.RoundToInt(value * GetRarityMultiplier());
        defaultRarity = ItemFactory.GetDefaultRarity(itemName);
    }
    
    public virtual void Apply(AttributeManager playerStats)
    {
        if (modifiers != null)
        {
            foreach (StatModifier modifier in modifiers)
            {
                playerStats.ApplyModifier(modifier);
            }
        }
        if (combatEffect != null)
        {
            Debug.Log($"Combat effect: {combatEffect} added.");
            PlayerCombat.Instance.RegisterCombatEffect(combatEffect);
        }
        if (passiveEffect != null)
        {
            Debug.Log($"Passive effect: {passiveEffect} added.");
            passiveEffect?.Apply();
        }
        Debug.Log($"{itemName} applied. Rarity: {currentRarity}.");
    }
    
    public virtual void Remove(AttributeManager playerStats)
    {
        if (modifiers != null)
        {
            foreach (StatModifier modifier in modifiers)
            {
                playerStats.RemoveModifier(modifier);
            }
        }
        if (combatEffect != null)
        {
            PlayerCombat.Instance.UnregisterCombatEffect(combatEffect);
        }
        if (passiveEffect != null)
        {
            Debug.Log($"Passive effect: {passiveEffect} added.");
            passiveEffect?.Remove();
        }
        Debug.Log($"{itemName} removed. Rarity: {currentRarity}.");
    }
    
    public virtual void Upgrade()
    {
        if (currentRarity < Rarity.Legendary)
        {
            currentRarity++;
        }
    }
    
    public float GetRarityMultiplier()
    {
        return currentRarity switch
        {
            Rarity.Common => 1.0f,
            Rarity.Rare => 1.5f,
            Rarity.Epic => 2.0f,
            Rarity.Legendary => 3.0f,
            _ => 1.0f
        };
    }
    
    public virtual string UpdateDescription(bool showUpgrade = false)
    {
        return description;
    }
    
}
