using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WitchPointEffect : ICombatEffect
{
    private readonly float executeThreshold;

    public WitchPointEffect(float threshold)
    {
        executeThreshold = threshold;
    }
    
    public void OnHit(CombatContext context)
    {
        EnemyHealthManager enemyHealth = context.target.GetComponent<EnemyHealthManager>();
        if (enemyHealth != null)
        {
            if (enemyHealth.GetHealthPercentage() <= executeThreshold)
            {
                FXManager.Instance.PlayFX("WitchPointFX", enemyHealth.transform.position);
                enemyHealth.TakeDamage(enemyHealth.currentHealth, Vector2.zero, 0f,  DamageSource.Execution);
                Debug.Log($"WitchPoint executed enemy at {executeThreshold * 100}% HP!");
            }
        }
    }
    public void OnHurt(float currentHealth, float maxHealth) {}
    public void OnEnemyKilled(GameObject enemy, AttributeManager playerStats) {}
    //public float ModifyStats(CombatContext context){}
}
