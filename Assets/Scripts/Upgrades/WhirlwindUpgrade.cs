using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*public class WhirlwindUpgrade : UpgradeData
{
    public float swingAmount = 1f; 

    public WhirlwindUpgrade(Sprite cleaveSizeIcon)
    {
        upgradeName = "Whirlwind";
        description = ("Cleave spins " + ((swingAmount + upgradeLevel) - 1) + " additional time(s)");
        icon = cleaveSizeIcon;
    }

    public override void Apply(AttributeManager playerStats)
    {
        var cleaveAbility = PlayerAbilityManager.Instance.GetAbilityOfType<CleaveAbility>();

        if (cleaveAbility != null)
        {
            StatModifier whirlwindModifier = new StatModifier(
                StatType.Whirlwind, flatBonus: 1f
            );

            playerStats.ApplyModifier(whirlwindModifier);
        }
        else
        {
            Debug.LogError("ZapAbility not found in PlayerAbilityManager!");
        }
    }

    public override void ScaleUpgrade(AttributeManager playerStats)
    {
        upgradeLevel++;
        Apply(playerStats);
        Debug.Log("WhirlwindUpgrade upgrade leveled up to: " + upgradeLevel);
    }
}*/