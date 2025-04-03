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
    [SerializeReference] 
    public List<WeaponUniqueAttribute> uniqueAttributes = new();
}

[System.Serializable]
public abstract class WeaponUniqueAttribute
{
    public string effectName;
    
    [Header("Rarity Scaling")]
    public float commonValue;
    public float rareValue;
    public float epicValue;
    public float legendaryValue;
    
    public abstract void ApplyEffect(WeaponBase weapon, Rarity rarity);
    public float GetValueByRarity(Rarity rarity)
    {
        return rarity switch
        {
            Rarity.Common => commonValue,
            Rarity.Rare => rareValue,
            Rarity.Epic => epicValue,
            Rarity.Legendary => legendaryValue,
            _ => commonValue
        };
    }
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
}
