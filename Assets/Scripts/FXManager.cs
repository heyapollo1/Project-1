
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXManager : BaseManager
{
    public static FXManager Instance;
    public override int Priority => 30;

    //[SerializeField] private List<FXData> fxEntries;
    private Dictionary<string, Queue<GameObject>> fxPools;

    protected override void OnInitialize()
    {
        EventManager.Instance.StartListening("SceneLoaded", OnSceneLoaded);
        EventManager.Instance.StartListening("SceneUnloaded", OnSceneUnloaded);
        EventManager.Instance.StartListening("FXPoolUpdate", OnFXPoolUpdate);
        EventManager.Instance.StartListening("TravelDeparture", HandleTransition);
    }

    private void OnDestroy()
    {
        EventManager.Instance.StopListening("SceneLoaded", OnSceneLoaded);
        EventManager.Instance.StopListening("SceneUnloaded", OnSceneUnloaded);
        EventManager.Instance.StopListening("FXPoolUpdate", OnFXPoolUpdate);
        EventManager.Instance.StopListening("TravelDeparture", HandleTransition);
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void OnSceneLoaded(string sceneName)
    {
        if (sceneName == "GameScene")
        {
            InitializeBaseFXPools();
        }
    }

    private void OnSceneUnloaded(string sceneName)
    {
        if (sceneName == "GameScene")
        {
            CleanupPools();
        }
    }

    private void InitializeBaseFXPools()
    {
        fxPools = new Dictionary<string, Queue<GameObject>>();
        string[] baseFX = { "HealingFX", "GoldPickupFX", "BulletImpactFX", "LevelUpFX", "BurningFX", "BleedingFX"};
        foreach (string fxName in baseFX)
        {
            GameObject fxPrefab = Resources.Load<GameObject>($"FX/{fxName}");
            if (fxPrefab != null)
            {
                CreatePool(fxName, fxPrefab, 5);
            }
            else
            {
                Debug.LogWarning($"Baseline FX {fxName} not found in Resources/Prefabs/FX.");
            }
        }
    }

    public void HandleTransition(string destination)
    {
        if (destination != "Game") return;

        foreach (var pool in fxPools)
        {
            Queue<GameObject> fxPrefabs = pool.Value;

            while (fxPrefabs.Count > 0)
            {
                GameObject fxPrefab = fxPrefabs.Dequeue();
                if (fxPrefab != null)
                {
                    Destroy(fxPrefab);
                }
            }
        }
    }

    private void CleanupPools()
    {
        foreach (var pool in fxPools)
        {
            Queue<GameObject> fxPrefabs = pool.Value;

            while (fxPrefabs.Count > 0)
            {
                GameObject fxPrefab = fxPrefabs.Dequeue();
                if (fxPrefab != null)
                {
                    Destroy(fxPrefab);
                }
            }
        }

        fxPools.Clear();
        Debug.Log("FX pools cleaned up.");
    }

    public void CreatePool(string fxName, GameObject fxPrefab, int poolSize)
    {
        if (fxPools.ContainsKey(fxName))
        {
            Debug.LogWarning($"Pool for {fxName} already exists.");
            return;
        }

        Queue<GameObject> pool = new Queue<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject fxInstance = Instantiate(fxPrefab, transform);
            fxInstance.SetActive(false);

            PooledObject pooledObject = fxInstance.AddComponent<PooledObject>();
            pooledObject.PoolName = fxName;
            pool.Enqueue(fxInstance);

            AutoDestroyParticle autoDestroy = fxInstance.GetComponent<AutoDestroyParticle>();
            if (autoDestroy != null)
            {
                autoDestroy.Initialize(fxName);
            }
            else
            {
                Debug.LogWarning($"No AutoDestroyParticle component found on {fxPrefab.name}.");
            }
        }

        fxPools.Add(fxName, pool);
        Debug.Log($"Created new pool for {fxName} with size {poolSize}.");
    }

    public void RemovePool(string fxName)
    {
        if (fxPools.TryGetValue(fxName, out Queue<GameObject> pool))
        {

            foreach (var fxObject in pool)
            {
                if (fxObject != null)
                {
                    Destroy(fxObject);
                }
            }

            fxPools.Remove(fxName);
            Debug.Log($"FX pool for {fxName} has been removed.");
        }
        else
        {
            Debug.LogWarning($"FX pool for {fxName} does not exist, cannot remove.");
        }
    }

    void OnFXPoolUpdate(string fxName, int initialPoolSize)
    {
        if (HasPool(fxName))
        {
            ExpandPool(fxName, initialPoolSize);
            Debug.Log($"{fxName} is already in the FX pool.");
            return;
        }

        GameObject fxPrefab = Resources.Load<GameObject>($"FX/{fxName}");
        if (fxPrefab != null)
        {
            CreatePool(fxName, fxPrefab, initialPoolSize);
            Debug.Log($"{fxName} FX pool created");
        }
        else
        {
            Debug.LogWarning($"Failed to load {fxName} FX");
        }
    }

    public bool HasPool(string fxName)
    {
        return fxPools.ContainsKey(fxName);
    }

    public GameObject PlayFX(string fxName, Vector3 position)
    {
        if (!fxPools.TryGetValue(fxName, out Queue<GameObject> pool) || pool.Count == 0)
        {
            Debug.LogWarning($"FX pool for {fxName} is empty. Expanding.");
            ExpandPool(fxName, 2);
        }

        if (pool.Count > 0)
        {
            GameObject fxInstance = pool.Dequeue();
            fxInstance.SetActive(true);
            fxInstance.transform.position = position;
            return fxInstance;
        }

        Debug.LogError($"Failed to retrieve FX from pool: {fxName}");
        return null;
    }

    public void ReturnToPool(GameObject fxInstance, string fxName)
    {
        PooledObject pooledObject = fxInstance.GetComponent<PooledObject>();
        if (pooledObject != null && fxPools.TryGetValue(pooledObject.PoolName, out Queue<GameObject> pool))
        {
            fxInstance.SetActive(false);
            pool.Enqueue(fxInstance);
        }
    }

    public void ExpandPool(string fxName, int additionalSize)
    {
        if (fxPools.TryGetValue(fxName, out Queue<GameObject> pool))
        {
            GameObject fxPrefab = Resources.Load<GameObject>($"FX/{fxName}");
            if (fxPrefab == null)
            {
                Debug.LogWarning("Cannot expand pool.");
                return;
            }

            for (int i = 0; i < additionalSize; i++)
            {
                GameObject fxInstance = Instantiate(fxPrefab, transform);
                fxInstance.SetActive(false);

                PooledObject pooledObject = fxInstance.AddComponent<PooledObject>();
                pooledObject.PoolName = fxName;
                pool.Enqueue(fxInstance);
            }
        }
    }
}