using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class UpgradeSystemUI : MonoBehaviour
{
    public static UpgradeSystemUI Instance { get; private set; }

    [SerializeField] public GameObject upgradePanel;
    [SerializeField] private GameObject upgradeCardPrefab;
    [SerializeField] private Transform upgradeContainer;
    [SerializeField] private Button upgradeRerollButton;

    private bool upgradePanelActive = false;
    private bool abilityPanelActive = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        EventManager.Instance.StartListening("UpgradeChoicesReady", DisplayUpgradeChoices);
        EventManager.Instance.StartListening("AbilityChoicesReady", DisplayAbilityChoices);

        upgradeRerollButton.onClick.RemoveAllListeners();
        upgradeRerollButton.onClick.AddListener(RerollUpgrades);
        upgradePanel.SetActive(false);
    }

    private void OnDestroy()
    {
        EventManager.Instance.StopListening("UpgradeChoicesReady", DisplayUpgradeChoices);
        EventManager.Instance.StopListening("AbilityChoicesReady", DisplayAbilityChoices);

        upgradeRerollButton.onClick.RemoveAllListeners();
    }

    public void DisplayUpgradeChoices(List<UpgradeData> upgrades)
    {
        upgradePanelActive = true;
        upgradePanel.SetActive(true);

        foreach (Transform child in upgradeContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var upgrade in upgrades)
        {
            GameObject upgradeCard = Instantiate(upgradeCardPrefab, upgradeContainer);
            SetUpgradeCardData(upgradeCard, upgrade);
        }
    }

    public void DisplayAbilityChoices(List<PlayerAbilityData> abilities)
    {
        abilityPanelActive = true;
        upgradePanel.SetActive(true);

        foreach (Transform child in upgradeContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var ability in abilities)
        {
            GameObject abilityCard = Instantiate(upgradeCardPrefab, upgradeContainer);
            SetAbilityCardData(abilityCard, ability);
        }
    }

    private void SetUpgradeCardData(GameObject upgradeCard, UpgradeData upgradeData)
    {
        if (upgradeData == null)
        {
            Debug.LogError("Upgrade data missing!");
            return;
        }

        TextMeshProUGUI titleText = upgradeCard.transform.Find("UpgradeTitle")?.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI descriptionText = upgradeCard.transform.Find("UpgradeDescription")?.GetComponent<TextMeshProUGUI>();
        Image iconImage = upgradeCard.transform.Find("UpgradeIcon")?.GetComponent<Image>();

        if (titleText != null) titleText.text = upgradeData.upgradeName;
        if (descriptionText != null) descriptionText.text = upgradeData.description;
        if (iconImage != null) iconImage.sprite = upgradeData.icon;

        Button cardButton = upgradeCard.GetComponent<Button>();
        if (cardButton != null)
        {
            cardButton.onClick.RemoveAllListeners();
            cardButton.onClick.AddListener(() => OnUpgradeSelected(upgradeData));
        }
    }

    private void SetAbilityCardData(GameObject abilityCard, PlayerAbilityData ability)
    {
        // Get the UI components on the card
        TextMeshProUGUI titleText = abilityCard.transform.Find("UpgradeTitle")?.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI descriptionText = abilityCard.transform.Find("UpgradeDescription")?.GetComponent<TextMeshProUGUI>();
        Image iconImage = abilityCard.transform.Find("UpgradeIcon")?.GetComponent<Image>();

        // Set the ability data directly in the UI elements
        if (titleText != null) titleText.text = ability.abilityTitle;
        if (descriptionText != null) descriptionText.text = ability.abilityDescription;
        if (iconImage != null) iconImage.sprite = ability.abilityIcon;

        Debug.Log($"Displayed Ability: {ability.abilityTitle}, {ability.abilityDescription}");

        // Add button listener for selecting this ability
        Button cardButton = abilityCard.GetComponent<Button>();
        if (cardButton != null)
        {
            cardButton.onClick.RemoveAllListeners();
            cardButton.onClick.AddListener(() => OnAbilitySelected(ability));
        }
    }

    private void OnUpgradeSelected(UpgradeData selectedUpgrade)
    {
        Debug.LogWarning($"Upgrade selected");
        AudioManager.Instance.PlayUISound("Upgrade_Select", 0.7f);
        EventManager.Instance.TriggerEvent("UpgradeSelected", selectedUpgrade);
        upgradePanelActive = false;
        CloseUpgradePanel();
    }

    private void OnAbilitySelected(PlayerAbilityData selectedAbility)
    {
        Debug.LogWarning($"Ability selected");
        AudioManager.Instance.PlayUISound("Upgrade_Select", 0.7f);
        EventManager.Instance.TriggerEvent("AbilitySelected", selectedAbility);
        abilityPanelActive = false;
        CloseUpgradePanel();
    }

    public void CloseUpgradePanel()
    {
        GameStateManager.Instance.SetGameState(GameState.Playing);
        upgradePanel.SetActive(false);
    }

    public void RerollUpgrades()
    {
        AudioManager.Instance.PlayUISound("UI_Reroll");
        if (upgradePanelActive)
        {
            EventManager.Instance.TriggerEvent("ShowUpgradeChoices", 3);
        }
        else if (abilityPanelActive)
        {
            EventManager.Instance.TriggerEvent("ShowAbilityChoices", 3);
        }
    }
}
