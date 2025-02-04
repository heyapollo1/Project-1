using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Cutscene/Actions/Wait For Animation")]
public class WaitForAnimationAction : CutsceneAction
{
    //public Animator animator;
    public string animationName;

    public override IEnumerator Execute(CutsceneManager cutsceneManager, Action onComplete = null)
    {
        //Transform currentTarget = CutsceneManager.Instance.currentTarget;
        if (cutsceneManager.currentTarget == null)
        {
            Debug.LogWarning("WaitForAnimationAction: No target assigned!");
            onComplete?.Invoke();
            yield break;
        }

        Animator animator = cutsceneManager.currentTarget.GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogWarning("WaitForAnimationAction: Target does not have an Animator component!");
            onComplete?.Invoke();
            yield break;
        }

        yield return cutsceneManager.WaitForAnimation(animator, animationName);
        onComplete?.Invoke();
    }
}