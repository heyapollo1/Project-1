using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameplayUI : MonoBehaviour
{
    [SerializeField] private Slider dodgeCooldownSlider;
    //[SerializeField] private Slider xpSlider;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI currencyText;
    [SerializeField] private TextMeshProUGUI xpText;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI playerLevelText;

    private void Start()
    {
        EventManager.Instance.StartListening("XPChanged", UpdateXPUI);
        EventManager.Instance.StartListening("UpdateLevelUI", UpdateLevelUI);
        EventManager.Instance.StartListening("HealthChanged", UpdateHealthUI);
        EventManager.Instance.StartListening("DodgeUsed", UpdateDodgeUI);
        EventManager.Instance.StartListening("CurrencyChanged", UpdateCurrencyUI);
        //InitializeUI();
    }

    private void OnDestroy()
    {
        EventManager.Instance.StopListening("XPChanged", UpdateXPUI);
        EventManager.Instance.StopListening("UpdateLevelUI", UpdateLevelUI);
        EventManager.Instance.StopListening("HealthChanged", UpdateHealthUI);
        EventManager.Instance.StopListening("CurrencyChanged", UpdateCurrencyUI);
    }

    private void UpdateHealthUI(float currentHealth, float maxHealth)
    {
        healthSlider.value = currentHealth / maxHealth;
        healthText.text = $"HP: {currentHealth} / {maxHealth}";
    }

    private void UpdateXPUI(float currentXP, float xpToNextLevel)
    {
        //xpSlider.value = currentXP / xpToNextLevel;
        xpText.text = $"XP: {currentXP} / { xpToNextLevel}";
    }

    private void UpdateLevelUI(float currentLevel)
    {
        playerLevelText.text = $"Lvl { currentLevel}";
    }

    private void UpdateCurrencyUI(float currentCurrency)
    {
        currencyText.text = $"G: {currentCurrency}";
    }

    private void UpdateDodgeUI(float cooldownTimer, float cooldownDuration)
    {
        dodgeCooldownSlider.maxValue = cooldownDuration;
        dodgeCooldownSlider.value = cooldownDuration - cooldownTimer;
    }
}
