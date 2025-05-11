using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cutscene/Actions/Instantiate Chest")]
public class InstantiateChestAction : CutsceneAction
{
    public GameObject chestPrefab;
    public Vector3 spawnPosition;
    public List<RewardDropDefinition> rewardDefinitions;
    
    public override IEnumerator Execute(CutsceneManager cutsceneManager, Action onComplete)
    {
        if (chestPrefab == null)
        {
            Debug.LogError("InstantiateItemChestAction: No chest prefab assigned!");
            yield break;
        }

        GameObject spawnedChest = Instantiate(chestPrefab, spawnPosition, Quaternion.identity);
        TreasureChest chestComponent = spawnedChest.GetComponentInChildren<TreasureChest>();
        chestComponent.Initialize();
        foreach (var reward in rewardDefinitions)
        {
            try
            {
                var newReward = reward.ToRewardDrop();
                Debug.Log($"Added rewards to chest: {newReward}");
                chestComponent.AddReward(newReward);
            }
            catch (Exception e)
            {
                Debug.LogError($"Reward conversion error: {e.Message}");
            }
        }


        yield return new WaitForEndOfFrame();

        onComplete?.Invoke();
    }
}
