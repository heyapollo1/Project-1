using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TreasureChest : MonoBehaviour, IInteractable, IDefaultTooltipData
{
    public Animator animator;
    public ParticleSystem chestParticleEffect;
    public Transform dropPosition;
    [SerializeField] private string chestID;
    public string ChestID => chestID;
    
    private List<IRewardDrop> rewardDrops = new();
    
    [HideInInspector]public bool isOpened = false;
    private bool isInitialized = false;
    private bool tooltipActive = false;
    
    private void Awake()
    {
        if (isInitialized) return;
        isInitialized = true;
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

    public List<string> GetRewardData()
    {
        List<string> rewards = new List<string>();
        foreach (var drop in rewardDrops)
        {
            var itemReward = (ItemRewardDrop)drop;
            string name = itemReward.itemName;
            string type = itemReward.rewardType.ToString();
            rewards.Add($"{type}:{name}");
        }
        return rewards;
    }
    
    public void Initialize()
    {
        chestID = $"{name}_{Guid.NewGuid()}";
        ChestStateManager.Instance.RegisterChest(this);
    }
    
    public void AddReward(IRewardDrop rewardDrop)
    {
        if (rewardDrop != null)
        {
            rewardDrops.Add(rewardDrop);
        }
        else
        {
            Debug.LogWarning("Attempted to add null rewardDrop to chest.");
        }
    }
    
    public void AddRandomReward(StageBracket bracket)
    {
        float roll = Random.value;
        if (roll < 0.8f)
        {
            BaseItem itemReward = ItemDatabase.Instance.CreateRandomItem(bracket);

            var itemPayload = new ItemPayload()
            {
                weaponScript = null,
                itemScript = itemReward,
                isWeapon = false
            };
            Debug.Log($"[Chest] Added random item reward: {itemReward.itemName}");
            var newReward = new ItemRewardDrop(itemPayload);
            AddReward(newReward);
        }
        else
        {
            WeaponInstance weaponReward = WeaponDatabase.Instance.CreateRandomWeapon(bracket);
            var itemPayload = new ItemPayload()
            {
                weaponScript = weaponReward,
                itemScript = null,
                isWeapon = true
            };
            Debug.Log($"[Chest] Added random item reward: {weaponReward.weaponTitle}");
            var newReward = new ItemRewardDrop(itemPayload);
            AddReward(newReward);
        }
    }
    
    public void Interact()
    {
        if (!tooltipActive || isOpened || rewardDrops.Count == 0) return;

        isOpened = true;
        tooltipActive = false;
        PlayerItemDetector.Instance.RemoveInteractableItem(this);
        AudioManager.Instance.PlayUISound("Item_PickUp");
        animator.SetTrigger("OpenChest");

        DropAllRewards();
        rewardDrops.Clear();
        ChestStateManager.Instance.MarkChestOpened(chestID);
    }
    
    private void DropAllRewards()
    {
        foreach (var reward in rewardDrops)
        {
            if(reward != null)Debug.Log("Dropping reward null");
            Vector3 randomOffset = Random.insideUnitCircle;
            reward.Drop(dropPosition.position + randomOffset);
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
            Debug.LogWarning("Attempted to open chest " + collision.gameObject.name);
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
    
    public void ForceOpen()
    {
        isOpened = true;
        tooltipActive = false;
        animator.Play("Open", 0);
    }
    
    public void DestroyTreasureChest()
    {
        Debug.Log("Destroying chest");
        StartCoroutine(FadeAndDestroy());
    }
    
    private IEnumerator FadeAndDestroy()
    {
        yield return new WaitForSeconds(2f);
        VisualFeedbackManager visualFeedback = GetComponent<VisualFeedbackManager>();
        if (visualFeedback != null)
        {
            yield return visualFeedback.FadeOut(2f, gameObject);
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

    public void LoadSavedChestStates(ChestSaveState savedChest)
    {
        if (savedChest.isOpened)
        {
            isOpened = true;
            ForceOpen();
        }
        else
        {
            isOpened = false;
            foreach (var reward in savedChest.chestRewards)
            {
                string[] parts = reward.Split(':');
                if (parts.Length == 2) // slotindex, isWeapon, name
                {
                    ItemRewardDrop chestReward;
                    string type = parts[0];
                    string name = parts[1];

                    if (type == "Item")
                    {
                        var item = ItemDatabase.CreateItem(name);
                        var itemPayload = new ItemPayload()
                        {
                            weaponScript = null,
                            itemScript = item,
                            isWeapon = false
                        };
                        chestReward = new ItemRewardDrop(itemPayload);
                    }
                    else
                    {
                        var weapon = WeaponDatabase.Instance.CreateWeaponInstance(name);
                        var weaponPayload = new ItemPayload()
                        {
                            weaponScript = weapon,
                            itemScript = null,
                            isWeapon = true
                        };
                        chestReward = new ItemRewardDrop(weaponPayload);
                    }
                    AddReward(chestReward);
                }
            }
        }

        Initialize();
    }
}
