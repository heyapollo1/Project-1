using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class UIContainer : BaseManager
{
    public static UIContainer Instance { get; private set; }
    public override int Priority => 15;

    [Header("General UI")]
    public Slider dodgeCooldownSlider;
    public Slider xpSlider;
    public Slider healthSlider;
    public TextMeshProUGUI currencyText;
    public TextMeshProUGUI dodgeCooldownText;
    public TextMeshProUGUI xpText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI playerLevelText;

    [Header("Panels and Screens")]
    public GameObject gameOverScreen;
    public Button rerollButton;
    public Button reviveButton;
    public Button resetButton;

    [Header("Upgrade UI")]
    public GameObject upgradePanel;
    public GameObject upgradeCardPrefab;
    public Transform upgradeContainer;

    [Header("Abilities UI")]
    public GameObject abilityPanel;
    public GameObject abilityIconPrefab;
    public Transform abilityIconContainer;
    public Slider abilitySlider;

    [Header("Inventory UI")]
    public GameObject inventoryPanel;
    public GameObject inventorySlotPrefab;
    public Transform inventorySlotContainer;

    [Header("Main Menu UI")]
    public GameObject mainMenuPanel;
    public Button playButton;
    public Button quitButton;

    [Header("Pause Menu UI")]
    public GameObject pauseMenuPanel;
    public Button pauseResumeButton;
    public Button pauseResetButton;
    public Button pauseReturnToMainMenuButton;

    private bool isGameplayUIActive = false;

    protected override void OnInitialize()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

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
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainMenu")
        {
            Debug.Log("Main Menu Scene loaded. Initializing UI references.");
            InitializeMainMenuUI();
            EnableMainMenuUI();
            DisableGameplayUI();
            DisablePauseUI();
        }
        else if (scene.name == "GameScene")
        {
            Debug.Log("Gameplay Scene loaded. Initializing UI references.");
            InitializeGameSceneUI();
            InitializePauseMenuUI();
            EnablePauseUI();
            EnableGameplayUI();
            DisableMainMenuUI();
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void InitializeMainMenuUI()
    {
        mainMenuPanel = GameObject.Find("MainMenuPanel");
        playButton = GameObject.Find("PlayButton")?.GetComponent<Button>();
        quitButton = GameObject.Find("QuitButton")?.GetComponent<Button>();

        if (mainMenuPanel == null || playButton == null || quitButton == null)
        {
            Debug.LogError("Main Menu UI elements not found. Ensure they exist in the Main Menu scene.");
            return;
        }

        EnableMainMenuUI();
        Debug.Log("Main Menu UI initialized.");
    }

    public void InitializePauseMenuUI()
    {
        pauseMenuPanel = GameObject.Find("PauseScreen");
        pauseResumeButton = GameObject.Find("ResumeButton")?.GetComponent<Button>();
        pauseResetButton = GameObject.Find("PauseRestartButton")?.GetComponent<Button>();
        pauseReturnToMainMenuButton = GameObject.Find("MenuButton")?.GetComponent<Button>();

        if (pauseMenuPanel == null || pauseResumeButton == null || pauseResetButton == null || pauseReturnToMainMenuButton == null)
        {
            Debug.LogError("Pause Menu UI elements not found. Ensure they exist in the scene.");
            return;
        }
        pauseMenuPanel.SetActive(false);
    }

    public void InitializeGameSceneUI()
    {
        dodgeCooldownSlider = GameObject.Find("DodgeSkill")?.GetComponent<Slider>();
        xpSlider = GameObject.Find("XPBar")?.GetComponent<Slider>();
        healthSlider = GameObject.Find("HealthBar")?.GetComponent<Slider>();
        currencyText = GameObject.Find("PlayerCurrency")?.GetComponent<TextMeshProUGUI>();
        dodgeCooldownText = GameObject.Find("DodgeCooldownText")?.GetComponent<TextMeshProUGUI>();
        xpText = GameObject.Find("XPText")?.GetComponent<TextMeshProUGUI>();
        healthText = GameObject.Find("HealthText")?.GetComponent<TextMeshProUGUI>();
        playerLevelText = GameObject.Find("PlayerLevelText")?.GetComponent<TextMeshProUGUI>();

        gameOverScreen = FindInactiveObject("GameOverScreen");
        rerollButton = FindInactiveObject("RerollButton")?.GetComponent<Button>();
        reviveButton = FindInactiveObject("ReviveButton")?.GetComponent<Button>();
        resetButton = FindInactiveObject("ResetButton")?.GetComponent<Button>();

        upgradePanel = FindInactiveObject("UpgradePanel");
        upgradeCardPrefab = Resources.Load<GameObject>("Prefabs/UpgradeCard");
        upgradeContainer = FindInactiveObject("UpgradeContainer")?.transform;

        abilityPanel = FindInactiveObject("AbilityPanel");
        abilityIconPrefab = Resources.Load<GameObject>("Prefabs/AbilityUIContainer");
        abilityIconContainer = GameObject.Find("AbilityPanel")?.transform;
        abilitySlider = FindInactiveObject("AbilityUIContainer")?.GetComponent<Slider>();

        inventoryPanel = GameObject.Find("InventoryPanel");
        inventorySlotPrefab = Resources.Load<GameObject>("Prefabs/ItemSlot");
        inventorySlotContainer = GameObject.Find("InventoryPanel")?.transform;

        Debug.Log("UIContainer initialized references.");
    }

    private void EnableGameplayUI()
    {
        if (isGameplayUIActive) return;

        // Enable all gameplay-related UI elements
        SetUIElementActive(dodgeCooldownSlider?.gameObject, true);
        SetUIElementActive(xpSlider?.gameObject, true);
        SetUIElementActive(healthSlider?.gameObject, true);
        SetUIElementActive(currencyText?.gameObject, true);
        SetUIElementActive(dodgeCooldownText?.gameObject, true);
        SetUIElementActive(xpText?.gameObject, true);
        SetUIElementActive(healthText?.gameObject, true);
        SetUIElementActive(playerLevelText?.gameObject, true);
        SetUIElementActive(inventoryPanel, true);
        SetUIElementActive(inventorySlotContainer?.gameObject, true);
        SetUIElementActive(abilityIconContainer?.gameObject, true);

        SetDefaultInactiveStates();
        isGameplayUIActive = true;
        Debug.Log("Gameplay UI enabled.");
    }

    private void SetDefaultInactiveStates()
    {
        SetUIElementActive(gameOverScreen, false);
        SetUIElementActive(upgradePanel, false);
        SetUIElementActive(upgradeContainer?.gameObject, false);
        SetUIElementActive(abilityPanel, false);
        SetUIElementActive(abilitySlider?.gameObject, false);
        SetUIElementActive(rerollButton?.gameObject, false);
        SetUIElementActive(reviveButton?.gameObject, false);
        SetUIElementActive(resetButton?.gameObject, false);

        Debug.Log("Default inactive states set for gameplay UI.");
    }

    private void DisableGameplayUI()
    {
        if (!isGameplayUIActive) return;

        SetUIElementActive(dodgeCooldownSlider?.gameObject, false);
        SetUIElementActive(xpSlider?.gameObject, false);
        SetUIElementActive(healthSlider?.gameObject, false);
        SetUIElementActive(currencyText?.gameObject, false);
        SetUIElementActive(dodgeCooldownText?.gameObject, false);
        SetUIElementActive(xpText?.gameObject, false);
        SetUIElementActive(healthText?.gameObject, false);
        SetUIElementActive(playerLevelText?.gameObject, false);
        SetUIElementActive(inventoryPanel, false);
        SetUIElementActive(inventorySlotContainer?.gameObject, false);
        SetUIElementActive(abilityIconContainer?.gameObject, false);
        SetUIElementActive(gameOverScreen, false);
        SetUIElementActive(upgradePanel, false);
        SetUIElementActive(upgradeContainer?.gameObject, false);
        SetUIElementActive(abilityPanel, false);
        SetUIElementActive(abilitySlider?.gameObject, false);
        SetUIElementActive(rerollButton?.gameObject, false);
        SetUIElementActive(reviveButton?.gameObject, false);
        SetUIElementActive(resetButton?.gameObject, false);

        Debug.Log("Gameplay UI disabled.");
        isGameplayUIActive = false;
    }

    private void EnableMainMenuUI()
    {
        if (mainMenuPanel == null || playButton == null || quitButton == null)
        {
            Debug.LogWarning("Main Menu UI elements are null. Reinitializing...");
            InitializeMainMenuUI();
        }

        SetUIElementActive(mainMenuPanel?.gameObject, true);
        SetUIElementActive(playButton?.gameObject, true);
        SetUIElementActive(quitButton?.gameObject, true);
        Debug.Log("Main Menu UI enabled.");
    }

    private void DisableMainMenuUI()
    {
        SetUIElementActive(mainMenuPanel?.gameObject, false);
        SetUIElementActive(playButton?.gameObject, false);
        SetUIElementActive(quitButton?.gameObject, false);
        Debug.Log("Main Menu UI disabled.");
    }

    private void EnablePauseUI()
    {
        SetUIElementActive(mainMenuPanel?.gameObject, false);
        SetUIElementActive(playButton?.gameObject, false);
        SetUIElementActive(quitButton?.gameObject, false);
        Debug.Log("Pause Menu UI enabled.");
    }

    private void DisablePauseUI()
    {
        SetUIElementActive(mainMenuPanel?.gameObject, false);
        SetUIElementActive(playButton?.gameObject, false);
        SetUIElementActive(quitButton?.gameObject, false);
        Debug.Log("Pause Menu UI disabled.");
    }

    private void SetUIElementActive(GameObject element, bool isActive)
    {
        if (element != null) element.SetActive(isActive);
    }

    private GameObject FindInactiveObject(string objectName)
    {
        // Find all GameObjects, including inactive ones
        var objects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (var obj in objects)
        {
            if (obj.name == objectName)
            {
                return obj;
            }
        }
        return null;
    }
}