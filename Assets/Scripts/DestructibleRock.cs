using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleRock : ObjectHealthManager
{
    //public GameObject rubblePrefab;

    protected override void CheckHealthThreshold()
    {
        if (currentHealth > 0)
        {
            if (currentHealth <= maxHealth * 0.66f && !hasMinorDamage)
            {
                Debug.Log("Rock has been minor hurt");
                TriggerDamageParticles(3);
                hasMinorDamage = true;
                animator.SetTrigger("MinorDamage");
            }
            else if (currentHealth <= maxHealth * 0.33f && !hasMajorDamage)
            {
                Debug.Log("Rock has major hurt");
                TriggerDamageParticles(4);
                hasMajorDamage = true;
                animator.SetTrigger("MajorDamage");
            }
        }
        else
        {
            if (!isDestroyed)
            {
                isDestroyed = true;
                Debug.Log("Rock has been killed");
                TriggerDamageParticles(8);
                StartCoroutine(DestroyObject());
            }
        }
    }

    private IEnumerator DestroyObject()
    {
        animator.SetTrigger("Die");
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Destroy"))
        {
            yield return null;
        }
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        Debug.Log("Rock has been killed");
        animator.enabled = false;
        spriteRenderer.enabled = false;

        Die();
    }
}
