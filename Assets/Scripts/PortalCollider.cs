using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalCollider : MonoBehaviour
{
    public Portal parentPortal;
    public bool isPlayerNearby = false;
    public bool enteredPortal = false;

    private void Update()
    {
        if (!enteredPortal && isPlayerNearby && Input.GetKeyDown(KeyCode.C))
        {
            enteredPortal = true;
            parentPortal?.HandlePlayerEnter();
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