using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*public class Zap_ChainLightning : UpgradeData
{
    public int ChainTargetAmountIncrease = 1;  // Base movement speed increment∂
    //private ZapAbility zapAbility;

    public Zap_ChainLightning(Sprite zapChainLightningIcon)
    {
        // Set default values for this upgrade
        upgradeName = "Chain Lightning";
        description = "Zap now chains to +" + upgradeLevel + " additional enemy.";
        icon = zapChainLightningIcon;
    }

    public override void Apply(AttributeManager playerStats)
    {
        var zapAbility = PlayerAbilityManager.Instance.GetAbilityOfType<ZapAbility>();
        if (zapAbility != null)
        {
            Debug.Log("Applying Chain Lightning upgrade...");
            zapAbility.UnlockChainLightning();

            StatModifier zap_ChainLightningModifier = new StatModifier(
                StatType.Zap_ChainLightning, flatBonus: ChainTargetAmountIncrease
            );

            playerStats.ApplyModifier(zap_ChainLightningModifier);
            Debug.Log("Chain Lightning unlocked and stat modifier applied!");
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
        Debug.Log("CC upgrade leveled up to: " + upgradeLevel);
    }
}*/