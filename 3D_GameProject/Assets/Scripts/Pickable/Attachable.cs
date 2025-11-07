using UnityEngine;

public class Attachable : Interactable
{
    private bool pickedUp = false;

    public override void Interact()
    {
        Debug.Log("Attachable interacted with");
    }

    void OnTriggerEnter(Collider other)
    {
        if (pickedUp) return;

        if (other.CompareTag("Player"))
        {
            pickedUp = true;
            // Call narrator
            NarratorSystem.Instance.TriggerEvent(NarratorState.Flashlight);
        }
    }

}

