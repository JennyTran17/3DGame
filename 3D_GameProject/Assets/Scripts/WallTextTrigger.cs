using UnityEngine;

public class WallTextTrigger : MonoBehaviour
{
    private bool triggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;
            NarratorSystem.Instance.TriggerEvent(NarratorState.WallText);
        }
    }
}
