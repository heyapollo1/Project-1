using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToothItem : ItemData, ICombatEffect
{
    float toothDamageIncrease = 50f;

    public ToothItem(Sprite toothIcon)
    {
        itemName = "Tooth";
        description = "Increase damage against Bleeding enemies by 50%.";
        icon = toothIcon;
        price = 50;
        itemType = ItemType.Combat;
    }

    public override void Apply(PlayerStatManager playerStats)
    {
    }

    public override void Remove(PlayerStatManager playerStats)
    {
    }

    public void OnHealthChanged(float currentHealth, float maxHealth, PlayerStatManager playerStats) { }
    public void OnEnemyKilled(GameObject enemy, PlayerStatManager playerStats) { }
    public void OnHit(GameObject target, PlayerStatManager playerStats) { }

    public float ModifyDamage(float damage, GameObject target)
    {
        EnemyHealthManager enemyHealth = target.GetComponent<EnemyHealthManager>();
        if (enemyHealth != null && enemyHealth.HasStatusEffect<BleedingEffect>()) // Dynamically check Bleeding
        {
            float modifiedDamage = damage * (1 + (toothDamageIncrease / 100f));

            // Round up the modified damage
            modifiedDamage = Mathf.Ceil(modifiedDamage);

            return modifiedDamage;
        }
        return Mathf.Ceil(damage);
    }
}