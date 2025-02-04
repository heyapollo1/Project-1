
using UnityEngine;
using Unity.Cinemachine;

public class CameraShakeManager : MonoBehaviour
{
    public static CameraShakeManager Instance { get; private set; }

    public CinemachineBasicMultiChannelPerlin perlin;

    private float shakeTimer;
    private float shakeTimerTotal;
    private float startingAmplitude;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Ensure the Perlin component is assigned
        if (perlin == null)
        {
            Debug.LogError("CinemachineBasicMultiChannelPerlin not assigned! Please assign it in the Inspector.");
        }
        else
        {
            // Reset the amplitude to avoid initial shaking
            perlin.AmplitudeGain = 0f;
        }
    }

    public void ShakeCamera(float amplitude, float frequency, float duration)
    {
        if (perlin != null)
        {
            perlin.AmplitudeGain = amplitude;
            perlin.FrequencyGain = frequency;

            shakeTimer = duration;
            shakeTimerTotal = duration;
            startingAmplitude = amplitude;
        }
    }


    private void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;

            if (shakeTimer <= 0f && perlin != null)
            {
                // Reset shake values
                perlin.AmplitudeGain = 0f;
                perlin.FrequencyGain = 0f;
            }
        }
    }

    void OnDisable()
    {
        if (perlin != null)
        {
            perlin.AmplitudeGain = 0f;  // Reset amplitude when exiting play mode
        }
    }
}