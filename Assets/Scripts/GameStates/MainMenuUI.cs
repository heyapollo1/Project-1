using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    public Button playButton;
    public Button quitButton;

    public void Start()
    {
        Debug.Log("Enter main menu");
        playButton.onClick.AddListener(OnPlayClicked);
        quitButton.onClick.AddListener(OnQuitClicked);
    }

    private void OnDestroy()
    {
        playButton.onClick.RemoveListener(OnPlayClicked);
        quitButton.onClick.RemoveListener(OnQuitClicked);
    }

    private void OnPlayClicked()
    {
        Debug.Log("Playbutton clicked");
        EventManager.Instance.TriggerEvent("LoadGameplay");
        Debug.Log("Playbutton ???");
    }

    private void OnQuitClicked()
    {
        Debug.Log("Quit Game clicked");
        EventManager.Instance.TriggerEvent("QuitGame");
    }
}