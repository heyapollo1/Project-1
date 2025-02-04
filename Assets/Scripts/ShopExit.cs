using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopExit : MonoBehaviour
{
    public Transform gameplaySpawnPoint;
    public bool isPlayerNearby = false;
    public bool enteredPortal = false;

    public void Initialize()
    {
        Debug.LogWarning("Initialized portal");

        GameManager.Instance.RegisterDependency("ShopExit", this);

        GameManager.Instance.MarkSystemReady("ShopExit");
    }

    public void HandlePlayerEnter()
    {
        //GameObject player = PersistentManager.Instance.playerInstance;

        TransitionManager.Instance.StartTransition("Game", gameplaySpawnPoint);
    }

    private void Update()
    {
        if (!enteredPortal && isPlayerNearby && Input.GetKeyDown(KeyCode.C))
        {
            enteredPortal = true;
            HandlePlayerEnter();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !enteredPortal)
        {
            isPlayerNearby = true;
            Tooltip.Instance.ShowTooltip("Portal", "Press C to travel");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !enteredPortal)
        {
            isPlayerNearby = false;
            Tooltip.Instance.HideTooltip();
        }
    }
}
