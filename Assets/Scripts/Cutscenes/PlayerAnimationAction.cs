using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Cutscene/Actions/Player Animation")]
public class PlayerAnimationAction : CutsceneAction
{
    public string playerAnimationName;

    public override IEnumerator Execute(CutsceneManager cutsceneManager, Action onComplete)
    {
        Transform player = PersistentManager.Instance.playerInstance.transform;
        Animator playerAnimator = player.GetComponent<Animator>();

        if (playerAnimator == null)
        {
            Debug.LogWarning("No animator assigned!");
            onComplete?.Invoke();
            yield break;
        }

        playerAnimator.Play(playerAnimationName);

        yield return cutsceneManager.WaitForAnimation(playerAnimator, playerAnimationName);
        onComplete?.Invoke();
    }
}
