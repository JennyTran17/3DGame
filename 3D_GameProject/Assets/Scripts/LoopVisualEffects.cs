using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class LoopVisualEffects : MonoBehaviour
{
    public Volume globalVolume;

    private Vignette vignette;
    private ChromaticAberration chromaticAberration;

    public float baseWeight = 0.3f;
    public float maxWeight = 1.0f;

    public float maxVignetteIntensity = 0.6f;
    public float maxChromaticAberration = 1.0f;

    void Start()
    {
        if (globalVolume == null)
        {
            Debug.Log("Global Volume not assigned!");
            return;
        }

        // Get effect components from the volume profile
        globalVolume.profile.TryGet(out vignette);
        globalVolume.profile.TryGet(out chromaticAberration);
    }

    void Update()
    {
        int loopCount = LoopManager.Instance.hallwayCount;

        // Normalize the loop count into a 0–1 range (adjust max loop threshold to taste)
        float t = Mathf.Clamp01(loopCount / 20f);

        // Update weight (overall intensity of the volume)
        globalVolume.weight = Mathf.Lerp(baseWeight, maxWeight, t);

        // Update Vignette intensity
        if (vignette != null)
        {
            vignette.intensity.value = Mathf.Lerp(0f, maxVignetteIntensity, t);
        }

        // Update Chromatic Aberration
        if (chromaticAberration != null)
        {
            chromaticAberration.intensity.value = Mathf.Lerp(0f, maxChromaticAberration, t);
        }
    }
}
