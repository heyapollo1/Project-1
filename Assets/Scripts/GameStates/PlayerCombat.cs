using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerCombat : MonoBehaviour
{
    public static PlayerCombat Instance;

    private AttributeManager playerAttributes;
    private List<ICombatEffect> playerCombatEffects = new List<ICombatEffect>();

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
        EventManager.Instance.StopListening("PlayerStatsReady", InitializePlayerCombat);
        EventManager.Instance.StopListening("HealthChanged", OnPlayerHealthChanged);
    }

    private void InitializePlayerCombat()
    {
        playerAttributes = AttributeManager.Instance;
    }

    public void RegisterCombatEffect(ICombatEffect effect)
    {
        Debug.Log($"Combat effect: {effect} added.");
        if (!playerCombatEffects.Contains(effect))
        {
            playerCombatEffects.Add(effect);
        }
    }
    
    public void UnregisterCombatEffect(ICombatEffect effect)
    {
        if (playerCombatEffects.Contains(effect))
        {
            playerCombatEffects.Remove(effect);
        }
    }
    
    private void OnPlayerHealthChanged()
    {
        if (PlayerHealthManager.Instance == null) return;
        float currentHealth = PlayerHealthManager.Instance.currentHealth;
        float maxHealth = PlayerHealthManager.Instance.maxHealth;

        foreach (var effect in playerCombatEffects)
        {
            effect.OnHurt(currentHealth, maxHealth);
        }
    }

    public void HandleEnemyKilled(GameObject enemy)
    {
        foreach (var effect in playerCombatEffects)
        {
            effect.OnEnemyKilled(enemy, playerAttributes);
        }
    }

    public void HandleOnHit(CombatContext context)
    {
        foreach (var effect in playerCombatEffects)
        {
            Debug.Log($"Triggering on hit effect:{effect}");
            effect.OnHit(context);
        }
    }
    
    /*public void HandleModifiedStats(CombatContext context)
    {
        foreach (var effect in playerCombatEffects)
        {
            effect.ModifyStats(context);
        }
    }*/
    
    public List<StatModifier> GetModifiedStats(CombatContext context)
    {
        List<StatModifier> allModifiers = new();

        foreach (var effect in playerCombatEffects)
        {
            List<StatModifier> statModifiers = effect.ModifyStats(context);
            if (statModifiers != null && statModifiers.Count > 0)
                allModifiers.AddRange(statModifiers);
        }
        return allModifiers;
    }

    public void ResetPlayerCombatEffects()
    {
        playerCombatEffects.Clear();
        Debug.Log("All combat effects have been removed.");
    }
}