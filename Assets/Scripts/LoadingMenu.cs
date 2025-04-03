using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingMenu : MonoBehaviour
{
    public static LoadingMenu Instance { get; private set; }

    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private TextMeshProUGUI loadingText; // Optional, for "Loading..." text
    [SerializeField] private Slider progressBar; // Optional, for progress bar

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ShowLoadingScreen()
    {
        Debug.LogWarning("load show");
        loadingScreen.SetActive(true);
    }

    public void HideLoadingScreen()
    {
        Debug.LogWarning("load hide");
        loadingScreen.SetActive(false);
    }

    public void UpdateProgress(float progress)
    {
        Debug.LogWarning("progress load");
        if (progressBar != null)
        {
            progressBar.value = progress;
        }
    }
}