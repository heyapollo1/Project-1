using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurningEffect : StatusEffect
{
    public float burnDamage;
    private float burnDuration;
    private int stackCount = 1;
    private float elapsedTime = 0f;
    
    private GameObject burningFX;
    private ILivingEntity burnTarget;

   public BurningEffect(float duration, float damage, ILivingEntity target, MonoBehaviour coroutineRunner, bool isEternal = false)
        : base(duration, damage, target, coroutineRunner, isEternal)
    {
        burnDuration = duration;
        burnDamage = damage;
        burnTarget = target;
    }


    public void IncreaseDamage(float additionalDamage)
    {
        stackCount++;
        burnDamage += additionalDamage;
        Debug.Log($"Burn damage increased. Burn damage added: {additionalDamage}. NEW total damage per tick: {burnDamage}, total stack count: {stackCount}");
    }

    protected override IEnumerator EffectCoroutine()
    {
        if (isFirstApplication)
        {
            yield return new WaitForSeconds(0.5f);
            isFirstApplication = false;

            if (burnTarget.GetEffectAnchor() != null)
            {
                burningFX = FXManager.Instance.PlayFX("BurningFX", burnTarget.GetEffectAnchor().position);
                burningFX.transform.SetParent(burnTarget.GetEffectAnchor());
                burningFX.transform.localPosition = new Vector3(0f, -0.6f, 0f);
                burningFX.transform.localScale = Vector3.one;
            }
        }

        Debug.Log($"Burn stack count: {stackCount}");

        while (elapsedTime < burnDuration && isActive)
        {
            if (burnTarget != null)
            {
                burnTarget.TakeDamage(burnDamage, Vector2.zero, 0f, DamageSource.StatusEffect);
                Debug.Log($"Burning applied {burnDamage} damage to {burnTarget}.");
                //burnDamage *= 0.9f;
            }
            yield return new WaitForSeconds(0.5f);
            elapsedTime += 0.5f;
            if (elapsedTime >= burnDuration || burnTarget.GetCurrentHealth() <= 0)
            {
                RemoveEffect();
            }
        }
    }

    public override void ResetDuration(float newDuration, float newDamage)
    {
        burnDuration = newDuration;
        elapsedTime = 0f;
        IncreaseDamage(newDamage);
        Debug.Log("Burn duration refreshed: " + newDuration);

        if (!isActive)
        {
            ApplyEffect();
        }
    }
    
    public override void RemoveEffect()
    {
        if (burningFX != null)
        {
            var autoDestroy = burningFX.GetComponent<AutoDestroyParticle>();
            if (autoDestroy != null) autoDestroy.StopEffect();
            burningFX.transform.SetParent(FXManager.Instance.transform);
            FXManager.Instance.ReturnToPool(burningFX, "BurningFX");
        }

        base.RemoveEffect();
    }
}