using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IInteractable
{
    void ShowTooltip();
    void HideTooltip();
    void Interact();
}

public interface IDefaultTooltipData
{
    string GetTitle();
    string GetDescription();
    Sprite GetIcon(); // return null if no icon
}