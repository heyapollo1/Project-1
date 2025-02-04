using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Cutscene/Actions/Instantiate Chest")]
public class InstantiateChestAction : CutsceneAction
{
    public GameObject chestPrefab;
    public string rewardName;
    public bool isItem = false;
    public bool isWeapon = false;
    public Vector3 spawnPosition;

    public override IEnumerator Execute(CutsceneManager cutsceneManager, Action onComplete)
    {
        if (chestPrefab == null)
        {
            Debug.LogError("InstantiateItemChestAction: No chest prefab assigned!");
            yield break;
        }

        GameObject spawnedChest = Instantiate(chestPrefab, spawnPosition, Quaternion.identity);
        ItemChest chestComponent = spawnedChest.GetComponentInChildren<ItemChest>();

        if (isItem)
        {
            chestComponent.InitializeItemReward(rewardName);
        }
        else if (isWeapon)
        {
            chestComponent.InitializeWeaponReward(rewardName);
        }
        else
        {
            Debug.LogError($"No valid reward found for name: {rewardName}");
        }

        yield return new WaitForEndOfFrame();

        Debug.Log($"Chest spawned at {spawnPosition}");
        onComplete?.Invoke();
    }
}
