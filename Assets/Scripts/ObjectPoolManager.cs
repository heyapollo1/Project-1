using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectPoolManager : BaseManager
{
    public static ObjectPoolManager Instance;
    public override int Priority => 30;

    private Dictionary<string, Queue<GameObject>> objectPools;
    private Dictionary<string, List<GameObject>> activeObjects = new Dictionary<string, List<GameObject>>();
    
    protected override void OnInitialize()
    {
        EventManager.Instance.StartListening("SceneLoaded", OnSceneLoaded);
        EventManager.Instance.StartListening("SceneUnloaded", OnSceneUnloaded);
        EventManager.Instance.StartListening("ObjectPoolUpdate", OnObjectPoolUpdate);
        EventManager.Instance.StartListening("PortalCleanup", HandlePortalTransition);
    }

    private void OnDestroy()
    {
        EventManager.Instance.StopListening("SceneLoaded", OnSceneLoaded);
        EventManager.Instance.StopListening("SceneUnloaded", OnSceneUnloaded);
        EventManager.Instance.StopListening("ObjectPoolUpdate", OnObjectPoolUpdate);
        EventManager.Instance.StopListening("PortalCleanup", HandlePortalTransition);
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
            InitializeBaseObjectPools();
        }
    }

    private void OnSceneUnloaded(string sceneName)
    {
        if (sceneName == "GameScene")
        {
            CleanupPools();
        }
    }

    private void InitializeBaseObjectPools()
    {
        objectPools = new Dictionary<string, Queue<GameObject>>();
        string[] baseFX = { "HealingOrb", "Gold", "Bullet" };
        foreach (string objectName in baseFX)
        {
            GameObject objectPrefab = Resources.Load<GameObject>($"Prefabs/{objectName}");
            if (objectPrefab != null)
            {
                CreatePool(objectName, objectPrefab, 10);
            }
            else
            {
                Debug.LogWarning($"Baseline FX {objectName} not found in Resources/Prefabs/FX.");
            }
        }
    }

    public void HandlePortalTransition()
    {
        CleanupPools();
    }
    
    public void ReturnAllActiveObjects()
    {
        foreach (var kvp in activeObjects)
        {
            //string poolName = kvp.Key;
            List<GameObject> activeList = kvp.Value;

            for (int i = activeList.Count - 1; i >= 0; i--) // Iterate backward to avoid removal issues
            {
                GameObject obj = activeList[i];
                ReturnToPool(obj); // Send it back to its respective pool
            }
        }

        activeObjects.Clear(); // Clear the tracking list after returning
        Debug.Log("All active objects have been returned to their pools.");
    }
    
    private void CleanupPools()
    {
        ReturnAllActiveObjects(); 
        
        foreach (var pool in objectPools)
        {
            Queue<GameObject> objects = pool.Value;
            while (objects.Count > 0)
            {
                GameObject objectInstance = objects.Dequeue();
                if (objectInstance != null)
                {
                    Destroy(objectInstance);
                }
            }
        }
        Debug.Log("Enemy pools cleaned up.");
    }

    void OnObjectPoolUpdate(string objectName, int poolSize)
    {
        if (HasPool(objectName))
        {
            ExpandPool(objectName, poolSize);
            Debug.Log($"Hi! {objectName} is already in the Object pool, expanding size by {poolSize}.");
            return;
        }

        GameObject fxPrefab = Resources.Load<GameObject>($"Prefabs/{objectName}");
        if (fxPrefab != null)
        {
            CreatePool(objectName, fxPrefab, poolSize);
            Debug.Log($"Hi! {objectName} FX pool created");
        }
    }

    public void CreatePool(string objectName, GameObject objectPrefab, int poolSize)
    {
        Queue<GameObject> pool = new Queue<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject prefabObject = Instantiate(objectPrefab, transform);
            prefabObject.SetActive(false);

            PooledObject pooledObject = prefabObject.AddComponent<PooledObject>();
            pooledObject.PoolName = objectName;
            pool.Enqueue(prefabObject);
        }

        objectPools.Add(objectName, pool);
        Debug.Log($"Created new pool for {objectName} with size {poolSize}.");
    }

    public void RemovePool(string objectName)
    {
        if (objectPools.TryGetValue(objectName, out Queue<GameObject> pool))
        {

            foreach (var prefabObject in pool)
            {
                if (prefabObject != null)
                {
                    Destroy(prefabObject);
                }
            }

            objectPools.Remove(objectName);
            Debug.Log($"FX pool for {objectName} has been removed.");
        }
        else
        {
            Debug.LogWarning($"FX pool for {objectName} does not exist, cannot remove.");
        }
    }

    public bool HasPool(string objectName)
    {
        return objectPools.ContainsKey(objectName);
    }

    public GameObject GetFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!objectPools.ContainsKey(tag))
        {
            objectPools[tag] = new Queue<GameObject>();
            Debug.LogWarning($"Created new pool for '{tag}'.");
        }

        Queue<GameObject> pool = objectPools[tag];

        if (pool.Count == 0)
        {
            Debug.Log($"Pool '{tag}' empty. Expanding by 5.");
            ExpandPool(tag, 5);

            // Re-check pool after expansion
            if (pool.Count == 0)
            {
                Debug.LogError($"Failed to expand pool for '{tag}'.");
                return null;
            }
        }

        // Now get a bullet from the pool
        GameObject obj = pool.Dequeue();
        if (obj == null)
        {
            Debug.LogError($"Null object retrieved from pool '{tag}'!");
            return null;
        }

        obj.SetActive(true);
        obj.transform.position = position;
        obj.transform.rotation = rotation;

        // Apply push if applicable
        obj.GetComponent<DropItem>()?.ApplyPush();
        if (!activeObjects.ContainsKey(tag))
        {
            activeObjects[tag] = new List<GameObject>();
        }
        activeObjects[tag].Add(obj);

        return obj;
    }

    public void ReturnToPool(GameObject objectInstance)
    {
        if (objectInstance == null)
        {
            Debug.LogWarning("Attempted to return a null object to the pool.");
            return;
        }

        PooledObject pooledObject = objectInstance.GetComponent<PooledObject>();
        if (pooledObject != null && objectPools.TryGetValue(pooledObject.PoolName, out var pool))
        {
            objectInstance.SetActive(false);
            pool.Enqueue(objectInstance);
            if (activeObjects.TryGetValue(pooledObject.PoolName, out var objectList))
            {
                objectList.Remove(objectInstance);
            }
        }
        else
        {
            Debug.LogWarning($"Object '{objectInstance.name}' doesn't belong to a pool. Destroying.");
            Destroy(objectInstance);
        }
    }

    public void ExpandPool(string objectName, int additionalSize)
    {
        if (!objectPools.TryGetValue(objectName, out var pool))
        {
            pool = new Queue<GameObject>();
            objectPools[objectName] = pool;
            Debug.Log($"Created pool for '{objectName}' during expansion.");
        }

        // Load prefab from Resources
        GameObject objectPrefab = Resources.Load<GameObject>($"Prefabs/{objectName}");
        if (objectPrefab == null)
        {
            Debug.LogError($"Prefab '{objectName}' not found in Resources/Prefabs.");
            return;
        }

        // Instantiate new objects and add them to the pool
        for (int i = 0; i < additionalSize; i++)
        {
            GameObject newObj = Instantiate(objectPrefab, transform);
            newObj.SetActive(false);

            // Track pool membership
            var pooledObject = newObj.AddComponent<PooledObject>();
            pooledObject.PoolName = objectName;

            pool.Enqueue(newObj);
        }

        Debug.Log($"Expanded pool '{objectName}' by {additionalSize}. New count: {pool.Count}");
    }
}
