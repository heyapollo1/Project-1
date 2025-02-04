using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum GameState
{
    MainMenu,
    Playing,
    Paused,
    LevelUp,
    GameOver,
}

public class GameStateManager : BaseManager
{
    public static GameStateManager Instance { get; private set; }

    public override int Priority => 20;

    public GameState CurrentState { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    protected override void OnInitialize()
    {
        SetGameState(GameState.MainMenu);
    }

    public void SetGameState(GameState newState)
    {
        if (CurrentState == newState) return;

        CurrentState = newState;

        if (newState == GameState.Paused)
        {
            EventManager.Instance.TriggerEvent("PauseAudio");
        }
        else if (newState == GameState.Playing || newState == GameState.MainMenu)
        {
            EventManager.Instance.TriggerEvent("ResumeAudio");
        }

        HandleStateChange(newState);
    }

    private void HandleStateChange(GameState state)
    {
        switch (state)
        {
            case GameState.MainMenu:
                AudioManager.Instance.PlayBackgroundMusic("Music_MainMenu");
                Time.timeScale = 1f;
                break;

            case GameState.Playing:
                AudioManager.Instance.PlayBackgroundMusic("Music_Gameplay");
                Time.timeScale = 1f;
                break;

            case GameState.Paused:
                Time.timeScale = 0f;
                break;

            case GameState.LevelUp:
                Time.timeScale = 0f;
                break;

            case GameState.GameOver:
                AudioManager.Instance.PlayBackgroundMusic("Music_GameOver");
                Time.timeScale = 0.3f;
                break;
        }
        Debug.Log($"GameState changed to {state}, Time.timeScale = {Time.timeScale}");
    }
}
