using System.Collections;
using UnityEngine;
using System;
using Unity.Cinemachine;

[CreateAssetMenu(menuName = "Cutscene/Actions/Move Camera To Position")]
public class MoveCameraToPositionAction : CutsceneAction
{
    public Vector3 targetPosition;
    public float duration = 1.5f;

    public override IEnumerator Execute(CutsceneManager cutsceneManager, Action onComplete)
    {
        CinemachineCamera cutsceneCamera = CameraManager.Instance.cutsceneCamera;
        if (cutsceneCamera == null)
        {
            Debug.LogError("MoveCameraToPositionAction: Cutscene camera not found!");
            yield break;
        }

        Vector3 startPosition = cutsceneCamera.transform.position;
        targetPosition.z = -10f; // Maintain depth

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            cutsceneCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        cutsceneCamera.transform.position = targetPosition;

        Debug.Log($"Camera moved to world position: {targetPosition}");
        onComplete?.Invoke();
    }
}
