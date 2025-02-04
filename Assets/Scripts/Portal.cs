using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [Header("Spawn Points")]
    public Transform shopSpawnPoint;
    public Transform gameplaySpawnPoint;
    public Transform victorySpawnPoint;

    [Header("Portal Prefab")]
    public ParticleSystem portalFXDeparture;
    public ParticleSystem portalFXArrival;
    public Cutscene portalCutscene;

    [HideInInspector] public string currentDestination;
    [HideInInspector] public bool isPlayerNearby = false;
    [HideInInspector] public bool enteredPortal = false;

    public void Initialize()
    {
        Debug.LogWarning("Initialized the portal");

        EventManager.Instance.StartListening("SpawnPortal", ActivatePortal);
        EventManager.Instance.StartListening("SpawnFixedPortal", ActivateFixedPortal);
    }

    private void OnDestroy()
    {
        EventManager.Instance.StopListening("SpawnPortal", ActivatePortal);
        EventManager.Instance.StopListening("SpawnFixedPortal", ActivateFixedPortal);
    }

    private void Update()
    {
        if (!enteredPortal && isPlayerNearby && Input.GetKeyDown(KeyCode.C))
        {
            enteredPortal = true;
            HandlePlayerEnter();
        }
    }

    private void ActivateFixedPortal(string destination, Vector3 destinationLocation)
    {
        currentDestination = destination;

        transform.position = destinationLocation;
        Debug.Log("Portal activated.");
    }

    private void ActivatePortal(string destination)
    {
        currentDestination = destination;

        Vector3 portalSpawnLocation = SpawnManager.Instance.GetValidSpawnPosition();
        transform.position = portalSpawnLocation;
    }

    public void HandlePlayerEnter()
    {
        portalFXDeparture.Play();
        CutsceneManager.Instance.RepositionCutsceneCamera(transform.position);
         Debug.Log("Portal activated.");
        if (currentDestination == "Shop")
        {
            EnemyManager.Instance.EndStage();
            StageUI.Instance.DisableStageUI();
            StartCoroutine(PortalTransition(shopSpawnPoint));
        }
        else if (currentDestination == "Game")
        {
            StartCoroutine(PortalTransition(gameplaySpawnPoint));
        }
        else if (currentDestination == "Victory")
        {
            StartCoroutine(PortalTransition(victorySpawnPoint));
        }
    }

    private IEnumerator PortalTransition(Transform teleportLocation)
    {
        AudioManager.Instance.PlayUISound("Player_TeleportDeparture");
        yield return new WaitForSeconds(0.2f);

        Debug.Log("Progress Transition");
        EventManager.Instance.TriggerEvent("TravelDeparture", currentDestination);
        CutsceneManager.Instance.StartCutscene(portalCutscene, teleportLocation);

        if (currentDestination == "Game")
        {
            StageManager.Instance.PrepareNextStage();
        }

        portalFXArrival.Play();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !enteredPortal)
        {
            isPlayerNearby = true;
            Tooltip.Instance.ShowTooltip("Portal", "Press C to travel");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = false;
            Tooltip.Instance.HideTooltip();
        }
    }

}