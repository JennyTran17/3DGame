using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class AnomalyManager : MonoBehaviour
{
    public static AnomalyManager Instance;

    [Header("Anomalies")]
    public List<GameObject> anomalyPrefabs = new List<GameObject>();
    public List<Transform> anomalySpawns = new List<Transform>();

    [Header("Settings")]
    public int totalHallways = 10;
    public int numberOfAnomalies = 7; // How many anomalies you want in total

    private GameObject currentAnomaly;
    private List<int> anomalyHallways = new List<int>();
    private int currentIndex = 0;
    private int lastUsedSpawnIndex = -1; // -1 means no spawn yet


    private void Awake()
    {
        Instance = this;
        
    }
    private void Start()
    {
        GenerateRandomAnomalyHallways();
    }

    void GenerateRandomAnomalyHallways()
    {
        List<int> allPossibleHallways = new List<int>();

        // Populate from 1 to totalHallways
        for (int i = 2; i < totalHallways; i++)
        {
            allPossibleHallways.Add(i);
        }

        // Shuffle
        for (int i = 0; i < allPossibleHallways.Count; i++)
        {
            int randomIndex = Random.Range(i, allPossibleHallways.Count);
            Debug.Log(randomIndex);
            int temp = allPossibleHallways[i];
            allPossibleHallways[i] = allPossibleHallways[randomIndex];
            allPossibleHallways[randomIndex] = temp;
        }

        // Take the first N and sort them
        anomalyHallways = allPossibleHallways.Distinct().ToList().GetRange(0, numberOfAnomalies);
        anomalyHallways.Sort(); // Ensure numerical order
        Debug.Log("Anomalies will spawn at hallways: " + string.Join(", ", anomalyHallways));

    }




    public void OnLoopProgress(int hallway)
    {
        Clear();

        if (currentIndex < anomalyHallways.Count && hallway == anomalyHallways[currentIndex])
        {
            if (anomalyPrefabs.Count > 0)
            {
                GameObject randomAnomaly = anomalyPrefabs[Random.Range(0, anomalyPrefabs.Count)];
                if (anomalySpawns.Count >= 2)
                {
                    // Alternate spawn index: if last was A(0), use B(1), and vice versa
                    bool reverseMode = LoopManager.Instance.isReverseMode;

                    Transform spawnPoint = reverseMode ? anomalySpawns[1] : anomalySpawns[0];
                    currentAnomaly = Instantiate(randomAnomaly, spawnPoint.position, spawnPoint.rotation);
                    Debug.Log($"Anomaly '{randomAnomaly.name}' spawned at hallway {hallway} | ReverseMode: {reverseMode}");


                    lastUsedSpawnIndex = reverseMode ? 1 : 0;
                }
                else
                {
                    Debug.LogWarning("You need at least 2 spawn points in 'anomalySpawns' for alternating positions.");
                }

            }
            else
            {
                Debug.LogWarning("No anomaly prefabs assigned to AnomalyManager.");
            }

            currentIndex++;
        }

        // Example: spawn elevator at hallway 10
        if (hallway == 10)
        {
            Debug.Log("Elevator appears!");
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
        currentIndex = 0;
        GenerateRandomAnomalyHallways();
    }

    public bool HasActiveAnomaly()
    {
        return currentAnomaly != null;
    }

    public bool IsPlayerCloseToAnomaly(Transform player, float distanceThreshold)
    {
        if (currentAnomaly == null)
            return false;

        return Vector3.Distance(player.position, currentAnomaly.transform.position) < distanceThreshold;
    }
}
