using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HubManager : MonoBehaviour
{
    public void Initialize()
    {
        EventManager.Instance.StartListening("ReturnToHub", HubReturn);
    }
    public void OnDestroy()
    {
        EventManager.Instance.StopListening("ReturnToHub", HubReturn);
    }

    private void HubReturn()
    {
        Debug.Log("Hub Return");
    }
}
