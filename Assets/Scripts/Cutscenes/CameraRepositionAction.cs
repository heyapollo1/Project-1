using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Cutscene/Actions/Reposition Camera")]
public class RepositionCameraAction : CutsceneAction
{
    public override IEnumerator Execute(CutsceneManager cutsceneManager, Action onComplete)
    {
        Transform targetTransform = cutsceneManager.currentTarget;

        if (targetTransform == null)
        {
            Debug.LogWarning("PanCameraAction: No target assigned!");
            yield break;
        }

        yield return cutsceneManager.RepositionCamera(targetTransform.position);
        onComplete?.Invoke();
    }
}
