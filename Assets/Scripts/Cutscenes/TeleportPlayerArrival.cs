using System;
using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

[CreateAssetMenu(menuName = "Cutscene/Actions/Teleport Arrival")]
public class TeleportArrivalAction : CutsceneAction
{
    //public Transform destination; // Target position for teleport
    public float fadeDuration = 1.0f; // Time for fade effect

    public override IEnumerator Execute(CutsceneManager cutsceneManager, Action onComplete)
    {
        //destination = cutsceneManager.currentTarget;
        //cutsceneManager.RepositionCutsceneCamera(destination.position);
        yield return new WaitForSeconds(1.0f);
        
        yield return cutsceneManager.FadeScreen(false, fadeDuration);
        
        AudioManager.Instance.PlayUISound("Player_TeleportArrival");
        //player.transform.position = destination.position;
        Debug.Log("Player & Camera Teleported!");
        onComplete?.Invoke();
    }
}