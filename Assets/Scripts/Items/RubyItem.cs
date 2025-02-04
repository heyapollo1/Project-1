using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RubyItem : ItemData, ICombatEffect
{
    private StatModifier rubyDamageModifier;
    private StatModifier rubyMovementSpeedModifier;
    private float healthThreshold = 0.5f;
    private bool isBuffActive = false;

    public RubyItem(Sprite rubyIcon)
    {
        itemName = "Ruby";
        description = "Gain increased damage and movement speed when health is below 50%.";
        icon = rubyIcon;
        price = 50;
        itemType = ItemType.Combat;

        rubyDamageModifier = new StatModifier(StatType.Damage, flatBonus: 0f, percentBonus: 30f);
        rubyMovementSpeedModifier = new StatModifier(StatType.MovementSpeed, flatBonus: 0f, percentBonus: 30f);
    }

    public override void Apply(PlayerStatManager playerStats) { /* Initial Setup */ }
    public override void Remove(PlayerStatManager playerStats) { /* Cleanup */ }

    public void OnHealthChanged(float currentHealth, float maxHealth, PlayerStatManager playerStats)
    {
        if (currentHealth / maxHealth < healthThreshold && !isBuffActive)
        {
            Debug.Log("Applying Ruby buff...");
            playerStats.ApplyModifier(rubyDamageModifier);
            playerStats.ApplyModifier(rubyMovementSpeedModifier);
            isBuffActive = true;
        }
        else if (currentHealth / maxHealth >= healthThreshold && isBuffActive)
        {
            Debug.Log("Removing Ruby buff...");
            playerStats.RemoveModifier(rubyDamageModifier);
            playerStats.RemoveModifier(rubyMovementSpeedModifier);
            isBuffActive = false;
        }
    }

    public void OnEnemyKilled(GameObject enemy, PlayerStatManager playerStats) {}
    public void OnHit(GameObject target, PlayerStatManager playerStats) {}
    public float ModifyDamage(float damage, GameObject target) => damage;
}