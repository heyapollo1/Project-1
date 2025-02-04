using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance { get; private set; }

    public Cutscene portalCutscene;

    //[SerializeField] private GameObject transitionPanel; // The UI canvas that blocks the screen
    private string currentDestination;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void StartTransition(string destination, Transform teleportLocation)
    {
        currentDestination = destination;
        StartCoroutine(HandleTransition(teleportLocation));
    }

    private IEnumerator HandleTransition(Transform teleportLocation)
    {
        yield return new WaitForSeconds(0.2f);

        Debug.Log("Progress Transition");
        EventManager.Instance.TriggerEvent("TravelDeparture", currentDestination);
        CutsceneManager.Instance.StartCutscene(portalCutscene, teleportLocation);

        if (currentDestination == "Game")
        {
            StageManager.Instance.PrepareNextStage();
        }
    }
}