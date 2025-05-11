using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance;
        
    [Header("Spawn Points")]
    [SerializeField] private Transform shopSpawnPoint;
    [SerializeField] private Transform treasureEncounterSpawnPoint;
    [SerializeField] private Transform gameplaySpawnPoint;
    [SerializeField] private Transform victorySpawnPoint;
    [SerializeField] private Transform hubSpawnPoint;

    // Methods to get spawn points
    public Transform GetShopSpawnPoint() => shopSpawnPoint;
    public Transform GetTreasureEncounterSpawnPoint() => treasureEncounterSpawnPoint;
    public Transform GetGameplaySpawnPoint() => gameplaySpawnPoint;
    public Transform GetVictorySpawnPoint() => victorySpawnPoint;
    public Transform GetHubSpawnPoint() => hubSpawnPoint;
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    private void TransitionCleanup()
    {
        //CleanupPools();
    }
    
}
