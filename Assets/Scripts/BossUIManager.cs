using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class BossUIManager : MonoBehaviour
{
    public GameObject bossHealthBar;
    public Slider healthSlider;
    public TextMeshProUGUI bossNameText;

    public void Initialize(string bossName, float maxHealth)
    {
        bossNameText.text = bossName;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;
    }

    public void UpdateHealth(float currentHealth)
    {
        healthSlider.value = currentHealth;
    }

    public void Show()
    {
        bossHealthBar.SetActive(true);
    }

    public void Hide()
    {
        bossHealthBar.SetActive(false);
    }
}
