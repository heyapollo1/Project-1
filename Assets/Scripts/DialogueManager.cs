using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;

    public event Action OnDialogueFinished;

    private int currentLineIndex = 0;
    private bool isTyping = false;
    private bool canProgress = false;
    private string[] currentDialogueLines;
    [SerializeField] private float textSpeed = 0.025f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        dialoguePanel.SetActive(false);
    }

    public void TriggerDialogue(string[] dialogueLines, Action callback = null)
    {
        StartCoroutine(StartDialogue(dialogueLines, callback));
    }

    public IEnumerator StartDialogue(string[] dialogueLines, Action callback = null)
    {
        Debug.Log("Starting Cutscene Dialogue");
        dialoguePanel.SetActive(true);
        currentDialogueLines = dialogueLines;
        currentLineIndex = 0;

        while (currentLineIndex < currentDialogueLines.Length)
        {
            yield return StartCoroutine(TypeLine(currentDialogueLines[currentLineIndex]));
            currentLineIndex++;
            canProgress = true;

            float timer = 0f;
            while (timer < 1.5f)
            {
                if (Input.GetKeyDown(KeyCode.C))
                {
                    break;
                }
                timer += Time.deltaTime;
                yield return null;
            }

            canProgress = false;
        }
        EndDialogue();

        OnDialogueFinished?.Invoke();
        callback?.Invoke();
    }

    private IEnumerator TypeLine(string line)
    {
        dialogueText.text = ""; // Clear current text
        isTyping = true;
        bool skipTyping = false;

        yield return new WaitForSeconds(0.1f);

        foreach (char c in line.ToCharArray())
        {
            if (Input.GetKeyDown(KeyCode.C) && isTyping)
            {
                skipTyping = true;
                dialogueText.text = line;
                break;
            }

            dialogueText.text += c;

            if (!skipTyping)
            {
                yield return new WaitForSeconds(textSpeed);
            }
        }

        isTyping = false;
    }

    private void EndDialogue()
    {
        dialoguePanel.SetActive(false);
    }
}