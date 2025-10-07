using System;
using UnityEngine;
using TMPro;

public class LoopManager : MonoBehaviour
{
    public static LoopManager Instance;

    public int hallwayCount = 0;
    public float anomalyResetDistance = 7f;
    private bool isReverseMode = false;
    public TMPro.TextMeshPro hallwayText;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (hallwayText != null)
        {
            UpdateText();
        }
    }

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
            else if (isForwardTrigger && anomalyActive)
            {
                // Wrong way, reset
                ResetLevel();
            }
            else if(isForwardTrigger && !anomalyActive)
            {
                DecrementHallway();
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
        isReverseMode = false;
        // TODO: teleport player back into elevator
    }

    public void DecrementHallway()
    {
        hallwayCount = Mathf.Max(0, hallwayCount - 1);
        Debug.Log("Went backward. Hallway: " + hallwayCount);

        // Trigger anomalies again based on new position
        AnomalyManager.Instance.OnLoopProgress(hallwayCount);
    }


    void UpdateText()
    {
        hallwayText.text = "Hall " + hallwayCount;
    }
}
