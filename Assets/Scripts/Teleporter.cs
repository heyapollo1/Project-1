using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public string destinationName;

    private bool isPlayerNearby = false;
    private bool summonedPortal = false;

    private void Awake()
    {
        EventManager.Instance.StartListening("PortalCleanup", ResetTeleporter);
        EventManager.Instance.StartListening("StageInitialized", TeleportPlayer);
    }

    private void OnDestroy()
    {
        EventManager.Instance.StopListening("PortalCleanup", ResetTeleporter);
        EventManager.Instance.StopListening("StageInitialized", TeleportPlayer);
    }
    
    private void Update()
    {
        if (!TutorialManager.Instance.IsTutorialComplete()) return;
        
        if (!summonedPortal && isPlayerNearby && Input.GetKeyDown(KeyCode.C))
        {
            summonedPortal = true;
            isPlayerNearby = false;
            ShowStageChoices();
        }
    }

    private void ShowStageChoices()
    {
        Debug.Log("Display stage choices");
        List<StageConfig> stages = StageDatabase.Instance.GetStages();
        Debug.Log($"Display stage choices {stages}");
        if (StageSelectUI.Instance == null)
        {
            Debug.Log("stageselect not available");
        }
        StageSelectUI.Instance.DisplayStageChoices(stages);
    }
        
    private void TeleportPlayer(StageConfig selectedStage)
    {
        Debug.Log($"Summoned portal to {selectedStage}");
        TooltipManager.Instance.HideTooltip();
        AudioManager.Instance.PlayUISound("Upgrade_Select");
        Transform spawnLocation = TransitionManager.Instance.GetGameplaySpawnPoint();
        CutsceneManager.Instance.StartCutscene("TeleportStageCutscene", spawnLocation);
    }

    private void ResetTeleporter()
    {
        summonedPortal = false;
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !summonedPortal)
        {
            isPlayerNearby = true;
            //TooltipManager.Instance.ShowStandardTooltip("Portal", $"Press C to travel to {destinationName}.");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && isPlayerNearby && !summonedPortal)
        {
            isPlayerNearby = false;
            TooltipManager.Instance.HideTooltip();
        }
    }
}
