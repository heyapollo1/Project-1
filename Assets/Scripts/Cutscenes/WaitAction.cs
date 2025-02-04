using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Cutscene/Actions/Wait")]
public class WaitAction : CutsceneAction
{
    public float duration;

    public override IEnumerator Execute(CutsceneManager cutsceneManager, Action onComplete)
    {
        yield return new WaitForSeconds(duration);
        onComplete?.Invoke();
    }
}