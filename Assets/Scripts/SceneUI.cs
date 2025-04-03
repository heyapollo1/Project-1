using UnityEngine;
using UnityEngine.UI;

public class SceneUI : MonoBehaviour
{
    public GameObject sceneUI;
    public void Awake()
    {
        Debug.LogWarning("Victory Menu Initialized");
        EventManager.Instance.StartListening("HideUI", HideAllUI);
        EventManager.Instance.StartListening("ShowUI", ShowAllUI);
        //menu.SetActive(false);
    }

    public void OnDestroy()
    {
        EventManager.Instance.StopListening("HideUI", HideAllUI);
        EventManager.Instance.StopListening("ShowUI", ShowAllUI);
    }

    private void HideAllUI()
    {
        sceneUI.SetActive(false);
    }

    private void ShowAllUI()
    {
        sceneUI.SetActive(true);
    }
}