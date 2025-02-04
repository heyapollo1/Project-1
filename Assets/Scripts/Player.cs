using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : MonoBehaviour
{
    public static Player Instance;
    private GameObject playerInstance;

    public void Initialize(GameObject playerPrefab)
    {
        playerInstance = Instantiate(playerPrefab);
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Make the player persistent
        }
        else
        {
            Destroy(gameObject); // Ensure there's only one player
        }
    }
}