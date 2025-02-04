using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    [Header("Grids")]
    public SceneGrid gameGrid;
    public SceneGrid shopGrid;
    public SceneGrid victoryGrid;

    [Header("Boundaries")]
    public BoundaryGenerator gameBoundary;
    public BoundaryGenerator shopBoundary;
    public BoundaryGenerator victoryBoundary;

    private Dictionary<SceneGrid, bool> gridReadiness = new Dictionary<SceneGrid, bool>();
    [HideInInspector] public SceneGrid activeGrid;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        EventManager.Instance.StartListening("GridReady", OnGridReady);
        EventManager.Instance.StartListening("EnterShopMap", SwitchToShopGrid);
        EventManager.Instance.StartListening("EnterGameMap", SwitchToGameGrid);
        //EventManager.Instance.StartListening("StageTransition", CleanupPools);
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
                Debug.Log($"Instance assigned tdo: {gameObject.name}");
            }
            else if (Instance != this)
            {
                Debug.LogWarning($"Duplicate instance detected: {gameObject.name}. Destroying it.");
                DestroyImmediate(gameObject); // Safly destry duplicates
                return;
            }
            Debug.Log($"Instance assigned to: {gameObject.name}");
            InitializeInEditMode();
        }
    }

    public void Initialize()
    {
        Debug.Log("initialized gridManager.");
        GameManager.Instance.RegisterDependency("GridManager", this);

        gridReadiness.Clear();
        foreach (var grid in GetAllGrids())
        {
            gridReadiness[grid] = false; // Start as not ready
        }

        if (gameGrid != null)
        {
            gameGrid.InitializeGrid(gameGrid);
        }
        if (shopGrid != null)
        {
            shopGrid.InitializeGrid(shopGrid);
        }
        if (victoryGrid != null)
        {
            victoryGrid.InitializeGrid(victoryGrid);
        }
        //SetActiveGrid(gameGrid); // default
    }


    public void InitializeInEditMode()
    {
        Debug.Log("initialized gridManager.");
        if (gameGrid != null)
        {
            gameGrid.InitializeGrid(gameGrid);
        }
        if (shopGrid != null)
        {
            shopGrid.InitializeGrid(shopGrid);
        }
        if (victoryGrid != null)
        {
            victoryGrid.InitializeGrid(victoryGrid);
        }

        if (gameBoundary != null && shopBoundary != null && victoryBoundary != null)
        {
            gameBoundary.TryGenerateBoundaries();
            shopBoundary.TryGenerateBoundaries();
            victoryBoundary.TryGenerateBoundaries();
            Debug.Log("initialized boundaries.");
        }
        else
        {
            Debug.LogWarning("boundaries problem.");
        }
    }

    private void OnGridReady(SceneGrid readyGrid)
    {
        if (readyGrid == null)
        {
            Debug.LogError("GridManager: Null grid received in GridReady event.");
            return;
        }

        // Mark the grid as ready
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
            Debug.Log("GridManager: All grids are ready.");
            gameBoundary.TryGenerateBoundaries();
            shopBoundary.TryGenerateBoundaries();
            victoryBoundary.TryGenerateBoundaries();
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

        if (activeGrid != null)
        {
            // Deactivate the current grid
            EventManager.Instance.TriggerEvent("GridDeactivated", activeGrid);
            activeGrid.gameObject.SetActive(false);
        }

        activeGrid = newGrid;

        if (activeGrid != null)
        {
            // Activate the new grid
            activeGrid.gameObject.SetActive(true);
            EventManager.Instance.TriggerEvent("GridActivated", activeGrid);
        }
    }

    public void InitializeObstacles()
    {
        Obstacle[] obstacles = FindObjectsOfType<Obstacle>();
        foreach (Obstacle obstacle in obstacles)
        {
            //Debug.Log($"Initializing obstacle: {obstacle.name}");
            obstacle.InitializeObstacle();
        }
        Debug.Log("All obstacles initialized.");
    }

    public List<SceneGrid> GetAllGrids()
    {
        List<SceneGrid> grids = new List<SceneGrid>();

        if (gameGrid != null) grids.Add(gameGrid);
        if (shopGrid != null) grids.Add(shopGrid);
        if (victoryGrid != null) grids.Add(victoryGrid);

        return grids;
    }

    public SceneGrid GetShopGrid()
    {
        return shopGrid;
    }

    public SceneGrid GetGameGrid()
    {
        return gameGrid;
    }

    public SceneGrid GetActiveGrid()
    {
        return activeGrid;
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