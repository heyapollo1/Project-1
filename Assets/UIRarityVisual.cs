using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRarityVisual : MonoBehaviour
{
    [SerializeField] private Image borderImage;
    
    private static readonly Dictionary<Rarity, Color> RarityColors = new()
    {
        { Rarity.Common,    new Color(0.85f, 0.85f, 0.85f) },
        { Rarity.Rare,      new Color(0.3f, 0.6f, 1f) },       
        { Rarity.Epic,      new Color(0.6f, 0.3f, 0.9f) },     
        { Rarity.Legendary, new Color(1f, 0.5f, 0f) }        
    };

    public void ApplyUIRarityVisual(Rarity rarity)
    {
        //Debug.Log($"Rarity for visuals: {rarity}");
        if (borderImage == null) return;

        if (RarityColors.TryGetValue(rarity, out var color))
        {
            //Debug.Log("Changing UIRarityVisual");
            borderImage.color = color;
        }
        else
        {
            Debug.Log($"No rarity found");
            borderImage.color = Color.gray;
        }
    }
    
    public void ApplyDefaultVisual()
    {
        borderImage.color = Color.gray;
    }
}