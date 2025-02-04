using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameRateScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Limit framerate to cinematic 24fps.
        QualitySettings.vSyncCount = 0; // Set vSyncCount to 0 so that using .targetFrameRate is enabled.
        Application.targetFrameRate = 60;
    }
        // Update is called once per frame
        void Update()
    {

    }
}
