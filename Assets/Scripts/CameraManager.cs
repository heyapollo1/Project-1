using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }
    
    public CinemachineBrain brain;
    public CinemachineCamera playerCamera;
    public CinemachineCamera cutsceneCamera;
    public Transform introCameraPosition;

    public CinemachineCamera GetPlayerCamera() => playerCamera;
    public CinemachineCamera GetCutsceneCamera() => cutsceneCamera;
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    public void Initialize()
    {
        GameManager.Instance.RegisterDependency("CameraManager", this);
        brain = Camera.main.GetComponent<CinemachineBrain>();

        if (brain == null)
        {
            Debug.LogError("CinemachineBrain not found on Main Camera!");
        }
        
        GameData gameData = SaveManager.LoadGame();
        if (gameData.isNewGame) 
        {
            Debug.Log("Starting a new game.");
            InitializeStartingCameras();
        }
    }
    
    private void InitializeStartingCameras()
    {
        if (brain == null)
        {
            brain = Camera.main?.GetComponent<CinemachineBrain>();
        }
        GameObject playerInstance = PlayerManager.Instance.playerInstance;

        playerCamera.Follow = playerInstance.transform;
        playerCamera.Priority = 10;

        Vector3 introPosition = introCameraPosition.position;
        introPosition.z = -10f;
        cutsceneCamera.transform.position = introPosition;
        cutsceneCamera.Priority = 0;

        cutsceneCamera.GetComponent<CinemachineCamera>().Lens.OrthographicSize = Camera.main.orthographicSize;
        playerCamera.GetComponent<CinemachineCamera>().Lens.OrthographicSize = Camera.main.orthographicSize;

        GameManager.Instance.MarkSystemReady("CameraManager");
        EventManager.Instance.TriggerEvent("CamerasInitialized", introCameraPosition);
    }
    
    public void LoadCamerasFromSave(CameraState state)
    {
        if (brain == null) // Find CinemachineBrain if not assigned
        {
            brain = Camera.main?.GetComponent<CinemachineBrain>();
        }
        brain.enabled = false;
        
        GameObject playerInstance = PlayerManager.Instance.playerInstance;

        playerCamera.Follow = playerInstance.transform;
        playerCamera.Priority = 10;
        
        playerCamera.transform.position = state.playerCameraPosition;
        cutsceneCamera.transform.position = state.cutsceneCameraPosition;
        playerCamera.GetComponent<CinemachineCamera>().Lens.OrthographicSize = state.playerCameraOrthographicSize;
        cutsceneCamera.GetComponent<CinemachineCamera>().Lens.OrthographicSize = state.cutsceneCameraOrthographicSize;
        
        StartCoroutine(ReenableBrainAfterDelay()); //so it doesnt pan over
    }
    
    private IEnumerator ReenableBrainAfterDelay()
    {
        //yield return new WaitForSeconds(1f);
        yield return new WaitForEndOfFrame();
        brain.enabled = true;
        GameManager.Instance.MarkSystemReady("CameraManager");
        EventManager.Instance.TriggerEvent("CamerasInitialized", introCameraPosition);
    }
    
}