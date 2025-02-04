using UnityEngine;
using UnityEngine.UI;

public class GameOverMenu : MonoBehaviour
{
    public GameObject menu;
    public Button resetButton;
    public Button mainMenuButton;
    public Button reviveButton;

    public void Awake()
    {
        Debug.LogWarning("GameOver Menu Initialized");

        resetButton.onClick.AddListener(OnResetClicked);
        mainMenuButton.onClick.AddListener(OnMainMenuClicked);
        reviveButton.onClick.AddListener(OnReviveClicked);
        menu.SetActive(false);
    }

    private void OnDestroy()
    {
        resetButton.onClick.RemoveListener(OnResetClicked);
        mainMenuButton.onClick.RemoveListener(OnMainMenuClicked);
        reviveButton.onClick.RemoveListener(OnReviveClicked);
    }

    private void OnReviveClicked()
    {
        EventManager.Instance.TriggerEvent("CloseGameOverMenu");
        EventManager.Instance.TriggerEvent("PlayerRevived");
    }

    private void OnMainMenuClicked()
    {
        Debug.Log($"mainmenu clicked");
        EventManager.Instance.TriggerEvent("CloseGameOverMenu");
        EventManager.Instance.TriggerEvent("LoadMainMenu");
    }

    private void OnResetClicked()
    {
        EventManager.Instance.TriggerEvent("CloseGameOverMenu");
        EventManager.Instance.TriggerEvent("LoadGameplay");
    }
}