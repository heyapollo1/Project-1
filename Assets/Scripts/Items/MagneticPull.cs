/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MagneticPull : MonoBehaviour
{
    public Action startPullAction;      // Action that triggers the pull effect
    private Transform playerTransform;  // Reference to the player's position
    private bool isBeingPulled = false; // Track if the item is in pull state
    private float currentPullSpeed = 0f; // Current pull speed that will increase over time
    private float maxPullSpeed = 12f;    // Maximum pull speed
    private float acceleration = 4f;    // Controls how quickly the pull speed increases

    private void Start()
    {
        // Get the player's transform reference from the PlayerStats singleton
        playerTransform = PlayerStats.Instance.transform;
    }

    private void Update()
    {
        // Only apply the pull if the item is within the pickup range and being pulled
        if (isBeingPulled)
        {
            // Gradually increase the pull speed up to maxPullSpeed
            currentPullSpeed = Mathf.Min(currentPullSpeed + acceleration * Time.deltaTime, maxPullSpeed);

            // Move the item toward the player with the accelerating speed
            transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, currentPullSpeed * Time.deltaTime);
        }
    }

    public void StartPull()
    {
        // This method activates the pull effect and resets the pull speed
        isBeingPulled = true;
        currentPullSpeed = 0f; // Reset speed to start from zero for a gradual pull
    }
}*/