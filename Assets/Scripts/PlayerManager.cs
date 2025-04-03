using UnityEngine;

public class PlayerManager : BaseManager
{
    public static PlayerManager Instance;
    public override int Priority => 10;
    
    public GameObject playerPrefab;
    public GameObject playerInstance;

    protected override void OnInitialize()
    {
        EventManager.Instance.StartListening("LoadPlayerFromSave", LoadPlayerFromSave);
        
        playerPrefab = Resources.Load<GameObject>("Prefabs/Player");
    }

    private void OnDestroy()
    {
        EventManager.Instance.StopListening("LoadPlayerFromSave", LoadPlayerFromSave);
    }
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    private void LoadPlayerFromSave(GameData saveData, bool isNewGame)
    {
        if (playerInstance != null) Destroy(playerInstance);

        Vector3 spawnPosition = isNewGame 
            ? GameObject.FindWithTag("PlayerSpawnPoint")?.transform.position ?? Vector3.zero 
            : saveData.playerState.position;

        playerInstance = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
        if (!isNewGame)
        {
            playerInstance.transform.position = saveData.playerState.position;
            Debug.Log($"Player correctly positioned at saved position: {saveData.playerState.position}");
        }
        // Move player to active scene to avoid DDOL hierarchy
        UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(playerInstance, UnityEngine.SceneManagement.SceneManager.GetActiveScene());

        Debug.Log($"Player spawned at {(isNewGame ? "intro point" : "saved position")} and moved to active scene.");
    }
    
    public void ApplyPlayerState(PlayerState state)
    {
        playerInstance.transform.position = state.position;
        WeaponManager.Instance.InitializeComponents();
        XPManager.Instance.LoadLevelProgressFromSave(state); //apply level and Xp progress
        ResourceManager.Instance.LoadResourcesFromSave(state); //apply saved resource amounts
        PlayerHealthManager.Instance.LoadHealthFromSave(state); //apply hp amounts
        
        Debug.Log("Player state fully restored.");
    }
}
