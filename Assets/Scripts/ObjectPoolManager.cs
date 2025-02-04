using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectPoolManager : BaseManager
{
    public static ObjectPoolManager Instance;
    public override int Priority => 30;

    private Dictionary<string, Queue<GameObject>> objectPools;

    protected override void OnInitialize()
    {
        EventManager.Instance.StartListening("SceneLoaded", OnSceneLoaded);
        EventManager.Instance.StartListening("SceneUnloaded", OnSceneUnloaded);
        EventManager.Instance.StartListening("ObjectPoolUpdate", OnObjectPoolUpdate);
        EventManager.Instance.StartListening("TravelDeparture", HandleTransition);
    }

    private void OnDestroy()
    {
        EventManager.Instance.StopListening("SceneLoaded", OnSceneLoaded);
        EventManager.Instance.StopListening("SceneUnloaded", OnSceneUnloaded);
        EventManager.Instance.StopListening("ObjectPoolUpdate", OnObjectPoolUpdate);
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

    public void HandleTransition(string destination)
    {
        if (destination != "Game") return;

        foreach (var pool in objectPools)
        {
            Queue<GameObject> objects = pool.Value;

            while (objects.Count > 0)
            {
                GameObject objectPrefab = objects.Dequeue();
                if (objectPrefab != null)
                {
                    Destroy(objectPrefab);
                }
            }
        }
    }

    private void CleanupPools()
    {
        foreach (var pool in objectPools)
        {
            Queue<GameObject> objects = pool.Value;

            while (objects.Count > 0)
            {
                GameObject objectPrefab = objects.Dequeue();
                if (objectPrefab != null)
                {
                    Destroy(objectPrefab);
                }
            }
        }

        objectPools.Clear();
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
            Debug.LogError($"Pool with tag {tag} doesn't exist.");
            return null;
        }

        Queue<GameObject> pool = objectPools[tag];

        if (pool.Count == 0)
        {
            Debug.LogWarning($"Pool for {tag} is empty. Expanding.");
            ExpandPool(tag, 2);
        }

        GameObject obj = objectPools[tag].Dequeue();

        obj.SetActive(true);
        obj.transform.position = position;
        obj.transform.rotation = rotation;

        DropItem dropItem = obj.GetComponent<DropItem>();//check if its a droppable item
        if (dropItem != null)
        {
            dropItem.ApplyPush();
        }

        objectPools[tag].Enqueue(obj);

        return obj;
    }

    public void ReturnToPool(GameObject objectInstance)
    {
        PooledObject pooledObject = objectInstance.GetComponent<PooledObject>();
        if (pooledObject != null && objectPools.TryGetValue(pooledObject.PoolName, out Queue<GameObject> pool))
        {
            if (objectInstance.activeSelf)
            {
                objectInstance.SetActive(false);
                pool.Enqueue(objectInstance);
                //Debug.Log($"Returned object '{objectInstance.name}' to pool '{pooledObject.PoolName}'.");
            }
            else
            {
                Debug.LogWarning($"Object '{objectInstance.name}' is already inactive. Skipping re-enqueue.");
            }
        }
        else
        {
            Debug.LogWarning($"No pool found for object '{objectInstance.name}'. Destroying object.");
            Destroy(objectInstance);
        }
    }

    public void ExpandPool(string objectName, int additionalSize)
    {
        if (objectPools.TryGetValue(objectName, out Queue<GameObject> pool))
        {
            GameObject objectPrefab = Resources.Load<GameObject>($"Prefabs/{objectName}");
            if (objectPrefab == null)
            {
                //Debug.LogWarning("Cannot expand pool.");
                return;
            }

            for (int i = 0; i < additionalSize; i++)
            {
                GameObject instance = Instantiate(objectPrefab, transform);
                instance.SetActive(false);

                PooledObject pooledObject = instance.AddComponent<PooledObject>();
                pooledObject.PoolName = objectName;
                pool.Enqueue(instance);
            }
            //Debug.LogWarning($"Expanding pool for {objectName} by {additionalSize}");
        }
    }
}
