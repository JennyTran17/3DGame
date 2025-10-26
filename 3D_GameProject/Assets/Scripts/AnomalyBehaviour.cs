using UnityEngine;
using System.Collections;

public class AnomalyBehavior : MonoBehaviour
{
    [Header("References")]
    public Animator animator;

    [Header("Anomaly Settings")]
    public string layerName;  // Animator layer name
    public Vector2 anomalyIntervalRange = new Vector2(5f, 12f);
    public float anomalyDuration = 2f;        // How long zombie layer stays visible
    public float blendDuration = 1f;          // Duration of the transition in/out

    private int zombieLayerIndex;
    private float currentWeight = 0f;
    private float nextAnomalyTime = 0f;
    private bool isInAnomaly = false;
    private bool isBlending = false;

    void Start()
    {
        if (animator == null)
            animator = gameObject.GetComponent<Animator>();

        zombieLayerIndex = animator.GetLayerIndex(layerName);
        SetNextAnomalyTime();
    }

    void Update()
    {
        if (isBlending) return;

        if (!isInAnomaly && Time.time >= nextAnomalyTime)
        {
            StartCoroutine(TriggerAnomaly());
        }

        animator.SetLayerWeight(zombieLayerIndex, currentWeight);
    }

    private void SetNextAnomalyTime()
    {
        nextAnomalyTime = Time.time + Random.Range(anomalyIntervalRange.x, anomalyIntervalRange.y);
    }

    private IEnumerator TriggerAnomaly()
    {
        isInAnomaly = true;
        isBlending = true;

        // Blend in zombie layer
        yield return StartCoroutine(BlendWeight(1f));

        // Hold full zombie layer for the duration
        yield return new WaitForSeconds(anomalyDuration);

        // Blend out zombie layer
        yield return StartCoroutine(BlendWeight(0f));

        isInAnomaly = false;
        isBlending = false;
        SetNextAnomalyTime();
    }

    private IEnumerator BlendWeight(float targetWeight)
    {
        float startWeight = currentWeight;
        float elapsed = 0f;

        while (elapsed < blendDuration)
        {
            elapsed += Time.deltaTime;
            currentWeight = Mathf.Lerp(startWeight, targetWeight, elapsed / blendDuration);
            animator.SetLayerWeight(zombieLayerIndex, currentWeight);
            yield return null;
        }

        currentWeight = targetWeight;
        animator.SetLayerWeight(zombieLayerIndex, currentWeight);
    }
}
