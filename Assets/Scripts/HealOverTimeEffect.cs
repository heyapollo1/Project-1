using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealOverTimeEffect : StatusEffect
{
    private float healAmount;
    private float interval;
    private PlayerHealthManager target;

    public HealOverTimeEffect(float healAmount, float interval, PlayerHealthManager target, MonoBehaviour owner, bool isEternal = false)
        : base(float.MaxValue, healAmount, owner, isEternal) // Using float.MaxValue for eternal effects
    {
        this.healAmount = healAmount;
        this.interval = interval;
        this.target = target;
    }

    protected override IEnumerator EffectCoroutine()
    {
        Debug.Log("PotionItem: Starting healing effect.");
        while (isActive)
        {
            yield return new WaitForSeconds(interval);

            // Heal the player
            if (target != null && target.CurrentHealth < target.MaxHealth)
            {
                target.Heal(healAmount);
                Debug.Log($"passively healed the player for {healAmount} HP.");
            }
        }
    }
}