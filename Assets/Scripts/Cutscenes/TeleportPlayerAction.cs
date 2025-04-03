using System;
using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

[CreateAssetMenu(menuName = "Cutscene/Actions/Teleport Player")]
public class TeleportDepartureAction : CutsceneAction
{
    public Transform destination;
    public float fadeDuration = 1.0f; // Time for fade effect

    public override IEnumerator Execute(CutsceneManager cutsceneManager, Action onComplete)
    {
        destination = cutsceneManager.currentTarget;
        GameObject player = PlayerManager.Instance.playerInstance;

        Debug.Log($"Teleporting player to {destination}...");

        yield return cutsceneManager.FadeScreen(true, fadeDuration);

        cutsceneManager.RepositionCutsceneCamera(destination.position);
   
        yield return new WaitForSeconds(0.5f); // Allow Unity to update positions
        
        player.transform.position = destination.position;
        Debug.Log("Player & Camera Teleported!");
        onComplete?.Invoke();
    }
}