using System.Collections;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Cutscene/Actions/Pan Back to Player")]
public class PanBackToPlayerAction : CutsceneAction
{
    public float duration;

    public override IEnumerator Execute(CutsceneManager cutsceneManager, Action onComplete)
    {
        Transform player = PersistentManager.Instance.playerInstance.transform;
        if (player == null)
        {
            Debug.LogWarning("PanBackToPlayerAction: Player instance not found!");
            yield break;
        }
        CutsceneManager.Instance.SetTarget(player);
        Vector3 playerPosition = player.transform.position;

        yield return cutsceneManager.PanCamera(playerPosition, duration);
        onComplete?.Invoke();
    }
}