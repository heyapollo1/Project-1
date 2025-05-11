using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemDetector : MonoBehaviour
{
    public static PlayerItemDetector Instance { get; private set; }
    
    private HashSet<IInteractable> nearbyInteractables = new HashSet<IInteractable>();
    //private List<IInteractable> nearbyItems = new List<IInteractable>();
    private IInteractable currentItem;
    private IInteractable bestItem;
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    public bool isInteractable(IInteractable obj) => nearbyInteractables.Contains(obj);
    
    public void AddInteractableItem(IInteractable obj)
    {
        if (obj != null)
        {
            bool added = nearbyInteractables.Add(obj); // HashSet prevents duplicates automatically
            if (added)
            {
                UpdateTooltip();
            }
        }
    }
    
    public void RemoveInteractableItem(IInteractable obj)
    {
        //Debug.Log($"Removed new item");
        if (obj != null && nearbyInteractables.Contains(obj))
        {
            //Debug.Log($"Trying Removal new item");
            bool removed = nearbyInteractables.Remove(obj); // HashSet prevents duplicates automatically
            if (removed)
            {
                //Debug.Log($"Successful Removed new item");
                UpdateTooltip();
            }
        }
    }

    private void Update()
    {
        bestItem = null;
        UpdateTooltip();
    }

    public void UpdateTooltip()
    {
        if (TooltipManager.Instance == null)
        {
            Debug.LogError("TooltipManager.Instance is null!");
            return;
        }
        if (TooltipManager.Instance.isHoverTooltipActive()) return;
        
        if (nearbyInteractables.Count == 0)
        {
            TooltipManager.Instance.ClearWorldTooltip();
            return;
        }
        
        float bestScore = float.MinValue;
        if (PlayerController.Instance == null)
        {
            Debug.LogError("PlayerController.Instance is null!");
            return;
        }
        Vector3 playerPos = PlayerController.Instance.transform.position;

        foreach (var item in nearbyInteractables)
        {
            MonoBehaviour monoItem = item as MonoBehaviour;
            if (monoItem != null)
            {
                Vector3 itemPos = monoItem.transform.position; // ✅ Safe from null reference exceptions
                float distance = Vector3.Distance(itemPos, playerPos);
                //Vector3 directionToItem = (itemPos - playerPos).normalized;
                
                float score = -distance;
                //Debug.DrawLine(itemPos, playerPos + directionToItem, Color.green);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestItem = item;
                }
            }
        }
        if (bestItem != currentItem && bestItem != null)
        {
            if (currentItem != null) currentItem.HideTooltip();
            currentItem = bestItem;
            currentItem.ShowTooltip();
            return;
        }
        
        currentItem = bestItem;
        currentItem.ShowTooltip();
    }

    public void PickUpItem()
    {
        if (currentItem != null)
        {
            //nearbyItems.Remove(currentItem);
            currentItem.Interact(); // Pick up the item
            currentItem = null;

            UpdateTooltip(); // Immediately switch to the next closest item
        }
    }
}
