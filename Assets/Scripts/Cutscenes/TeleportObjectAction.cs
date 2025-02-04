using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Cutscene/Actions/Teleport Object")]
public class TeleportObjectAction : CutsceneAction
{
    public GameObject passenger; 

    public override IEnumerator Execute(CutsceneManager cutsceneManager, Action onComplete)
    {
        Debug.Log($"Teleporting object to {cutsceneManager.currentTarget.position}...");
        passenger.transform.position = cutsceneManager.currentTarget.position;

        yield return new WaitForSeconds(0.1f); // Allow Unity to update positions

        onComplete?.Invoke();
    }
}