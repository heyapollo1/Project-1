using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Cinemachine;


public class BossHealthManager : EnemyHealthManager, IDamageable
{
    [Header("Boss Settings")]
    public BossUIManager bossUIManager;
    public float phaseThreshold = 0.5f;
    private bool hasPhaseChanged = false;
    private float staggerHealth;
    private bool isDead = false;
    
    [Header("Cutscene Settings")]
    public Cutscene bossDefeatCutscene;

    protected override void Awake()
    {
        base.Awake();
        bossUIManager = FindObjectOfType<BossUIManager>();
        if (bossUIManager != null)
        {
            bossUIManager.Initialize(gameObject.name, maxHealth);
            bossUIManager.Show();
        }
    }

    public override void TakeDamage(float damage, Vector2 knockbackDirection, float knockbackForce, DamageSource sourceType, bool isCriticalHit = false)
    {
        base.TakeDamage(damage, knockbackDirection, knockbackForce, sourceType, isCriticalHit);

        if (bossUIManager != null)
        {
            bossUIManager.UpdateHealth(currentHealth);
        }

        if (!hasPhaseChanged && currentHealth <= maxHealth * phaseThreshold)
        {
            TriggerPhaseChange();
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    protected override void TriggerVisualFeedback()
    {
        base.TriggerVisualFeedback();
        Debug.Log("trigger boss vfx");
    }
    
    protected override bool ShouldTriggerStagger()
    {
        if (isDead) return false;
        float staggerThreshold = maxHealth * staggerThresholdPercentage;
        return staggerHealth >= staggerThreshold && currentHealth > 0;
    }
    
    private void TriggerPhaseChange()
    {
        hasPhaseChanged = true;
        Debug.Log("Boss entering phase 2!");
        enemy.animator.SetTrigger("Phase2");
    }

    protected override void DieWithKnockback(Vector2 knockbackDirection, float knockbackForce)
    {
        knockbackManager.ApplyKnockback(knockbackDirection, knockbackForce, isPlayer: false);
        Die();
    }

    protected override void Die()
    {
        if (enemy.isDead) return;
        enemy.deathType = DeathType.Boss;
        AudioManager.Instance.PlayUISound("Boss_Death");
        enemy.TransitionToState(enemy.deathState);
        CutsceneManager.Instance.StartCutscene("BossDefeatCutscene", transform);
        bossUIManager.Hide();
    }
}