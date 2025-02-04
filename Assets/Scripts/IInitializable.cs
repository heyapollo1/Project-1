using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInitializable
{
    int Priority { get; } // Determines the initialization order (lower values = earlier init)
    void Initialize();    // Initialization logic for the manager
}