using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }
    public string[] firstWeaponDialogue;

    private bool tutorialActive = true;
    private bool weaponAcquired = false;

    public Transform TutorialExitPortal;
    public Cutscene introCutscene;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void StartTutorialIntro(Transform introCameraPosition)
    {
        EventManager.Instance.TriggerEvent("HideUI");
        CutsceneManager.Instance.StartCutscene(introCutscene, introCameraPosition);
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
        Debug.Log("tutorial weapon continue");
        weaponAcquired = true;
        DialogueManager.Instance.TriggerDialogue(firstWeaponDialogue, OnFirstWeaponDialogueFinished);
    }

    private void OnFirstWeaponDialogueFinished()
    {
        Debug.Log("Proceeding...");
        EventManager.Instance.TriggerEvent("SpawnFixedPortal", "Shop", TutorialExitPortal.position);
    }

    public void EndTutorial()
    {
        EventManager.Instance.TriggerEvent("ShowUI");
        tutorialActive = false;
    }
}