using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseManager : MonoBehaviour, IInitializable
{
    public virtual int Priority => 30;
    private bool isInitialized = false;

    public void Initialize()
    {
        if (isInitialized) return;
        isInitialized = true;
        Debug.Log(Priority + " - " + gameObject.name + " initialized.");
        OnInitialize();
    }

    protected abstract void OnInitialize();
}