using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BleedingEffect : StatusEffect
{
    public float bleedDamage;
    private float bleedDelay;
    private float bleedDuration;
    private EnemyHealthManager bleedTarget;
    private int stackCount = 1;
    float elapsedTime = 0f;
    private GameObject bleedingFX;

    public BleedingEffect(float duration, float damage, float interval, EnemyHealthManager targetEnemy, MonoBehaviour owner, bool isEternal = false)
        : base(duration, damage, owner, isEternal)
    {
        bleedDuration = duration;
        bleedDamage = damage;
        bleedDelay = interval;
        bleedTarget = targetEnemy;
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
            yield return new WaitForSeconds(1f);
            isFirstApplication = false;
            Debug.Log($"Bleed stack count: {stackCount}");
            bleedingFX = FXManager.Instance.PlayFX("BleedingFX", bleedTarget.transform.position);
            bleedingFX.transform.SetParent(bleedTarget.transform);
            bleedingFX.transform.localPosition = new Vector3(0f, 0.7f, 0f);
            bleedingFX.transform.localScale = new Vector3(1,1,1);
        }

        while (elapsedTime < bleedDuration && isActive)
        {
            if (bleedTarget != null)
            {
                bleedTarget.TakeDamage(bleedDamage, Vector2.zero, 0f, false, true);
                bleedTarget.ShowDamageNumber(bleedDamage, Color.red, 10f);
                //Debug.Log($"Bleeding applied {bleedDamage} damage to {bleedTarget.name}.");
            }

            yield return new WaitForSeconds(bleedDelay);
            elapsedTime += bleedDelay;
        }

        if (bleedingFX != null)
        {
            bleedingFX.transform.SetParent(null);
            FXManager.Instance.ReturnToPool(bleedingFX, "BleedingFX");
        }

        Remove(bleedTarget.gameObject, false);
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
            Apply();
        }
    }
}