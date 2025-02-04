using System;
using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

[CreateAssetMenu(menuName = "Cutscene/Actions/Zoom Camera")]
public class ZoomCameraAction : CutsceneAction
{
    public float targetOrthographicSize = 5f;  // Desired zoom
    public float duration = 0.25f; 

    public override IEnumerator Execute(CutsceneManager cutsceneManager, Action onComplete)
    {
        CinemachineCamera cutsceneCamera = CameraManager.Instance.cutsceneCamera;
        if (cutsceneCamera == null)
        {
            Debug.LogError("ZoomCameraAction: Cutscene camera not found!");
            yield break;
        }

        float startSize = cutsceneCamera.Lens.OrthographicSize;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            // Smoothly interpolate between the current and target zoom level
            cutsceneCamera.Lens.OrthographicSize = Mathf.Lerp(startSize, targetOrthographicSize, t);

            yield return null;
        }

        cutsceneCamera.Lens.OrthographicSize = targetOrthographicSize;

        Debug.Log($"Zoomed camera to {targetOrthographicSize}");
        onComplete?.Invoke();
    }
}