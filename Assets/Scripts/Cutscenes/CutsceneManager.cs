using System.Collections;
using UnityEngine;
using Unity.Cinemachine;
using System;

public class CutsceneManager : MonoBehaviour
{
    public static CutsceneManager Instance { get; private set; }
    public Transform currentTarget { get; private set; }

    [HideInInspector] public PlayerController playerController;

    private CinemachineCamera playerCamera;
    private CinemachineCamera cutsceneCamera;

    [Header("Cutscene Settings")]
    //public Cutscene introCutscene;
    public TutorialManager tutorialManager;
    public CanvasGroup fadeCanvas;

    private bool cutsceneActive = false;

    public void Initialize()
    {
        GameManager.Instance.RegisterDependency("CutsceneManager", this);
        
        playerController = PlayerManager.Instance.playerInstance.GetComponent<PlayerController>();
        EventManager.Instance.StartListening("CamerasInitialized", GetCamerasAndStartIntro);
    }

    private void OnDestroy()
    {
        EventManager.Instance.StopListening("CamerasInitialized", GetCamerasAndStartIntro);
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public bool CutsceneActive() => cutsceneActive;
    
    private void GetCamerasAndStartIntro(Transform introCameraPosition)
    {
        GameManager.Instance.MarkSystemReady("CutsceneManager");
        playerCamera = CameraManager.Instance.playerCamera;
        cutsceneCamera = CameraManager.Instance.cutsceneCamera;

        if (!TutorialManager.Instance.IsTutorialComplete())
        {
            tutorialManager.StartTutorialIntro(introCameraPosition);
        }
        else
        {
            Debug.Log("Skipping tutorial!");
        }
    }

    public void StartCutscene(string cutsceneName, Transform target = null)
    {
        cutsceneActive = true;
        var database = Resources.Load<CutsceneDatabase>("ScriptableObjects/Cutscenes/CutsceneDatabase");
        if (database == null)
        {
            Debug.LogError("Cutscene database not found.");
            return;
        }
        Cutscene cutscene = database.GetCutscene(cutsceneName);
        currentTarget = target;
        cutsceneCamera.Priority = 20;
        Debug.Log($"currentTarget Target: {currentTarget}");
        StartCoroutine(HandleCutscene(cutscene));
    }

    private IEnumerator HandleCutscene(Cutscene cutscene)
    {
        playerController.DisableControls();
        EventManager.Instance.TriggerEvent("HideUI");
        PlayerAbilityManager.Instance.DisableAbilities();
        WeaponManager.Instance.DisableWeapons();

        foreach (var action in cutscene.actions)
        {
            Debug.Log($"Triggering {action.name} cutscene");

            bool actionComplete = false;

            yield return StartCoroutine(action.Execute(this, () => actionComplete = true));
            yield return new WaitUntil(() => actionComplete);
        }

        Debug.Log($"finished {cutscene.name} cutscene");

        if (currentTarget != playerController.transform)
        {
            currentTarget = null;
            playerCamera.Follow = playerController.transform;
        }

        cutsceneCamera.Priority = 0;
        playerCamera.Priority = 10;
    
        playerController.EnableControls();
        PlayerAbilityManager.Instance.EnableAbilities();
        EventManager.Instance.TriggerEvent("EnablePlayerWeapons");
        cutsceneActive = false;
    }

    public void RepositionCutsceneCamera(Vector3 newPosition)
    {
        cutsceneCamera.transform.position = new Vector3(newPosition.x, newPosition.y, cutsceneCamera.transform.position.z);
        Debug.Log($"currentTarget Target: {newPosition}");
    }

    public void SetTarget(Transform target)
    {
        currentTarget = target;
        Debug.Log($"currentTarget switched to Target: {currentTarget}");
    }

    public IEnumerator PanCamera(Vector3 targetPosition, float duration)
    {
        //Transform originalFollow = cutsceneCamera.Follow;
        //cutsceneCamera.Follow = null;
        targetPosition.z = -10f;

        Vector3 startPosition = cutsceneCamera.transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            cutsceneCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cutsceneCamera.transform.position = targetPosition;

        // Optionally re-enable Follow if needed
        //cutsceneCamera.Follow = originalFollow;

        Debug.Log("Pan complete.");
    }

    public IEnumerator RepositionCamera(Vector3 targetPosition)
    {
        //cutsceneCamera.Follow = currentTarget;

        cutsceneCamera.transform.position = targetPosition;

        yield return null;
    }


    public IEnumerator WaitForAnimation(Animator animator, string animationName)
    {
        bool animationPlaying = false;

        while (true)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            if (stateInfo.IsName(animationName))
            {
                animationPlaying = true;

                // Break out if the animation has started
                if (stateInfo.normalizedTime > 0f)
                {
                    Debug.Log($"Animation {animationName} started.");
                    break;
                }
            }

            yield return null;
        }

        while (animationPlaying)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            if (stateInfo.IsName(animationName))
            {
                // Check if the animation has completed (normalizedTime >= 1.0)
                if (stateInfo.normalizedTime >= 0.99f)
                {
                    Debug.Log($"Animation {animationName} finished.");
                    animationPlaying = false;
                }
            }
            else
            {
                // Animation has transitioned away
                Debug.LogWarning($"Animation {animationName} transitioned to another state.");
                animationPlaying = false;
            }

            yield return null;
        }
    }

    public IEnumerator FadeScreen(bool fadeOut, float duration)
    {
        if (fadeCanvas == null)
        {
            Debug.LogError("CutsceneManager: No fade canvas found!");
            yield break;
        }

        float startAlpha = fadeOut ? 0 : 1;
        float endAlpha = fadeOut ? 1 : 0;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            fadeCanvas.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            yield return null;
        }

        fadeCanvas.alpha = endAlpha;
    }
}