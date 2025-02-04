using System;
using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

[CreateAssetMenu(menuName = "Cutscene/Actions/Teleport Player")]
public class TeleportPlayerAction : CutsceneAction
{
    public Transform destination;
    //public Vector3 destination; // Target position for teleport
    public float fadeDuration = 1.0f; // Time for fade effect
    public bool instantTeleport = false; // If true, skips fade

    public override IEnumerator Execute(CutsceneManager cutsceneManager, Action onComplete)
    {
        AudioManager.Instance.PlayUISound("Player_TeleportDeparture");
        destination = cutsceneManager.currentTarget;
        GameObject player = PersistentManager.Instance.playerInstance;

        Debug.Log($"Teleporting player to {destination}...");

        if (!instantTeleport)
        {
            yield return cutsceneManager.FadeScreen(true, fadeDuration);
        }

        cutsceneManager.RepositionCutsceneCamera(destination.position);
   
        yield return new WaitForSeconds(0.5f); // Allow Unity to update positions

        if (!instantTeleport)
        {
            yield return cutsceneManager.FadeScreen(false, fadeDuration);
        }

        AudioManager.Instance.PlayUISound("Player_TeleportArrival");
        player.transform.position = destination.position;
        Debug.Log("Player & Camera Teleported!");
        onComplete?.Invoke();
    }
}