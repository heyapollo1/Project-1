using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewShopEncounter", menuName = "Encounters/Types/ShopEncounter")]
public class ShopEncounter : EncounterData
{
    public override EncounterType encounterType => EncounterType.Shop;
    
    public override void TriggerEncounter(int currentStageNumber)
    {
        StageUI.Instance.DisableStageUI();
        StageBracket bracket = StageManager.Instance.GetCurrentStageConfig().GetStageBracket(currentStageNumber);
        Debug.Log($"Triggering {encounterName}. Shop uses item rarity for bracket: {bracket}");

        ShopManager.Instance.PrepareShop(bracket);
        Transform spawnPoint = TransitionManager.Instance.GetShopSpawnPoint();
        CutsceneManager.Instance.StartCutscene("TeleportShopCutscene", spawnPoint);
    }
    
    public override void EndEncounter()
    {
        Debug.Log($"Leaving {encounterName}");
        ShopManager.Instance.LeaveShop();
        EncounterManager.Instance.IsInEncounter(false);
        Transform spawnPoint = TransitionManager.Instance.GetGameplaySpawnPoint();
        AudioManager.Instance.PlayUISound("Player_TeleportDeparture");
        CutsceneManager.Instance.StartCutscene("TeleportStageCutscene", spawnPoint);
    }
}
