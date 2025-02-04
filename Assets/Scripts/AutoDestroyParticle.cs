using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroyParticle : MonoBehaviour
{
    private ParticleSystem particleSystem;
    private string fxName;

    private void OnEnable()
    {
        particleSystem = GetComponent<ParticleSystem>();
        StartCoroutine(ReturnToPoolAfterParticleEffect());
    }

    public void Initialize(string fxName)
    {
        this.fxName = fxName; // Set the FX name to know which pool to return to
    }

    public void StopEffect()
    {
        if (particleSystem.isPlaying)
        {
            StopAllCoroutines();
            particleSystem.Stop(); // Stop the particle system
        }
    }

    private IEnumerator ReturnToPoolAfterParticleEffect()
    {
        yield return new WaitUntil(() => !particleSystem.IsAlive(true));
        transform.SetParent(FXManager.Instance.transform);
        FXManager.Instance.ReturnToPool(gameObject, fxName);
    }
}