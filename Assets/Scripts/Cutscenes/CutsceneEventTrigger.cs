using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Cutscene/Actions/Event")]
public class CutsceneEventTrigger : CutsceneAction
{
    public string eventName; // The name of the event to trigger
    public EncounterData encounter;
    public bool isEncounterEvent;

    public override IEnumerator Execute(CutsceneManager cutsceneManager, Action onComplete)
    {
        if (string.IsNullOrEmpty(eventName))
        {
            Debug.LogWarning("CutsceneEventAction: No event name provided!");
            onComplete?.Invoke();
            yield break;
        }

        if (isEncounterEvent)
        {
            Debug.Log($"Triggering Encounter Event: {eventName}, Encounter Name: {encounter}");

            if (encounter == null)
            {
                Debug.LogError("EncounterData is NULL! Event won't trigger correctly.");
            }

            Debug.Log($"Triggering Encounter Event: {eventName}, Encounter Name: {encounter}");
            EventManager.Instance.TriggerEvent("EnterEncounter", encounter);
        }
        else
        {
            Debug.Log($"Triggering Cutscene Event: {eventName}");
            EventManager.Instance.TriggerEvent(eventName);
        }

        // Complete this action
        onComplete?.Invoke();
        yield break;
    }
}