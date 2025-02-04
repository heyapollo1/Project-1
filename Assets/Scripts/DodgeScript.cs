/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DodgeScript : MonoBehaviour
{
    public Image dodgeIcon;        // UI image for dodge icon
    public Text cooldownText;      // UI text to display remaining cooldown
    public float dodgeCooldown = 2f; // Cooldown time for dodge ability
    public float dodgeSpeed = 10f;
    public float dodgeDuration = 0.5f;

    private float currentCooldown = 0f;
    private bool isDodging = false;
    //private float dodgeTimer = 0f;
//    private float dodgeCooldownTimer = 0f;
    //private bool canDodge = true;
    private Collider2D playerCollider;


    private void Start()
    {
        playerCollider = GetComponent<Collider2D>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && currentCooldown <= 0f && !isDodging)
        {
            Dodge();
        }

        UpdateUI();
    }

    void Dodge()
    {
        // Your dodge logic here
        isDodging = true;
        currentCooldown = dodgeCooldown;
    }

    void UpdateUI()
    {
        // Update cooldown
        if (currentCooldown > 0)
        {
            currentCooldown -= Time.deltaTime;
            cooldownText.text = Mathf.Ceil(currentCooldown).ToString();
            dodgeIcon.fillAmount = currentCooldown / dodgeCooldown;
        }
        else
        {
            cooldownText.text = "";
            dodgeIcon.fillAmount = 1;
            isDodging = false;
        }
    }
}*/