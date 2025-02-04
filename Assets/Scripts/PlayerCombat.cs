using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerCombat : MonoBehaviour
{
    public static PlayerCombat Instance;

    private PlayerStatManager playerStats;
    private List<ICombatEffect> combatEffects = new List<ICombatEffect>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);  // Ensure only one instance exists
        }
        DontDestroyOnLoad(gameObject);

        EventManager.Instance.StartListening("PlayerStatsReady", InitializePlayerCombat);
        EventManager.Instance.StartListening("HealthChanged", OnPlayerHealthChanged);
    }

    private void OnDestroy()
    {
        EventManager.Instance.StopListening("HealthChanged", OnPlayerHealthChanged);
        EventManager.Instance.StopListening("PlayerStatsReady", InitializePlayerCombat);
    }

    private void InitializePlayerCombat()
    {
        playerStats = PlayerStatManager.Instance;
    }

    public void AddCombatItem(ItemData item)
    {
        if (item is ICombatEffect combatItem)
        {
            combatEffects.Add(combatItem);
            Debug.Log($"Added combat item effect: {item.itemName}");

            combatItem.Apply(playerStats);
        }
    }

    private void OnPlayerHealthChanged()
    {
        if (PlayerHealthManager.Instance == null) return;
        float currentHealth = PlayerHealthManager.Instance.CurrentHealth;
        float maxHealth = PlayerHealthManager.Instance.MaxHealth;

        foreach (var effect in combatEffects)
        {
            effect.OnHealthChanged(currentHealth, maxHealth, playerStats);
        }
    }

    public void HandleEnemyKilled(GameObject enemy)
    {
        foreach (var effect in combatEffects)
        {
            effect.OnEnemyKilled(enemy, playerStats);
        }
    }

    public void HandleOnHit(GameObject target)
    {
        foreach (var effect in combatEffects)
        {
            Debug.Log($"triggering on hit effect on {target}");
            effect.OnHit(target, playerStats);
        }
    }

    public float ApplyDamageModifiers(float baseDamage, GameObject target)
    {
        float finalDamage = baseDamage;
        foreach (var effect in combatEffects)
        {
            finalDamage = effect.ModifyDamage(finalDamage, target);
        }

        return finalDamage;
    }

    public float ApplyOnHitModifiers(float baseDamage, GameObject target)
    {
        float finalDamage = baseDamage;
        foreach (var effect in combatEffects)
        {
            finalDamage = effect.ModifyOnHitDamage(finalDamage, target);
        }

        return finalDamage;
    }


    public void AddEffect(GameObject target, Func<StatusEffect> effect)
    {
        Debug.Log($"PlayerCombat: Applying {effect.GetType().Name} effect.");
        StatusEffectManager.Instance.ApplyStatusEffect(target, effect);
    }

    public void ResetPlayerCombatEffects()
    {
        combatEffects.Clear(); // Clear the list of abilities
        Debug.Log("All combat effects have been removed.");
    }
}