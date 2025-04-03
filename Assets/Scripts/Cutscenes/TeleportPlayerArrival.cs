using System;
using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

[CreateAssetMenu(menuName = "Cutscene/Actions/Teleport Arrival")]
public class TeleportArrivalAction : CutsceneAction
{
    //public Transform destination;
    //public Vector3 destination; // Target position for teleport
    public float fadeDuration = 1.0f; // Time for fade effect
    //public bool instantTeleport = false; // If true, skips fade

    public override IEnumerator Execute(CutsceneManager cutsceneManager, Action onComplete)
    {
        yield return cutsceneManager.FadeScreen(false, fadeDuration);

        AudioManager.Instance.PlayUISound("Player_TeleportArrival");
        //player.transform.position = destination.position;
        Debug.Log("Player & Camera Teleported!");
        onComplete?.Invoke();
    }
}