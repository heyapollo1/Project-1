using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponUpgradeEffect
{
    public Rarity rarityTier;
    public float additionalDamage = 0f;
    public float cooldownReduction = 0f;
    public float rangeIncrease = 0f;
}

[CreateAssetMenu(fileName = "WeaponData", menuName = "Weapons/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public string weaponTitle;
    public string weaponDescription;

    public float baseDamage;
    public float baseCooldownRate;
    public float baseRange;
    public float baseKnockbackForce;
    public float baseCriticalHitChance;
    public float baseRangeIncrease;
    public float baseCriticalHitDamage;
    public int price;

    public GameObject weaponPrefab;
    public Sprite[] weaponSprites;
    public Sprite weaponIcon;
    public LayerMask enemyLayer;
    
    public Rarity rarity;
    public List<WeaponUpgradeEffect> rarityUpgrades = new List<WeaponUpgradeEffect>();
    public List<TagType> associatedTags = new List<TagType>();
}

/*[System.Serializable]
public class WeaponUpgradeEffect
{
    public Rarity rarityTier; 
    public float additionalDamage = 0f;  
    public float cooldownReduction = 0f;  
    public float rangeIncrease = 0f;
}

[CreateAssetMenu(fileName = "WeaponData", menuName = "Weapons/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public string weaponTitle;
    public string weaponDescription;

    public float baseDamage;
    public float baseCooldownRate;
    public float baseRange;
    public float baseKnockbackForce;
    public float baseCriticalHitChance;
    public float baseRangeIncrease;
    public float baseCriticalHitDamage;
    public int price;
    
    public GameObject weaponPrefab;
    public Sprite[] weaponSprites;
    public Sprite weaponIcon;
    public LayerMask enemyLayer;
    
    public Rarity defaultRarity; 
    public List<WeaponUpgradeEffect> rarityUpgrades = new List<WeaponUpgradeEffect>(); 
    public List<TagType> associatedTags = new List<TagType>();
}*/
