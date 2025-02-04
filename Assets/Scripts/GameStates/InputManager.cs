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
        else if (GameStateManager.Instance.CurrentState == GameState.Paused)
        {
            HandlePauseMenuInput();
        }
        else if (GameStateManager.Instance.CurrentState == GameState.LevelUp)
        {
            HandleLevelUpInput();
        }
    }

    private void HandleGameplayInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            EventManager.Instance.TriggerEvent("OpenPauseMenu");
            Debug.Log("Pause triggered");
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            EventManager.Instance.TriggerEvent("TriggerDodge");
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            EventManager.Instance.TriggerEvent("ToggleAutoAim");
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            EventManager.Instance.TriggerEvent("InstantLevelUp");
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            AudioManager.Instance.PlayUISound("UI_Open");
            EventManager.Instance.TriggerEvent("TestShop");
        }
    }

    private void HandlePauseMenuInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseMenu.Instance.OnResumeClicked();
        }
    }

    private void HandleLevelUpInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UpgradeSystemUI.Instance.CloseUpgradePanel();
        }
    }

    private void HandleShoppingInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ShopUI.Instance.CloseShop();
        }
    }
}
