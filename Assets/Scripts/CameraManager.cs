using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    //private List<CinemachineCamera> virtualCameras = new List<CinemachineCamera>();
    public CinemachineBrain brain;
    public CinemachineCamera playerCamera;
    public CinemachineCamera cutsceneCamera;
    public Transform introCameraPosition;

    public void Initialize()
    {
        GameManager.Instance.RegisterDependency("CameraManager", this);

        brain = Camera.main.GetComponent<CinemachineBrain>();

        if (brain == null)
        {
            Debug.LogError("CinemachineBrain not found on Main Camera!");
        }

        InitializeCameras();
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void InitializeCameras()
    {
        GameObject playerInstance = PersistentManager.Instance.playerInstance;

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
}