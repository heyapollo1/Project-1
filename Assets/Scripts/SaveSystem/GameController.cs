
/*using UnityEngine;

public class GameController : MonoBehaviour {
    private GameData gameData;

    private void Start() {
        gameData = SaveManager.LoadGame();
        Debug.Log("Player Position: " + gameData.playerData.position);
    }

    public void SaveGame() {
        gameData.playerData.position = transform.position;
        SaveManager.SaveGame(gameData);
    }

    public void ResetGame() {
        SaveManager.DeleteSave();
        gameData = new GameData();
    }

    private void OnApplicationQuit() {
        SaveGame();
    }
}*/
