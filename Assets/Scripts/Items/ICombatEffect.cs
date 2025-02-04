using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICombatEffect
{
    void OnHealthChanged(float currentHealth, float maxHealth, PlayerStatManager playerStats);
    void OnEnemyKilled(GameObject enemy, PlayerStatManager playerStats);
    void OnHit(GameObject target, PlayerStatManager playerStats);
    void Apply(PlayerStatManager playerStats);
    float ModifyDamage(float damage, GameObject target) => damage;
    float ModifyOnHitDamage(float damage, GameObject target) => damage;
}