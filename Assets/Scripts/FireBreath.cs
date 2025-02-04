using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBreath : MonoBehaviour
{
    private ParticleSystem fireBreathParticles;
    private float fireBreathAngle;

    private void Awake()
    {
        fireBreathParticles = GetComponent<ParticleSystem>();
        //var mainModule = fireBreathParticles.main;
        //mainModule.stopAction = ParticleSystemStopAction.Callback;
    }

    private void OnEnable()
    {
        //SetStats(fireBreathAngle);

        if (!fireBreathParticles.isPlaying)
        {
            fireBreathParticles.Play();
        }
    }

    public void Initialize(float angle)
    {
        fireBreathAngle = angle;

        SetStats(fireBreathAngle);
    }

    private void SetStats(float angle)
    {
        var shape = fireBreathParticles.shape;
        shape.angle = angle;
    }

    private void OnDisable()
    {
        transform.SetParent(FXManager.Instance.transform);

        if (fireBreathParticles.isPlaying)
        {
            fireBreathParticles.Stop();
        }
    }
}