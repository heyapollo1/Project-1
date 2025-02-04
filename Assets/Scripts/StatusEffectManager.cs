using System;
using UnityEngine;

public class StatusEffectManager : BaseManager
{
    public static StatusEffectManager Instance { get; private set; }
    public override int Priority => 30;

    protected override void OnInitialize()
    {

    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Add and apply a new effect
    public void ApplyStatusEffect(GameObject target, Func<StatusEffect> createEffect, bool isPlayer = false)
    {
        if (isPlayer)
        {
            PlayerHealthManager playerHealth = target.GetComponent<PlayerHealthManager>();
            if (playerHealth != null && playerHealth.CurrentHealth > 0)
            {
                StatusEffect newEffect = createEffect.Invoke();
                Type effectType = newEffect.GetType();

                StatusEffect existingEffect = playerHealth.GetStatusEffectByType(effectType);

                if (existingEffect != null)
                {
                    existingEffect.ResetDuration(newEffect.Duration, newEffect.Damage);
                    Debug.Log($"Refreshed duration of {effectType.Name} on Player");
                }
                else
                {
                    playerHealth.activeEffects.Add(newEffect);
                    newEffect.Apply();
                }
            }
        }
        else
        {
            EnemyHealthManager enemyHealth = target.GetComponent<EnemyHealthManager>();
            if (enemyHealth != null && enemyHealth.currentHealth > 0)
            {
                StatusEffect newEffect = createEffect.Invoke();
                Type effectType = newEffect.GetType();

                StatusEffect existingEffect = enemyHealth.GetStatusEffectByType(effectType);

                if (existingEffect != null)
                {
                    existingEffect.ResetDuration(newEffect.Duration, newEffect.Damage);
                    Debug.Log($"Refreshed duration of {effectType.Name} on {target.name}");
                }
                else
                {
                    enemyHealth.activeEffects.Add(newEffect);
                    newEffect.Apply();
                }
            }
        }
    }

    public void ApplyBleedEffect(GameObject target, float duration, float damage, float interval)
    {
        ApplyStatusEffect(target, () => new BleedingEffect(duration, damage, interval, target.GetComponent<EnemyHealthManager>(), this));
    }

    public void ApplyKamikazeEffect(GameObject target, float duration, float radius)
    {
        ApplyStatusEffect(target, () => new KamikazeEffect(target, duration, radius, 0f, this));
    }

    public void ApplyBurnEffect(GameObject target, float duration, float damage)
    {
        ApplyStatusEffect(target, () => new BurningEffect(duration, damage, target.GetComponent<EnemyHealthManager>(), this));
    }
}