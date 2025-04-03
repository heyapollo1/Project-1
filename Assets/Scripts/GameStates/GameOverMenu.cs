using UnityEngine;
using UnityEngine.UI;

public class GameOverMenu : MonoBehaviour
{
    public GameObject menu;
    public Button returnToHubButton;
    public Button reviveButton;

    public void Awake()
    {
        Debug.LogWarning("GameOver Menu Initialized");

        returnToHubButton.onClick.AddListener(OnReturnClicked);
        reviveButton.onClick.AddListener(OnReviveClicked);
        menu.SetActive(false);
    }

    private void OnDestroy()
    {
        returnToHubButton.onClick.RemoveListener(OnReturnClicked);
        reviveButton.onClick.RemoveListener(OnReviveClicked);
    }

    private void OnReviveClicked()
    {
        UIManager.Instance.HideGameOverUI();
        EventManager.Instance.TriggerEvent("PlayerRevived");
    }

    private void OnReturnClicked()
    {
        UIManager.Instance.HideGameOverUI();
        GameManager.Instance.ClaimLoss();
    }
}