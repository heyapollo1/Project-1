using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*public class BurnerWeaponEffect : ICombatEffect
{
    private readonly float burnChanceMultiplier;

    public BurnerWeaponEffect(float multiplier)
    {
        burnChanceMultiplier = multiplier;
    }

    public void OnHit(CombatContext context){}
    public void OnHurt(float currentHealth, float maxHealth) {}
    public void OnEnemyKilled(GameObject enemy, AttributeManager playerStats) {}
    public List<StatModifier> ModifyStats(CombatContext context)
    {
        var modifiers = new List<StatModifier>();
        if (context.source.TryGetComponent(out BurnerProjectile burnerProjectile) && burnerProjectile != null)
        {
            Debug.Log($"Burner projectile: {burnerProjectile.name} adding {burnChanceMultiplier}% burn chance to collision");
            modifiers.Add(new StatModifier(StatType.BurnChance, 0f, burnChanceMultiplier));
        }
        return modifiers;
    }
}*/

/*public class BurnerWeaponEffect : WeaponUniqueAttribute
{
    private readonly float burnChanceMultiplier;
    
    public string effectName;
    
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
    public BurnerWeaponEffect(float multiplier)
    {
        burnChanceMultiplier = multiplier;
    }

    public void ApplyEffect(WeaponBase weapon)
    {
        weapon.
    }
}*/
