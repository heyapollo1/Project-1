
using UnityEngine;

public class Pedestal : MonoBehaviour
{
    private ItemData currentItem;
    private WeaponData currentWeapon;
    public Transform itemSpawnPoint;

    public void SetItem(ItemData item)
    {
        currentItem = item;
    }

    public ItemData GetCurrentItem()
    {
        if (currentItem == null) return null;

        return currentItem;
    }

    public void DestroyItem()
    {
        ItemDatabase.Instance.RemoveActiveItem(currentItem);
        Destroy(transform.GetChild(0).gameObject);
        SetItem(null);
    }

    public void RerollItem()
    {
        ItemDatabase.Instance.RemoveActiveItem(currentItem);
        ItemData newItem = ItemDatabase.Instance.GetRandomItem();
        SetItem(newItem);
    }
}