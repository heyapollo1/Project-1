using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadingScreenUI : MonoBehaviour
{
    public Slider progressBar; // Link your Slider UI
    public TextMeshProUGUI loadingText;   // Optional: Link your "Loading..." text

    private void Start()
    {
        Debug.Log("Loading screen shown");
        EventManager.Instance.StartListening("ShowLoadingScreen", Show);
        //EventManager.Instance.StartListening("UpdateLoadingProgress", UpdateProgress);
        EventManager.Instance.StartListening("SceneLoaded", Hide);
    }

    private void OnDestroy()
    {
        EventManager.Instance.StopListening("ShowLoadingScreen", Show);
        //EventManager.Instance.StopListening("UpdateLoadingProgress", UpdateProgress);
        EventManager.Instance.StopListening("SceneLoaded", Hide);
    }

    private void Show()
    {
        Debug.Log("Loading screen shown");
        gameObject.SetActive(true);
    }

    private void Hide(string sceneName)
    {
        Debug.Log("Loading screen hidden");
        gameObject.SetActive(false);
    }

    private void UpdateProgress(float progress)
    {
        if (progressBar != null)
        {
            progressBar.value = progress;
        }
        if (loadingText != null)
        {
            loadingText.text = $"Loading... {Mathf.RoundToInt(progress * 100)}%";
        }
    }
}