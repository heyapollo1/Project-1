using System;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectManager : MonoBehaviour
{
    public static StatusEffectManager Instance { get; private set; }
    
    public void Initialize()
    {
    }
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    public void TriggerStatusEffect(GameObject target, StatType statType, float damage = 0f)
    {
        if (!target.TryGetComponent<ILivingEntity>(out var entity)) return;
        bool isPlayerAfflicted = entity.GetEntityType() == EntityType.Player;
        
        switch (statType)
        {
            case StatType.BleedChance:
                ApplyBleedEffect(target, 5f, damage/10);
                break;
            case StatType.BurnChance:
                ApplyBurnEffect(target, 5f, damage/10);
                break;
            case StatType.StunChance:
                ApplyStunEffect(target, 2f);
                break;
            default:
                Debug.LogWarning($"No status effect linked to stat type: {statType}");
                break;
        }
    }

    
    public void ApplyStatusEffect(GameObject statusEffectTarget, Func<StatusEffect> createEffect)
    {
        if (!statusEffectTarget.TryGetComponent<ILivingEntity>(out var target)) return;
        
        StatusEffect newEffect = createEffect();
        var existingEffect = target.GetActiveEffects()?.Find(effect => effect.GetType() == newEffect.GetType());
        
        if (existingEffect != null)
        {
            existingEffect.ResetDuration(newEffect.Duration, newEffect.Damage);
        }
        else if (target.GetCurrentHealth() > 0)
        {
            target.ApplyStatusEffect(newEffect);
            newEffect.ApplyEffect();
        }
    }

    public void ApplyBleedEffect(GameObject statusEffectTarget, float duration, float damage)
    {
        if (!statusEffectTarget.TryGetComponent<ILivingEntity>(out var target)) return;
        ApplyStatusEffect(statusEffectTarget, () => new BleedingEffect(duration, damage, target, this));
    }

    public void ApplyKamikazeEffect(GameObject statusEffectTarget, float duration, float radius)
    {
        if (!statusEffectTarget.TryGetComponent<ILivingEntity>(out var target)) return;
        ApplyStatusEffect(statusEffectTarget, () => new KamikazeEffect(target, duration, radius, 0f, this));
    }

    public void ApplyBurnEffect(GameObject statusEffectTarget, float duration, float damage, bool isPlayer = false)
    {
        if (!statusEffectTarget.TryGetComponent<ILivingEntity>(out var target)) return;
        ApplyStatusEffect(statusEffectTarget, () => new BurningEffect(duration, damage, target, this));
    }
    
    public void ApplyStunEffect(GameObject statusEffectTarget, float duration)
    {
        if (!statusEffectTarget.TryGetComponent<ILivingEntity>(out var target)) return;
        ApplyStatusEffect(statusEffectTarget, () => new StunEffect(duration, target, this));
    }
}