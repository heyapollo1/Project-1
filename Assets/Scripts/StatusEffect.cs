using System.Collections;
using UnityEngine;

public abstract class StatusEffect
{
    public float Duration { get; protected set; }
    public float Damage { get; protected set; }
    protected bool isActive;
    protected bool isEternal;
    protected bool isFirstApplication = true;
    protected Coroutine effectCoroutine;
    protected ILivingEntity target;
    protected MonoBehaviour affectedTarget; // The object that has Mono to run coroutine (e.g., PlayerCombat, Enemy), bc public abstract class cant. No mono.

    public StatusEffect(float duration, float damage, ILivingEntity target, MonoBehaviour coroutineRunner, bool isEternal)
    {
        Duration = duration;
        Damage = damage;
        affectedTarget = coroutineRunner;
        this.target = target;
        this.isEternal = isEternal;
    }

    public void ApplyEffect()
    {
        if (!isActive)
        {
            isActive = true;
            effectCoroutine = affectedTarget.StartCoroutine(EffectCoroutine());
        }
        Debug.LogWarning("Attempted to apply an already active status effect.");
    }

    public virtual void RemoveEffect()
    {
        if (!isActive || effectCoroutine == null)
        {
            Debug.LogWarning("Attempted to remove an inactive effect.");
            return;
        }
        affectedTarget.StopCoroutine(effectCoroutine);
        isFirstApplication = true;
        effectCoroutine = null;
        isActive = false;
        target.RemoveStatusEffect(this);
    }

    protected abstract IEnumerator EffectCoroutine();

    public virtual void ResetDuration(float newDuration, float newDamage) { }
}
