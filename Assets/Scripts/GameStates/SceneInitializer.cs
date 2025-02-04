
using UnityEngine;
using Unity.Cinemachine;

public class SceneInitializer : MonoBehaviour
{
    [Header("Player Setup")]
    public Transform player;

    [Header("Maps")]
    public GameObject gameMap;
    public GameObject shopMap;
    public GameObject victoryMap;

    [Header("Scene Managers")]
    public ShopManager shopManager;
    public SpawnManager spawnManager;
    public GridManager gridManager; // Single GridManager handles both grids
    public CutsceneManager cutsceneManager;
    public CameraManager cameraManager;

    [Header("Others")]
    public StageUI stageUI;
    public SoundSettingsUI soundUI;
    public AbilityUI abilityUI;
    public Portal portal;
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
            Debug.LogWarning("gridManager initialized.");
        }
        else
        {
            Debug.LogWarning("gridManager fail.");
        }

        if (shopManager != null)
        {
            shopManager.Initialize();
            Debug.LogWarning("ShopManager initialized.");
        }
        else
        {
            Debug.LogWarning("shopManager fail.");
        }

        if (spawnManager != null)
        {
            spawnManager.Initialize();
            Debug.LogWarning("spawnManager initialized.");
        }

        if (stageUI != null)
        {
            stageUI.Initialize();
            Debug.LogWarning("stageUI initialized.");
        }

        if (abilityUI != null)
        {
            abilityUI.Initialize();
            Debug.LogWarning("abilityUI initialized.");
        }

        if (soundUI != null)
        {
            soundUI.Initialize();
            Debug.LogWarning("stageUI initialized.");
        }

        if (portal != null)
        {
            portal.Initialize();
            Debug.LogWarning("portal initialized.");
        }

        if (cutsceneManager != null)
        {
            cutsceneManager.Initialize();
            Debug.LogWarning("cutscenemanager initialized.");
        }

        if (cameraManager != null)
        {
            cameraManager.Initialize();
            Debug.LogWarning("cameraManager initialized.");
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
    /*private void HandleMapPreparation(string destination)
    {
        if (destination == "Game")
        {
            gameMap.SetActive(true);
            shopMap.SetActive(false);
        }

        if (destination == "Shop")
        {
            gameMap.SetActive(false);
            shopMap.SetActive(true);
        }
    }*/
}