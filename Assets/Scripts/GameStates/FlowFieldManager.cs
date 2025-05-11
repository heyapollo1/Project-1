using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowFieldManager : MonoBehaviour
{
    [SerializeField] private Transform player;
    private Dictionary<SceneGrid, FlowField> flowFields = new Dictionary<SceneGrid, FlowField>();
    private FlowField activeFlowField;
    private SceneGrid activeGrid;

    public float cellSizeMultiplier = 3f;
    private float updateInterval = 0.2f;
    private float updateTimer = 0f;

    private void Awake()
    {
        // Listen for grid events
        EventManager.Instance.StartListening("ActivateFlowFields", OnGridsReady);
        EventManager.Instance.StartListening("GridActivated", OnGridActivated);
        EventManager.Instance.StartListening("GridDeactivated", OnGridDeactivated);
    }

    private void OnDestroy()
    {
        // Stop listening for grid events
        EventManager.Instance.StopListening("ActivateFlowFields", OnGridsReady);
        EventManager.Instance.StopListening("GridActivated", OnGridActivated);
        EventManager.Instance.StopListening("GridDeactivated", OnGridDeactivated);
    }

    private void OnGridsReady()
    {
        Debug.Log("FlowFieldManager: GridReady event received.");
        GameManager.Instance.RegisterDependency("FlowFieldManager", this);

        InitializeFlowFields();
    }

    private void OnGridActivated(SceneGrid grid)
    {
        SetActiveFlowField(grid);
    }

    private void OnGridDeactivated(SceneGrid grid)
    {
        if (activeGrid == grid)
        {
            activeFlowField = null;
            Debug.Log($"FlowFieldManager: Deactivated flow field for grid {grid.name}.");
        }
    }

    private void InitializeFlowFields()
    {
        player = PlayerManager.Instance.playerInstance?.transform;

        if (player == null)
        {
            Debug.LogError("FlowFieldManager: Player Transform is not set or PersistentManager player is null.");
            return;
        }

        // Initialize flow fields for all grids managed by the GridManager
        foreach (var grid in GridManager.Instance.GetAllGrids())
        {
            if (!flowFields.ContainsKey(grid))
            {
                FlowField flowField = new FlowField(grid, player, cellSizeMultiplier);
                flowFields[grid] = flowField;
            }
        }

        activeGrid = GridManager.Instance.GetActiveGrid();
        SetActiveFlowField(activeGrid);

        GameManager.Instance.MarkSystemReady("FlowFieldManager");
        Debug.Log("FlowFieldManager: Flow fields initialized for all grids.");
    }

    private void SetActiveFlowField(SceneGrid newActiveGrid)
    {
        activeGrid = newActiveGrid;

        if (flowFields.TryGetValue(activeGrid, out var newFlowField))
        {
            activeFlowField = newFlowField;
            Debug.Log($"FlowFieldManager: Active flow field set to {activeGrid.name}.");
        }
        else
        {
            Debug.LogError($"FlowFieldManager: No flow field found for grid {activeGrid.name}.");
        }
    }

    private void Update()
    {
        if (activeFlowField == null) return;

        updateTimer -= Time.deltaTime;
        if (updateTimer <= 0f)
        {
            activeFlowField.UpdateFlowField();
            updateTimer = updateInterval;
        }
    }

    private void OnDrawGizmos()
    {
        if (activeFlowField != null)
        {
            activeFlowField.DrawFlowFieldGizmos();
        }
    }

    public FlowField GetActiveFlowField()
    {
        return activeFlowField;
    }
}