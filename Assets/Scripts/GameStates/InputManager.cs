using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : BaseManager
{
    public static InputManager Instance { get; private set; }

    public override int Priority => 20;

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
                UIManager.Instance.ShowPauseMenu();
                Debug.Log("Pause triggered");
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                Debug.Log("C");
                IInteractable currentTooltip = TooltipManager.Instance.GetActiveTooltip();
                Debug.Log("C");
                if (currentTooltip == null) return;
                Debug.Log($"C: {currentTooltip}");
                currentTooltip.Interact();
            }
            
            if (Input.GetKeyDown(KeyCode.R) &&
                EncounterManager.Instance.GetCurrentEncounterType() == EncounterType.Shop)
            {
                ShopManager.Instance.RerollShop();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                DodgeManager.Instance.TriggerDodge();
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                XPManager.Instance.GetFreeXP(50);
            }

            if (Input.GetKeyDown(KeyCode.H))
            {
                PlayerHealthManager.Instance.AdjustHealthByPercentage(0.2f);
            }

            if (Input.GetKeyDown(KeyCode.J))
            {
                PlayerHealthManager.Instance.AdjustHealthByPercentage(-0.2f);
            }
        }
    }

    private void HandleLevelUpInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIManager.Instance.ShowPauseMenu();
            Debug.Log("Pause triggered");
        }
    }

    private void HandlePauseMenuInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseMenu.Instance.OnResumeClicked();
        }
    }
}
