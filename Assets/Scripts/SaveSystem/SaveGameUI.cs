using UnityEngine;
using UnityEngine.UI;

public class SaveGameUI : MonoBehaviour
{
    public GameObject saveGamePanel;
    public Button SaveGameButton;

    private bool isSaving = false;
        
    public void Initialize()
    {
        SaveGameButton.onClick.AddListener(SaveGameClicked);
        saveGamePanel.SetActive(false);
    }

    public void SaveGameClicked()
    {
        if (isSaving) return;

        isSaving = true;
        saveGamePanel.SetActive(true);
        
        GameData currentData = SaveManager.LoadGame();  // Load or create new game data
        SaveManager.SaveGame(currentData);

        Debug.Log($"Game saved at: {Application.persistentDataPath}/savegame.json");
        
        // Simulate save completion after short delay
        Invoke(nameof(FinishSave), 2f);
    }
    
    private void FinishSave()
    {
        isSaving = false;
        saveGamePanel.SetActive(false);
        Debug.Log("Save completed!");
    }
}