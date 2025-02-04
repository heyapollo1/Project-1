using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionItem : ItemData
{
    private float healAmount = 10f;
    private float healInterval = 5f;
    private bool isHealing = false;
    private HealOverTimeEffect activeEffect;

    public PotionItem(Sprite potionIcon)
    {
        itemName = "Potion";
        description = "Heals the player for a small amount every 5 seconds.";
        icon = potionIcon;
        price = 50;
        itemType = ItemType.Simple;
    }

    public override void Apply(PlayerStatManager playerStats)
    {
        PlayerHealthManager healthManager = PlayerHealthManager.Instance;
        if (!isHealing && healthManager != null)
        {
            Debug.Log("PotionItem: Begin");
            isHealing = true;

            activeEffect = new HealOverTimeEffect(healAmount, healInterval, healthManager, PlayerCombat.Instance, isEternal: true);

            StatusEffectManager.Instance.ApplyStatusEffect(healthManager.gameObject, () => activeEffect, true);
        }
        else
        {
            Debug.Log("PotionItem: Healing effect not started. Either already healing or healthManager is null.");
        }
    }

    public override void Remove(PlayerStatManager playerStats)
    {
        PlayerHealthManager healthManager = PlayerHealthManager.Instance;

        if (isHealing && activeEffect != null && healthManager != null)
        {
            Debug.Log("PotionItem: Stopping healing effect.");

            healthManager.RemoveStatusEffect(activeEffect);

            isHealing = false;
        }
    }
}