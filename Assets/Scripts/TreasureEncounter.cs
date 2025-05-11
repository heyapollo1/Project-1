using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TreasureEncounter : BaseEncounter
{
    public static TreasureEncounter Instance { get; private set; }
    
    [Header("Treasure Assets")]
    public GameObject chestPrefab;
    public TreasureChest activeChest;
    public Transform chestSpawnLocation;
    [HideInInspector] public StageBracket stageBracket;

    private bool chestOpened = false;
    
    protected override void Awake()
    {
        base.Awake();
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    public override void InitializeEncounter()
    {
        GameData gameData = SaveManager.LoadGame();
        if (gameData.isNewGame) 
        {
            Debug.Log("Starting a new game. Deactivating treasure map.");
            entryPoint = TransitionManager.Instance.GetTreasureEncounterSpawnPoint();
            encounterIsActive = false;
            encounterMap.SetActive(encounterIsActive);
        }
    }

    public override void PrepareEncounter()
    {
        Debug.Log("Treasure Encounter activated.");
        if (encounterIsActive) return;
        encounterIsActive = true;
        stageBracket = CurrentStageBracket();
        encounterMap.SetActive(true);
        EncounterManager.Instance.SaveEncounterState(encounterData);

        CreateTreasureChest(stageBracket);
    }
    
    public override void EnterEncounter()
    {
        Debug.Log($"Encounter entered: {encounterData.name}");
        MapUI.Instance.EnableMapUI();
        AudioManager.Instance.PlayBackgroundMusic("Music_Shopping");
        EventManager.Instance.TriggerEvent("ShowUI");
        
        GameData currentData = SaveManager.LoadGame();
        SaveManager.SaveGame(currentData);
    }

    private void CreateTreasureChest(StageBracket stageBracket)
    {
        GameObject spawnedChest = Instantiate(chestPrefab, chestSpawnLocation.position, Quaternion.identity);
        activeChest = spawnedChest.GetComponentInChildren<TreasureChest>();
        activeChest.Initialize();
        activeChest.AddRandomReward(stageBracket);
    }
    
    public override void EndEncounter()
    {
        if (!encounterIsActive) return;
        encounterIsActive = false;
        ClearChest();
        EncounterManager.Instance.ExitEncounterState();
        encounterMap.SetActive(false);
    }

    private void ClearChest()
    {
        Debug.LogError("Clearing treasure encounter");
        activeChest.DestroyTreasureChest();
    }
    
    public override void LoadEncounterState(WorldState state)
    {
        
    }
}
