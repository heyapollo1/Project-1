using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class ChestSaveState
{
    public string chestID;
    public bool isOpened;
    public Vector3 position;
    public List<string> chestRewards;
}

public class ChestStateManager : MonoBehaviour
{
    public static ChestStateManager Instance;

    public GameObject chestPrefab;
    private Dictionary<string, ChestSaveState> chestStates = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void OnDestroy()
    {
        ClearChestStates();
    }

    public void RegisterChest(TreasureChest chest)
    {
        if (!chestStates.ContainsKey(chest.ChestID))
        {
            chestStates[chest.ChestID] = new ChestSaveState
            {
                chestID = chest.ChestID,
                isOpened = chest.isOpened,
                position = chest.transform.position,
                chestRewards = chest.GetRewardData()
            };
        }
    }

    public void MarkChestOpened(string id)
    {
        if (chestStates.TryGetValue(id, out var state))
            state.isOpened = true;
    }

    public bool IsChestOpened(string id)
    {
        return chestStates.TryGetValue(id, out var state) && state.isOpened;
    }

    public void ClearChestStates()
    {
        chestStates.Clear();
    }
        
    public List<ChestSaveState> GetAllChestStates()
    {
        return new List<ChestSaveState>(chestStates.Values);
    }
    
    public void LoadWorldChestStates(WorldState worldState)
    {
        ClearChestStates();
        foreach (var chest in worldState.savedChestStates)
        {
            GameObject spawnedChest = Instantiate(chestPrefab, chest.position, Quaternion.identity);
            var chestScript = spawnedChest.GetComponentInChildren<TreasureChest>();
            chestScript.LoadSavedChestStates(chest);
            //var chestScript = spawnedChest.GetComponentInChildren<TreasureChest>();
            chestScript.Initialize();
            foreach (var reward in chest.chestRewards)
            {
                string[] parts = reward.Split(':');
                if (parts.Length == 2) // slotindex, isWeapon, name
                {
                    ItemRewardDrop chestReward;
                    string type = parts[0];
                    string name = parts[1];

                    if (type == "Item")
                    {
                        var item = ItemDatabase.CreateItem(name);
                        var itemPayload = new ItemPayload()
                        {
                            weaponScript = null,
                            itemScript = item,
                            isWeapon = false
                        };
                        chestReward = new ItemRewardDrop(itemPayload);
                    }
                    else
                    {
                        var weapon = WeaponDatabase.Instance.CreateWeaponInstance(name);
                        var weaponPayload = new ItemPayload()
                        {
                            weaponScript =  weapon,
                            itemScript = null,
                            isWeapon = true
                        };
                        chestReward = new ItemRewardDrop(weaponPayload);
                    }
                    chestScript.AddReward(chestReward);
                }
            }
        }
    }
}