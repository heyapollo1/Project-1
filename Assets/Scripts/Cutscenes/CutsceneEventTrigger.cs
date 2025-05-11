using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Cutscene/Actions/Event")]
public class CutsceneEventTrigger : CutsceneAction
{
    public string eventName;
    public string eventStringParameter;
    public bool hasStringParameter;

    public override IEnumerator Execute(CutsceneManager cutsceneManager, Action onComplete)
    {
        if (string.IsNullOrEmpty(eventName))
        {
            Debug.LogWarning("CutsceneEventAction: No event name provided!");
            onComplete?.Invoke();
            yield break;
        }

        if (hasStringParameter)
        {
            EventManager.Instance.TriggerEvent(eventName, eventStringParameter);
        }
        else
        {
            Debug.Log($"Triggering Cutscene Event: {eventName}");
            EventManager.Instance.TriggerEvent(eventName);
        }
        
        onComplete?.Invoke();
        yield break;
    }
}