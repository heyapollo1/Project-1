using UnityEngine;
using UnityEngine.UI;

public class VictoryMenu : MonoBehaviour
{
    public GameObject menu;
    public Button resetButton;
    public Button mainMenuButton;

    public void Awake()
    {
        Debug.LogWarning("Vicyory Menu Initialized");

        resetButton.onClick.AddListener(OnResetClicked);
        mainMenuButton.onClick.AddListener(OnMainMenuClicked);
        EventManager.Instance.StartListening("ShowVictoryUI", Show);
        menu.SetActive(false);
    }

    public void OnDestroy()
    {
        EventManager.Instance.StopListening("ShowVictoryUI", Show);
        resetButton.onClick.RemoveListener(OnResetClicked);
        mainMenuButton.onClick.RemoveListener(OnMainMenuClicked);
    }

    public void Show()
    {
        Debug.LogWarning("Vicyory Menu shown");
        menu.SetActive(true);
    }

    private void OnMainMenuClicked()
    {
        Debug.Log($"mainmenu clicked");
        EventManager.Instance.TriggerEvent("CloseVictoryMenu");
        EventManager.Instance.TriggerEvent("LoadMainMenu");
        menu.SetActive(false);
    }

    private void OnResetClicked()
    {
        EventManager.Instance.TriggerEvent("CloseVictoryMenu");
        EventManager.Instance.TriggerEvent("LoadGameplay");
        menu.SetActive(false);
    }
}