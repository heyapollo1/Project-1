using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunEffect : StatusEffect
{
    private float stunDuration;
    private float elapsedTime = 0f;
    
    private GameObject stunFX;
    private ILivingEntity stunTarget;

    private bool isStunned = false;

    public StunEffect(float duration, ILivingEntity target, MonoBehaviour owner)
        : base(duration, 0, target, owner, isEternal: false)
    {
        stunTarget = target;
        stunDuration = duration;
    }
    
    public void IncreaseStunDuration(float additionalDuration)
    {
        stunDuration += additionalDuration;
        Debug.Log("Stun duration refreshed to " + stunDuration);
    }
    
    protected override IEnumerator EffectCoroutine()
    {
        if (!isStunned && stunTarget != null)
        {
            isStunned = true;
            stunTarget.InflictStun(stunDuration);
            Debug.Log($"Stun applied to {stunTarget}, for {stunDuration} seconds.");
        }

        while (elapsedTime < stunDuration && isActive)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        RemoveEffect();
    }

    public override void ResetDuration(float newDuration, float newDamage)
    {
        //play stun fx
        IncreaseStunDuration(newDuration);
        if (!isActive)
        {
            ApplyEffect();
        }
    }
    
    public override void RemoveEffect()
    {
        var autoDestroy = stunFX.GetComponent<AutoDestroyParticle>();
        if (autoDestroy != null) autoDestroy.StopEffect();
        stunFX.transform.SetParent(FXManager.Instance.transform);
        FXManager.Instance.ReturnToPool(stunFX, "StunFX");
        base.RemoveEffect();
    }
}