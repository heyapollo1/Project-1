using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Cutscene/Actions/Pan Camera")]
public class PanCameraAction : CutsceneAction
{
    public float duration = 0.25f;

    public override IEnumerator Execute(CutsceneManager cutsceneManager, Action onComplete)
    {
        Transform targetTransform = cutsceneManager.currentTarget;

        if (targetTransform == null)
        {
            Debug.LogWarning("PanCameraAction: No target assigned!");
            yield break;
        }

        // Pan to the target position
        yield return cutsceneManager.PanCamera(targetTransform.position, duration);
        onComplete?.Invoke();
    }
}