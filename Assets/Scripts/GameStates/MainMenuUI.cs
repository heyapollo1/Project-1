using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("Main Panel")]
    public GameObject mainPanel;
    public Button loadSaveButton;
    public Button newGameButton;
    public Button exitGameButton;
    
    [Header("Load Save Panel")]
    public GameObject loadSavePanel;
    public Button loadSaveFileButton;
    public Button backButton;
    
    [Header("PromptPanel")]
    public GameObject DeleteSavePrompt;
    public GameObject OverwritePrompt;
    public Button ConfirmDeleteButton;
    public Button CancelDeleteButton;
    public Button OverwriteConfirmButton;
    public Button OverwriteCancelButton;
    
    private void Awake()
    {
        // Check if save file exists
        string savePath = $"{Application.persistentDataPath}/gameState.json";

        if (System.IO.File.Exists(savePath))
        {
            var loadedSave = SaveManager.LoadGame();
            // Ensure the save isn't just a default "new game"
            if (loadedSave != null && !loadedSave.isNewGame)
            {
                loadSaveButton.interactable = true;
                Debug.Log("Valid save file found. Load button enabled.");
            }
            else
            {
                loadSaveButton.interactable = false;
                Debug.Log("Save file exists but indicates a new game.");
            }
        }
        else
        {
            loadSaveButton.interactable = false;
            Debug.Log("No save file found. Load button disabled.");
        }
    }
    
    public void Start()
    {
        Debug.Log("Enter main menu");
        loadSaveButton.onClick.AddListener(OnLoadSaveClicked);
        newGameButton.onClick.AddListener(OnNewGameClicked);
        exitGameButton.onClick.AddListener(OnExitGameClicked);
        loadSaveFileButton.onClick.AddListener(OnLoadSaveFileClicked);
        backButton.onClick.AddListener(OnBackClicked);
        ConfirmDeleteButton.onClick.AddListener(DeleteSaveYesClicked);
        CancelDeleteButton.onClick.AddListener(DeleteSaveNoClicked);
        OverwriteConfirmButton.onClick.AddListener(OverwriteSaveYesClicked);
        OverwriteCancelButton.onClick.AddListener(OverwriteSaveNoClicked);
    }

    private void OnDestroy()
    {
        loadSaveButton.onClick.RemoveListener(OnLoadSaveClicked);
        newGameButton.onClick.RemoveListener(OnNewGameClicked);
        exitGameButton.onClick.RemoveListener(OnExitGameClicked);
        loadSaveFileButton.onClick.RemoveListener(OnLoadSaveFileClicked);
        backButton.onClick.RemoveListener(OnBackClicked);
        ConfirmDeleteButton.onClick.RemoveListener(DeleteSaveYesClicked);
        CancelDeleteButton.onClick.RemoveListener(DeleteSaveNoClicked);
        OverwriteConfirmButton.onClick.RemoveListener(OverwriteSaveYesClicked);
        OverwriteCancelButton.onClick.RemoveListener(OverwriteSaveNoClicked);
    }
    
    private void OnLoadSaveClicked()
    {
        Debug.Log("Load Save clicked");
        loadSavePanel.SetActive(true);
        mainPanel.SetActive(false);
    }
    
    private void OnNewGameClicked()
    {
        if (SaveManager.CheckSaveExists())
        {
            OverwritePrompt.SetActive(true);
            Debug.Log("Save exists! Prompting to overwrite.");
        }
        else
        {
            EventManager.Instance.TriggerEvent("StartNewGame");
        }
    }

    private void OnExitGameClicked()
    {
        //Application.Quit();
       // UnityEditor.EditorApplication.isPlaying = false;
    }
    
    private void OnLoadSaveFileClicked()
    {
        Debug.Log("Load Game clicked");
        EventManager.Instance.TriggerEvent("LoadSaveFile");
    }
    
    private void DeleteSaveYesClicked()
    {
        SaveManager.DeleteSave();
        loadSaveButton.interactable = false;
        Debug.Log("Save deleted from UI.");
    }
    
    private void DeleteSaveNoClicked()
    {
        DeleteSavePrompt.SetActive(false);
    }
    
    private void OverwriteSaveYesClicked()
    {
        SaveManager.DeleteSave();
        EventManager.Instance.TriggerEvent("StartNewGame");
    }
    
    private void OverwriteSaveNoClicked()
    {
        OverwritePrompt.SetActive(false);
    }
    
    private void OnBackClicked()
    {
        mainPanel.SetActive(true);
        loadSavePanel.SetActive(false);
    }
}