using UnityEngine;

public class LineOfSight3D : MonoBehaviour
{
    [Header("References")]
    public Transform target;

    [Header("Settings")]
    public float speed = 3f;
    public float detectionRange = 15f;
    public float rotationSpeed = 5f;

    void Update()
    {

        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
        

        Vector3 enemyPos = transform.position;
        Vector3 targetPos = target.position;
        Vector3 direction = targetPos - enemyPos;
        direction.y = 0f;

        float distance = direction.magnitude;

        // Debug visualization
        Debug.DrawRay(enemyPos, direction, distance < detectionRange ? Color.red : Color.white);

        // Normalize direction for consistent movement
        Vector3 normalizedDir = direction.normalized;
        Debug.DrawRay(enemyPos, normalizedDir, Color.blue);

        if (distance < detectionRange)
        {
            // Rotate smoothly on Y axis only
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                Quaternion flatRotation = Quaternion.Euler(0f, targetRotation.eulerAngles.y, 0f);
                transform.rotation = Quaternion.Slerp(transform.rotation, flatRotation, rotationSpeed * Time.deltaTime);
            }

            // Move forward on the local Z axis
            transform.position += transform.forward * speed * Time.deltaTime;
        }
    }
}
