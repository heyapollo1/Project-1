using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    
    [Header("Menus and GUI")]
    public GameObject sceneUI;
    public GameOverMenu gameOverMenu;
    public PauseMenu pauseMenu;
    public VictoryMenu victoryMenu;
    public UpgradeSystemUI upgradeMenu;
    
    [Header("Gameplay UI")]
    [SerializeField] private Slider dodgeCooldownSlider;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI currencyText;
    [SerializeField] private TextMeshProUGUI xpText;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI playerLevelText;
    
    private bool isDraggingItem = false;
    public bool IsDraggingItem => isDraggingItem;
    
    public void Initialize()
    {
        Debug.LogWarning("UIManager Initialized");
        EventManager.Instance.StartListening("HideUI", HideAllUI);
        EventManager.Instance.StartListening("ShowUI", ShowAllUI);
        EventManager.Instance.StartListening("ShowGameOverUI", ShowGameOverMenu);
        EventManager.Instance.StartListening("ShowVictoryUI", ShowVictoryMenu);
        EventManager.Instance.StartListening("HideVictoryUI", HideVictoryMenu);
        
        EventManager.Instance.StartListening("XPChanged", UpdateXPUI);
        EventManager.Instance.StartListening("UpdateLevelUI", UpdateLevelUI);
        EventManager.Instance.StartListening("HealthChanged", UpdateHealthUI);
        EventManager.Instance.StartListening("CurrencyChanged", UpdateCurrencyUI);
        //DodgeManager.Instance.OnCooldownUpdated += OnDodgeCooldownUpdated;
    }

    private void OnDestroy()
    {
        Debug.LogWarning("UIManager destroyed");
        EventManager.Instance.StopListening("HideUI", HideAllUI);
        EventManager.Instance.StopListening("ShowUI", ShowAllUI);
        EventManager.Instance.StopListening("ShowGameOverUI", ShowGameOverMenu);
        EventManager.Instance.StopListening("ShowVictoryUI", ShowVictoryMenu);
        EventManager.Instance.StopListening("HideVictoryUI", HideVictoryMenu);
        
        EventManager.Instance.StopListening("XPChanged", UpdateXPUI);
        EventManager.Instance.StopListening("UpdateLevelUI", UpdateLevelUI);
        EventManager.Instance.StopListening("HealthChanged", UpdateHealthUI);
        EventManager.Instance.StopListening("CurrencyChanged", UpdateCurrencyUI);
        //DodgeManager.Instance.OnCooldownUpdated -= OnDodgeCooldownUpdated;
    }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    public void SetDragging(bool dragging)
    {
        isDraggingItem = dragging;
    }
    
    private void HideAllUI()
    {
        Debug.LogWarning("UIManager HideAllUI");
        sceneUI.SetActive(false);
    }

    private void ShowAllUI()
    {
        Debug.LogWarning("UIManager ShowAllUI");
        sceneUI.SetActive(true);
    }
    
    public void ShowPauseMenu()
    {
        pauseMenu.menu.SetActive(true);
        AudioManager.Instance.PlayUISound("UI_Open");
        PauseMenu.Instance.OpenPauseMenu();
    }

    public void HidePauseMenu()
    {
        pauseMenu.menu.SetActive(false);
        AudioManager.Instance.PlayUISound("UI_Close");
    }

    private void ShowGameOverMenu()
    {
        StartCoroutine(ShowGameOverScreenWithDelay(1f));
    }
    
    private IEnumerator ShowGameOverScreenWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameOverMenu.menu.SetActive(true);
    }
    
    public void HideGameOverUI()
    {
        gameOverMenu.menu.SetActive(false);
        AudioManager.Instance.PlayUISound("UI_Close");
    }
    
    private void ShowVictoryMenu()
    {
        StartCoroutine(ShowVictoryScreenWithDelay(1f));
    }
    
    private void HideVictoryMenu()
    {
        victoryMenu.menu.SetActive(false);
    }

    private IEnumerator ShowVictoryScreenWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        victoryMenu.menu.SetActive(true);
    }
    
    public void HideLevelUpUI()
    {
        upgradeMenu.upgradePanel.SetActive(false);
    }
    
    public void UpdateHealthUI(float currentHealth, float maxHealth)
    {
        Debug.LogWarning("hp changed");
        healthSlider.value = currentHealth / maxHealth;
        healthText.text = $"HP: {currentHealth} / {maxHealth}";
    }

    public void UpdateXPUI(float currentXP, float xpToNextLevel)
    {
        Debug.LogWarning("XP changed");
        xpText.text = $"XP: {currentXP} / { xpToNextLevel}";
    }

    public void UpdateLevelUI(float currentLevel)
    {
        Debug.LogWarning("Level changed");
        playerLevelText.text = $"Lvl { currentLevel}";
    }

    public void UpdateCurrencyUI(float currentCurrency)
    {
        Debug.LogWarning("currency changed");
        currencyText.text = $"G: {currentCurrency}";
    }
    
    public void StartDodgeUICooldown(float cooldownDuration)
    {
        dodgeCooldownSlider.value = 1;
        OnDodgeCooldownUpdated(cooldownDuration, cooldownDuration);
    }
    
    private void OnDodgeCooldownUpdated(float currentCooldown, float totalCooldown)
    {
        UpdateDodgeCooldownUI(currentCooldown, totalCooldown);
    }
    
    public void UpdateDodgeCooldownUI(float currentCooldown, float totalCooldown)
    {
        if (currentCooldown > 0)
        {
            dodgeCooldownSlider.value = currentCooldown / totalCooldown;
        }
        else
        {
            dodgeCooldownSlider.value = 0;
        }
    }
    
    public void ApplyUIModifications()
    {
        UpdateXPUI(XPManager.Instance.currentXP, XPManager.Instance.xpToNextLevel);
        UpdateLevelUI(XPManager.Instance.currentLevel);
        UpdateHealthUI(PlayerHealthManager.Instance.currentHealth, PlayerHealthManager.Instance.maxHealth);
        UpdateCurrencyUI(ResourceManager.Instance.currentCurrency);

        Debug.LogWarning("UI Initialized Successfully.");
    }

}
