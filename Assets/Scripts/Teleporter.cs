using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour, IInteractable, IDefaultTooltipData
{
    private bool portalTriggered = false;
    private bool tooltipActive = false;
    
    public string GetTitle()
    {  
        return "Teleporter";
    }
    
    public string GetDescription()
    {  
        return "Press 'C' to start game.";
    }
    
    public Sprite GetIcon() => null;
    
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
    
    public void Interact()
    {
        if (!TutorialManager.Instance.IsTutorialComplete()) return;
        if (!tooltipActive || portalTriggered) return;
        
        tooltipActive = false;
        portalTriggered = true;
        PlayerItemDetector.Instance.RemoveInteractableItem(this);
        ShowStageChoices();
    }
    
    private void ShowStageChoices()
    {
        List<StageConfig> stages = StageDatabase.Instance.GetStages();
        if (StageSelectUI.Instance == null)
        {
            Debug.Log("stageselect not available");
        }
        StageSelectUI.Instance.DisplayStageChoices(stages);
    }
        
    private void TeleportPlayer(StageConfig selectedStage)
    {
        Debug.Log($"Summoned portal to {selectedStage}");
        Transform spawnLocation = TransitionManager.Instance.GetGameplaySpawnPoint();
        
        AudioManager.Instance.PlayUISound("Player_TeleportDeparture");
        CutsceneManager.Instance.StartCutscene("TeleportStageCutscene", spawnLocation);
    }

    private void ResetTeleporter()
    {
        portalTriggered = false;
    }
    
    public void ShowTooltip()
    {
        TooltipManager.Instance.SetWorldTooltip(this, "Default");
    }
    
    public void HideTooltip()
    {
        TooltipManager.Instance.ClearWorldTooltip();
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!tooltipActive && !portalTriggered && collision.CompareTag("Player"))
        {
            Debug.LogWarning("Attempted to open chest " + collision.gameObject.name);
            tooltipActive = true;
            PlayerItemDetector.Instance.AddInteractableItem(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (tooltipActive && collision.CompareTag("Player"))
        {
            tooltipActive = false;
            PlayerItemDetector.Instance.RemoveInteractableItem(this);
        }
    }
}
