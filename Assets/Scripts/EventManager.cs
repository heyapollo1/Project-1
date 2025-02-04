using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : BaseManager
{
    public static EventManager Instance { get; private set; }
    public override int Priority => 5;

    //private Dictionary<string, Delegate> eventDictionary = new Dictionary<string, Delegate>();
    //private Dictionary<string, string> queuedEvents = new Dictionary<string, string>();

    private Dictionary<string, UnityEvent> eventDictionary;
    private Dictionary<string, UnityEvent<string>> eventDictionaryWithString;
    private Dictionary<string, UnityEvent<string, int>> eventDictionaryWithStringAndInt;
    private Dictionary<string, UnityEvent<string, float>> eventDictionaryWithStringAndFloat;
    private Dictionary<string, UnityEvent<float>> eventDictionaryWithFloat;
    private Dictionary<string, UnityEvent<float, float>> eventDictionaryWithFloatAndFloat;

    #region UpgradeEvents
    private Dictionary<string, UnityEvent<UpgradeData>> eventDictionaryWithUpgradeData;
    private Dictionary<string, UnityEvent<List<UpgradeData>>> eventDictionaryWithUpgradeDataList;
    #endregion

    #region AbilityEvents
    private Dictionary<string, UnityEvent<PlayerAbilityData>> eventDictionaryWithAbilityData;
    private Dictionary<string, UnityEvent<List<PlayerAbilityData>>> eventDictionaryWithAbilityDataList;
    private Dictionary<string, UnityEvent<PlayerAbilityData, float>> eventDictionaryWithAbilityDataAndFloat;
    #endregion

    #region ItemEvents
    private Dictionary<string, UnityEvent<ItemData>> eventDictionaryWithItemData;
    private Dictionary<string, UnityEvent<List<ItemData>>> eventDictionaryWithItemDataList;
    private Dictionary<string, UnityEvent<List<InventoryUISlot>>> eventDictionaryWithInventorySlotEvents;
    #endregion

    #region WaveEvents
    private Dictionary<string, UnityEvent<WaveSettings>> eventDictionaryWithWaveSettings;
    #endregion

    #region SoundEvents
    private Dictionary<string, UnityEvent<SoundRequest>> eventDictionaryWithSoundRequest;
    #endregion

    #region GridEvents
    private Dictionary<string, UnityEvent<SceneGrid>> eventDictionaryWithGrid;
    #endregion

    #region LocationEvents
    private Dictionary<string, UnityEvent<Vector3>> eventDictionaryWithVector3;
    private Dictionary<string, UnityEvent<string, Vector3>> eventDictionaryWithStringAndVector3;
    private Dictionary<string, UnityEvent<Transform>> eventDictionaryWithTransform;
    #endregion

    protected override void OnInitialize()
    {
        eventDictionary = new Dictionary<string, UnityEvent>();
        eventDictionaryWithString = new Dictionary<string, UnityEvent<string>>();
        eventDictionaryWithStringAndInt = new Dictionary<string, UnityEvent<string, int>>();
        eventDictionaryWithStringAndFloat = new Dictionary<string, UnityEvent<string, float>>();
        eventDictionaryWithFloat = new Dictionary<string, UnityEvent<float>>();
        eventDictionaryWithFloatAndFloat = new Dictionary<string, UnityEvent<float, float>>();

        eventDictionaryWithUpgradeData = new Dictionary<string, UnityEvent<UpgradeData>>();
        eventDictionaryWithUpgradeDataList = new Dictionary<string, UnityEvent<List<UpgradeData>>>();

        eventDictionaryWithAbilityData = new Dictionary<string, UnityEvent<PlayerAbilityData>>();
        eventDictionaryWithAbilityDataList = new Dictionary<string, UnityEvent<List<PlayerAbilityData>>>();
        eventDictionaryWithAbilityDataAndFloat = new Dictionary<string, UnityEvent<PlayerAbilityData, float>>();

        eventDictionaryWithItemData = new Dictionary<string, UnityEvent<ItemData>>();
        eventDictionaryWithItemDataList = new Dictionary<string, UnityEvent<List<ItemData>>>();
        eventDictionaryWithInventorySlotEvents = new Dictionary<string, UnityEvent<List<InventoryUISlot>>>();

        eventDictionaryWithWaveSettings = new Dictionary<string, UnityEvent<WaveSettings>>();

        eventDictionaryWithSoundRequest = new Dictionary<string, UnityEvent<SoundRequest>>();

        eventDictionaryWithGrid = new Dictionary<string, UnityEvent<SceneGrid>>();

        eventDictionaryWithVector3 = new Dictionary<string, UnityEvent<Vector3>>();
        eventDictionaryWithStringAndVector3 = new Dictionary<string, UnityEvent<string, Vector3>>();
        eventDictionaryWithTransform = new Dictionary<string, UnityEvent<Transform>>();
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //Base Events 
    public void StartListening(string eventName, UnityAction listener)
    {
        if (eventDictionary.TryGetValue(eventName, out UnityEvent thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent();
            thisEvent.AddListener(listener);
            eventDictionary.Add(eventName, thisEvent);
        }
    }

    public void StopListening(string eventName, UnityAction listener)
    {
        if (eventDictionary.TryGetValue(eventName, out UnityEvent thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public void TriggerEvent(string eventName)
    {
        if (eventDictionary.TryGetValue(eventName, out UnityEvent thisEvent))
        {
            thisEvent.Invoke();
        }
    }

    //Events with strings
    public void StartListening(string eventName, UnityAction<string> listener)
    {
        if (eventDictionaryWithString.TryGetValue(eventName, out UnityEvent<string> thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent<string>();
            thisEvent.AddListener(listener);
            eventDictionaryWithString.Add(eventName, thisEvent);
        }
    }

    public void StopListening(string eventName, UnityAction<string> listener)
    {
        if (eventDictionaryWithString.TryGetValue(eventName, out UnityEvent<string> thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public void TriggerEvent(string eventName, string parameter)
    {
        if (eventDictionaryWithString.TryGetValue(eventName, out UnityEvent<string> thisEvent))
        {
            Debug.LogWarning($"Event {eventName} triggered.");
            thisEvent.Invoke(parameter);
        }
        else
        {
            Debug.LogWarning($"Event {eventName} has no listeners. Queuing the event.");
            //queuedEvents[eventName] = parameter;
        }
    }

    // String and Int-based events
    public void StartListening(string eventName, UnityAction<string, int> listener)
    {
        if (eventDictionaryWithStringAndInt.TryGetValue(eventName, out UnityEvent<string, int> thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent<string, int>();
            thisEvent.AddListener(listener);
            eventDictionaryWithStringAndInt.Add(eventName, thisEvent);
        }
    }

    public void StopListening(string eventName, UnityAction<string, int> listener)
    {
        if (eventDictionaryWithStringAndInt.TryGetValue(eventName, out UnityEvent<string, int> thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public void TriggerEvent(string eventName, string stringParameter, int intParameter)
    {
        if (eventDictionaryWithStringAndInt.TryGetValue(eventName, out UnityEvent<string, int> thisEvent))
        {
            thisEvent.Invoke(stringParameter, intParameter);
        }
    }
    //string and float
    public void StartListening(string eventName, UnityAction<string, float> listener)
    {
        if (eventDictionaryWithStringAndFloat.TryGetValue(eventName, out UnityEvent<string, float> thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent<string, float>();
            thisEvent.AddListener(listener);
            eventDictionaryWithStringAndFloat.Add(eventName, thisEvent);
        }
    }

    public void StopListening(string eventName, UnityAction<string, float> listener)
    {
        if (eventDictionaryWithStringAndFloat.TryGetValue(eventName, out UnityEvent<string, float> thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public void TriggerEvent(string eventName, string stringParameter, float floatParameter)
    {
        if (eventDictionaryWithStringAndFloat.TryGetValue(eventName, out UnityEvent<string, float> thisEvent))
        {
            thisEvent.Invoke(stringParameter, floatParameter);
        }
    }

    // Float based events
    public void StartListening(string eventName, UnityAction<float> listener)
    {
        if (eventDictionaryWithFloat.TryGetValue(eventName, out UnityEvent<float> thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent<float>();
            thisEvent.AddListener(listener);
            eventDictionaryWithFloat.Add(eventName, thisEvent);
        }
    }

    public void StopListening(string eventName, UnityAction<float> listener)
    {
        if (eventDictionaryWithFloat.TryGetValue(eventName, out UnityEvent<float> thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public void TriggerEvent(string eventName, float floatParameter)
    {
        if (eventDictionaryWithFloat.TryGetValue(eventName, out UnityEvent<float> thisEvent))
        {
            thisEvent.Invoke(floatParameter);
        }
    }

    // Float and Float-based events
    public void StartListening(string eventName, UnityAction<float, float> listener)
    {
        if (eventDictionaryWithFloatAndFloat.TryGetValue(eventName, out UnityEvent<float, float> thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent<float, float>();
            thisEvent.AddListener(listener);
            eventDictionaryWithFloatAndFloat.Add(eventName, thisEvent);
        }
    }

    public void StopListening(string eventName, UnityAction<float, float> listener)
    {
        if (eventDictionaryWithFloatAndFloat.TryGetValue(eventName, out UnityEvent<float, float> thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public void TriggerEvent(string eventName, float firstFloatParameter, float SecondFloatParameter)
    {
        if (eventDictionaryWithFloatAndFloat.TryGetValue(eventName, out UnityEvent<float, float> thisEvent))
        {
            thisEvent.Invoke(firstFloatParameter, SecondFloatParameter);
        }
    }

    // UpgradeData based events ---

    // UpgradeData-based events
    public void StartListening(string eventName, UnityAction<UpgradeData> listener)
    {
        if (eventDictionaryWithUpgradeData.TryGetValue(eventName, out UnityEvent<UpgradeData> thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent<UpgradeData>();
            thisEvent.AddListener(listener);
            eventDictionaryWithUpgradeData.Add(eventName, thisEvent);
        }
    }

    public void StopListening(string eventName, UnityAction<UpgradeData> listener)
    {
        if (eventDictionaryWithUpgradeData.TryGetValue(eventName, out UnityEvent<UpgradeData> thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public void TriggerEvent(string eventName, UpgradeData upgradeData)
    {
        if (eventDictionaryWithUpgradeData.TryGetValue(eventName, out UnityEvent<UpgradeData> thisEvent))
        {
            thisEvent.Invoke(upgradeData);
        }
    }

    // List<UpgradeData> based events
    public void StartListening(string eventName, UnityAction<List<UpgradeData>> listener)
    {
        if (eventDictionaryWithUpgradeDataList.TryGetValue(eventName, out UnityEvent<List<UpgradeData>> thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent<List<UpgradeData>>();
            thisEvent.AddListener(listener);
            eventDictionaryWithUpgradeDataList.Add(eventName, thisEvent);
        }
    }

    public void StopListening(string eventName, UnityAction<List<UpgradeData>> listener)
    {
        if (eventDictionaryWithUpgradeDataList.TryGetValue(eventName, out UnityEvent<List<UpgradeData>> thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public void TriggerEvent(string eventName, List<UpgradeData> upgradeDataList)
    {
        if (eventDictionaryWithUpgradeDataList.TryGetValue(eventName, out UnityEvent<List<UpgradeData>> thisEvent))
        {
            thisEvent.Invoke(upgradeDataList);
        }
    }

    // Ability Data based events ---

    // Ability Data based events
    public void StartListening(string eventName, UnityAction<PlayerAbilityData> listener)
    {
        if (eventDictionaryWithAbilityData.TryGetValue(eventName, out UnityEvent<PlayerAbilityData> thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent<PlayerAbilityData>();
            thisEvent.AddListener(listener);
            eventDictionaryWithAbilityData.Add(eventName, thisEvent);
        }
    }

    public void StopListening(string eventName, UnityAction<PlayerAbilityData> listener)
    {
        if (eventDictionaryWithAbilityData.TryGetValue(eventName, out UnityEvent<PlayerAbilityData> thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public void TriggerEvent(string eventName, PlayerAbilityData upgradeData)
    {
        if (eventDictionaryWithAbilityData.TryGetValue(eventName, out UnityEvent<PlayerAbilityData> thisEvent))
        {
            thisEvent.Invoke(upgradeData);
        }
    }

    // List<AbilityData> based events
    public void StartListening(string eventName, UnityAction<List<PlayerAbilityData>> listener)
    {
        if (eventDictionaryWithAbilityDataList.TryGetValue(eventName, out UnityEvent< List <PlayerAbilityData>> thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent<List<PlayerAbilityData>>();
            thisEvent.AddListener(listener);
            eventDictionaryWithAbilityDataList.Add(eventName, thisEvent);
        }
    }

    public void StopListening(string eventName, UnityAction<List<PlayerAbilityData>> listener)
    {
        if (eventDictionaryWithAbilityDataList.TryGetValue(eventName, out UnityEvent<List<PlayerAbilityData>> thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public void TriggerEvent(string eventName, List<PlayerAbilityData> upgradeDataList)
    {
        if (eventDictionaryWithAbilityDataList.TryGetValue(eventName, out UnityEvent<List<PlayerAbilityData>> thisEvent))
        {
            thisEvent.Invoke(upgradeDataList);
        }
    }

    // Ability Data and Float based events
    public void StartListening(string eventName, UnityAction<PlayerAbilityData, float> listener)
    {
        if (eventDictionaryWithAbilityDataAndFloat.TryGetValue(eventName, out UnityEvent<PlayerAbilityData, float> thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent<PlayerAbilityData, float>();
            thisEvent.AddListener(listener);
            eventDictionaryWithAbilityDataAndFloat.Add(eventName, thisEvent);
        }
    }

    public void StopListening(string eventName, UnityAction<PlayerAbilityData, float> listener)
    {
        if (eventDictionaryWithAbilityDataAndFloat.TryGetValue(eventName, out UnityEvent<PlayerAbilityData, float> thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public void TriggerEvent(string eventName, PlayerAbilityData abilityData, float cooldownDuration)
    {
        if (eventDictionaryWithAbilityDataAndFloat.TryGetValue(eventName, out UnityEvent<PlayerAbilityData, float> thisEvent))
        {
            thisEvent.Invoke(abilityData, cooldownDuration);
        }
    }

    // List<ItemData> based events
    public void StartListening(string eventName, UnityAction<ItemData> listener)
    {
        if (eventDictionaryWithItemData.TryGetValue(eventName, out UnityEvent<ItemData> thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent<ItemData>();
            thisEvent.AddListener(listener);
            eventDictionaryWithItemData.Add(eventName, thisEvent);
        }
    }

    public void StopListening(string eventName, UnityAction<ItemData> listener)
    {
        if (eventDictionaryWithItemData.TryGetValue(eventName, out UnityEvent<ItemData> thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public void TriggerEvent(string eventName, ItemData itemData)
    {
        if (eventDictionaryWithItemData.TryGetValue(eventName, out UnityEvent<ItemData> thisEvent))
        {
            thisEvent.Invoke(itemData);
        }
    }

    // List<ItemData> based events
    public void StartListening(string eventName, UnityAction<List<ItemData>> listener)
    {
        if (eventDictionaryWithItemDataList.TryGetValue(eventName, out UnityEvent<List<ItemData>> thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent<List<ItemData>>();
            thisEvent.AddListener(listener);
            eventDictionaryWithItemDataList.Add(eventName, thisEvent);
        }
    }

    public void StopListening(string eventName, UnityAction<List<ItemData>> listener)
    {
        if (eventDictionaryWithItemDataList.TryGetValue(eventName, out UnityEvent<List<ItemData>> thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public void TriggerEvent(string eventName, List<ItemData> upgradeDataList)
    {
        if (eventDictionaryWithItemDataList.TryGetValue(eventName, out UnityEvent<List<ItemData>> thisEvent))
        {
            thisEvent.Invoke(upgradeDataList);
        }
    }

    // InventorySlot events
    public void StartListening(string eventName, UnityAction<List<InventoryUISlot>> listener)
    {
        if (eventDictionaryWithInventorySlotEvents.TryGetValue(eventName, out UnityEvent<List<InventoryUISlot>> thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent<List<InventoryUISlot>>();
            thisEvent.AddListener(listener);
            eventDictionaryWithInventorySlotEvents.Add(eventName, thisEvent);
        }
    }

    public void StopListening(string eventName, UnityAction<List<InventoryUISlot>> listener)
    {
        if (eventDictionaryWithInventorySlotEvents.TryGetValue(eventName, out UnityEvent<List<InventoryUISlot>> thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public void TriggerEvent(string eventName, List<InventoryUISlot> inventorySlots)
    {
        if (eventDictionaryWithInventorySlotEvents.TryGetValue(eventName, out UnityEvent<List<InventoryUISlot>> thisEvent))
        {
            thisEvent.Invoke(inventorySlots);
        }
    }

    // Add a listener for WaveSettings events
    public void StartListening(string eventName, UnityAction<WaveSettings> listener)
    {
        if (eventDictionaryWithWaveSettings.TryGetValue(eventName, out UnityEvent<WaveSettings> thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent<WaveSettings>();
            thisEvent.AddListener(listener);
            eventDictionaryWithWaveSettings.Add(eventName, thisEvent);
        }
    }

    public void StopListening(string eventName, UnityAction<WaveSettings> listener)
    {
        if (eventDictionaryWithWaveSettings.TryGetValue(eventName, out UnityEvent<WaveSettings> thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public void TriggerEvent(string eventName, WaveSettings eventData)
    {
        if (eventDictionaryWithWaveSettings.TryGetValue(eventName, out UnityEvent<WaveSettings> thisEvent))
        {
            thisEvent.Invoke(eventData);
        }
    }

    // Add a listener for Sound events
    public void StartListening(string eventName, UnityAction<SoundRequest> listener)
    {
        if (eventDictionaryWithSoundRequest.TryGetValue(eventName, out var thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            UnityEvent<SoundRequest> newEvent = new UnityEvent<SoundRequest>();
            newEvent.AddListener(listener);
            eventDictionaryWithSoundRequest.Add(eventName, newEvent);
        }
    }

    public void StopListening(string eventName, UnityAction<SoundRequest> listener)
    {
        if (eventDictionaryWithSoundRequest.TryGetValue(eventName, out var thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public void TriggerEvent(string eventName, SoundRequest request)
    {
        if (eventDictionaryWithSoundRequest.TryGetValue(eventName, out var thisEvent))
        {
            thisEvent.Invoke(request);
        }
        else
        {
            Debug.LogWarning($"Event {eventName} not found.");
        }
    }

    public void StartListening(string eventName, UnityAction<SceneGrid> listener)
    {
        if (eventDictionaryWithGrid.TryGetValue(eventName, out var thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            UnityEvent<SceneGrid> newEvent = new UnityEvent<SceneGrid>();
            newEvent.AddListener(listener);
            eventDictionaryWithGrid.Add(eventName, newEvent);
        }
    }

    public void StopListening(string eventName, UnityAction<SceneGrid> listener)
    {
        if (eventDictionaryWithGrid.TryGetValue(eventName, out var thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public void TriggerEvent(string eventName, SceneGrid grid)
    {
        if (grid == null)
        {
            Debug.LogError($"TriggerEvent: Null SceneGrid passed for event {eventName}");
            return;
        }

        if (eventDictionaryWithGrid.TryGetValue(eventName, out var thisEvent))
        {
            thisEvent.Invoke(grid);
        }
        else
        {
            Debug.LogWarning($"Event {eventName} not found.");
        }
    }

     // Location Vector events
     public void StartListening(string eventName, UnityAction<Vector3> listener)
    {
        if (eventDictionaryWithVector3.TryGetValue(eventName, out UnityEvent<Vector3> thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent<Vector3>();
            thisEvent.AddListener(listener);
            eventDictionaryWithVector3.Add(eventName, thisEvent);
        }
    }

    public void StopListening(string eventName, UnityAction<Vector3> listener)
    {
        if (eventDictionaryWithVector3.TryGetValue(eventName, out var thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public void TriggerEvent(string eventName, Vector3 size)
    {
        if (eventDictionaryWithVector3.TryGetValue(eventName, out UnityEvent<Vector3> thisEvent))
        {
            thisEvent.Invoke(size);
        }
        else
        {
            Debug.LogWarning($"Event {eventName} not found.");
        }
    }

    public void StartListening(string eventName, UnityAction<string, Vector3> listener)
    {
        if (eventDictionaryWithStringAndVector3.TryGetValue(eventName, out UnityEvent<string, Vector3> thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent<string, Vector3>();
            thisEvent.AddListener(listener);
            eventDictionaryWithStringAndVector3.Add(eventName, thisEvent);
        }
    }

    public void StopListening(string eventName, UnityAction<string, Vector3> listener)
    {
        if (eventDictionaryWithStringAndVector3.TryGetValue(eventName, out UnityEvent<string, Vector3> thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public void TriggerEvent(string eventName, string stringParameter, Vector3 locationParamter)
    {
        if (eventDictionaryWithStringAndVector3.TryGetValue(eventName, out UnityEvent<string, Vector3> thisEvent))
        {
            thisEvent.Invoke(stringParameter, locationParamter);
        }
        else
        {
            Debug.LogWarning($"Event {eventName} not found.");
        }
    }

    public void StartListening(string eventName, UnityAction<Transform> listener)
    {
        if (eventDictionaryWithTransform.TryGetValue(eventName, out UnityEvent<Transform> thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent<Transform>();
            thisEvent.AddListener(listener);
            eventDictionaryWithTransform.Add(eventName, thisEvent);
        }
    }

    public void StopListening(string eventName, UnityAction<Transform> listener)
    {
        if (eventDictionaryWithTransform.TryGetValue(eventName, out UnityEvent<Transform> thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public void TriggerEvent(string eventName, Transform locationParamter)
    {
        if (eventDictionaryWithTransform.TryGetValue(eventName, out UnityEvent<Transform> thisEvent))
        {
            thisEvent.Invoke(locationParamter);
        }
        else
        {
            Debug.LogWarning($"Event {eventName} not found.");
        }
    }
}