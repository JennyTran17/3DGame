using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class LoopVisualEffects : MonoBehaviour
{
    [Header("References")]
    public Volume globalVolume;

    [Header("General Intensity Control")]
    public float baseWeight = 0.3f;
    public float maxWeight = 1.0f;
    public float smoothSpeed = 0.8f;        // How quickly values interpolate
    public float intensityGrowthRate = 0.001f; // How fast the effects increase over time

    [Header("Vignette Settings")]
    public float maxVignetteIntensity = 0.4f;

    [Header("Chromatic Aberration Settings")]
    public float maxChromaticAberration = 1.0f;

    [Header("Film Grain Settings")]
    public bool enableFilmGrain = true;
    public float maxFilmGrainIntensity = 0.7f;

    private Vignette vignette;
    private ChromaticAberration chromaticAberration;
    private FilmGrain filmGrain;

    private float progressT;         // 0-1 over game progression
    private float currentWeight;
    private float currentVignette;
    private float currentChromatic;
    private float currentGrain;

    private float gameTime;   

    void Start()
    {
        if (globalVolume == null)
        {
            Debug.LogWarning("Global Volume not assigned!");
            return;
        }

        globalVolume.profile.TryGet(out vignette);
        globalVolume.profile.TryGet(out chromaticAberration);
        globalVolume.profile.TryGet(out filmGrain);

        currentWeight = baseWeight;
        currentVignette = 0f;
        currentChromatic = 0f;
        currentGrain = 0f;
        progressT = 0f;
        gameTime = 0f;
    }

    void Update()
    {
        // Gradually increase progression over time
        gameTime += Time.deltaTime * intensityGrowthRate;
        progressT = Mathf.Clamp01(gameTime); // normalized 0-1 range

        // Smooth interpolation of effects
        currentWeight = Mathf.Lerp(currentWeight, Mathf.Lerp(baseWeight, maxWeight, progressT), Time.deltaTime * smoothSpeed);
        currentVignette = Mathf.Lerp(currentVignette, Mathf.Lerp(0f, maxVignetteIntensity, progressT), Time.deltaTime * smoothSpeed);
        currentChromatic = Mathf.Lerp(currentChromatic, Mathf.Lerp(0f, maxChromaticAberration, progressT), Time.deltaTime * smoothSpeed);

        if (enableFilmGrain)
            currentGrain = Mathf.Lerp(currentGrain, Mathf.Lerp(0f, maxFilmGrainIntensity, progressT), Time.deltaTime * smoothSpeed);

        // Apply the values
        if (vignette != null) vignette.intensity.value = currentVignette;
        if (chromaticAberration != null) chromaticAberration.intensity.value = currentChromatic;
        if (filmGrain != null && enableFilmGrain) filmGrain.intensity.value = currentGrain;

        globalVolume.weight = currentWeight;
    }
    public void AdvanceProgress(float amount)
    {
        progressT = Mathf.Clamp01(progressT + amount);
    }
}
