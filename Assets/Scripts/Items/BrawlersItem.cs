using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrawlersItem : ItemData, ICombatEffect
{
    float brawlerDamageIncrease = 30f;
    float range = 3.5f;

    public BrawlersItem(Sprite brawlersIcon)
    {
        itemName = "Brawlers";
        description = "Deal 30% increased damage to nearby enemies.";
        icon = brawlersIcon;
        price = 50;
        itemType = ItemType.Combat;

        player = GameObject.FindWithTag("Player");
        enemyDetector = player.GetComponent<EnemyDetector>();
    }

    public override void Apply(PlayerStatManager playerStats)
    {
    }

    public override void Remove(PlayerStatManager playerStats)
    {
    }

    public void OnHit(GameObject target, PlayerStatManager playerStats) { }

    public void OnHealthChanged(float currentHealth, float maxHealth, PlayerStatManager playerStats) { }

    public void OnEnemyKilled(GameObject enemy, PlayerStatManager playerStats) { }

    public float ModifyDamage(float damage, GameObject target)
    {
        enemyDetector.GetComponent<EnemyDetector>();

        if (target != null && enemyDetector != null)
        {
            Debug.Log("Enemy Detector: " + enemyDetector);
            if (enemyDetector.IsEnemyInRange(range, target))
            {
                float modifiedDamage = damage * (1 + (brawlerDamageIncrease / 100f));

                modifiedDamage = Mathf.Ceil(modifiedDamage);

                return modifiedDamage;
            }
        }
        return Mathf.Ceil(damage);
    }


}
