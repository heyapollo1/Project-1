using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RarityVisual : MonoBehaviour
{
    [SerializeField] private SpriteRenderer targetRenderer;
    [SerializeField] private Material baseMaterial;

    private Material instanceMat;

    private static readonly int OutlineColorAID = Shader.PropertyToID("_OutlineColorA");
    private static readonly int OutlineColorBID = Shader.PropertyToID("_OutlineColorB");
    private static readonly int OutlineThicknessID = Shader.PropertyToID("_OutlineThickness");
    private static readonly int ShimmerSpeedID = Shader.PropertyToID("_ShimmerSpeed");
    private static readonly int ShimmerIntensityID = Shader.PropertyToID("_ShimmerIntensity");
    private static readonly int ColorShiftSpeedID = Shader.PropertyToID("_ColorShiftSpeed");
    private static readonly int WavyDistortionID = Shader.PropertyToID("_WavyDistortion");

    private void Awake()
    {
        instanceMat = new Material(baseMaterial); // Avoid shared material issues
        if (targetRenderer != null)
            targetRenderer.material = instanceMat;
    }

    public void ApplyRarityMaterial(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common:
                SetOutline(new Color(0.85f, 0.82f, 0.82f), new Color(0.70f, 0.68f, 0.61f), 0.1f, 0.5f, 0.5f);
                break;
            case Rarity.Rare:
                SetOutline(new Color(0.55f, 0.82f, 0.82f), new Color(0.40f, 0.65f, 0.85f), 0.1f, 0.5f, 0.5f);
                break;
            case Rarity.Epic:
                SetOutline(new Color(0.80f, 0.35f, 0.90f), new Color(0.85f, 0.50f, 0.90f), 0.13f, 1.0f, 0.75f);
                break;
            case Rarity.Legendary:
                SetOutline(new Color(0.90f, 0.40f, 0.30f), new Color(1.0f, 0.45f, 0.20f), 0.20f, 1.75f, 1.0f);
                break;
        }

        instanceMat.SetFloat(OutlineThicknessID, 2f);
        instanceMat.SetFloat(ColorShiftSpeedID, 1f);
    }

    private void SetOutline(Color a, Color b, float waveDistortion, float shimmerIntensity, float shimmerSpeed)
    {
        instanceMat.SetColor(OutlineColorAID, a);
        instanceMat.SetColor(OutlineColorBID, b);
        instanceMat.SetFloat(WavyDistortionID, waveDistortion);
        instanceMat.SetFloat(ShimmerIntensityID, shimmerIntensity);
        instanceMat.SetFloat(ShimmerSpeedID, shimmerSpeed);
    }
}
