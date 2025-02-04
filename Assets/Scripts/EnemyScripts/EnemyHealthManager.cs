using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyHealthManager : MonoBehaviour, IDamageable
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
    private XPManager xpManager;
    private Vector3 offsetPosition;
    public List<StatusEffect> activeEffects = new List<StatusEffect>();

    protected virtual void Awake()
    {
        enemy = GetComponent<EnemyAI>();
        xpManager = FindObjectOfType<XPManager>();
        feedbackManager = GetComponent<VisualFeedbackManager>();
        knockbackManager = GetComponent<KnockbackManager>();
    }

    protected virtual void Start()
    {
        maxHealth = enemy.currentHealth;
        currentHealth = enemy.currentHealth;
        Debug.Log("Enemy hp: " + currentHealth + "and max: " + maxHealth);
        offsetPosition = transform.position + new Vector3(0.1f, 0.8f, 0);
    }

    public virtual void TakeDamage(float damage, Vector2 knockbackDirection, float knockbackForce, bool isCriticalHit = false, bool isFromStatusEffect = false)
    {
        if (enemy.isDisabled) return;

        float modifiedDamage = PlayerCombat.Instance.ApplyDamageModifiers(damage, gameObject);
        float onHitDamage = PlayerCombat.Instance.ApplyOnHitModifiers(0f, gameObject);
        float totalDamage = modifiedDamage + onHitDamage;
        currentHealth -= totalDamage;
        staggerHealth += totalDamage;
        TriggerVisualFeedback();

        if (!isFromStatusEffect)
        {
            PlayerCombat.Instance.HandleOnHit(gameObject);
            ShowDamageNumber(modifiedDamage, isCriticalHit ? Color.yellow : Color.white, isCriticalHit ? 14f : 12f);
            if (onHitDamage != 0)
            {
                ShowDamageNumber(onHitDamage, Color.yellow, 10f, offsetPosition);
            }

            if (currentHealth <= 0)
            {
                DieWithKnockback(knockbackDirection, knockbackForce + 8);
                return;
            }

            if (ShouldTriggerStagger())
            {
                if (!enemy.isPerformingAbility)
                {
                    enemy.animator.SetTrigger("TakeDamage");
                }
                knockbackManager.ApplyKnockback(knockbackDirection, (knockbackForce + 3), isPlayer: false);
                staggerHealth = 0f;
            }
            else if (knockbackForce > 0)
            {
                knockbackManager.ApplyKnockback(knockbackDirection, knockbackForce, isPlayer: false);
            }
        }
        else if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Execute(float damage, Vector2 knockbackDirection, float knockbackForce, bool giveRewards)
    {
        float modifiedDamage = PlayerCombat.Instance.ApplyDamageModifiers(damage, gameObject);
        float onHitDamage = PlayerCombat.Instance.ApplyOnHitModifiers(0f, gameObject);
        float totalDamage = modifiedDamage + onHitDamage;
        currentHealth -= totalDamage;
        TriggerVisualFeedback();

        PlayerCombat.Instance.HandleOnHit(gameObject);
        if (currentHealth > 0)
        {
            currentHealth = 0;
        }

        ShowDamageNumber(modifiedDamage, Color.red, 16f);
        if (onHitDamage != 0)
        {
            ShowDamageNumber(onHitDamage, Color.yellow, 10f, offsetPosition);
        }

        if (giveRewards)
        {
            ShowDamageNumber(modifiedDamage, Color.red, 16f);
            if (onHitDamage != 0)
            {
                ShowDamageNumber(onHitDamage, Color.yellow, 10f, offsetPosition);
            }

            DieWithKnockback(knockbackDirection, knockbackForce);
        }
        else
        {
            knockbackManager.ApplyKnockback(knockbackDirection, knockbackForce, isPlayer: false);
            Delete();
        }
    }

    protected virtual void TriggerVisualFeedback()
    {
        if (feedbackManager != null)
        {
            feedbackManager.TriggerHitEffect(0.2f, 0f, Color.white);
        }
    }

    private bool ShouldTriggerStagger()
    {
        float staggerThreshold = maxHealth * staggerThresholdPercentage;
        return staggerHealth >= staggerThreshold && currentHealth > 0;
    }

    public void InstantKill(float currentHealth, Vector2 knockbackDirection, float knockbackForce)
    {
        if (currentHealth > 0)
        {
            Debug.Log("Enemy executed!");
            Execute(currentHealth, knockbackDirection, knockbackForce, true);
        }
    }

    public void DeathOnStageEnd(Transform knockbackPoint, bool hasKnockback = false)
    {
        if (hasKnockback)
        {
            Vector2 knockbackDirection = (enemy.transform.position - knockbackPoint.position).normalized;

            if (currentHealth > 0)
            {
                Debug.Log("Enemy deleted!");

                float distanceToPlayer = Vector3.Distance(transform.position, knockbackPoint.position);
                float knockbackForce = Mathf.Clamp(1f / distanceToPlayer * 50f, 10f, 100f);

                Execute(currentHealth, knockbackDirection, knockbackForce, false);
            }
        }
        else
        {
            Execute(currentHealth, Vector2.zero, 0f, false);
        }
    }

    protected virtual void DieWithKnockback(Vector2 knockbackDirection, float knockbackForce)
    {
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

    public void RemoveStatusEffect(StatusEffect effect)
    {
        if (activeEffects.Contains(effect))
        {
            activeEffects.Remove(effect);
        }
    }

    public void RemoveAllStatusEffects()
    {
        // Iterate over a copy of the list to avoid modifying the list while iterating
        List<StatusEffect> effectsToRemove = new List<StatusEffect>(activeEffects);

        foreach (StatusEffect effect in effectsToRemove)
        {
            effect.Remove(gameObject, false);
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

    public void ShowDamageNumber(float damage, Color color, float size = 12, Vector3? customPosition = null)
    {
        if (enemy.damageCanvas == null) return;

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

    private void GrantXP()
    {
        if (xpManager != null && !enemy.isDead)
        {
            Debug.Log("Enemy gave" + xpReward);
            xpManager.AddXP(xpReward);
        }
    }
}
