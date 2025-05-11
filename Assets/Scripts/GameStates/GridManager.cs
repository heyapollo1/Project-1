using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }
    public Dictionary<string, SceneGrid> SceneGrids = new Dictionary<string, SceneGrid>();
    private Dictionary<SceneGrid, bool> gridReadiness = new Dictionary<SceneGrid, bool>();
    
    [Header("Grids")]
    public SceneGrid hubGrid;
    public SceneGrid eventGrid;
    public SceneGrid gameGrid;
    public SceneGrid shopGrid;
    public SceneGrid victoryGrid;
    
    [HideInInspector] public SceneGrid activeGrid;
    public SceneGrid GetActiveGrid() => activeGrid;
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        EventManager.Instance.StartListening("GridReady", OnGridReady);
        EventManager.Instance.StartListening("EnterShopMap", SwitchToShopGrid);
        EventManager.Instance.StartListening("EnterGameMap", SwitchToGameGrid);
    }

    private void OnDestroy()
    {
        EventManager.Instance.StopListening("GridReady", OnGridReady);
        EventManager.Instance.StopListening("EnterShopMap", SwitchToShopGrid);
        EventManager.Instance.StopListening("EnterGameMap", SwitchToGameGrid);
    }

    private void OnEnable()
    {
        if (!Application.isPlaying)
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Debug.LogWarning($"Duplicate instance detected: {gameObject.name}. Destroying it.");
                DestroyImmediate(gameObject); // Safely destroy duplicates
                return;
            }
            Debug.Log($"Instance assigned to: {gameObject.name}");
            InitializeInEditMode();
        }
    }
    
    public List<SceneGrid> GetAllGrids()
    {
        List<SceneGrid> grids = new List<SceneGrid>();
        
        if (hubGrid != null) grids.Add(hubGrid);
        if (eventGrid != null) grids.Add(eventGrid);
        if (gameGrid != null) grids.Add(gameGrid);
        if (shopGrid != null) grids.Add(shopGrid);
        if (victoryGrid != null) grids.Add(victoryGrid);
        return grids;
    }
    
    public void Initialize()
    {
        Debug.Log("initialized gridManager.");
        GameManager.Instance.RegisterDependency("GridManager", this);

        gridReadiness.Clear();
        foreach (var grid in GetAllGrids())
        {
            Debug.Log($"FUCK grid: {grid.name}");
            if (grid != null)
            {
                gridReadiness[grid] = false;
                grid.InitializeGrid();
            }
        }
    }


    public void InitializeInEditMode()
    {
        Debug.Log("initialized gridManager.");
        foreach (var grid in GetAllGrids())
        {
            if (grid != null)
            {
                gridReadiness[grid] = false;
                grid.InitializeGrid();
                //grid.boundaryGenerator.GenerateBoundaries(grid);
            }
        }
        
        foreach (var grid in GetAllGrids())
        {
            if (grid != null)
            {
                grid.CreateBoundaries();
            }
        }
    }

    private void OnGridReady(SceneGrid readyGrid)
    {
        if (readyGrid == null)
        {
            Debug.LogError("GridManager: Null grid received in GridReady event.");
            return;
        }

        if (gridReadiness.ContainsKey(readyGrid))
        {
            gridReadiness[readyGrid] = true;
            Debug.Log($"GridManager: {readyGrid.name} is ready.");
        }
        else
        {
            Debug.LogWarning($"GridManager: Received readiness for an unknown grid: {readyGrid.name}");
        }

        if (AreAllGridsReady())
        {
            Debug.Log("GridManager: All grids are now ready, generating boundary walls.");
            foreach (var grid in GetAllGrids())
            {
                if (grid != null)
                {
                    grid.boundaryGenerator.GenerateBoundaries(grid);
                    Debug.Log($"GridManager: {grid.name} is generating boundary walls.");
                }
            }
            EventManager.Instance.TriggerEvent("ActivateFlowFields");
        }
        GameManager.Instance.MarkSystemReady("GridManager");
    }

    private bool AreAllGridsReady()
    {
        foreach (var ready in gridReadiness.Values)
        {
            if (!ready) return false;
        }
        return true;
    }

    private void SetActiveGrid(SceneGrid newGrid)
    {
        if (activeGrid == newGrid) return;

        EventManager.Instance.TriggerEvent("GridDeactivated", activeGrid);
        activeGrid.gameObject.SetActive(false);

        activeGrid = newGrid;
        
        activeGrid.gameObject.SetActive(true);
        EventManager.Instance.TriggerEvent("GridActivated", activeGrid);
    }

    public void InitializeObstacles()
    {
        Obstacle[] obstacles = FindObjectsOfType<Obstacle>();
        foreach (Obstacle obstacle in obstacles)
        {
            obstacle.InitializeObstacle();
        }
        Debug.Log("All obstacles initialized.");
    }

    public void SwitchToShopGrid()
    {
        SetActiveGrid(shopGrid);
        Debug.Log("All obstacles initialized.");
    }

    public void SwitchToGameGrid()
    {
        SetActiveGrid(gameGrid);
        Debug.Log("All obstacles initialized.");
    }

    public Vector3 GetSnappedPosition(Vector3 position, Vector2Int size)
    {
         if (activeGrid == null)
         {
             Debug.LogWarning("No active grid is set.");
             return position;
         }
         Debug.Log($"Snapping position to active grid: {activeGrid.name}");
         return activeGrid.SnappedPosition(position, size);
    }
}