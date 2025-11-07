using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Scene References")]
    public GameObject elevatorA;
    public GameObject elevatorB;         
    public GameObject triggerBackwards;
    public GameObject triggerForward;
    public GameObject flashlight;    
    public List<GameObject> hintObjects; // Preplaced hints in scene

    [Header("Hint Control")]
    public List<int> hintHallways = new List<int> { 2, 3, 5, 7, 8 }; 

    [Header("Progress Tracking")]
    public bool flashlightActivated = false;
    private bool elevatorActive = true;
    public Queue<GameObject> hintQueue = new Queue<GameObject>();
    private GameObject previousHint;
    bool firstTime;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        StateManager.Instance.SetState(GameState.Normal);
        if (elevatorA != null) elevatorA.SetActive(true);
        if (elevatorB != null) elevatorB.SetActive(false);

        if (triggerBackwards != null)
        {
            triggerBackwards.SetActive(false);
            triggerForward.SetActive(true);
        }

        if (flashlight != null)
            flashlight.SetActive(false);

        foreach (var hint in hintObjects)
            if (hint != null) hint.SetActive(false);

        ResetHintQueue();

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
            DeactivateElevators();
        }

        // Random hint activation at configured hallways
        if (hintHallways.Contains(hallway))
        {
            TryActivateHint(hallway);
        }

        // Elevator reappears at hallway 10
        if (hallway == 10)
        {
            ActivateElevatorBasedOnDirection();
            NarratorSystem.Instance.TriggerEvent(NarratorState.Ending);

            Debug.Log($"Reached hallway 10.");
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

    private void DeactivateElevators()
    {
        if (elevatorA != null) elevatorA.SetActive(false);
        if (elevatorB != null) elevatorB.SetActive(false);
        elevatorActive = false;
        Debug.Log("[GameManager] Both elevators deactivated after first loop.");

        StateManager.Instance.SetState(GameState.ElevatorGone);
        triggerBackwards.SetActive(true);
        triggerForward.SetActive(true);
    }

    private void ActivateElevatorBasedOnDirection()
    {
        // Determine which elevator to activate based on player's direction
        bool isReversed = LoopManager.Instance.isReverseMode;

        if (isReversed)
        {
            if (elevatorA != null) elevatorA.SetActive(true);
            if (elevatorB != null) elevatorB.SetActive(false);
            Debug.Log("[GameManager] Player reversed, activated Elevator A.");
            if (triggerBackwards != null)
                triggerBackwards.SetActive(false);
        }
        else
        {
            if (elevatorB != null) elevatorB.SetActive(true);
            if (elevatorA != null) elevatorA.SetActive(false);
            Debug.Log("[GameManager] Player forward, activated Elevator B.");
            if (triggerBackwards != null)
                triggerForward.SetActive(false);

        }

        StateManager.Instance.SetState(GameState.FinalHallway);
        elevatorActive = true;
        //flashlightActivated = false;
        //flashlight.SetActive(false);

        

        // Reset hints for new floor
        foreach (var hint in hintObjects)
        {
            if (hint != null)
                hint.SetActive(false);
        }

        ResetHintQueue();
    
    }
    private void ResetHintQueue()
    {
        List<GameObject> shuffledHints = new List<GameObject>(hintObjects);

        // Shuffle hints randomly
        for (int i = 0; i < shuffledHints.Count; i++)
        {
            int randIndex = Random.Range(i, shuffledHints.Count);
            var temp = shuffledHints[i];
            shuffledHints[i] = shuffledHints[randIndex];
            shuffledHints[randIndex] = temp;
        }

        hintQueue.Clear();
        foreach (var hint in shuffledHints)
        {
            if (hint != null)
            {
                hint.SetActive(false);
                hintQueue.Enqueue(hint);
            }
        }

        Debug.Log("[GameManager] Hint queue reset and shuffled.");
    }


    private void TryActivateHint(int hallway)
    {
        if (hintQueue.Count == 0)
        {
            ResetHintQueue();
            Debug.Log("[GameManager] All hints used once. Queue reshuffled.");
        }

        GameObject hintToActivate = hintQueue.Dequeue();
        if (hintToActivate != null)
        {
            if (previousHint != null && previousHint.activeSelf) {
                previousHint.SetActive(false); 
            }

            hintToActivate.SetActive(true);
            Debug.Log($"[GameManager] Activated hint '{hintToActivate.name}' at hallway {hallway}.");
            previousHint = hintToActivate;
            if(!firstTime)
            {
                firstTime = true;
            }
        }
    }

}
