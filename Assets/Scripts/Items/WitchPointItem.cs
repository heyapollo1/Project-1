using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WitchPointItem : ItemData, ICombatEffect
{
    private float executeThreshold = 0.15f; // 10% health threshold for instant kill

    public WitchPointItem(Sprite witchPointIcon)
    {
        itemName = "Witch Point";
        description = "Instantly kills enemies under 15% health.";
        icon = witchPointIcon;
        price = 50;
        itemType = ItemType.Combat;
    }

    public override void Apply(PlayerStatManager playerStats)
    {
        EventManager.Instance.TriggerEvent("FXPoolUpdate", "WitchPointFX", 10);
    }

    public override void Remove(PlayerStatManager playerStats)
    {
        FXManager.Instance.RemovePool("WitchPointFX");
    }

    public void OnHit(GameObject target, PlayerStatManager playerStats)
    {
        EnemyHealthManager enemyHealth = target.GetComponent<EnemyHealthManager>();
        if (enemyHealth != null)
        {
            float healthPercentage = enemyHealth.currentHealth / enemyHealth.maxHealth;
            if (healthPercentage <= executeThreshold)
            {
                FXManager.Instance.PlayFX("WitchPointFX", target.transform.position);
                enemyHealth.InstantKill(enemyHealth.currentHealth, Vector2.zero, 0f);
                Debug.Log($"WicthPoint executed enemy!");
            }
        }
    }

    public void OnHealthChanged(float currentHealth, float maxHealth, PlayerStatManager playerStats) {}
    public void OnEnemyKilled(GameObject enemy, PlayerStatManager playerStats) {}
    public float ModifyDamage(float damage, GameObject target) => damage;
}