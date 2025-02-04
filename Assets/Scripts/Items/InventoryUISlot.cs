using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryUISlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image itemIcon;
    public Button slotButton;
    //public BoxCollider2D collider;

    private ItemData assignedItem;

    private void Awake()
    {
        if (slotButton == null)
        {
            slotButton = GetComponent<Button>();
        }
        itemIcon.enabled = false;
    }

    public void Initialize(ItemData item)
    {
        assignedItem = item;

        if (item != null)
        {
            itemIcon.sprite = item.icon;
            itemIcon.enabled = true;
        }
    }

    public void ClearSlot()
    {
        assignedItem = null;
        itemIcon.sprite = null;
        itemIcon.enabled = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (assignedItem != null)
        {
            Tooltip.Instance.ShowTooltip(assignedItem.itemName, assignedItem.description);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Tooltip.Instance.HideTooltip();
    }

    public ItemData GetAssignedItem()
    {
        return assignedItem;
    }
}