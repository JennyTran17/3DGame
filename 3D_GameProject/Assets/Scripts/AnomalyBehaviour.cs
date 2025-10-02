using UnityEngine;

public class AnomalyBehavior : MonoBehaviour
{
    private bool playerInZone = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInZone = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInZone = false;
    }

    private void Update()
    {
        //if (playerInZone)
        //{
        //    Vector3 toAnomaly = (transform.position - Camera.main.transform.position).normalized;
        //    Vector3 forward = Camera.main.transform.forward;

        //    // Check if player walking *towards* anomaly
        //    float dot = Vector3.Dot(forward, toAnomaly);

        //    if (dot > 0.5f) // facing anomaly
        //    {
        //        Debug.Log("Player walked toward anomaly -> Reset!");
        //        LoopManager.Instance.ResetLevel();
        //    }
        //    else // facing away from anomaly
        //    {
        //        Debug.Log("Player avoided anomaly -> Progress!");
        //        LoopManager.Instance.IncrementHallway();
        //    }
        //}
    }
}
