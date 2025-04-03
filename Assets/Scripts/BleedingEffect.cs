using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BleedingEffect : StatusEffect
{
    public float bleedDamage;
    private float bleedDuration;
    private int stackCount = 1;
    float elapsedTime = 0f;
    
    private ILivingEntity bleedTarget;
    private GameObject bleedingFX;

    public BleedingEffect(float duration, float damage, ILivingEntity target, MonoBehaviour owner, bool isEternal = false)
        : base(duration, damage, target, owner, isEternal)
    {
        bleedDuration = duration;
        bleedDamage = damage;
        bleedTarget = target;
    }

    public void IncreaseDamage(float additionalDamage)
    {
        stackCount++;
        bleedDamage += additionalDamage;
        Debug.Log($"Bleed damage increased. Bleed damage added: {additionalDamage}. NEW total damage per tick: {bleedDamage}, total stack count: {stackCount}");
    }

    protected override IEnumerator EffectCoroutine()
    {
        if (isFirstApplication)
        {
            yield return new WaitForSeconds(0.5f);
            isFirstApplication = false;

            if (bleedTarget.GetEffectAnchor() != null)
            {
                bleedingFX = FXManager.Instance.PlayFX("BurningFX", bleedTarget.GetEffectAnchor().position);
                bleedingFX.transform.SetParent(bleedTarget.GetEffectAnchor());
                bleedingFX.transform.localPosition = new Vector3(0f, -0.6f, 0f);
                bleedingFX.transform.localScale = Vector3.one;
            }
        }

        while (elapsedTime < bleedDuration && isActive)
        {
            if (bleedTarget != null)
            {
                bleedTarget.TakeDamage(bleedDamage, Vector2.zero, 0f, DamageSource.StatusEffect);
                Debug.Log($"Bleeding applied {bleedDamage} damage to {bleedTarget}.");
            }

            yield return new WaitForSeconds(0.5f);
            elapsedTime += 0.5f;
            if (elapsedTime >= bleedDuration)
            {
                RemoveEffect();
            }
        }
    }

    public override void ResetDuration(float newDuration, float newDamage)
    {
        bleedDuration = newDuration;
        elapsedTime = 0f;
        IncreaseDamage(newDamage);
        Debug.Log("Duration of effect refreshed to " + newDuration);
        if (!isActive && EffectCoroutine() != null)
        {
            affectedTarget.StopCoroutine(effectCoroutine);
            ApplyEffect();
        }
    }
    
    public override void RemoveEffect()
    {
        var autoDestroy = bleedingFX.GetComponent<AutoDestroyParticle>();
        if (autoDestroy != null) autoDestroy.StopEffect();
        bleedingFX.transform.SetParent(FXManager.Instance.transform);
        FXManager.Instance.ReturnToPool(bleedingFX, "BurningFX");
        base.RemoveEffect();
    }
}