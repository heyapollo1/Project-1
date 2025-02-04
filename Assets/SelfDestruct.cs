using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    private void Start()
    {
        Animator animator = GetComponent<Animator>();
        bool hasTriggered = false;

        if (animator != null && !hasTriggered)
        {
            hasTriggered = true;
            AnimatorStateInfo animationState = animator.GetCurrentAnimatorStateInfo(0);
            float animationLength = animationState.length;

            Destroy(gameObject, animationLength);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}