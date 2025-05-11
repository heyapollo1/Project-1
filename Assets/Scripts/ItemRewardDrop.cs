using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemRewardDrop : IRewardDrop
{
    public ItemPayload rewardPayload;
    public RewardType rewardType;
    public string itemName;
    private readonly GameObject prefab;
    private readonly bool isWeapon;

    public ItemRewardDrop(ItemPayload payload)
    {
        rewardPayload = payload;
        isWeapon = payload.isWeapon;
        rewardType = isWeapon ? RewardType.Weapon : RewardType.Item;
        itemName = isWeapon ? payload.weaponScript.weaponTitle : payload.itemScript.itemName;
        prefab = Resources.Load<GameObject>("Prefabs/ItemPrefab");
    }
    
    public void Drop(Vector3 dropPosition)
    {
        GameObject dropObj = Object.Instantiate(prefab, dropPosition, Quaternion.identity);
        var itemDrop = dropObj.GetComponent<ItemDrop>();
        
        ItemTracker.Instance.AssignItemToTracker(rewardPayload, ItemLocation.World, -1, -1, dropObj);
        itemDrop.DropItemReward(dropObj.transform, rewardPayload);
    }
}
