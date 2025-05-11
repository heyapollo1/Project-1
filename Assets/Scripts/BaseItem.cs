using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseItem : ItemData
{
    public BaseItem(string name, Sprite icon, int value, ItemType itemType, Rarity rarity, 
        List<StatModifier> modifiers = null, List<TagType> classTags = null, ICombatEffect combatEffect = null, IPassiveEffect passiveEffect = null)
    {
        itemName = name;
        uniqueID = $"{name}_{Guid.NewGuid()}";
        this.icon = icon;
        this.itemType = itemType;
        this.rarity = rarity;
        this.combatEffect = combatEffect;
        this.passiveEffect = passiveEffect;
        this.modifiers = modifiers ?? new List<StatModifier>();
        this.classTags = classTags ?? new List<TagType>();
        this.value = Mathf.RoundToInt(value * GetRarityMultiplier());
        
        Debug.Log($"[BaseItem Created] {itemName} ({rarity}) [ID: {uniqueID}]");
    }
    
    public virtual void Apply()
    {
        if (modifiers != null)
        {
            foreach (StatModifier modifier in modifiers)
            {
                AttributeManager.Instance.ApplyModifier(modifier);
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
        Debug.Log($"{itemName} applied. Rarity: {rarity}.");
    }
    
    public virtual void Remove()
    {
        if (modifiers != null)
        {
            foreach (StatModifier modifier in modifiers)
            {
                AttributeManager.Instance.RemoveModifier(modifier);
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
        Debug.Log($"{itemName} removed. Rarity: {rarity}.");
    }
    
    /*public virtual void Upgrade()
    {
        if (rarity < Rarity.Legendary)
        {
            rarity++;
        }
    }*/
    
    public void AssignUI(ItemUI ui)
    {
        assignedUI = ui;
    }
    
    public float GetRarityMultiplier()
    {
        return rarity switch
        {
            Rarity.Common => 1.0f,
            Rarity.Rare => 1.5f,
            Rarity.Epic => 2.0f,
            Rarity.Legendary => 3.0f,
            _ => 1.0f
        };
    }
    
    public virtual string UpdateDescription()
    {
        return description;
    }
    
}
