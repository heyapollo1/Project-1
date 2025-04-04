using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*public class GameplayUI : MonoBehaviour
{
    [SerializeField] private Slider dodgeCooldownSlider;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI currencyText;
    [SerializeField] private TextMeshProUGUI xpText;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI playerLevelText;

    public void Initialize()
    {
        EventManager.Instance.StartListening("XPChanged", UpdateXPUI);
        EventManager.Instance.StartListening("UpdateLevelUI", UpdateLevelUI);
        EventManager.Instance.StartListening("HealthChanged", UpdateHealthUI);
        EventManager.Instance.StartListening("DodgeUsed", UpdateDodgeUI);
        EventManager.Instance.StartListening("CurrencyChanged", UpdateCurrencyUI);
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
        Debug.LogWarning("hp changed");
        healthSlider.value = currentHealth / maxHealth;
        healthText.text = $"HP: {currentHealth} / {maxHealth}";
    }

    private void UpdateXPUI(float currentXP, float xpToNextLevel)
    {
        //xpSlider.value = currentXP / xpToNextLevel;
        Debug.LogWarning("XP changed");
        xpText.text = $"XP: {currentXP} / { xpToNextLevel}";
    }

    private void UpdateLevelUI(float currentLevel)
    {
        Debug.LogWarning("Level changed");
        playerLevelText.text = $"Lvl { currentLevel}";
    }

    private void UpdateCurrencyUI(float currentCurrency)
    {
        Debug.LogWarning("currency changed");
        currencyText.text = $"G: {currentCurrency}";
    }

    private void UpdateDodgeUI(float cooldownTimer, float cooldownDuration)
    {
        dodgeCooldownSlider.maxValue = cooldownDuration;
        dodgeCooldownSlider.value = cooldownDuration - cooldownTimer;
    }
}*/
