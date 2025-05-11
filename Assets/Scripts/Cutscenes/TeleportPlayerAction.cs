using System;
using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

[CreateAssetMenu(menuName = "Cutscene/Actions/Teleport Player")]
public class TeleportDepartureAction : CutsceneAction
{
    //public Transform destination;
    public float fadeDuration = 1.0f; // Time for fade effect

    public override IEnumerator Execute(CutsceneManager cutsceneManager, Action onComplete)
    {
        Transform player = PlayerController.Instance.GetPlayerTransform();
        
        Debug.Log($"Teleporting player to {cutsceneManager.currentTarget}...");

        cutsceneManager.RepositionCutsceneCamera(player.position);
        
        yield return cutsceneManager.FadeScreen(true, fadeDuration);

        //Debug.Log($"Teleporting player to {destination}...");
        cutsceneManager.RepositionCutsceneCamera(cutsceneManager.currentTarget.position);
        
        //;yield return new WaitForSeconds(1.0f); // Allow Unity to update positions
        
        player.position = cutsceneManager.currentTarget.position;
        Debug.Log("Player & Camera Teleported!");
        onComplete?.Invoke();
    }
}