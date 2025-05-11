/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitPoint : MonoBehaviour
{
    public GameObject exitPointPrefab;
    private string destinationName;
    private bool isPlayerNearby = false;
    private bool pickingEncounter = false;
    private bool encounterpicked = false;

    public void Initialize()
    {
        exitPointPrefab.SetActive(false);
        EventManager.Instance.StartListening("SpawnExit", ActivateExitPoint);
        EventManager.Instance.StartListening("PortalCleanup", ResetExitPoint);
        //EventManager.Instance.StartListening("StageInitialized", SummonPortal);
    }

    private void OnDestroy()
    {
        EventManager.Instance.StopListening("SpawnExit", ActivateExitPoint);
        EventManager.Instance.StopListening("PortalCleanup", ResetExitPoint);
        //EventManager.Instance.StopListening("StageInitialized", SummonPortal);
    }
    
    private void ActivateExitPoint(Vector3 exitPointLocation)
    {
        Debug.Log($"Exit point summoned to: {destinationName}");
        exitPointPrefab.SetActive(false);
        transform.position = exitPointLocation;
        exitPointPrefab.SetActive(true);
    }
    
    private void Update()
    {
        if (!pickingEncounter && isPlayerNearby && Input.GetKeyDown(KeyCode.C))
        {
            TooltipManager.Instance.HideTooltip();
            pickingEncounter = true;
            isPlayerNearby = false;
            ShowEventChoices();
        }
    }
        
    private void ShowEventChoices()
    {
        var encounterPool = Resources.Load<EncounterPool>("ScriptableObjects/EncounterPool");
        if (encounterPool == null || encounterPool.availableEncounters.Count == 0)
        {
            Debug.LogError("No events found in the pool.");
            return;
        }
        
        List<EncounterData> selectedEncounters = new List<EncounterData>();
        while (selectedEncounters.Count < 2 && selectedEncounters.Count < encounterPool.availableEncounters.Count)
        {
            EncounterData randomEncounter = encounterPool.availableEncounters[Random.Range(0, encounterPool.availableEncounters.Count)];
            if (!selectedEncounters.Contains(randomEncounter))
                selectedEncounters.Add(randomEncounter);
        }
        
        //EncounterSelectUI.Instance.DisplayEncounterChoices(selectedEncounters);
    }

    private void ResetExitPoint()
    {
        pickingEncounter = false;
        encounterpicked = false;
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !encounterpicked)
        {
            isPlayerNearby = true;
            //TooltipManager.Instance.ShowStandardTooltip("Portal", $"Press C to travel to {destinationName}.");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && isPlayerNearby && !encounterpicked)
        {
            isPlayerNearby = false;
            TooltipManager.Instance.HideTooltip();
        }
    }
}*/
