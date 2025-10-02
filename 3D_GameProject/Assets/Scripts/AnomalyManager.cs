using UnityEngine;

public class AnomalyManager : MonoBehaviour
{
    public static AnomalyManager Instance;

    [Header("Anomalies")]
    public GameObject anomalyPrefab;
    public Transform anomalySpawn;

    private GameObject currentAnomaly;

    private void Awake()
    {
        Instance = this;
    }

    public void OnLoopProgress(int hallway)
    {
        Clear();

        // Example rule: spawn anomaly every 3 hallways
        if (hallway == 3 || hallway == 5 || hallway == 8)
        {
            currentAnomaly = Instantiate(anomalyPrefab, anomalySpawn.position, anomalySpawn.rotation);
        }

        // Example: spawn elevator at hallway 10
        if (hallway == 10)
        {
            Debug.Log("Elevator appears!");
            // Instantiate elevator prefab
        }
    }

    public void Clear()
    {
        if (currentAnomaly != null)
        {
            Destroy(currentAnomaly);
            currentAnomaly = null;
        }
    }

    public void ResetAnomalies()
    {
        Clear();
    }

    public bool HasActiveAnomaly()
    {
        return currentAnomaly != null;
    }

    public bool IsPlayerCloseToAnomaly(Transform player, float distanceThreshold)
    {
        if (currentAnomaly == null)
        {
            return false;
        }
        return Vector3.Distance(player.position, currentAnomaly.transform.position) < distanceThreshold;
    }
}
