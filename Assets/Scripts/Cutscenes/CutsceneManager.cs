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

    public void Initialize()
    {
        GameManager.Instance.RegisterDependency("CutsceneManager", this);

        playerController = PersistentManager.Instance.playerInstance.GetComponent<PlayerController>();
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

    private void GetCamerasAndStartIntro(Transform introCameraPosition)
    {
        playerCamera = CameraManager.Instance.playerCamera;
        cutsceneCamera = CameraManager.Instance.cutsceneCamera;
        GameManager.Instance.MarkSystemReady("CutsceneManager");
        tutorialManager.StartTutorialIntro(introCameraPosition);
        //StartCutscene(introCutscene, introCameraPosition);
    }

    public void StartCutscene(Cutscene cutscene, Transform target = null)
    {
        currentTarget = target;
        cutsceneCamera.Priority = 20;
        Debug.Log($"currentTarget Target: {currentTarget}");
        StartCoroutine(HandleCutscene(cutscene));
        EventManager.Instance.TriggerEvent("HideUI");
    }

    private IEnumerator HandleCutscene(Cutscene cutscene)
    {
        playerController.DisableControls();
        PlayerAbilityManager.Instance.DisableAbilities();
        WeaponManager.Instance.DisableWeapon();

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
        WeaponManager.Instance.EnableWeapon();

        EventManager.Instance.TriggerEvent("CutsceneFinished", cutscene.name);
        EventManager.Instance.TriggerEvent("ShowUI");
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
/*public class CutsceneManager : MonoBehaviour
{
    public static CutsceneManager Instance { get; private set; }
    public Transform currentTarget { get; private set; }

    //public CinemachineCamera cutsceneCamera;
    [HideInInspector] public PlayerController playerController;

    [Header("General")]
    public Transform introCameraPosition;

    [Header("Cutscene Settings")]
    public Cutscene introCutscene;

    public void Initialize()
    {
        GameManager.Instance.RegisterDependency("CutsceneManager", this);
        //playerController = PersistentManager.Instance.playerInstance.GetComponent<PlayerController>();
        EventManager.Instance.StartListening("GameStarted", StartIntro);
        PrepareIntroScene();
        //StartCoroutine(PrepareIntroSceneAsync());
    }

    private void OnDestroy()
    {
        EventManager.Instance.StopListening("GameStarted", StartIntro);
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void PrepareIntroScene()
    {
        // Lower the player camera's priority
        CinemachineCamera playerCamera = CameraManager.Instance.mainCamera;
        //playerCamera.Priority = 0;

        //cutsceneCamera.Priority = 10;
        playerCamera.Follow = introCameraPosition;
        playerCamera.transform.position = introCameraPosition.position;

        //playerCamera.OnTargetObjectWarped(introCameraPosition, Vector3.zero);

        // Wait for a frame to ensure the camera has updated (if needed)
        //yield return null;

        Debug.Log("Intro camera positioned and ready.");
        GameManager.Instance.MarkSystemReady("CutsceneManager");
    }

    private void StartIntro()
    {
        StartCutscene(introCutscene, introCameraPosition);
    }

    public void StartCutscene(Cutscene cutscene, Transform target = null)
    {
        currentTarget = target;
        Debug.Log($"currentTarget Target: {currentTarget}");
        StartCoroutine(HandleCutscene(cutscene));
        EventManager.Instance.TriggerEvent("HideUI");
    }

    public void SetTarget(Transform target)
    {
        currentTarget = target;
        Debug.Log($"currentTarget switched to Target: {currentTarget}");
    }

    private IEnumerator HandleCutscene(Cutscene cutscene)
    {
        playerController.DisableControls();
        PlayerAbilityManager.Instance.DisableAbilities();
        WeaponManager.Instance.DisableWeapon();

        foreach (var action in cutscene.actions)
        {
            Debug.Log($"Triggering {action.name} cutscene");

            bool actionComplete = false;

            yield return StartCoroutine(action.Execute(this, () => actionComplete = true));
            yield return new WaitUntil(() => actionComplete);
        }

        Debug.Log($"finished {cutscene.name} cutscene");
        currentTarget = null;
        cutsceneCamera.Priority = 0;
  
        playerController.EnableControls();
        PlayerAbilityManager.Instance.EnableAbilities();
        WeaponManager.Instance.EnableWeapon();

        EventManager.Instance.TriggerEvent("CutsceneFinished", cutscene.name);
        EventManager.Instance.TriggerEvent("ShowUI");
    }

    public IEnumerator PanCamera(Vector3 targetPosition, float duration)
    {
        Debug.Log($"triggering {currentTarget.name} cutscene");
        cutsceneCamera.Priority = 20;
        cutsceneCamera.Follow = currentTarget;
        cutsceneCamera.LookAt = currentTarget;

        Vector3 startPosition = cutsceneCamera.transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            cutsceneCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cutsceneCamera.transform.position = targetPosition;

        Debug.Log("Pan complete.");
    }

    public IEnumerator RepositionCamera(Vector3 targetPosition)
    {
        cutsceneCamera.Priority = 20;
        cutsceneCamera.Follow = currentTarget;
        cutsceneCamera.LookAt = currentTarget;

        cutsceneCamera.transform.position = targetPosition;

        // Log the action and wait zero frames (optional, for coroutine compatibility)
        Debug.Log("Camera instantly repositioned (smash cut).");
        yield return null; // Coroutine-compatible, but no visual delay
    }


    public IEnumerator WaitForAnimation(Animator animator, string animationName)
    {
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(animationName) ||
               animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            yield return null;
        }
    }

    public IEnumerator TriggerEvent(string eventName)
    {
        EventManager.Instance.TriggerEvent(eventName);
        yield return null;
    }
}*/