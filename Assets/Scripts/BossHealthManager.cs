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

    [Header("Cutscene Settings")]
    public Cutscene bossDefeatCutscene;

    protected override void Awake()
    {
        base.Awake();
        bossUIManager = FindObjectOfType<BossUIManager>();
    }

    protected override void Start()
    {
        base.Start();

        if (bossUIManager != null)
        {
            bossUIManager.Initialize(gameObject.name, maxHealth);
            bossUIManager.Show();
        }
    }

    public override void TakeDamage(float damage, Vector2 knockbackDirection, float knockbackForce, bool isCriticalHit = false, bool isFromStatusEffect = false)
    {
        base.TakeDamage(damage, knockbackDirection, knockbackForce, isCriticalHit, isFromStatusEffect);

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
        CutsceneManager.Instance.StartCutscene(bossDefeatCutscene, transform);
        bossUIManager.Hide();
    }
}