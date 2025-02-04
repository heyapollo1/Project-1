using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemChest : MonoBehaviour
{
    public Animator animator;
    public ParticleSystem chestParticleEffect;
    public GameObject itemPrefab;
    public GameObject weaponPrefab;

    private ItemData itemReward;
    private WeaponData weaponReward;

    private bool isItemReward = false;
    private bool isWeaponReward = false;
    private bool isPlayerNearby = false;
    private bool isOpened = false;
    private bool isInitialized = false;

    private void Start()
    {
        //if (isInitialized) return;
        //isInitialized = true;
        //GameObject chestReward = Resources.Load<GameObject>($"Weapons/TheGunPrefab");
        //Debug.Log($"Initialized random reward {chestReward}");
        //InitializeFixedReward("Witch Point", true, false);
        //InitializeRandomReward();
    }

    public void InitializeItemReward(string itemName)
    {
        if (isInitialized) return;
        isItemReward = true;
        itemReward = ItemDatabase.Instance.GetItem(itemName);
        Debug.Log($"Initialized item reward {itemReward}");

        isInitialized = true;
    }

    public void InitializeWeaponReward(string weaponName)
    {
        if (isInitialized) return;
        isWeaponReward = true;
        weaponReward = WeaponDatabase.Instance.GetWeapon(weaponName);
        Debug.Log($"Initialized item reward {weaponReward}");

        isInitialized = true;
    }

    public void InitializeRandomReward()
    {
        if (isInitialized) return;
        Debug.Log($"Initialized random reward");
        bool itemsAvailable = ItemDatabase.Instance.AreThereItems();
        bool weaponsAvailable = WeaponDatabase.Instance.AreThereWeapons();

        bool chooseItem = Random.value < 0.5f;

        if (chooseItem && itemsAvailable)
        {
            isItemReward = true;
            itemReward = ItemDatabase.Instance.GetRandomItem();
        }
        else if (!chooseItem && weaponsAvailable)
        {
            isWeaponReward = true;
            weaponReward = WeaponDatabase.Instance.GetRandomWeapon();
        }
        else
        {
            if (itemsAvailable)
            {
                isItemReward = true;
                itemReward = ItemDatabase.Instance.GetRandomItem();
            }
            else if (weaponsAvailable)
            {
                isWeaponReward = true;
                weaponReward = WeaponDatabase.Instance.GetRandomWeapon();
            }
        }
        Debug.Log($"chest reward: {weaponReward}");
        Debug.Log($"chest reward: {itemReward}");
        isInitialized = true;
    }

    /*public void InitializeRandomReward()
    {
        if (isInitialized) return;
        Debug.Log($"Initialized random reward");
        bool itemsAvailable = ItemDatabase.Instance.AreThereItems();
        bool weaponsAvailable = WeaponDatabase.Instance.AreThereWeapons();

        bool chooseItem = Random.value < 0.5f;

        if (chooseItem && itemsAvailable)
        {
            isItemReward = true;
            itemReward = ItemDatabase.Instance.GetRandomItem();
        }
        else if (!chooseItem && weaponsAvailable)
        {
            isWeaponReward = true;
            weaponReward = WeaponDatabase.Instance.GetRandomWeapon();
        }
        else
        {
            if (itemsAvailable)
            {
                isItemReward = true;
                itemReward = ItemDatabase.Instance.GetRandomItem();
            }
            else if (weaponsAvailable)
            {
                isWeaponReward = true;
                weaponReward = WeaponDatabase.Instance.GetRandomWeapon();
            }
        }
        Debug.Log($"chest reward: {weaponReward}");
        Debug.Log($"chest reward: {itemReward}");
        isInitialized = true;
    }*/

    private void Update()
    {
        //if (!isInitialized) return;

        if (isPlayerNearby && Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log($"open chest");
            OpenChest();
        }
    }

    private void OpenChest()
    {
        if (isOpened) return;
        isOpened = true;
        AudioManager.Instance.PlayUISound("Item_PickUp");
        animator.SetTrigger("OpenChest");

        if (isItemReward)
        {
            DropItem();
        }
        else if (isWeaponReward)
        {
            DropWeapon();
        }
    }

    private void DropWeapon()
    {
        WeaponDrop weaponDrop = weaponPrefab.GetComponent<WeaponDrop>();
        weaponPrefab.SetActive(true);
        Debug.Log($"Weapon dropping: {weaponReward}");
        weaponDrop.SpawnWeaponReward(weaponReward);
    }

    private void DropItem()
    {
        Item itemDrop = itemPrefab.GetComponent<Item>();
        itemPrefab.SetActive(true);
        Debug.Log($"Item dropping: {itemReward}");
        itemDrop.SpawnItemReward(itemReward);
    }

    public void PlayChestParticles()
    {
        if (chestParticleEffect != null)
        {
            chestParticleEffect.Play();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isOpened)
        {
            isPlayerNearby = true;

            Tooltip.Instance.ShowTooltip("Item Chest", "Press C to open");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerNearby = false;

            Tooltip.Instance.HideTooltip();
        }
    }
}
