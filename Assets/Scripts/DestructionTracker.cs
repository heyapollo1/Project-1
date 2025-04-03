using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructionTracker : MonoBehaviour
{
    private void OnDestroy()
    {
        Debug.LogError($"Object '{gameObject.name}' destroyed at {Time.time}! Called from:\n{new System.Diagnostics.StackTrace()}");
    }
}
