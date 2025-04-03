using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DodgeManager : MonoBehaviour
{
    public static DodgeManager Instance;
    
    public float dodgeCooldown = 5.0f;
    public float dodgeSpeed = 10f;
    
    [HideInInspector]public bool isDodging = false;
    private float dodgeCooldownTimer = 0f;

    private PlayerController player;
    public event System.Action<float, float> OnCooldownUpdated; 
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        player = PlayerController.Instance;
    }
    
    public bool IsPlayerDodging() => isDodging;
    
    public void TriggerDodge()
    {
        if (player.IsPlayerDead() || player.IsPlayerDisabled()) return;
        
        if (!isDodging && dodgeCooldownTimer <= 0)
        {
            Debug.Log("Dodge Triggered");
            isDodging = true;
            AudioManager.TriggerSound("Player_Dodge", transform.position);
            StartCoroutine(Dodging());
        }
    }
    
    private IEnumerator Dodging()
    {
        Vector2 dodgeDirection = player.moveDirection.normalized;
        player.gameObject.layer = LayerMask.NameToLayer("Dodge"); 

        player.rb.velocity = dodgeDirection * dodgeSpeed;
        PlayerHealthManager.Instance.StartInvincibility(0.3f);
        
        yield return new WaitForSeconds(0.3f);
        
        player.rb.velocity = Vector2.zero;
        player.gameObject.layer = player.defaultLayer;
        
        dodgeCooldownTimer = dodgeCooldown;
        StartCoroutine(UpdateCooldownTimer());
        UIManager.Instance.StartDodgeUICooldown(dodgeCooldown);
        isDodging = false;
    }
    
    private IEnumerator UpdateCooldownTimer()
    {
        while (dodgeCooldownTimer > 0)
        {
            dodgeCooldownTimer -= Time.deltaTime;
            OnCooldownUpdated?.Invoke(dodgeCooldownTimer, dodgeCooldown);

            yield return null;
        }
        dodgeCooldownTimer = 0;
    }
}
