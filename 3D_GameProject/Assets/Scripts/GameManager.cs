using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Scene References")]
    public GameObject elevator;         // Elevator object in scene
    public GameObject triggerBackwards;
    public GameObject flashlight;       // Flashlight object in scene
    public List<GameObject> hintObjects; // Preplaced hints in scene

    [Header("Hint Control")]
    [Tooltip("Hallway indexes where hints can appear")]
    public List<int> hintHallways = new List<int> { 2, 4, 6, 8 }; // configurable

    [Range(0f, 1f)]
    public float hintAppearChance = 0.5f; // 50% chance for hint to appear when eligible

    [Header("Progress Tracking")]
    public int levelCount = 1;
    private bool flashlightActivated = false;
    private bool elevatorActive = true;
    private HashSet<int> usedHintHallways = new HashSet<int>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        // Make sure all hints start inactive
        foreach (var hint in hintObjects)
        {
            if (hint != null)
                hint.SetActive(false);
        }

        if(triggerBackwards != null)
        {
            triggerBackwards.SetActive(false);
        }

        if (flashlight != null)
            flashlight.SetActive(false);

        StartCoroutine(TrackHallwayProgress());
    }

    private IEnumerator TrackHallwayProgress()
    {
        int lastHallway = LoopManager.Instance.hallwayCount;

        while (true)
        {
            int currentHallway = LoopManager.Instance.hallwayCount;

            if (currentHallway != lastHallway)
            {
                OnHallwayChanged(currentHallway);
                lastHallway = currentHallway;
            }

            yield return new WaitForSeconds(0.2f);
        }
    }

    private void OnHallwayChanged(int hallway)
    {
        Debug.Log($"[GameManager] Hallway changed: {hallway}");

        // Flashlight appears after first loop
        if (hallway >= 1 && !flashlightActivated)
        {
            ActivateFlashlight();
        }

        // Elevator disappears after first loop
        if (hallway >= 1 && elevatorActive)
        {
            DeactivateElevator();
        }

        // Random hint activation at configured hallways
        if (hintHallways.Contains(hallway))
        {
            TryActivateHint(hallway);
        }

        // Elevator reappears at hallway 10
        if (hallway == 10)
        {
            ActivateElevator();
            levelCount++;
            Debug.Log($"Reached hallway 10. Floor {levelCount} started.");
        }
    }

    private void ActivateFlashlight()
    {
        if (flashlight != null)
        {
            flashlight.SetActive(true);
            flashlightActivated = true;
            Debug.Log("[GameManager] Flashlight activated after first loop.");
        }
    }

    private void DeactivateElevator()
    {
        if (elevator != null)
        {
            elevator.SetActive(false);
            elevatorActive = false;
            Debug.Log("[GameManager] Elevator deactivated after first loop.");
        }

        triggerBackwards.SetActive(true);
    }

    private void ActivateElevator()
    {
        if (elevator != null)
        {
            elevator.SetActive(true);
            elevatorActive = true;
            flashlightActivated = false; // reset for next level if needed
            usedHintHallways.Clear();
            triggerBackwards.SetActive(false);

            // Deactivate all hints for next floor
            foreach (var hint in hintObjects)
            {
                if (hint != null)
                    hint.SetActive(false);
            }

            Debug.Log("[GameManager] Elevator reactivated at hallway 10.");
        }
    }

    private void TryActivateHint(int hallway)
    {
        // Prevent multiple activations in the same hallway
        if (usedHintHallways.Contains(hallway))
            return;

        if (Random.value <= hintAppearChance && hintObjects.Count > 0)
        {
            GameObject hint = hintObjects[Random.Range(0, hintObjects.Count)];
            if (hint != null && !hint.activeSelf)
            {
                hint.SetActive(true);
                usedHintHallways.Add(hallway);
                Debug.Log($"[GameManager] Hint activated at hallway {hallway}.");
            }
        }
    }
}
