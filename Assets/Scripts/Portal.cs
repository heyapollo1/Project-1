/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [Header("Spawn Points")]
    public Transform shopSpawnPoint;
    public Transform gameplaySpawnPoint;
    public Transform victorySpawnPoint;
    public Transform hubSpawnPoint;

    [Header("Portal Essentials")]
    public GameObject portalObject;
    public ParticleSystem portalFXDeparture;
    public ParticleSystem portalFXArrival;
    
    [Header("Portal Cutscenes")]
    public Cutscene portalCutscene;
    public Cutscene tutorialPortalCutscene;
    public Cutscene gameplayPortalCutscene;
    public Cutscene hubPortalCutscene;
    
    [HideInInspector] public string currentDestination;
    [HideInInspector] public bool isPlayerNearby = false;
    [HideInInspector] public bool enteredPortal = false;

    public void Initialize()
    {
        Debug.Log("Portal Initialized");
        EventManager.Instance.StartListening("SpawnPortal", SummonPortal);
        EventManager.Instance.StartListening("PortalCleanup", HandlePortalTransition);
        portalObject.SetActive(false);
    }

    private void OnDestroy()
    {
        EventManager.Instance.StopListening("SpawnPortal", SummonPortal);
        EventManager.Instance.StartListening("PortalCleanup", HandlePortalTransition);
    }

    private void SummonPortal(string destinationName, Vector3 portalEntryLocation)
    {
        Debug.Log($"Portal summoned to: {destinationName}");
        portalObject.SetActive(false);
        currentDestination = destinationName;
        transform.position = portalEntryLocation;
        portalObject.SetActive(true);
    }
    
    private void Update()
    {
        if (!enteredPortal && isPlayerNearby && Input.GetKeyDown(KeyCode.C))
        {
            enteredPortal = true;
            isPlayerNearby = false;
            EnterPortal();
        }
    }
    
    public void EnterPortal()
    {
        portalFXDeparture.Play();
        TooltipManager.Instance.HideTooltip();
        EnemyManager.Instance.DisableAllEnemies();
        CutsceneManager.Instance.RepositionCutsceneCamera(transform.position);
        AudioManager.Instance.PlayUISound("Player_TeleportDeparture");
        
         Debug.Log("Portal activated.");
        if (currentDestination == "Shop")
        {
            StageUI.Instance.DisableStageUI();
            StartCoroutine(PortalTransition(shopSpawnPoint));
        }
        else if (currentDestination == "Game")
        {
            StartCoroutine(PortalTransition(gameplaySpawnPoint));
        }
        else if (currentDestination == "Victory")
        {
            StageUI.Instance.DisableStageUI();
            StartCoroutine(PortalTransition(victorySpawnPoint));
        }
        else if (currentDestination == "Hub")
        {
            StageUI.Instance.DisableStageUI();
            StartCoroutine(PortalTransition(hubSpawnPoint));
        }
    }

    private IEnumerator PortalTransition(Transform teleportLocation)
    {
        Debug.Log("Entering Portal");
        yield return new WaitForSeconds(0.2f);

        if (TutorialManager.Instance.IsTutorialActive())
        {
            CutsceneManager.Instance.StartCutscene("TeleportTutorialCutscene", teleportLocation);
        }
        else if (currentDestination == "Game")
        {
            CutsceneManager.Instance.StartCutscene("TeleportStageCutscene", teleportLocation);
        }
        else if (currentDestination == "Hub")
        {
            Debug.Log("Portal to gameplay");
            CutsceneManager.Instance.StartCutscene("TeleportHubCutscene", teleportLocation);
        }
        else if (currentDestination == "Shop")
        {
            Debug.Log("Portal regular transit");
            CutsceneManager.Instance.StartCutscene("TeleportShopCutscene", teleportLocation);
        }
        enteredPortal = false;
    }
    
    private IEnumerator SwitchBGMusic()
    {
        switch (currentDestination)
        {
            case "Shop":
            {
                AudioManager.Instance.PlayBackgroundMusic("Music_Shopping");
                yield break;
            }
            case "Game":
            {
                AudioManager.Instance.PlayBackgroundMusic("Music_Gameplay");
                yield break;
            }
            case "Victory":
            {
                AudioManager.Instance.PlayBackgroundMusic("Music_Victory");
                yield break;
            }
            case "Hub":
            {
                AudioManager.Instance.PlayBackgroundMusic("Music_Shopping");
                yield break;
            }
        }
    }

    private void HandlePortalTransition()
    {
        StartCoroutine(SwitchBGMusic());
        enteredPortal = false;
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !enteredPortal)
        {
            isPlayerNearby = true;
            //TooltipManager.Instance.ShowStandardTooltip("Portal", "Press C to travel");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = false;
            TooltipManager.Instance.HideTooltip();
        }
    }

}*/