using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryExit : MonoBehaviour
{
    public Transform portalSpawnPoint;
    public bool isPlayerNearby = false;
    public bool summonedPortal = false;

    private void Awake()
    {
        EventManager.Instance.StartListening("PortalCleanup", ResetShopExit);
    }

    private void OnDestroy()
    {
        EventManager.Instance.StopListening("PortalCleanup", ResetShopExit);
    }
    
    private void Update()
    {
        if (!summonedPortal && isPlayerNearby && Input.GetKeyDown(KeyCode.C))
        {
            summonedPortal = true;
            isPlayerNearby = false;
            SummonPortal();
        }
    }

    private void SummonPortal()
    {
        TooltipManager.Instance.HideTooltip();
        AudioManager.Instance.PlayUISound("Upgrade_Select");
        EventManager.Instance.TriggerEvent("SpawnPortal","Hub", portalSpawnPoint.position);
        //summonedPortal = false;
    }

    private void ResetShopExit()
    {
        summonedPortal = false;
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !summonedPortal)
        {
            isPlayerNearby = true;
            //TooltipManager.Instance.ShowStandardTooltip("Portal", "Press C to travel");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !summonedPortal)
        {
            isPlayerNearby = false;
            TooltipManager.Instance.HideTooltip();
        }
    }
}
