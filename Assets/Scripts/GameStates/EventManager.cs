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
    private Dictionary<string, UnityEvent<string, bool, bool>> eventDictionaryWithBoolAndBool;
    private Dictionary<string, UnityEvent<string, bool>> eventDictionaryWithBool;

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
    private Dictionary<string, UnityEvent<List<BaseItem>>> eventDictionaryWithItemBaseList;
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
    private Dictionary<string, UnityEvent<string, Transform>> eventDictionaryWithStringAndTransform;
    private Dictionary<string, UnityEvent<Transform>> eventDictionaryWithTransform;
    #endregion
    
    #region WeaponEvents
    private Dictionary<string, UnityEvent<WeaponData>> eventDictionaryWithWeaponData;
    #endregion
    
    #region GameDataEvents
    private Dictionary<string, UnityEvent<GameData>> eventDictionaryWithGameData;
    private Dictionary<string, UnityEvent<GameData, bool>> eventDictionaryWithGameDataandBool;
    #endregion
    
    #region StageConfigEvents
    private Dictionary<string, UnityEvent<StageConfig>> eventDictionaryWithStageConfig;
    #endregion
    
    #region EncounterEvents
    private Dictionary<string, UnityEvent<EncounterData>> eventDictionaryWithEncounterData;
    #endregion
    
    protected override void OnInitialize()
    {
        eventDictionary = new Dictionary<string, UnityEvent>();
        eventDictionaryWithString = new Dictionary<string, UnityEvent<string>>();
        eventDictionaryWithStringAndInt = new Dictionary<string, UnityEvent<string, int>>();
        eventDictionaryWithStringAndFloat = new Dictionary<string, UnityEvent<string, float>>();
        eventDictionaryWithFloat = new Dictionary<string, UnityEvent<float>>();
        eventDictionaryWithFloatAndFloat = new Dictionary<string, UnityEvent<float, float>>();
        eventDictionaryWithBoolAndBool = new Dictionary<string, UnityEvent<string, bool, bool>>();
        eventDictionaryWithBool = new Dictionary<string, UnityEvent<string, bool>>();

        eventDictionaryWithUpgradeData = new Dictionary<string, UnityEvent<UpgradeData>>();
        eventDictionaryWithUpgradeDataList = new Dictionary<string, UnityEvent<List<UpgradeData>>>();

        eventDictionaryWithAbilityData = new Dictionary<string, UnityEvent<PlayerAbilityData>>();
        eventDictionaryWithAbilityDataList = new Dictionary<string, UnityEvent<List<PlayerAbilityData>>>();
        eventDictionaryWithAbilityDataAndFloat = new Dictionary<string, UnityEvent<PlayerAbilityData, float>>();

        eventDictionaryWithItemData = new Dictionary<string, UnityEvent<ItemData>>();
        eventDictionaryWithItemBaseList = new Dictionary<string, UnityEvent<List<BaseItem>>>();
        eventDictionaryWithInventorySlotEvents = new Dictionary<string, UnityEvent<List<InventoryUISlot>>>();

        eventDictionaryWithWaveSettings = new Dictionary<string, UnityEvent<WaveSettings>>();

        eventDictionaryWithSoundRequest = new Dictionary<string, UnityEvent<SoundRequest>>();

        eventDictionaryWithGrid = new Dictionary<string, UnityEvent<SceneGrid>>();

        eventDictionaryWithVector3 = new Dictionary<string, UnityEvent<Vector3>>();
        eventDictionaryWithStringAndVector3 = new Dictionary<string, UnityEvent<string, Vector3>>();
        eventDictionaryWithStringAndTransform = new Dictionary<string, UnityEvent<string, Transform>>();
        eventDictionaryWithTransform = new Dictionary<string, UnityEvent<Transform>>();
        
        eventDictionaryWithWeaponData = new Dictionary<string, UnityEvent<WeaponData>>();
        
        eventDictionaryWithGameData = new Dictionary<string, UnityEvent<GameData>>();
        eventDictionaryWithGameDataandBool = new Dictionary<string, UnityEvent<GameData, bool>>();
        
        eventDictionaryWithStageConfig = new Dictionary<string, UnityEvent<StageConfig>>();
        
        eventDictionaryWithEncounterData = new Dictionary<string, UnityEvent<EncounterData>>();
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
            //Debug.LogWarning($"Event {eventName} triggered.");
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
    
    //string bool bool
    public void StartListening(string eventName, UnityAction<string, bool, bool> listener)
    {
        if (eventDictionaryWithBoolAndBool.TryGetValue(eventName, out UnityEvent<string, bool, bool> thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent<string, bool, bool>();
            thisEvent.AddListener(listener);
            eventDictionaryWithBoolAndBool.Add(eventName, thisEvent);
        }
    }

    public void StopListening(string eventName, UnityAction<string, bool, bool> listener)
    {
        if (eventDictionaryWithBoolAndBool.TryGetValue(eventName, out UnityEvent<string, bool, bool> thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public void TriggerEvent(string eventName, string stringParameter, bool firstBoolParameter, bool secondBoolParameter)
    {
        if (eventDictionaryWithBoolAndBool.TryGetValue(eventName, out UnityEvent<string, bool, bool> thisEvent))
        {
            thisEvent.Invoke(stringParameter, firstBoolParameter, secondBoolParameter);
        }
    }
    
    //string bool
    //public void StartListening(string eventName, UnityAction<string, Transform> listener)
    public void StartListening(string eventName, UnityAction<string, bool> listener)
    {
        if (eventDictionaryWithBool.TryGetValue(eventName, out UnityEvent<string, bool> thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent<string, bool>();
            thisEvent.AddListener(listener);
            eventDictionaryWithBool.Add(eventName, thisEvent);
        }
    }

    public void StopListening(string eventName, UnityAction<string, bool> listener)
    {
        if (eventDictionaryWithBool.TryGetValue(eventName, out UnityEvent<string, bool> thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public void TriggerEvent(string eventName, string stringParameter, bool boolParameter)
    {
        if (eventDictionaryWithBool.TryGetValue(eventName, out UnityEvent<string, bool> thisEvent))
        {
            thisEvent.Invoke(stringParameter, boolParameter);
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
    public void StartListening(string eventName, UnityAction<List<BaseItem>> listener)
    {
        if (eventDictionaryWithItemBaseList.TryGetValue(eventName, out UnityEvent<List<BaseItem>> thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent<List<BaseItem>>();
            thisEvent.AddListener(listener);
            eventDictionaryWithItemBaseList.Add(eventName, thisEvent);
        }
    }

    public void StopListening(string eventName, UnityAction<List<BaseItem>> listener)
    {
        if (eventDictionaryWithItemBaseList.TryGetValue(eventName, out UnityEvent<List<BaseItem>> thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public void TriggerEvent(string eventName, List<BaseItem> baseItemList)
    {
        if (eventDictionaryWithItemBaseList.TryGetValue(eventName, out UnityEvent<List<BaseItem>> thisEvent))
        {
            thisEvent.Invoke(baseItemList);
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
    
    /*public void StartListening(string eventName, UnityAction<string, Transform> listener)
    {
        if (!eventDictionaryWithStringAndTransform.TryGetValue(eventName, out UnityEvent<string, Transform> thisEvent))
        {
            thisEvent = new UnityEvent<string, Transform>();
            eventDictionaryWithStringAndTransform[eventName] = thisEvent;
        }

        thisEvent.AddListener(listener);
    }

    public void StopListening(string eventName, UnityAction<string, Transform> listener)
    {
        if (eventDictionaryWithStringAndTransform.TryGetValue(eventName, out UnityEvent<string, Transform> thisEvent))
        {
            if (thisEvent != null)
            {
                thisEvent.RemoveListener(listener);
            }
        }
    }

    public void TriggerEvent(string eventName, string stringParameter, Transform locationParameter)
    {
        if (eventDictionaryWithStringAndTransform.TryGetValue(eventName, out UnityEvent<string, Transform> thisEvent))
        {
            thisEvent.Invoke(stringParameter, locationParameter);
        }
        else
        {
            Debug.LogWarning($"Event '{eventName}' not found.");
        }
    }*/
    
    public void StartListening(string eventName, UnityAction<string, Vector3> listener)
    {
        if (!eventDictionaryWithStringAndVector3.TryGetValue(eventName, out UnityEvent<string, Vector3> thisEvent))
        {
            thisEvent = new UnityEvent<string, Vector3>();
            eventDictionaryWithStringAndVector3[eventName] = thisEvent;
        }

        thisEvent.AddListener(listener);
    }

    public void StopListening(string eventName, UnityAction<string, Vector3> listener)
    {
        if (eventDictionaryWithStringAndVector3.TryGetValue(eventName, out UnityEvent<string, Vector3> thisEvent))
        {
            if (thisEvent != null)
            {
                thisEvent.RemoveListener(listener);
            }
        }
    }

    public void TriggerEvent(string eventName, string stringParameter, Vector3 locationParameter)
    {
        if (eventDictionaryWithStringAndVector3.TryGetValue(eventName, out UnityEvent<string, Vector3> thisEvent))
        {
            thisEvent.Invoke(stringParameter, locationParameter);
        }
        else
        {
            Debug.LogWarning($"Event '{eventName}' not found.");
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
    
    //weapon events
    public void StartListening(string eventName, UnityAction<WeaponData> listener)
    {
        if (eventDictionaryWithWeaponData.TryGetValue(eventName, out UnityEvent<WeaponData> thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent<WeaponData>();
            thisEvent.AddListener(listener);
            eventDictionaryWithWeaponData.Add(eventName, thisEvent);
        }
    }

    public void StopListening(string eventName, UnityAction<WeaponData> listener)
    {
        if (eventDictionaryWithWeaponData.TryGetValue(eventName, out UnityEvent<WeaponData> thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public void TriggerEvent(string eventName, WeaponData weaponData)
    {
        if (eventDictionaryWithWeaponData.TryGetValue(eventName, out UnityEvent<WeaponData> thisEvent))
        {
            thisEvent.Invoke(weaponData);
        }
        else
        {
            Debug.LogWarning($"Event {eventName} not found.");
        }
    }
    
    //Game data events
    //eventDictionaryWithGameData
    public void StartListening(string eventName, UnityAction<GameData> listener)
    {
        if (eventDictionaryWithGameData.TryGetValue(eventName, out UnityEvent<GameData> thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent<GameData>();
            thisEvent.AddListener(listener);
            eventDictionaryWithGameData.Add(eventName, thisEvent);
        }
    }

    public void StopListening(string eventName, UnityAction<GameData> listener)
    {
        if (eventDictionaryWithGameData.TryGetValue(eventName, out UnityEvent<GameData> thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public void TriggerEvent(string eventName, GameData gameData)
    {
        if (eventDictionaryWithGameData.TryGetValue(eventName, out UnityEvent<GameData> thisEvent))
        {
            thisEvent.Invoke(gameData);
        }
        else
        {
            Debug.LogWarning($"Event {eventName} not found.");
        }
    }

    public void StartListening(string eventName, UnityAction<GameData, bool> listener)
    {
        if (eventDictionaryWithGameDataandBool.TryGetValue(eventName, out UnityEvent<GameData, bool> thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent<GameData, bool>();
            thisEvent.AddListener(listener);
            eventDictionaryWithGameDataandBool.Add(eventName, thisEvent);
        }
    }

    public void StopListening(string eventName, UnityAction<GameData, bool> listener)
    {
        if (eventDictionaryWithGameDataandBool.TryGetValue(eventName, out UnityEvent<GameData, bool> thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public void TriggerEvent(string eventName, GameData gameData, bool boolParameter)
    {
        if (eventDictionaryWithGameDataandBool.TryGetValue(eventName, out UnityEvent<GameData, bool> thisEvent))
        {
            thisEvent.Invoke(gameData, boolParameter);
        }
        else
        {
            Debug.LogWarning($"Event {eventName} not found.");
        }
    }
    
    //Stage Config Events
    public void StartListening(string eventName, UnityAction<StageConfig> listener)
    {
        if (eventDictionaryWithStageConfig.TryGetValue(eventName, out UnityEvent<StageConfig> thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent<StageConfig>();
            thisEvent.AddListener(listener);
            eventDictionaryWithStageConfig.Add(eventName, thisEvent);
        }
    }

    public void StopListening(string eventName, UnityAction<StageConfig> listener)
    {
        if (eventDictionaryWithStageConfig.TryGetValue(eventName, out UnityEvent<StageConfig> thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public void TriggerEvent(string eventName, StageConfig stageConfig)
    {
        if (eventDictionaryWithStageConfig.TryGetValue(eventName, out UnityEvent<StageConfig> thisEvent))
        {
            thisEvent.Invoke(stageConfig);
        }
        else
        {
            Debug.LogWarning($"Event {eventName} not found.");
        }
    }
    
    //EncountertType Events
    public void StartListening(string eventName, UnityAction<EncounterData> listener)
    {
        if (eventDictionaryWithEncounterData.TryGetValue(eventName, out UnityEvent<EncounterData> thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityEvent<EncounterData>();
            thisEvent.AddListener(listener);
            eventDictionaryWithEncounterData.Add(eventName, thisEvent);
        }
    }

    public void StopListening(string eventName, UnityAction<EncounterData> listener)
    {
        if (eventDictionaryWithEncounterData.TryGetValue(eventName, out UnityEvent<EncounterData> thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public void TriggerEvent(string eventName, EncounterData encounter)
    {
        if (eventDictionaryWithEncounterData.TryGetValue(eventName, out UnityEvent<EncounterData> thisEvent))
        {
            thisEvent.Invoke(encounter);
        }
        else
        {
            Debug.LogWarning($"Event {eventName} not found.");
        }
    }
}