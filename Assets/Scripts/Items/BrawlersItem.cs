using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*public class BrawlersItem : ItemData, ICombatEffect
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

    public override void Apply(AttributeManager playerStats)
    {
    }

    public override void Remove(AttributeManager playerStats)
    {
    }

    public void OnHit(GameObject target, AttributeManager playerStats) { }

    public void OnHealthChanged(float currentHealth, float maxHealth, AttributeManager playerStats) { }

    public void OnEnemyKilled(GameObject enemy, AttributeManager playerStats) { }

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


}*/
