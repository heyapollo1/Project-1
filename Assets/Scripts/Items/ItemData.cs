using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Hand,
    Inventory,
    Both
}

[System.Serializable]
public class ItemData
{
    public string uniqueID;
    public string itemName;
    public string description;
    public int value;
    
    public ItemType itemType;
    public Rarity rarity;
    public TagType mainTag;

    public Sprite icon;
    public ItemUI assignedUI;
    public List<StatModifier> modifiers;
    public ICombatEffect combatEffect = null;
    public IPassiveEffect passiveEffect = null;
    public List<TagType> classTags = new List<TagType>();
}