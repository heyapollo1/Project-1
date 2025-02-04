using UnityEngine;
using TMPro;

public class Tooltip : MonoBehaviour
{
    public static Tooltip Instance { get; private set; }

    [SerializeField] private GameObject tooltipPanel;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    private RectTransform toolTipTransform;
    //private Vector3 offset;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        toolTipTransform = tooltipPanel.GetComponent<RectTransform>();
        HideTooltip();
    }

    /*private void Update()
    {
        if (tooltipPanel.activeSelf)
        {
            if (offset != Vector3.zero)
            {
                toolTipTransform.position = Input.mousePosition + offset;
            }
        }
    }*/

     public void ShowTooltip(string title, string description)
    {
        //Transform player = PersistentManager.Instance.playerInstance.transform;

        titleText.text = title;
        descriptionText.text = description;

        //Vector3 screenPosition = Camera.main.WorldToScreenPoint(player.position + new Vector3(0, -1f, 0));
        //toolTipTransform.position = new Vector3(screenPosition.x, 50f, 0);

        tooltipPanel.SetActive(true);
    }


    public void HideTooltip()
    {
        tooltipPanel.SetActive(false);
    }

    public bool IsVisible()
    {
        return gameObject.activeSelf;
    }
}