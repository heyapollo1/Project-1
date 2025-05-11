
using Unity.VisualScripting;
using UnityEngine;

public class Pedestal : MonoBehaviour
{
    [Header("Pedestal Display")] public ItemDrop displayItem;

    [Header("General")] public bool isWeapon;
    public Transform displaySpawnPosition;
    public GameObject pedestalItem;

    private bool isDisplaying = false;

    public bool IsActive => isDisplaying;
    
    public void SetItem(ItemPayload payload)
    {
        if (isDisplaying) return;
        isDisplaying = true;
        isWeapon = payload.isWeapon;
        pedestalItem = Instantiate(Resources.Load<GameObject>("Prefabs/ItemPrefab"), displaySpawnPosition);
        pedestalItem.transform.localPosition = Vector3.zero;
        pedestalItem.transform.localScale = Vector3.one;

        displayItem = pedestalItem.GetComponent<ItemDrop>();
        displayItem.InitializeDroppedItem(payload, true);
        ItemTracker.Instance.AssignItemToTracker(payload, ItemLocation.Shop);

        displayItem.SetPedestal(this);
    }

    public void ResetPedestal()
    {
        if (!isDisplaying) return;
        isDisplaying = false;
        ItemTracker.Instance.UnassignItemFromTracker(displayItem.uniqueID);
        Debug.Log($"Pedestal reset and item '{displayItem.name}' is unregistered");
        Destroy(pedestalItem);
        displayItem = null;
    }
}