using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance { get; private set; }

    public GameObject menu;
    public Button resumeButton;
    public Button mainMenuButton;
    public Button resetButton;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void Start()
    {
        Debug.LogWarning("Pause Menu Initialized");
        resumeButton.onClick.AddListener(OnResumeClicked);
        mainMenuButton.onClick.AddListener(OnMainMenuClicked);
        resetButton.onClick.AddListener(OnResetClicked);
        menu.SetActive(false);
    }

    private void OnDestroy()
    {
        resumeButton.onClick.RemoveListener(OnResumeClicked);
        mainMenuButton.onClick.RemoveListener(OnMainMenuClicked);
        resetButton.onClick.RemoveListener(OnResetClicked);
    }

    public void OnResumeClicked()
    {
        EventManager.Instance.TriggerEvent("HidePauseUI");
        GameStateManager.Instance.SetGameState(GameState.Playing);
    }

    private void OnMainMenuClicked()
    {
        EventManager.Instance.TriggerEvent("HidePauseUI");
        GameStateManager.Instance.SetGameState(GameState.MainMenu);
        EventManager.Instance.TriggerEvent("LoadMainMenu");
    }

    private void OnResetClicked()
    {
        EventManager.Instance.TriggerEvent("HidePauseUI");
        GameStateManager.Instance.SetGameState(GameState.Playing);
        EventManager.Instance.TriggerEvent("LoadGameplay");
    }
}