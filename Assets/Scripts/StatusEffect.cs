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
    protected MonoBehaviour affectedTarget; // The object that has Mono to run coroutine (e.g., PlayerCombat, Enemy), bc public abstract class cant.

    public StatusEffect(float duration, float damage, MonoBehaviour owner, bool isEternal)
    {
        Duration = duration;
        Damage = damage;
        affectedTarget = owner;
        this.isEternal = isEternal;
    }

    public virtual void Apply()
    {
        if (!isActive)
        {
            isActive = true;
            effectCoroutine = affectedTarget.StartCoroutine(EffectCoroutine());
        }
        else
        {
            Debug.LogWarning("Attempted to apply an already active status effect.");
            return;
        }
    }

    public virtual void Remove(GameObject target, bool isPlayer)
    {
        if (!isActive || target == null)
        {
            Debug.LogWarning("Attempted to remove an inactive effect or target is null.");
            return;
        }

        if (effectCoroutine != null)
        {
            Debug.Log($"{affectedTarget.name} is stopping coroutine.");
            affectedTarget.StopCoroutine(effectCoroutine);
            effectCoroutine = null;
        }

        isActive = false;

        if (!isPlayer)
        {
            EnemyHealthManager enemyHealth = target.GetComponent<EnemyHealthManager>();
            if (enemyHealth != null)
            {
                Debug.Log($"{enemyHealth.name} is removing effect.");
                enemyHealth.RemoveStatusEffect(this);
            }
            else
            {
                Debug.LogWarning("EnemyHealthManager component missing on target or target is null.");
            }
        }
        else
        {
            PlayerHealthManager playerHealth = target.GetComponent<PlayerHealthManager>();
            if (playerHealth != null)
            {
                Debug.Log($"{playerHealth.name} is removing effect.");
                playerHealth.RemoveStatusEffect(this);
            }
            else
            {
                Debug.LogWarning("PlayerHealthManager component missing on target or target is null.");
            }
        }
    }

    protected abstract IEnumerator EffectCoroutine();

    public virtual void ResetDuration(float newDuration, float newDamage) { }
}
