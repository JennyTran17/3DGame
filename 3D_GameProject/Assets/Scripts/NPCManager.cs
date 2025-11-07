using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    [Header("Man NPCs (Inactive in Scene)")]
    public GameObject manA; // Walks 4 - 1 (Normal mode)
    public GameObject manB; // Walks 1 - 4 (Reverse mode)

    [Header("Spawn Settings")]
    [Tooltip("How many unique hallways each NPC can appear in per loop.")]
    public int totalRandomHallwaysPerMan = 3;

    public int minHallway = 2;
    public int maxHallway = 8;
    public Vector2 spawnDelayRange = new Vector2(3f, 6f);

    private List<int> spawnHallways = new List<int>();
    private bool lastSpawnWasA = false;
    private bool isSpawning = false;
    private int lastKnownHallway = -1;
    bool firstTime;

    private void Start()
    {
        if (manA != null) manA.SetActive(false);
        if (manB != null) manB.SetActive(false);

        GenerateSpawnHallways();
        StartCoroutine(MonitorHallwayProgress());
    }

    private void GenerateSpawnHallways()
    {
        spawnHallways = GenerateUniqueRandoms(totalRandomHallwaysPerMan, minHallway, maxHallway);
        Debug.Log($"NPCManager: Active Hallways -> {string.Join(", ", spawnHallways)}");
    }

    private List<int> GenerateUniqueRandoms(int count, int min, int max)
    {
        HashSet<int> unique = new HashSet<int>();
        while (unique.Count < count)
            unique.Add(Random.Range(min, max + 1));
        return new List<int>(unique);
    }

    private IEnumerator MonitorHallwayProgress()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.25f);

            if (LoopManager.Instance == null)
                continue;

            int currentHallway = LoopManager.Instance.hallwayCount;
            if (currentHallway != lastKnownHallway)
            {
                lastKnownHallway = currentHallway;
                TryActivateMan(currentHallway);
            }
        }
    }

    private void TryActivateMan(int hallway)
    {
        if (LoopManager.Instance == null || isSpawning)
            return;

        bool isReverse = LoopManager.Instance.isReverseMode;

        // Reset when returning to elevator
        if (hallway == 0)
        {
            ResetAllNPCs();
            return;
        }

        // Only trigger when hallway matches the predefined random list
        if (!spawnHallways.Contains(hallway))
            return;

        // Alternate spawn points
        bool spawnAtA = !lastSpawnWasA;
        lastSpawnWasA = spawnAtA;

        GameObject targetMan = isReverse ? manB : manA;
        if (targetMan == null) return;

        StartCoroutine(SpawnManAfterDelay(targetMan));
    }

    private IEnumerator SpawnManAfterDelay(GameObject man)
    {
        isSpawning = true;
        yield return new WaitForSeconds(Random.Range(spawnDelayRange.x, spawnDelayRange.y));

        if (man == null)
        {
            isSpawning = false;
            yield break;
        }

        // Reset and prepare movement
        WaypointPatternMovement movement = man.GetComponent<WaypointPatternMovement>();
        if (movement != null)
        {
            movement.ResetMovement();
        }

        man.SetActive(true);
        if (!firstTime)
        {
            NarratorSystem.Instance.TriggerEvent(NarratorState.Businessman);
            firstTime = true;

        }
        StartCoroutine(WatchAndDeactivate(man, movement));
    }

    private IEnumerator WatchAndDeactivate(GameObject man, WaypointPatternMovement movement)
    {
        while (man.activeSelf)
        {
            if (movement == null || movement.wpPattern.Length == 0)
                break;

            WayPointCD lastWP = movement.wpPattern[movement.wpPattern.Length - 1];
            float distance = Vector3.Distance(man.transform.position, lastWP.waypoint.transform.position);

            if (distance < 0.5f)
            {
                movement.ResetMovement();
                man.SetActive(false);
                break;
            }

            yield return new WaitForSeconds(0.25f);
        }

        isSpawning = false;
    }

    private void ResetAllNPCs()
    {
        ResetMan(manA);
        ResetMan(manB);
        GenerateSpawnHallways();
        isSpawning = false;
    }

    private void ResetMan(GameObject man)
    {
        if (man == null) return;
        WaypointPatternMovement move = man.GetComponent<WaypointPatternMovement>();
        if (move != null)
        {
            move.ResetMovement();
        }
        man.SetActive(false);
    }
}
