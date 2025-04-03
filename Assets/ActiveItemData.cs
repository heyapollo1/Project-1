using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActiveItemUpgrade
{
    public Rarity rarityTier; 
    public float additionalDamage = 0f;  
    public float cooldownReduction = 0f;  
    public float rangeIncrease = 0f;
}

[CreateAssetMenu(fileName = "ActiveItemData", menuName = "Items/Active Item Data")]
public class ActiveItemData : ScriptableObject
{
    public string itemName;
    public string description;
    public Sprite icon;
    public int baseValue;
    public Rarity defaultRarity;
    public List<TagType> tags = new();

    // Active behavior fields
    public float baseDamage;
    public float cooldown;
    public float knockback;
    public float range;
    public float criticalChance;
    public float criticalDamage;
    
    //public GameObject effectPrefab;
}