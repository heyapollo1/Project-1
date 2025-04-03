using System;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Cutscene/Actions/Instantiate Chest")]
public class InstantiateChestAction : CutsceneAction
{
    public GameObject chestPrefab;
    public string rewardName;
    [SerializeField] private Rarity rarity;
    
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
        TreasureChest chestComponent = spawnedChest.GetComponentInChildren<TreasureChest>();

        if (isItem)
        {
            chestComponent.InitializeItemReward(rewardName, rarity);
        }
        else if (isWeapon)
        {
            chestComponent.InitializeWeaponReward(rewardName, rarity);
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
