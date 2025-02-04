using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }
    public int startingCurrency = 500;
    public Canvas statDisplayCanvas;
    public GameObject goldNumberPrefab;
    private int currentCurrency;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        EventManager.Instance.StartListening("SceneLoaded", OnSceneLoaded);
        EventManager.Instance.StartListening("CurrencyIncrease", UpdateCurrency);
        InitializeCurrency();
    }

    private void InitializeCurrency()
    {
        currentCurrency = startingCurrency;
        EventManager.Instance.TriggerEvent("CurrencyChanged", currentCurrency);
    }

    private void OnSceneLoaded(string scene)
    {
        Debug.LogWarning("SceneLoadedTriggered!");
        if (scene == "GameScene")
        {
            goldNumberPrefab = Resources.Load<GameObject>("Prefabs/DamageNumbers");
            statDisplayCanvas = GameObject.FindWithTag("StatCanvas").GetComponent<Canvas>();
        }
    }

    private void OnDestroy()
    {
        EventManager.Instance.StopListening("SceneLoaded", OnSceneLoaded);
        EventManager.Instance.StopListening("CurrencyIncrease", UpdateCurrency);
    }

    /*public void AssignStatDisplayCanvas(Canvas canvas)
    {
        statDisplayCanvas = canvas;
    }*/

    public int GetCurrency()
    {
        return currentCurrency;
    }

    public void UpdateCurrency(float amount)
    {
        if (amount > 0)
        {
            currentCurrency += (int)amount;
            EventManager.Instance.TriggerEvent("CurrencyChanged", currentCurrency);
            ShowGoldAmountAcquired((int)amount, 15f);
        }
    }

    public bool HasEnoughFunds(float itemPrice)
    {
        return currentCurrency >= itemPrice;
    }

    public void SpendCurrency(int amount)
    {
        if (HasEnoughFunds(amount))
        {
            currentCurrency -= amount;
            EventManager.Instance.TriggerEvent("CurrencyChanged", currentCurrency);
            Debug.Log($"Funds deducted: {amount}. Remaining: {currentCurrency}.");
        }
    }

    public void ShowGoldAmountAcquired(float amount, float size, Vector3? customPosition = null)
    {
        Color goldColor = new(255 / 255f, 215 / 255f, 0 / 255f);

        Vector3 position = customPosition ?? transform.position + new Vector3(0, 1, 0);

        GameObject damageNumber = Instantiate(goldNumberPrefab, position, Quaternion.identity, statDisplayCanvas.transform);
        Debug.Log($"Instantiate gold amount: {amount}");
        DamageNumber damageNumberScript = damageNumber.GetComponent<DamageNumber>();
        if (damageNumberScript != null)
        {
            damageNumberScript.SetValue(amount);
            damageNumberScript.SetTextColor(goldColor);
            damageNumberScript.SetTextSize(size);
        }
    }

    public void ResetCurrency()
    {
        InitializeCurrency();
    }
}