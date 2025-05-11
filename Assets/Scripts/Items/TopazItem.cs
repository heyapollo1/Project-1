using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopazItem : BaseItem
{
    private static readonly List<TagType> ClassTags = new()
    {
        TagType.Junk
    };
    
    private float stunChance;
    
    public TopazItem(Sprite topazIcon, Rarity rarity) : base(
        "Topaz", topazIcon, 50, ItemType.Inventory, rarity, 
        new List<StatModifier> { new (StatType.StunChance, 10f) })
    {
        stunChance = 10f;
        UpdateDescription();
    }
    
    public override string UpdateDescription()
    {
        return description = $"{(stunChance)}% to inflict Stun.";
    }
    
    public override void Apply()
    {
        base.Apply();

        EventManager.Instance.TriggerEvent("FXPoolUpdate", "BurningFX", 10);
        Debug.Log("BurningFX added to FX pool.");
    }
    
    public override void Remove()
    {
        base.Remove();
        FXManager.Instance.RemovePool("BurningFX");
        Debug.Log("BurningFX removed from FX pool.");
    }
}
