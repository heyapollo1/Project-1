using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance;
        
    [Header("Spawn Points")]
    [SerializeField] private Transform shopSpawnPoint;
    [SerializeField] private Transform gameplaySpawnPoint;
    [SerializeField] private Transform victorySpawnPoint;
    [SerializeField] private Transform hubSpawnPoint;

    // Methods to get spawn points
    public Transform GetShopSpawnPoint() => shopSpawnPoint;
    public Transform GetGameplaySpawnPoint() => gameplaySpawnPoint;
    public Transform GetVictorySpawnPoint() => victorySpawnPoint;
    public Transform GetHubSpawnPoint() => hubSpawnPoint;
    
    public void Initialize()
    {
        
    }
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
}
