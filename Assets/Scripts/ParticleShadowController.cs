using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleShadowController : MonoBehaviour
{
    public GameObject shadowPrefab;
    private List<GameObject> activeShadows = new List<GameObject>();
    public float initialShadowAlpha = 0.5f;  // Set the initial shade transparency

    private ParticleSystem ps;
    private ParticleSystem.Particle[] particles;

    public float baseShadowOffsetY = -0.5f;  // Adjust this to position shadows slightly "beneath" particles
    public float shadowHeightFactor = 0.1f;  // How much to offset the Y-position based on Z height


    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        particles = new ParticleSystem.Particle[ps.main.maxParticles];

    }
    void Update()
    {
        // Get the currently alive particles
        int numParticlesAlive = ps.GetParticles(particles);

        // Ensure there are enough shadows for each particle
        while (activeShadows.Count < numParticlesAlive)
        {
            GameObject shadow = Instantiate(shadowPrefab);
            activeShadows.Add(shadow);

            // Set the initial transparency of the shadow when it's first instantiated
            SpriteRenderer shadowRenderer = shadow.GetComponent<SpriteRenderer>();
            Color shadowColor = shadowRenderer.color;
            shadowColor.a = initialShadowAlpha;  // Use the initial alpha set in the inspector
        }

        for (int i = 0; i < numParticlesAlive; i++)
        {
            Vector3 particlePosition = particles[i].position;
            activeShadows[i].SetActive(true);

            // Calculate the Y-offset based on the particle's Z height
            float yOffset = baseShadowOffsetY - (particlePosition.z * shadowHeightFactor);

            // Keep the shadow at a fixed Z-position (ground level)
            Vector3 shadowPosition = new Vector3(particlePosition.x, particlePosition.y + yOffset, 0);
            activeShadows[i].transform.position = shadowPosition;

            // Get lifetime of particle
            float lifetimeRatio = particles[i].remainingLifetime / particles[i].startLifetime;

            float shadowAlpha = initialShadowAlpha;
            if (lifetimeRatio <= 0.2f)  // If remaining lifetime is less than 20%, start fading
            {
                shadowAlpha = Mathf.Lerp(initialShadowAlpha, 0, 1 - (lifetimeRatio / 0.2f));  // Gradually fade out
            }
            // Apply the shadow scale based on the particle size
            float particleSize = particles[i].GetCurrentSize(ps);

            // Adjust X-stretching factor to make shadow appear more stretched horizontally
            float xStretch = 1.2f;  // Adjust this value to control how much the shadow is stretched on the X-axis
            float scale = Mathf.Clamp(particleSize, 0.8f, 1f);  // Adjust size
            //activeShadows[i].transform.localScale = new Vector3(scale, scale, 1.0f);

            // Apply the X-stretch to the shadow's local scale
            activeShadows[i].transform.localScale = new Vector3(scale * xStretch, scale, 1.0f);
            // Apply the alpha value to the shadow's SpriteRenderer
            SpriteRenderer shadowRenderer = activeShadows[i].GetComponent<SpriteRenderer>();
            Color shadowColor = shadowRenderer.color;
            shadowColor.a = shadowAlpha;  // Set the computed alpha
            shadowRenderer.color = shadowColor;  // Apply the color
        }

        // Deactivate
        for (int i = numParticlesAlive; i < activeShadows.Count; i++)
        {
            activeShadows[i].SetActive(false);
        }
    }
}