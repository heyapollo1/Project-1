
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXManager : BaseManager
{
    public static FXManager Instance;
    public override int Priority => 30;
    private Dictionary<string, Queue<GameObject>> fxPools = new Dictionary<string, Queue<GameObject>>();

    //private Dictionary<string, Queue<GameObject>> fxPools;

    protected override void OnInitialize()
    {
        EventManager.Instance.StartListening("SceneLoaded", OnSceneLoaded);
        EventManager.Instance.StartListening("SceneUnloaded", OnSceneUnloaded);
        EventManager.Instance.StartListening("FXPoolUpdate", OnFXPoolUpdate);
        EventManager.Instance.StartListening("PortalCleanup", HandlePortalTransition);
    }

    private void OnDestroy()
    {
        EventManager.Instance.StopListening("SceneLoaded", OnSceneLoaded);
        EventManager.Instance.StopListening("SceneUnloaded", OnSceneUnloaded);
        EventManager.Instance.StopListening("FXPoolUpdate", OnFXPoolUpdate);
        EventManager.Instance.StopListening("PortalCleanup", HandlePortalTransition);
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
        if (sceneName != "GameScene")
        {
            CleanupPools();
        }
    }
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void InitializeBaseFXPools()
    {
        string[] baseFX = { "HealingFX", "GoldPickupFX", "BulletImpactFX", "LevelUpFX"};
        foreach (string fxName in baseFX)
        {
            GameObject fxPrefab = Resources.Load<GameObject>($"FX/{fxName}");
            if (fxPrefab != null)
            {
                Debug.LogWarning($"Baseline FX {fxName} is in Resources/Prefabs/FX.");
                CreatePool(fxName, fxPrefab, 5);
            }
            else
            {
                Debug.LogWarning($"Baseline FX {fxName} not found in Resources/Prefabs/FX.");
            }
        }
    }

    public void HandlePortalTransition()
    {
        if (fxPools.Count == 0) return;
        //CleanupPools();
    }

    private void CleanupPools()
    {
        foreach (var pool in fxPools)
        {
            string fxName = pool.Key;
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
        //fxPools.Clear();
        //InitializeBaseFXPools();
        Debug.Log("FX pools cleaned up.");
    }

    public void CreatePool(string fxName, GameObject fxPrefab, int poolSize)
    {
        Debug.Log($"FX Created new pool for {fxName} with size {poolSize}.");
        if (fxPools == null)
        {
            Debug.LogError("fxPools is NULL! Re-initializing...");
            fxPools = new Dictionary<string, Queue<GameObject>>();
        }
        
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
        Debug.Log($"FX Created new pool for {fxName} with size {poolSize}.");
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
            return;
        }

        GameObject fxPrefab = Resources.Load<GameObject>($"FX/{fxName}");
        if (fxPrefab != null)
        {
            CreatePool(fxName, fxPrefab, initialPoolSize);
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
        if (!fxPools.ContainsKey(fxName))
        {
            ExpandPool(fxName, 2);
        }
        
        if (fxPools.TryGetValue(fxName, out Queue<GameObject> pool) && pool.Count > 0)
        {
            GameObject fxInstance = pool.Dequeue();

            if (fxInstance != null)
            {
                fxInstance.SetActive(true);
                fxInstance.transform.position = position;
                fxInstance.transform.localScale = Vector3.one;
                return fxInstance;
            }
            ExpandPool(fxName, 2);
            Debug.LogError($"FX instance from '{fxName}' pool is null!");
        }
        else
        {
            ExpandPool(fxName, 5);
            //GameObject fxInstance = pool.Dequeue();

            if (fxPools.TryGetValue(fxName, out Queue<GameObject> updatedPool) && updatedPool.Count > 0)
            {
                GameObject fxInstance = updatedPool.Dequeue();
                fxInstance.SetActive(true);
                fxInstance.transform.position = position;
                fxInstance.transform.localScale = Vector3.one;
                return fxInstance;
            }
            Debug.LogError($"Failed to retrieve FX from pool: '{fxName}' even after expansion.");
        }
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
                Debug.LogWarning($"Expanding pool by {additionalSize}");
                GameObject fxInstance = Instantiate(fxPrefab, transform);
                fxInstance.SetActive(false);

                PooledObject pooledObject = fxInstance.AddComponent<PooledObject>();
                pooledObject.PoolName = fxName;
                pool.Enqueue(fxInstance);
            }
        }
    }
}