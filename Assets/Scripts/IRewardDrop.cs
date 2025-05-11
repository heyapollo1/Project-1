using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IRewardDrop
{
    void Drop(Vector3 dropPosition);
}

public enum RewardType
{
    Item, Weapon
}

[System.Serializable]
public class RewardDropDefinition
{
    public RewardType rewardType;
    public string rewardName;

    public IRewardDrop ToRewardDrop()
    {
        switch (rewardType)
        {
            case RewardType.Item:
                var item = ItemDatabase.CreateItem(rewardName);
                var itemPayload = new ItemPayload()
                {
                    weaponScript =  null,
                    itemScript = item,
                    isWeapon = false
                };
                return new ItemRewardDrop(itemPayload);
            
            case RewardType.Weapon:
                var weapon = WeaponDatabase.Instance.CreateWeaponInstance(rewardName);
                var weaponPayload = new ItemPayload()
                {
                    weaponScript =  weapon,
                    itemScript = null,
                    isWeapon = true
                };
                return new ItemRewardDrop(weaponPayload);
            
            default:
                throw new ("Unsupported reward type.");
        }
    }
}