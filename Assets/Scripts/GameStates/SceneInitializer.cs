
using UnityEngine;
using Unity.Cinemachine;

public class SceneInitializer : MonoBehaviour
{
    [Header("Scene Managers")]
    public ShopManager shopManager;
    public HubManager hubManager;
    public SpawnManager spawnManager;
    public GridManager gridManager; // Single GridManager handles all grids
    public CutsceneManager cutsceneManager;
    public CameraManager cameraManager;
    public StageManager stageManager;
    public UIManager uiManager;
    public EncounterManager encounterManager;
    public TransitionManager transitionManager;
    public EnemyManager enemyManager;
    public StatusEffectManager statusEffectManager;
    
    [Header("Scene UI")]
    public StageUI stageUI;
    public AudioSettingsUI soundSettingsUI;
    public AbilityUI abilityUI;
    public InventoryUI inventoryUI;
    public SaveGameUI saveGameUI;
    public StageSelectUI stageSelectUI;
    public LoadoutUIManager loadoutUIManager;
    public MapUI mapUI;
    
    [Header("Others")]
    public Canvas statDisplayCanvas;
    
    private void Awake()
    {
        EventManager.Instance.StartListening("SceneLoaded", ActivateScene);
    }

    private void OnDestroy()
    {
        EventManager.Instance.StopListening("SceneLoaded", ActivateScene);
    }

    private void ActivateScene(string scene)
    {
        if (scene == "GameScene")
        {
            InitializeSceneManagers();
            AssignEssentials();
        }
    }

    private void InitializeSceneManagers()
    {
        if (gridManager != null)
        {
            gridManager.Initialize();
        }
        else
        {
            Debug.LogWarning("gridManager fail.");
        }

        if (shopManager != null)
        {
            shopManager.Initialize();
        }
        else
        {
            Debug.LogWarning("shopManager fail.");
        }
        
        if (hubManager != null)
        {
            hubManager.Initialize();
        }
        else
        {
            Debug.LogWarning("hubManager fail.");
        }

        if (spawnManager != null)
        {
            spawnManager.Initialize();
        }
        else
        {
            Debug.LogWarning("spawnManager fail.");
        }
        
        if (stageManager != null)
        {
            stageManager.Initialize();
        }
        else
        {
            Debug.LogWarning("stageManager fail.");
        }
        
        if (encounterManager != null)
        {
            encounterManager.Initialize();
        }
        else
        {
            Debug.LogWarning("encounter manager fail.");
        }
        
        if (stageUI != null)
        {
            stageUI.Initialize();
        }
        else
        {
            Debug.LogWarning("stageUI fail.");
        }
        
        if (uiManager != null)
        {
            uiManager.Initialize();
        }
        else
        {
            Debug.LogWarning("gameplayUI fail.");
        }

        if (abilityUI != null)
        {
            abilityUI.Initialize();
        }
        else
        {
            Debug.LogWarning("abilityUI fail.");
        }

        
        if (inventoryUI != null)
        {
            inventoryUI.Initialize();
        }
        else
        {
            Debug.LogWarning("inventoryUI fail.");
        }


        if (soundSettingsUI != null)
        {
            soundSettingsUI.Initialize();
        }
        else
        {
            Debug.LogWarning("soundSettingsUI fail.");
        }

        
        if (saveGameUI != null)
        {
            saveGameUI.Initialize();
        }
        else
        {
            Debug.LogWarning("saveGameUI fail.");
        }

        if (cutsceneManager != null)
        {
            cutsceneManager.Initialize();
        }
        else
        {
            Debug.LogWarning("cutsceneManager fail.");
        }

        if (enemyManager != null)
        {
            enemyManager.Initialize();
        }
        else
        {
            Debug.LogWarning("enemyManager fail.");
        }

        if (cameraManager != null)
        {
            cameraManager.Initialize();
        }
        else
        {
            Debug.LogWarning("cameraManager fail.");
        }

        if (stageSelectUI != null)
        {
            stageSelectUI.Initialize();
        }
        else
        {
            Debug.LogWarning("stageUI fail.");
        }

        if (transitionManager != null)
        {
            transitionManager.Initialize();
        }
        else
        {
            Debug.LogWarning("transitionManager fail.");
        }
        
        if (loadoutUIManager != null)
        {
            loadoutUIManager.Initialize();
        }
        else
        {
            Debug.LogWarning("loadoutUIManager fail.");
        }
        
        if (statusEffectManager != null)
        {
            statusEffectManager.Initialize();
        }
        else
        {
            Debug.LogWarning("statusEffectManager fail.");
        }
        
        if (mapUI != null)
        {
            mapUI.Initialize();
        }
        else
        {
            Debug.LogWarning("encounterUI fail.");
        }
    }

    private void AssignEssentials()
    {
        if (statDisplayCanvas != null)
        {
            PlayerHealthManager.Instance.AssignStatDisplayCanvas(statDisplayCanvas);
            Debug.LogWarning("statDisplayCanvas initialized.");
        }
    }
}