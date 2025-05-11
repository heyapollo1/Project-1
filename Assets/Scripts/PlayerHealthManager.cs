using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class PlayerHealthManager : MonoBehaviour, ILivingEntity
{
    public static PlayerHealthManager Instance { get; private set; }

    public List<StatusEffect> activeEffects = new List<StatusEffect>();

    public GameObject damageNumberPrefab;
    public Canvas statDisplayCanvas;
    private AttributeManager playerAttributes;
    private VisualFeedbackManager feedbackManager;
    private KnockbackManager knockbackManager;

    private bool isInvincible = false; 
    private SpriteRenderer spriteRenderer;

    [HideInInspector] public bool playerIsDead = false;
    
    public float maxHealth = 100f;
    public float currentHealth;

    public EntityType GetEntityType() => EntityType.Player;
    public Transform GetEffectAnchor() => transform;
    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
    public float GetHealthPercentage() => currentHealth/maxHealth;
    public void ApplyStatusEffect(StatusEffect effect) => activeEffects.Add(effect);
    public void RemoveStatusEffect(StatusEffect effect) => activeEffects.Remove(effect);
    public List<StatusEffect> GetActiveEffects() => activeEffects;

    public void InflictStun(float stunDuration)
    {
        Debug.Log("Inflicted stun");
    }
    
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        EventManager.Instance.StartListening("StatsChanged", UpdateHealthStats);
        EventManager.Instance.StartListening("SceneLoaded", OnSceneLoaded);
        EventManager.Instance.StartListening("SceneUnloaded", OnSceneUnloaded);
        EventManager.Instance.StartListening("PlayerRevived", RevivePlayer);
        
        damageNumberPrefab = Resources.Load<GameObject>("Prefabs/DamageNumbers");
        feedbackManager = GetComponent<VisualFeedbackManager>();
        knockbackManager = GetComponent<KnockbackManager>();
        playerAttributes = AttributeManager.Instance;
    }

    private void OnDestroy()
    {
        EventManager.Instance.StopListening("StatsChanged", UpdateHealthStats);
        EventManager.Instance.StopListening("SceneLoaded", OnSceneLoaded);
        EventManager.Instance.StopListening("SceneUnloaded", OnSceneUnloaded);
        EventManager.Instance.StopListening("PlayerRevived", RevivePlayer);
    }

    public void AssignStatDisplayCanvas(Canvas statDisplayCanvas)
    {
        this.statDisplayCanvas = statDisplayCanvas;
    }
    
    private void OnSceneLoaded(string scene)
    {
        if (scene == "GameScene")
        {
            GameData gameData = SaveManager.LoadGame();
            if (gameData.isNewGame) 
            {
                maxHealth = playerAttributes.baseMaxHealth;
                currentHealth = playerAttributes.currentMaxHealth;
                EventManager.Instance.TriggerEvent("HealthChanged");
                UpdateHealthStats();
            }
        }
    }

    private void OnSceneUnloaded(string scene)
    {
        if (scene == "GameScene")
        {
            playerIsDead = false;
        }
    }
    
    public void TakeDamage(float damage,  Vector2 knockbackDirection, float knockbackForce, DamageSource sourceType = DamageSource.Enemy, bool isCriticalHit = false)
    {
        if (playerIsDead || isInvincible) return;
        if (playerAttributes.ShouldTrigger(playerAttributes.currentDodgeChance)) 
        {
            Debug.Log("Attack dodged!");
            return;
        }
        float totalDamage = playerAttributes.ArmourMitigation(damage, playerAttributes.currentArmour);
        currentHealth -= totalDamage;
        TriggerVisualFeedback();
        EventManager.Instance.TriggerEvent("HealthChanged", currentHealth, maxHealth);

        switch (sourceType)
        {
            case DamageSource.Enemy:
                ShowDamageNumber(totalDamage, isCriticalHit ? Color.yellow : Color.white, isCriticalHit ? 15f : 12f);
                break;
            case DamageSource.StatusEffect:
                ShowDamageNumber(totalDamage, isCriticalHit ? Color.red : Color.yellow, isCriticalHit ? 15f : 12f);
                break;
            case DamageSource.Execution:
                if (currentHealth > 0) currentHealth = 0;
                ShowDamageNumber(totalDamage, Color.blue, 16f);
                DieWithKnockback(knockbackDirection, knockbackForce);
                return;
        }
        
        if (currentHealth <= 0)
        {
            playerIsDead = true;
            DieWithKnockback(knockbackDirection, knockbackForce + 8);
            return;
        }
        
        if (!playerIsDead && knockbackForce > 0)
        {
            knockbackManager.ApplyKnockback(knockbackDirection, knockbackForce, isPlayer: true);
        }
        CameraShakeManager.Instance.ShakeCamera(1f, 4f, 0.25f);
    }
    
    public void ShowDamageNumber(float damage, Color color, float size = 12, Vector3? customPosition = null)
    {
        if (statDisplayCanvas == null) return;
        damage = Mathf.Round(damage);
        Vector3 damagePosition = transform.position + new Vector3(0, 1, 0);

        GameObject damageNumber = Instantiate(damageNumberPrefab, damagePosition, Quaternion.identity, statDisplayCanvas.transform);

        DamageNumber damageNumberScript = damageNumber.GetComponent<DamageNumber>();
        if (damageNumberScript != null)
        {
            damageNumberScript.SetValue(damage);
            damageNumberScript.SetTextColor(color);
            damageNumberScript.SetTextSize(size);
        }
    }
    
    private void TriggerVisualFeedback()
    {
        if (feedbackManager != null)
        {
            feedbackManager.TriggerHitEffect(0.2f, 0f, Color.white);
        }
    }
    
    public void Heal(float amount)
    {
        if (playerIsDead) return;
        float healableAmount = Mathf.Min(amount, maxHealth - currentHealth);
        currentHealth += healableAmount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        EventManager.Instance.TriggerEvent("HealthChanged", currentHealth, maxHealth);
        if (healableAmount > 0)
        {
            ShowDamageNumber(healableAmount, Color.green, 12f);
        }
    }

    private void UpdateHealthStats()
    {
        float previousMaxHealth = maxHealth;

        maxHealth = playerAttributes.GetStatValue(StatType.MaxHealth);

        currentHealth = Mathf.Round(currentHealth * (maxHealth / previousMaxHealth));

        EventManager.Instance.TriggerEvent("HealthChanged", currentHealth, maxHealth);
    }

    private void DieWithKnockback(Vector2 knockbackDirection, float knockbackForce)
    {
        knockbackManager.ApplyKnockback(knockbackDirection, knockbackForce, isPlayer: true);
        Die();
    }

    private void Die()
    {
        if (playerIsDead) return;
        CameraShakeManager.Instance.ShakeCamera(2f, 6f, 0.25f);
        AudioManager.TriggerSound(clipPrefix: "Player_Death", position: transform.position);
        EventManager.Instance.TriggerEvent("PlayerDied");
        playerIsDead = true;

        Debug.Log("Player died.");
    }

    public void RevivePlayer() //For play testing only
    {
        if (!playerIsDead) return;

        playerIsDead = false;
        currentHealth = maxHealth;
        UpdateHealthStats();
        Debug.Log("Player revived!");
        EventManager.Instance.TriggerEvent("PlayerRevived");
        StartInvincibility(2f);
    }

    public void AdjustHealthByPercentage(float percentage)//FOr TESTING
    {
        if (playerIsDead) return;
        float adjustment = maxHealth * percentage;
        if (percentage > 0)
        {
            Heal(adjustment);
        }
        else
        {
            TakeDamage(-adjustment, Vector2.zero, 0);
        }
    }

    public void StartInvincibility(float duration)
    {
        if (!isInvincible)
        {
            isInvincible = true;
            StartCoroutine(InvincibilityTimer(duration));
        }
    }

    private IEnumerator InvincibilityTimer(float duration)
    {
        yield return new WaitForSeconds(duration); // Wait for invincibility duration
        isInvincible = false;
    }

    public StatusEffect GetStatusEffectByType(Type effectType)
    {
        return activeEffects.FirstOrDefault(effect => effect.GetType() == effectType);
    }
    
    public bool HasStatusEffect<T>() where T : StatusEffect
    {
        return activeEffects.Any(effect => effect is T);
    }
    
    public bool HasAnyStatusEffect()
    {
        return activeEffects.Count > 0;
    }
    
    public void LoadHealthFromSave(PlayerState playerState)
    {
        maxHealth = playerAttributes.baseMaxHealth;
        currentHealth = playerState.health;
        Debug.Log($"Loading player health...{currentHealth} and max health {maxHealth}");
        //EventManager.Instance.TriggerEvent("HealthChanged", currentHealth, maxHealth);
    }
}