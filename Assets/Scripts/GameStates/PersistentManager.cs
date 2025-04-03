using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Cinemachine;

public class PersistentManager : MonoBehaviour
{
    public static PersistentManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        LoadPersistentManagers();
        InitializeManagers();
    }
    
    private void LoadPersistentManagers()
    {
        GameObject persistentManagers = GameObject.Find("PersistentManagers");
        if (persistentManagers == null)
        {
            persistentManagers = new GameObject("PersistentManagers");
            DontDestroyOnLoad(persistentManagers);
        }
        
        LoadPersistentManager<CustomSceneManager>("CustomSceneManager", persistentManagers); //10
        LoadPersistentManager<GameManager>("GameManager", persistentManagers); //10
        LoadPersistentManager<EventManager>("EventManager", persistentManagers); //10
        LoadPersistentManager<StageDatabase>("StageDatabase", persistentManagers); //10
        LoadPersistentManager<UpgradeDatabase>("UpgradeDatabase", persistentManagers); //10
        LoadPersistentManager<GameStateManager>("GameStateManager", persistentManagers); //20
        LoadPersistentManager<PlayerAbilityDatabase>("PlayerAbilityDatabase", persistentManagers); //20
        LoadPersistentManager<ItemDatabase>("ItemDatabase", persistentManagers); //20
        //LoadPersistentManager<EnemyManager>("EnemyManager", persistentManagers); //20
        LoadPersistentManager<AudioManager>("AudioManager", persistentManagers); //20
        LoadPersistentManager<FXManager>("FXManager", persistentManagers); //30
        LoadPersistentManager<DeathEffectManager>("DeathEffectManager", persistentManagers); //30
        LoadPersistentManager<ObjectPoolManager>("ObjectPoolManager", persistentManagers); //30
        LoadPersistentManager<EnemyPoolManager>("EnemyPoolManager", persistentManagers); //30
        LoadPersistentManager<InputManager>("InputManager", persistentManagers); //30
        LoadPersistentManager<ExplosionQueueManager>("ExplosionQueueManager", persistentManagers); //30
        LoadPersistentManager<WeaponDatabase>("WeaponDatabase", persistentManagers); //20
        LoadPersistentManager<PlayerManager>("PlayerManager", persistentManagers); //20
    }

    private void LoadPersistentManager<T>(string managerName, GameObject parent) where T : MonoBehaviour
    {
        if (FindObjectOfType<T>() == null)
        {
            GameObject manager = new GameObject(managerName);
            manager.AddComponent<T>();
            manager.transform.SetParent(parent.transform);
            Debug.Log($"{managerName} created and set under {parent.name}");
        }
        else
        {
            Debug.Log($"{managerName} already exists.");
        }
    }

    private void InitializeManagers()
    {
        // Collect all managers that implement IInitializable
        IInitializable[] managers = FindObjectsOfType<MonoBehaviour>().OfType<IInitializable>().ToArray();

        // Sort managers by their priority
        System.Array.Sort(managers, (a, b) => a.Priority.CompareTo(b.Priority));

        foreach (var manager in managers)
        {
            Debug.Log($"Initializing: {manager.GetType().Name} with Priority {manager.Priority}");
            manager.Initialize();
        }
    }
    /*public static PersistentManager Instance { get; private set; }

    [Header("Persistent References")]
    public GameObject player;
    public CinemachineCamera cinemachineCamera;

    public GameObject playerInstance;
    private bool managersReady = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        LoadPersistentManagers();
        InitializeManagers();
    }

    private void Start()
    {
        StartCoroutine(InitializePlayerAndCamera());
    }

    private IEnumerator InitializePlayerAndCamera()
    {
        while (!managersReady)
        {
            Debug.Log("Waiting for managers...");
            yield return null;
        }
        InitializePlayer();
        InitializeCinemachineCamera();
    }

    private void InitializePlayer()
    {
        if (playerInstance == null)
        {
            playerInstance = Instantiate(player);
            DontDestroyOnLoad(playerInstance);
            EventManager.Instance.TriggerEvent("PlayerReady");
            Debug.Log("Player instance created and marked as DontDestroyOnLoad.");
        }
        else
        {
            Debug.Log("Player instance already exists.");
        }
    }

    private void InitializeCinemachineCamera()
    {
        DontDestroyOnLoad(cinemachineCamera.gameObject); // Make camera persistent
        cinemachineCamera.Follow = playerInstance.transform; // Link camera to player
        Debug.Log("Cinemachine Camera initialized and linked to the player.");
    }

    private void LoadPersistentManagers()
    {
        GameObject persistentManagers = GameObject.Find("PersistentManagers");
        if (persistentManagers == null)
        {
            persistentManagers = new GameObject("PersistentManagers");
            DontDestroyOnLoad(persistentManagers);
        }

        // Dynamically load all required managers
        LoadPersistentManager<CustomSceneManager>("CustomSceneManager", persistentManagers); //10
        LoadPersistentManager<GameManager>("GameManager", persistentManagers); //10
        LoadPersistentManager<EventManager>("EventManager", persistentManagers); //10
        LoadPersistentManager<StageManager>("StageManager", persistentManagers); //10
        LoadPersistentManager<UpgradeDatabase>("UpgradeDatabase", persistentManagers); //10
        LoadPersistentManager<UIManager>("UIManager", persistentManagers); //20
        LoadPersistentManager<GameStateManager>("GameStateManager", persistentManagers); //20
        LoadPersistentManager<PlayerAbilityDatabase>("PlayerAbilityDatabase", persistentManagers); //20
        LoadPersistentManager<ItemDatabase>("ItemDatabase", persistentManagers); //20
        LoadPersistentManager<EnemyManager>("EnemyManager", persistentManagers); //20
        LoadPersistentManager<AudioManager>("AudioManager", persistentManagers); //20
        LoadPersistentManager<FXManager>("FXManager", persistentManagers); //30
        LoadPersistentManager<StatusEffectManager>("StatusEffectManager", persistentManagers); //30
        LoadPersistentManager<DeathEffectManager>("DeathEffectManager", persistentManagers); //30
        LoadPersistentManager<ObjectPoolManager>("ObjectPoolManager", persistentManagers); //30
        LoadPersistentManager<EnemyPoolManager>("EnemyPoolManager", persistentManagers); //30
        LoadPersistentManager<InputManager>("InputManager", persistentManagers); //30
        LoadPersistentManager<ExplosionQueueManager>("ExplosionQueueManager", persistentManagers); //30
        LoadPersistentManager<WeaponDatabase>("WeaponDatabase", persistentManagers); //20
        //LoadPersistentManager<CameraManager>("CameraManager", persistentManagers); //30

        managersReady = true;
    }

    private void LoadPersistentManager<T>(string managerName, GameObject parent) where T : MonoBehaviour
    {
        if (FindObjectOfType<T>() == null)
        {
            GameObject manager = new GameObject(managerName);
            manager.AddComponent<T>();
            manager.transform.SetParent(parent.transform);
            Debug.Log($"{managerName} created and set under {parent.name}");
        }
        else
        {
            Debug.Log($"{managerName} already exists.");
        }
    }

    private void InitializeManagers()
    {
        // Collect all managers that implement IInitializable
        IInitializable[] managers = FindObjectsOfType<MonoBehaviour>().OfType<IInitializable>().ToArray();

        // Sort managers by their priority
        System.Array.Sort(managers, (a, b) => a.Priority.CompareTo(b.Priority));

        foreach (var manager in managers)
        {
            Debug.Log($"Initializing: {manager.GetType().Name} with Priority {manager.Priority}");
            manager.Initialize();
        }
    }*/
}