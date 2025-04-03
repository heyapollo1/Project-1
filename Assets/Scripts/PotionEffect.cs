using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionEffect : IPassiveEffect
{
    private PlayerHealthManager player;
    private Coroutine potionCoroutine;
    private float healAmount;
    private float interval = 5f;
    
    public PotionEffect(float healAmount)
    {
        this.healAmount = healAmount;
    }
    
    public void Apply()
    {
        player = PlayerHealthManager.Instance;
        potionCoroutine = player.StartCoroutine(HealOverTime());
    }

    public void Remove()
    {
        if (potionCoroutine != null)
        {
            Debug.Log("Potion Effect Removed");
            player.StopCoroutine(potionCoroutine);
        }
    }

    private IEnumerator HealOverTime()
    {
        Debug.Log("PotionItem: Starting healing effect.");
        while (true)
        {
            yield return new WaitForSeconds(interval);
            if (player != null && player.currentHealth < player.maxHealth)
            {
                PlayerHealthManager.Instance.Heal(healAmount);
                Debug.Log($"Potion healed player for {healAmount} HP.");
            }
        }
    }
    
    public void IncreaseHealAmount(float additionalHealAmount)
    {
        healAmount += additionalHealAmount;
        Debug.Log($"New heal amount: {healAmount}");
    }
}