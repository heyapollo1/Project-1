using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : BaseManager
{
    public static InputManager Instance { get; private set; }

    public override int Priority => 20;
    
    private float lastPickupTime = -999f;
    private const float pickupCooldown = 0.25f; // 200ms

    protected override void OnInitialize()
    {
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (Time.time - lastPickupTime < pickupCooldown)
        {
            return;
        }
        
        if (GameStateManager.Instance.CurrentState == GameState.GameOver || GameStateManager.Instance.CurrentState == GameState.MainMenu) return;

        if (GameStateManager.Instance.CurrentState == GameState.Playing)
        {
            HandleGameplayInput();
        }
        else if (GameStateManager.Instance.CurrentState == GameState.LevelUp)
        {
            HandleLevelUpInput();
        }
        else if (GameStateManager.Instance.CurrentState == GameState.Paused)
        {
            HandlePauseMenuInput();
        }
    }

    private void HandleGameplayInput()
    {
        if (CutsceneManager.Instance.CutsceneActive())
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                lastPickupTime = Time.time; 
                UIManager.Instance.ShowPauseMenu();
                Debug.Log("Pause triggered");
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                lastPickupTime = Time.time; 
                UIManager.Instance.ShowPauseMenu();
                Debug.Log("Pause triggered");
            }
            
            if (Input.GetMouseButton(0))
            {
                WeaponManager.Instance.GetActiveWeapon(true)?.TryUseWeapon();
            }
            
            if (Input.GetMouseButton(1))
            {
                WeaponManager.Instance.GetActiveWeapon(false)?.TryUseWeapon();
            }
            
            if (Input.GetKeyDown(KeyCode.C))
            {
                IInteractable currentTooltip = TooltipManager.Instance.GetActiveTooltip();
                if (currentTooltip == null) return;
                lastPickupTime = Time.time;
                currentTooltip.Interact();
            }
            
            /*if (Input.GetKeyDown(KeyCode.R) &&
                EncounterManager.Instance.GetCurrentEncounterType() == EncounterType.Shop)
            {
                lastPickupTime = Time.time;
                ShopEncounter shopEncounter = EncounterManager.Instance.GetCurrentEncounter();
                shopEncounter.RerollShop();
            }*/

            if (Input.GetKeyDown(KeyCode.Space))
            {
                lastPickupTime = Time.time;
                DodgeManager.Instance.TriggerDodge();
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                lastPickupTime = Time.time;
                XPManager.Instance.GetFreeXP(50);
            }

            if (Input.GetKeyDown(KeyCode.H))
            {
                lastPickupTime = Time.time;
                PlayerHealthManager.Instance.AdjustHealthByPercentage(0.2f);
            }

            if (Input.GetKeyDown(KeyCode.J))
            {
                lastPickupTime = Time.time;
                PlayerHealthManager.Instance.AdjustHealthByPercentage(-0.2f);
            }
        }
    }

    private void HandleLevelUpInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            lastPickupTime = Time.time;
            UIManager.Instance.ShowPauseMenu();
            Debug.Log("Pause triggered");
        }
    }

    private void HandlePauseMenuInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            lastPickupTime = Time.time;
            PauseMenu.Instance.OnResumeClicked();
        }
    }
}
