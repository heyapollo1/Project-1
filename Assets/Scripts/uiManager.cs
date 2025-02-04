using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIManager : BaseManager
{
    public static UIManager Instance { get; private set; }
    public override int Priority => 20;

    public GameObject sceneUI;
    public GameOverMenu gameOverMenu;
    public PauseMenu pauseMenu;
    public UpgradeSystemUI upgradeMenu;
    public ShopUI shopUI;
    public VictoryMenu victoryMenu;

    protected override void OnInitialize()
    {
        EventManager.Instance.StartListening("ShowPauseUI", ShowPauseMenu);
        EventManager.Instance.StartListening("HidePauseUI", HidePauseMenu);
        EventManager.Instance.StartListening("ShowGameOverUI", ShowGameOverMenu);
        EventManager.Instance.StartListening("HideGameOverUI", HideGameOverMenu);
        EventManager.Instance.StartListening("ShowVictoryUI", ShowVictoryMenu);
        EventManager.Instance.StartListening("HideVictoryUI", HideVictoryMenu);
    }

    private void OnDestroy()
    {
        EventManager.Instance.StopListening("ShowPauseUI", ShowPauseMenu);
        EventManager.Instance.StopListening("HidePauseUI", HidePauseMenu);
        EventManager.Instance.StopListening("ShowGameOverUI", ShowGameOverMenu);
        EventManager.Instance.StopListening("HideGameOverUI", HideGameOverMenu);
        EventManager.Instance.StopListening("HideLevelUpUI", HideLevelUpMenu);
        EventManager.Instance.StopListening("ShowVictoryUI", ShowVictoryMenu);
        EventManager.Instance.StopListening("HideVictoryUI", HideVictoryMenu);
    }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void ShowPauseMenu()
    {
        Debug.Log("showing");

        AudioManager.Instance.PlayUISound("UI_Open");
        pauseMenu = FindObjectOfType<PauseMenu>();
        pauseMenu.menu.SetActive(true);
    }

    private void HidePauseMenu()
    {
        Debug.Log("hiding");
        AudioManager.Instance.PlayUISound("UI_Close");
        pauseMenu = FindObjectOfType<PauseMenu>();
        pauseMenu.menu.SetActive(false);
    }

    private void ShowGameOverMenu()
    {
        gameOverMenu = FindObjectOfType<GameOverMenu>();
        StartCoroutine(ShowGameOverScreenWithDelay(0.7f));
    }

    private IEnumerator ShowGameOverScreenWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameOverMenu.menu.SetActive(true);
    }

    private void HideGameOverMenu()
    {
        gameOverMenu = FindObjectOfType<GameOverMenu>();
        gameOverMenu.menu.SetActive(false);
    }

    private void HideLevelUpMenu()
    {
        upgradeMenu = FindObjectOfType<UpgradeSystemUI>();
        upgradeMenu.upgradePanel.SetActive(false);
    }

    private void ShowVictoryMenu()
    {
        victoryMenu = FindObjectOfType<VictoryMenu>();
        StartCoroutine(ShowVictoryScreenWithDelay(0.7f));
    }

    private IEnumerator ShowVictoryScreenWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        victoryMenu.menu.SetActive(true);
    }

    private void HideVictoryMenu()
    {
        victoryMenu = FindObjectOfType<VictoryMenu>();
        victoryMenu.menu.SetActive(false);
    }
}
