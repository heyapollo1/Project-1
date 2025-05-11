using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShutdownInterceptor : MonoBehaviour
{
    void OnApplicationQuit()
    {
        Debug.LogWarning("!!! UNITY EXIT TRIGGERED HERE !!!");
        Debug.LogWarning(Environment.StackTrace);
    }
}
