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
        Debug.LogWarning("Pause Menu Destroyed");
        resumeButton.onClick.RemoveListener(OnResumeClicked);
        mainMenuButton.onClick.RemoveListener(OnMainMenuClicked);
        resetButton.onClick.RemoveListener(OnResetClicked);
    }
    
    public void OpenPauseMenu()
    {
        GameStateManager.Instance.SetGameState(GameState.Paused);
    }
    
    public void OnResumeClicked()
    {
        GameStateManager.Instance.SetGameState(GameState.Playing);
        UIManager.Instance.HidePauseMenu();
    }

    private void OnMainMenuClicked()
    {
        GameStateManager.Instance.SetGameState(GameState.MainMenu);
        GameManager.Instance.LoadMainMenu();
        UIManager.Instance.HidePauseMenu();
    }

    private void OnResetClicked()
    {
        GameStateManager.Instance.SetGameState(GameState.Playing);
        EventManager.Instance.TriggerEvent("ReturnToHub");
        UIManager.Instance.HidePauseMenu();
    }
}