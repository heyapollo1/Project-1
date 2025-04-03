using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortingOrderManager : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private ParticleSystemRenderer[] particleRenderers;  // Hold references to child particle systems
    private float previousYPosition;
    private float offset = 0f; 
    private float positionChangeThreshold = 0.01f;  // Set the threshold for significant position changes
    public int sortingLayerOffset = 0;  // Offset to customize the sorting for different types of objects



    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        particleRenderers = GetComponentsInChildren<ParticleSystemRenderer>();  // Get all particle systems in children
        previousYPosition = transform.position.y;
        UpdateSortingOrder();  // Initial sorting order set
    }

    void Update()
    {
        float currentYPosition = transform.position.y;

        // Only update if the Y-position changes by more than the threshold
        if (Mathf.Abs(currentYPosition - previousYPosition) > positionChangeThreshold)
        {
            UpdateSortingOrder();
            previousYPosition = currentYPosition;
        }
    }
    void UpdateSortingOrder()
    {
        // Calculate sorting order based on Y-position
        int sortingOrder = -(int)((transform.position.y + offset) * 100) + sortingLayerOffset;

        // Update sorting order for the rock
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = sortingOrder;
        }

        foreach (var particleRenderer in particleRenderers)
        {
            if (particleRenderer != null)  // Check if the particleRenderer is still valid
            {
                particleRenderer.sortingOrder = sortingOrder - 1;
            }
        }
    }
}