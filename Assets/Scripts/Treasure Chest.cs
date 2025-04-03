using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureChest : MonoBehaviour, IInteractable, IDefaultTooltipData
{
    public Animator animator;
    public ParticleSystem chestParticleEffect;
    public GameObject itemPrefab;
    public GameObject weaponPrefab;
    public Transform dropPosition;

    private BaseItem itemReward;
    private WeaponInstance weaponReward;
    
    private bool isItemReward = false;
    private bool isWeaponReward = false;
    private bool isOpened = false;
    private bool isInitialized = false;
    private bool tooltipActive = false;
    
    private void Start()
    {
        if (isInitialized) return;
    }
    
    public string GetTitle()
    {  
        return "Treasure Chest";
    }
    
    public string GetDescription()
    {  
        return "Press 'C' to open.";
    }
    
    public Sprite GetIcon() => null;
    
    /*public void InitializeRandomReward()
    {
        if (isInitialized) return;
        Debug.Log($"Initialized random reward");
        bool weaponsAvailable = WeaponDatabase.Instance.AreThereWeapons();

        bool chooseItem = Random.value < 0.5f;
        if (chooseItem || !weaponsAvailable)
        {
            Debug.Log($"chest reward: {itemReward}");
            isItemReward = true;
            
            itemReward = ItemDatabase.Instance.GetRandomItem(stageBracket);
        }
        else
        {
            Debug.Log($"chest reward: {weaponReward}");
            isWeaponReward = true;
            weaponReward = WeaponDatabase.Instance.GetRandomWeapon();
        }
  
        Debug.Log($"chest reward: {weaponReward}");
        Debug.Log($"chest reward: {itemReward}");
        isInitialized = true;
    }*/
    
    public void InitializeItemReward(string itemName, Rarity rarity)
    {
        if (isInitialized) return;
        isItemReward = true;
        Sprite itemIcon = ItemDatabase.Instance.LoadItemIcon(itemName);
        itemReward = ItemFactory.CreateItem(itemName, itemIcon, rarity);
        isInitialized = true;
    }

    public void InitializeWeaponReward(string weaponName, Rarity rarity)
    {
        if (isInitialized) return;
        isWeaponReward = true;
        weaponReward = WeaponDatabase.Instance.CreateWeaponInstance(weaponName, rarity);
        Debug.Log($"Initialized weapon reward {weaponReward}");

        isInitialized = true;
    }

    private void DropWeapon()
    {
        WeaponDrop weaponDrop = weaponPrefab.GetComponent<WeaponDrop>();
        weaponPrefab.SetActive(true);
        weaponDrop.DropWeaponReward(weaponReward, dropPosition);
        Debug.Log($"Weapon dropping: {weaponReward.weaponTitle}");
        //StartCoroutine(FadeAndDestroy());
    }

    private void DropItem()
    {
        ItemDrop itemDrop = itemPrefab.GetComponent<ItemDrop>();
        itemPrefab.SetActive(true);
        ItemDatabase.Instance.RegisterActiveItem(itemReward.itemName);
        itemDrop.DropItemReward(itemReward, dropPosition);
        Debug.Log($"Item dropping: {itemReward.itemName}");
        //StartCoroutine(FadeAndDestroy());
    }
    
    public void Interact()
    {
        if (tooltipActive && !isOpened && isInitialized)
        {
            isOpened = true;
            tooltipActive = false;
            PlayerItemDetector.Instance.RemoveInteractableItem(this);
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
    }
    
    public void ShowTooltip()
    {
        if (!isInitialized || isOpened) return;
        TooltipManager.Instance.SetWorldTooltip(this, "Default");
    }
    
    public void HideTooltip()
    {
        TooltipManager.Instance.ClearWorldTooltip();
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!tooltipActive && !isOpened && collision.CompareTag("Player"))
        {
            tooltipActive = true;
            PlayerItemDetector.Instance.AddInteractableItem(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (tooltipActive && collision.CompareTag("Player"))
        {
            tooltipActive = false;
            PlayerItemDetector.Instance.RemoveInteractableItem(this);
        }
    }
    
    private IEnumerator FadeAndDestroy()
    {
        yield return new WaitForSeconds(2f);
        VisualFeedbackManager visualFeedback = GetComponent<VisualFeedbackManager>();
        if (visualFeedback != null)
        {
            yield return visualFeedback.FadeOut(1f, gameObject); // 1 second fade-out
        }
        Destroy(gameObject);
    }

    public void PlayChestParticles()
    {
        if (chestParticleEffect != null)
        {
            chestParticleEffect.Play();
        }
    }
}
