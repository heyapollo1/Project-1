using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagSynergyManager : MonoBehaviour
{
    public static TagSynergyManager Instance;

    private Dictionary<TagType, int> tagCounts = new();
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void RecalculateSynergies(List<TagType> allPlayerTags)
    {
        tagCounts.Clear();
        
        foreach (TagType tag in allPlayerTags)
        {
            if (!tagCounts.ContainsKey(tag))
                tagCounts[tag] = 0;

            tagCounts[tag]++;
        }

        ApplyTagBonuses();
    }

    private void ApplyTagBonuses()
    {
        AttributeManager.Instance.ResetAllStats(); // Optional: resets before applying synergies

        foreach (var tag in tagCounts)
        {
            int count = tag.Value;

            switch (tag.Key)
            {
                case TagType.Food:
                    if (count >= 2)
                        AttributeManager.Instance.ApplyModifier(new StatModifier(StatType.MaxHealth, 10f));
                    break;
                case TagType.Tool:
                    if (count >= 3)
                        AttributeManager.Instance.ApplyModifier(new StatModifier(StatType.MovementSpeed, 10f));
                    break;
                case TagType.Junk:
                    if (count >= 2)
                        AttributeManager.Instance.ApplyModifier(new StatModifier(StatType.MaxHealth, 10f));
                    break;
            }
        }
    }
}
