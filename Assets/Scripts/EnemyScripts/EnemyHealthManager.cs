using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyHealthManager : MonoBehaviour, ILivingEntity
{
    [Header("General Settings")]
    public float maxHealth;
    public int xpReward = 50;
    public float staggerThresholdPercentage = 0.3f;

    [HideInInspector] public float currentHealth;
    [HideInInspector] public KnockbackManager knockbackManager;
    [HideInInspector] public VisualFeedbackManager feedbackManager;
    [HideInInspector] public EnemyAI enemy;

    private float staggerHealth;
    private bool isDead = false;
    private XPManager xpManager;
    //private Vector3 offsetPosition;
    public List<StatusEffect> activeEffects = new List<StatusEffect>();

    public EntityType GetEntityType() => EntityType.Enemy;
    public Transform GetEffectAnchor() => transform;
    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
    public float GetHealthPercentage() => currentHealth/maxHealth;
    public void ApplyStatusEffect(StatusEffect effect) => activeEffects.Add(effect);
    public void RemoveStatusEffect(StatusEffect effect) => activeEffects.Remove(effect);
    public List<StatusEffect> GetActiveEffects() => activeEffects;
    
    protected virtual void Awake()
    {
        enemy = GetComponent<EnemyAI>();
        xpManager = FindObjectOfType<XPManager>();
        feedbackManager = GetComponent<VisualFeedbackManager>();
        knockbackManager = GetComponent<KnockbackManager>();
        maxHealth = enemy.currentHealth;
        currentHealth = enemy.currentHealth;
        //offsetPosition = transform.position + new Vector3(0.1f, 0.8f, 0);
        Debug.Log("Enemy hp: " + currentHealth + "and max: " + maxHealth);
    }
    
    public virtual void TakeDamage(float damage,  Vector2 knockbackDirection, float knockbackForce, DamageSource sourceType = DamageSource.Player, bool isCriticalHit = false)
    {
        if (isDead) return;
        //float totalDamage = PlayerCombat.Instance.ApplyDamageModifiers(damage, gameObject);
        currentHealth -= damage;
        TriggerVisualFeedback();

        switch (sourceType)
        {
            case DamageSource.Player:
                staggerHealth += damage;
                //PlayerCombat.Instance.HandleOnHit(gameObject, totalDamage);
                ShowDamageNumber(damage, isCriticalHit ? Color.yellow : Color.white, isCriticalHit ? 15f : 12f);
                break;
            case DamageSource.StatusEffect:
                staggerHealth += damage;
                ShowDamageNumber(damage, isCriticalHit ? Color.red : Color.yellow, isCriticalHit ? 15f : 12f);
                break;
            case DamageSource.Execution:
                if (currentHealth > 0) currentHealth = 0;
                ShowDamageNumber(damage, Color.blue, 16f);
                DieWithKnockback(knockbackDirection, knockbackForce);
                return;
        }
        
        if (currentHealth <= 0)
        {
            isDead = true;
            DieWithKnockback(knockbackDirection, knockbackForce + 8);
            return;
        }
        
        if (ShouldTriggerStagger() && !isDead)
        {
            Debug.LogWarning("staggering enemy");
            if (!enemy.isPerformingAbility)  enemy.animator.SetTrigger("TakeDamage");

            knockbackManager.ApplyKnockback(knockbackDirection, (knockbackForce + 3), isPlayer: false);
            staggerHealth = 0f;
        }
        else if (knockbackForce > 0)
        {
            knockbackManager.ApplyKnockback(knockbackDirection, knockbackForce, isPlayer: false);
        }
    }
    
    public void ShowDamageNumber(float damage, Color color, float size = 12, Vector3? customPosition = null)
    {
        if (enemy.damageCanvas == null) return;
        damage = Mathf.Round(damage);
        Vector3 damagePosition = customPosition ?? transform.position + new Vector3(0, 1, 0);

        GameObject damageNumber = Instantiate(enemy.damageNumberPrefab, damagePosition, Quaternion.identity, enemy.damageCanvas.transform);

        DamageNumber damageNumberScript = damageNumber.GetComponent<DamageNumber>();
        if (damageNumberScript != null)
        {
            damageNumberScript.SetValue(damage);
            damageNumberScript.SetTextColor(color);
            damageNumberScript.SetTextSize(size);
        }
    }

    public void Heal(float amount)
    {
        float healableAmount = Mathf.Min(amount, maxHealth - currentHealth);

        currentHealth += healableAmount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        if (healableAmount > 0)
        {
            ShowDamageNumber(healableAmount, Color.green);
        }
    }

    public void InflictStun(float duration)
    {
        Debug.Log("Stun afflicted");
    }
    
    protected virtual void TriggerVisualFeedback()
    {
        if (feedbackManager != null)
        {
            feedbackManager.TriggerHitEffect(0.2f, 0f, Color.white);
        }
    }

    protected virtual bool ShouldTriggerStagger()
    {
        if (isDead) return false;
        float staggerThreshold = maxHealth * staggerThresholdPercentage;
        return staggerHealth >= staggerThreshold && currentHealth > 0;
    }

    protected virtual void DieWithKnockback(Vector2 knockbackDirection, float knockbackForce)
    {
        Debug.LogWarning("DEATH with knockback!");
        knockbackManager.ApplyKnockback(knockbackDirection, knockbackForce, isPlayer: false);
        Die();
    }

    protected virtual void Die()
    {
        if (enemy.isDead) return;
        Debug.Log("Trigger die");
        AudioManager.TriggerSound(clipPrefix: "Enemy_Death", position: transform.position);
        DeathEffectManager.Instance.TriggerDeathEffects(gameObject);
        GrantXP();
        enemy.TransitionToState(enemy.deathState);
    }

    private void GrantXP()
    {
        if (xpManager != null && !enemy.isDead)
        {
            Debug.Log("Enemy gave" + xpReward);
            xpManager.AddXP(xpReward);
        }
    }
    private void Delete()
    {
        if (enemy.isDead) return;
        enemy.deathType = DeathType.Delete;
        AudioManager.TriggerSound(clipPrefix: "Enemy_Death", position: transform.position);
        enemy.TransitionToState(enemy.deathState);
    }

    public StatusEffect GetStatusEffectByType(Type effectType)
    {
        return activeEffects.FirstOrDefault(effect => effect.GetType() == effectType);
    }

    // Check if any status effect is active by type
    public bool HasStatusEffect<T>() where T : StatusEffect
    {
        return activeEffects.Any(effect => effect is T );
    }

    public void RemoveAllStatusEffects()
    {
        // Iterate over a copy of the list to avoid modifying the list while iterating
        List<StatusEffect> effectsToRemove = new List<StatusEffect>(activeEffects);

        foreach (StatusEffect effect in effectsToRemove)
        {
            effect.RemoveEffect();
            activeEffects.Remove(effect);
            Debug.Log($"Removed all active effects.");
        }
    }

    public void RemoveAllFX()
    {
        foreach (Transform child in gameObject.transform)
        {
            if (child.CompareTag("FX"))
            {

                Destroy(child.gameObject);
            }
        }
        Debug.Log("All FX attached to enemy have been removed.");
    }
    
    public void ResetDeathState()
    {
        staggerHealth = 0;
        isDead = false;
    }
    
    /*public void DeathOnStageEnd(Transform knockbackPoint, bool hasKnockback = false)
   {
       if (hasKnockback)
       {
           Vector2 knockbackDirection = (enemy.transform.position - knockbackPoint.position).normalized;

           if (currentHealth > 0)
           {
               Debug.Log("Enemy deleted!");

               float distanceToPlayer = Vector3.Distance(transform.position, knockbackPoint.position);
               float knockbackForce = Mathf.Clamp(1f / distanceToPlayer * 50f, 10f, 100f);

               TakeDamage(currentHealth, knockbackDirection, knockbackForce, DamageSource.Execution);
           }
       }
       else
       {
           TakeDamage(currentHealth, Vector2.zero, 0f, DamageSource.Execution);
       }
   }*/
}
