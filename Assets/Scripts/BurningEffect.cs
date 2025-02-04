using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurningEffect : StatusEffect
{
    public float burnDamage;
    //public float burnExplosionDamage;
    //private float burnDelay;
    private float burnDuration;
    //private float burnExplosionRadius;
    private EnemyHealthManager burnTarget;
    private int stackCount = 1;
    private float elapsedTime = 0f;
    private GameObject burningFX;
    //private BurnDeathEffect burnDeathEffect;

    public BurningEffect(float duration, float damage, EnemyHealthManager targetEnemy, MonoBehaviour owner, bool isEternal = false)
        : base(duration, damage, owner, isEternal)
    {
        burnDuration = duration;
        burnDamage = damage;
        burnTarget = targetEnemy;
        //burnExplosionRadius = radius;
        //burnExplosionDamage = burnDamage * 5;
        //burnDeathEffect = new BurnDeathEffect(burnExplosionRadius, burnExplosionDamage, 3f);
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
            //DeathEffectManager.Instance.RegisterOnDeathEffect(burnTarget.gameObject, burnDeathEffect);

            burningFX = FXManager.Instance.PlayFX("BurningFX", burnTarget.transform.position);
            burningFX.transform.SetParent(burnTarget.transform);
            burningFX.transform.localPosition = new Vector3(0f, -0.6f, 0f);
            burningFX.transform.localScale = new Vector3(1, 1, 1);
        }

        Debug.Log($"Burn stack count: {stackCount}");

        while (elapsedTime < burnDuration && isActive)
        {
            if (burnTarget != null)
            {
                burnTarget.TakeDamage(burnDamage, Vector2.zero, 0f, false, true);
                burnTarget.ShowDamageNumber(burnDamage, Color.yellow, 10f);
                Debug.Log($"Burning applied {burnDamage} damage to {burnTarget.name}.");
            }

            yield return new WaitForSeconds(0.5f);
            elapsedTime += 0.5f;
        }

        AutoDestroyParticle autoDestroy = burningFX.GetComponent<AutoDestroyParticle>();
        autoDestroy.StopEffect();
        burningFX.transform.SetParent(FXManager.Instance.transform);
        FXManager.Instance.ReturnToPool(burningFX, "BurningFX");
    }

    public override void ResetDuration(float newDuration, float newDamage)
    {
        burnDuration = newDuration;
        elapsedTime = 0f;
        IncreaseDamage(newDamage);
        Debug.Log("Duration of effect refreshed to " + newDuration);
        if (!isActive && EffectCoroutine() != null)
        {
            affectedTarget.StopCoroutine(effectCoroutine);
            Apply();
        }
    }
}