using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Simple, Combat 
}

[System.Serializable]
public class ItemData
{
    public string itemName;
    public string description;
    public int value;

    public ItemType itemType;
    public Rarity defaultRarity;
    public Rarity currentRarity;
    public TagType equipmentTag = TagType.Equipment;

    public Sprite icon;
    public List<StatModifier> modifiers;
    public ICombatEffect combatEffect = null;
    public IPassiveEffect passiveEffect = null;
    public List<TagType> classTags = new List<TagType>();
}