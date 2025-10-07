using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class AnomalyManager : MonoBehaviour
{
    public static AnomalyManager Instance;

    [Header("Anomalies")]
    public GameObject anomalyPrefab;
    public Transform anomalySpawn;

    [Header("Settings")]
    public int totalHallways = 10;
    public int numberOfAnomalies = 5; // How many anomalies you want in total

    private GameObject currentAnomaly;
    private List<int> anomalyHallways = new List<int>();
    private int currentIndex = 0;

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
        for (int i = 1; i <= totalHallways; i++)
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
            currentAnomaly = Instantiate(anomalyPrefab, anomalySpawn.position, anomalySpawn.rotation);
            Debug.Log("Anomaly spawned at hallway " + hallway);
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
