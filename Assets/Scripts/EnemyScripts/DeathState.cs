using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathState : IEnemyState
{
    public void EnterState(EnemyAI enemy)
    {
        Debug.Log("trasnsition to death state");
        if (enemy.deathType == DeathType.Instant)
        {
            enemy.animator.SetTrigger("InstantDeath");  // Set a faster animation or special effect
            enemy.InterruptCurrentAbility();
            enemy.RemoveComponentsOnDeath();
            enemy.StartCoroutine(HandleInstantDeath(enemy));
        }
        else if (enemy.deathType == DeathType.Standard)
        {
            Debug.Log("Standard death");
            enemy.animator.SetBool("isDead", true);
            enemy.InterruptCurrentAbility();
            enemy.RemoveComponentsOnDeath();
            enemy.StartCoroutine(HandleStandardDeath(enemy));
        }
        else if (enemy.deathType == DeathType.Boss)
        {
            enemy.animator.SetTrigger("BossDeath");
            AudioManager.Instance.TurnOffMusic();
            enemy.InterruptCurrentAbility();
            enemy.RemoveComponentsOnDeath();
            enemy.StartCoroutine(HandleBossDeath(enemy));
        }
        else
        {
            enemy.animator.SetTrigger("Die");
            enemy.InterruptCurrentAbility();
            enemy.RemoveComponentsOnDeath();
            enemy.StartCoroutine(HandleDeletion(enemy));
        }
    }

    private IEnumerator HandleBossDeath(EnemyAI enemy)
    {
        yield return new WaitForSeconds(3f);
        
        VisualFeedbackManager visualFeedback = enemy.GetComponent<VisualFeedbackManager>();
        if (visualFeedback != null)
        {
            yield return visualFeedback.FadeOut(1f, enemy.gameObject); // 1 second fade-out
        }
        EventManager.Instance.TriggerEvent("BossDeath");
        //EnemyPoolManager.Instance.ReturnEnemy(enemy.gameObject);
    }

    private IEnumerator HandleStandardDeath(EnemyAI enemy)
    {
        Debug.Log("Animating death");
        yield return new WaitForSeconds(enemy.animator.GetCurrentAnimatorStateInfo(0).length);
        enemy.animator.SetBool("isDead", false);

        yield return new WaitForSeconds(1f);

        VisualFeedbackManager visualFeedback = enemy.GetComponent<VisualFeedbackManager>();
        if (visualFeedback != null)
        {
            yield return visualFeedback.FadeOut(1f, enemy.gameObject); // 1 second fade-out
        }
        Debug.Log("death complete");
        EnemyPoolManager.Instance.ReturnEnemy(enemy.gameObject);
    }

    private IEnumerator HandleInstantDeath(EnemyAI enemy)
    {
        Debug.Log("Instantdeathcall");

        VisualFeedbackManager visualFeedback = enemy.GetComponent<VisualFeedbackManager>();
        if (visualFeedback != null)
        {
            yield return visualFeedback.FadeOut(0.3f, enemy.gameObject);
        }
        EnemyPoolManager.Instance.ReturnEnemy(enemy.gameObject);
    }

    private IEnumerator HandleDeletion(EnemyAI enemy)
    {
        yield return new WaitForSeconds(enemy.animator.GetCurrentAnimatorStateInfo(0).length);

        VisualFeedbackManager visualFeedback = enemy.GetComponent<VisualFeedbackManager>();
        if (visualFeedback != null)
        {
            yield return visualFeedback.FadeOut(1f, enemy.gameObject); // 1 second fade-out
        }
        EnemyPoolManager.Instance.DestroyEnemy(enemy.gameObject);
    }


    public void UpdateState(EnemyAI enemy) { }

    public void ExitState(EnemyAI enemy) { }
}
