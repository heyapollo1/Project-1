using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }
    public string[] firstWeaponDialogue;

    private bool tutorialActive = true;
    private bool weaponAcquired = false;
    private bool tutorialComplete = false;

    public GameObject hubTeleporter;
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        //EventManager.Instance.StartListening("EndTutorial", CompleteTutorial);
    }

    public bool IsTutorialComplete() => tutorialComplete;

    public void StartTutorialIntro(Transform introCameraPosition)
    {
        EventManager.Instance.TriggerEvent("HideUI");
        CutsceneManager.Instance.StartCutscene("IntroCutscene", introCameraPosition);
    }

    public bool IsTutorialActive()
    {
        return tutorialActive;
    }
    
    public bool hasPickedUpTutorialWeapon
    {
        get { return weaponAcquired; }
    }

    public void TutorialWeaponAcquired()
    {
        if (weaponAcquired) return;
        Debug.Log("tutorial weapon continue");
        weaponAcquired = true;
        DialogueManager.Instance.TriggerDialogue(firstWeaponDialogue, OnFirstWeaponDialogueFinished);
    }

    private void OnFirstWeaponDialogueFinished()
    {
        Debug.Log("Proceeding...");
        hubTeleporter.SetActive(true);
        CompleteTutorial();
        tutorialActive = false;
    }
    
    public void CompleteTutorial()
    {
        tutorialComplete = true;
        Debug.Log("Tutorial marked as complete.");
        EventManager.Instance.TriggerEvent("ShowUI");
        GameData currentData = SaveManager.LoadGame();
        SaveManager.SaveGame(currentData);
    }
    
    public void SetTutorialComplete()
    {
        tutorialComplete = true;
    }
}