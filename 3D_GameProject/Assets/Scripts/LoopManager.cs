using System;
using UnityEngine;

public class LoopManager : MonoBehaviour
{
    public static LoopManager Instance;

    public int hallwayCount = 0;
    public float anomalyResetDistance = 4f;
    private bool isReverseMode = false;

    private void Awake()
    {
        Instance = this;
    }
    //public void HandleTriggerEntry(bool isForward)
    //{
    //    // Get player movement direction since the last frame
    //    Transform player = GameObject.FindGameObjectWithTag("Player").transform;
    //    Vector3 playerMovement = player.GetComponent<CharacterController>().velocity;

    //    // This checks if the player is moving in the expected direction
    //    // The dot product is crucial here. Let's assume your hallway's forward is Vector3.forward.
    //    Vector3 hallwayDirection = Vector3.forward; // You can set this manually or from your level design
    //    float dotProduct = Vector3.Dot(playerMovement.normalized, hallwayDirection.normalized);
    //    bool isMovingHallwayForward = dotProduct > 0.1f;

    //    // Correctly handle the hallway progression based on direction
    //    if (isForward && isMovingHallwayForward)
    //    {
    //        IncrementHallway();
    //    }
    //    else if (!isForward && !isMovingHallwayForward)
    //    {
    //        DecrementHallway();
    //    }
    //    else
    //    {
    //        // Player entered the wrong trigger for their current direction, do nothing.
    //        Debug.Log("Player tried to cheat the loop. No change.");
    //    }
    //}

    public void HandleTriggerEntry(bool isForwardTrigger)
    {
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;
        if (player == null) return;

        Vector3 playerMovement = Vector3.zero;
        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null)
        {
            playerMovement = cc.velocity;
        }

        Vector3 hallwayDirection = Vector3.forward;
        float dotProduct = Vector3.Dot(playerMovement.normalized, hallwayDirection.normalized);
        bool isMovingHallwayForward = dotProduct > 0.1f;

        bool anomalyActive = AnomalyManager.Instance.HasActiveAnomaly();
        bool isCloseToAnomaly = AnomalyManager.Instance.IsPlayerCloseToAnomaly(player, anomalyResetDistance);

        // State change logic
        if (anomalyActive && !isReverseMode)
        {
            // Player just encountered the first anomaly, switch to reverse mode.
            isReverseMode = true;
        }
        else if (!anomalyActive && isReverseMode)
        {
            // Anomaly is gone, but we're still in reverse mode
            isReverseMode = true;
        }
        else if(anomalyActive && isReverseMode)
        {
            isReverseMode = false;
        }

        // Progression Logic based on the current mode
        if (isReverseMode)
        {
            // In reverse mode, backward is "correct"
            if (!isForwardTrigger && !isMovingHallwayForward)
            {
                IncrementHallway();
            }
            else if (isForwardTrigger && isMovingHallwayForward && isCloseToAnomaly)
            {
                // Wrong way, reset
                ResetLevel();
            }
        }
        else // Normal forward mode
        {
            if (isForwardTrigger && isMovingHallwayForward)
            {
                IncrementHallway();
            }
            else if (!isForwardTrigger && !isMovingHallwayForward)
            {
                DecrementHallway();
            }
        }
    }

    public void IncrementHallway()
    {
        hallwayCount++;
        Debug.Log("Hallway: " + hallwayCount);

        // Trigger anomaly progression
        AnomalyManager.Instance.OnLoopProgress(hallwayCount);

    }

    public void ResetLevel()
    {
        hallwayCount = 0;
        Debug.Log("Game Reset - back in elevator");

        AnomalyManager.Instance.ResetAnomalies();
        // TODO: teleport player back into elevator
    }

    public void DecrementHallway()
    {
        hallwayCount = Mathf.Max(0, hallwayCount - 1);
        Debug.Log("Went backward. Hallway: " + hallwayCount);

        // Trigger anomalies again based on new position
        AnomalyManager.Instance.OnLoopProgress(hallwayCount);
    }

}
