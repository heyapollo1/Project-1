using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopazItem : ItemData, ICombatEffect
{
    private float stunChance = 20f;
    private float stunDuration = 2f;

    public TopazItem(Sprite topazIcon)
    {
        itemName = "Topaz";
        description = "Attacks have a 20% chance to Stun enemies.";
        icon = topazIcon;
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

    public void OnHit(GameObject target, PlayerStatManager playerStats)
    {
        EnemyHealthManager enemyHealth = target.GetComponent<EnemyHealthManager>();

        if (enemyHealth != null)
        {
            float roll = Random.Range(0f, 100f);
            if (roll <= stunChance)
            {
                StatusEffectManager.Instance.ApplyStatusEffect(target, () => new StunEffect(stunDuration, enemyHealth, PlayerCombat.Instance));
                Debug.Log($"{target.name} has been stunned by Topaz!");
            }
        }
    }

    public void OnHealthChanged(float currentHealth, float maxHealth, PlayerStatManager playerStats) { }

    public void OnEnemyKilled(GameObject enemy, PlayerStatManager playerStats) { }
}
