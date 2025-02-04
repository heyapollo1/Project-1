using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Cutscene/Actions/Dialogue")]
public class CutsceneDialogueAction : CutsceneAction
{
    [TextArea] public string[] dialogueLines;

    public override IEnumerator Execute(CutsceneManager cutsceneManager, Action onComplete)
    {
        if (dialogueLines.Length == 0)
        {
            Debug.LogWarning("CutsceneDialogueAction: No dialogue lines provided!");
            onComplete?.Invoke();
            yield break;
        }

        // Start the dialogue sequence and wait for it to finish
        yield return DialogueManager.Instance.StartDialogue(dialogueLines);

        // When dialogue is done, complete this action
        onComplete?.Invoke();
    }
}