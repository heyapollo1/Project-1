using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTreasureEncounter", menuName = "Encounters/Types/TreasureEncounter")]
public class TreasureEncounter : EncounterData
{
    public override EncounterType encounterType => EncounterType.Treasure;
    
    public override void TriggerEncounter(int currentStageNumber)
    {
        Debug.Log($"Triggering {encounterName}");
        StageBracket bracket = StageManager.Instance.GetCurrentStageConfig().GetStageBracket(currentStageNumber);
        Debug.Log($"Shop uses item rarity for bracket: {bracket}");

        //ShopManager.Instance.StartEncounter(bracket);
        //EncounterManager.Instance.SetCurrentEncounter(EncounterType.Shop, this); 
        Transform spawnPoint = TransitionManager.Instance.GetShopSpawnPoint();
        CutsceneManager.Instance.StartCutscene("TeleportShopCutscene", spawnPoint);
    }
    
    public override void EndEncounter()
    {
        Debug.Log($"Triggering {encounterName}");
        //ShopManager.Instance.EndEncounter();
        //EncounterManager.Instance.SetCurrentEncounter(EncounterType.None, null); 
        Transform spawnPoint = TransitionManager.Instance.GetGameplaySpawnPoint();
        CutsceneManager.Instance.StartCutscene("TeleportStageCutscene", spawnPoint);
    }
}
