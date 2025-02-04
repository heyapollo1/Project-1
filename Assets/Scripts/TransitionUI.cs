using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class TransitionUI : MonoBehaviour
{
    public GameObject transitionPanel;

    public void Awake()
    {
        Debug.LogWarning("TransitionUI Initialized");
        transitionPanel.SetActive(false);
    }
}
