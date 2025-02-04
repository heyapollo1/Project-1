using UnityEngine;
using Unity.Cinemachine;

public class ScreenShakeManager : MonoBehaviour
{
    public static ScreenShakeManager Instance { get; private set; }

    [Header("Cinemachine")]
    public CinemachineVirtualCamera virtualCamera;

    private CinemachineBasicMultiChannelPerlin perlinNoise;
    private float shakeTimer;
    private float shakeTimerTotal;
    private float startingAmplitude;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (virtualCamera != null)
        {
            perlinNoise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }
    }

    private void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;

            if (shakeTimer <= 0f && perlinNoise != null)
            {
                // Reset shake values
                perlinNoise.AmplitudeGain = 0f;
                perlinNoise.FrequencyGain = 0f;
            }
        }
    }

    public void ShakeCamera(float amplitude, float frequency, float duration)
    {
        if (perlinNoise != null)
        {
            perlinNoise.AmplitudeGain = amplitude;
            perlinNoise.FrequencyGain = frequency;

            shakeTimer = duration;
            shakeTimerTotal = duration;
            startingAmplitude = amplitude;
        }
    }
}