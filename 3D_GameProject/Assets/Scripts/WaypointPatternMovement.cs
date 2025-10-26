using System.Collections;
using UnityEngine;

[System.Serializable]
public class WayPointCD
{
    public GameObject waypoint;
    public float speed = 2f;
    public float waitTime = 1f;
}

public class WaypointPatternMovement : MonoBehaviour
{
    public WayPointCD[] wpPattern;
    private int patternIndex = 0;
    private bool isWaiting = false;

    [Header("Rotation Settings")]
    public float rotationSpeed = 5f;

    void Update()
    {
        if (!isWaiting)
            MoveAlongWaypoints();
    }

    void MoveAlongWaypoints()
    {
        if (wpPattern.Length == 0) return;

        WayPointCD wayPointCD = wpPattern[patternIndex];
        Vector3 targetPos = wayPointCD.waypoint.transform.position;

        Vector3 directionToWaypoint = targetPos - transform.position;
        directionToWaypoint.y = 0f;

        float distance = directionToWaypoint.magnitude;
        float step = wayPointCD.speed * Time.deltaTime;

        if (distance <= 0.1f)
        {
            StartCoroutine(WaitAtWaypoint(wayPointCD.waitTime));
        }
        else
        {
            transform.position += directionToWaypoint.normalized * step;

            if (directionToWaypoint != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToWaypoint);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }

    private IEnumerator WaitAtWaypoint(float duration)
    {
        isWaiting = true;
        yield return new WaitForSeconds(duration);

        patternIndex++;
        if (patternIndex >= wpPattern.Length)
        {
            // Reached last waypoint, reset back to start and deactivate
            ResetMovement();
            gameObject.SetActive(false);
        }

        isWaiting = false;
    }

    public void ResetMovement()
    {
        StopAllCoroutines();
        patternIndex = 0;
        isWaiting = false;

        if (wpPattern.Length > 0 && wpPattern[0].waypoint != null)
        {
            transform.position = wpPattern[0].waypoint.transform.position;
        }
    }
}
