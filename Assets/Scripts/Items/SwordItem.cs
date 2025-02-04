using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordItem : ItemData, ICombatEffect
{
    private float damageOnHit = 5f;

    public SwordItem(Sprite swordIcon)
    {
        itemName = "Sword";
        description = "Deal 5 bonus damage On-Hit.";
        icon = swordIcon;
        price = 50;
        itemType = ItemType.Combat;
    }

    public override void Apply(PlayerStatManager playerStats)
    {
        // No need to apply immediate stat modifiers, handled via OnHit
    }

    public override void Remove(PlayerStatManager playerStats)
    {
        // Clean up if needed
    }

    public void OnHit(GameObject target, PlayerStatManager playerStats) { }

    public void OnHealthChanged(float currentHealth, float maxHealth, PlayerStatManager playerStats) { }

    public void OnEnemyKilled(GameObject enemy, PlayerStatManager playerStats) { }

    public float ModifyOnHitDamage(float damage, GameObject target)
    {
        return damage + damageOnHit;
    }
}