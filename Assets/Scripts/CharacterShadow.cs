using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterShadow : MonoBehaviour
{
    public GameObject shadowPrefab; 
    private GameObject shadow;
    public Vector3 shadowOffset = new Vector3(0f, -0.2f, 0f); // Offset to position shadow
    public float shadowScale = 0.8f; // Adjust the scale

    void Start()
    {
        shadow = Instantiate(shadowPrefab, transform.position + shadowOffset, Quaternion.identity);
        shadow.transform.SetParent(transform);
        shadow.transform.localScale = new Vector3(shadowScale, shadowScale, 1f);  
    }

    void Update()
    {
        // Update the shadow's position to follow the character
        shadow.transform.position = transform.position + shadowOffset;
    }
}