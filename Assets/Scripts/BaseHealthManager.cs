using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseHealthManager : MonoBehaviour
{
    protected float maxHealth = 100f;
    protected float currentHealth;
    private VisualFeedbackManager feedbackManager;
    protected KnockbackManager knockbackManager;

    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;

    protected virtual void Awake()
    {
        feedbackManager = GetComponent<VisualFeedbackManager>();
        knockbackManager = GetComponent<KnockbackManager>();
    }

    protected virtual void Start() { }

    // Take damage and apply knockback if necessary (can be overridden)
    protected virtual void TakeDamage(float damage)
    {
        if (currentHealth <= 0)
        {
            return;
        }

        currentHealth -= damage;
        TriggerVisualFeedback();
    }

    protected virtual void TriggerVisualFeedback()
    {
        if (feedbackManager != null)
        {
            feedbackManager.TriggerHitEffect(0.2f, 0f, Color.white);
        }
    }

    protected abstract void Die(); // Override for specific death handling

    protected abstract void CheckHealthThreshold();
}
