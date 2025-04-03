using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStatusApplicable
{
    void ApplyStatusEffect(StatusEffect effect);
    void RemoveStatusEffect(StatusEffect effect);
    Transform GetEffectAnchor(); // For VFX parenting
    List<StatusEffect> GetActiveEffects();
}

public interface IDamageable
{
    void TakeDamage(float damage, Vector2 knockbackDirection, float knockbackForce, DamageSource sourceType = DamageSource.Player, bool isCriticalHit = false);
    void ShowDamageNumber(float number, Color color, float size = 12f, Vector3? customPosition = null);
}

public interface IHealable
{
    void Heal(float amount);
}

public enum DamageSource
{
    Player,
    Enemy,
    StatusEffect,
    Execution
}

public enum EntityType
{
    Player,
    Enemy,
    Object
}

public interface ILivingEntity : IDamageable, IStatusApplicable, IHealable
{
    EntityType GetEntityType();
    float GetCurrentHealth();
    float GetMaxHealth();
    void InflictStun(float duration);
}