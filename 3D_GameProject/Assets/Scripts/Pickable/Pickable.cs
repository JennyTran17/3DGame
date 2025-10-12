using UnityEngine;

public class Pickable : Interactable
{
    public override void Interact()
    {
        // Let PlayerInteract handle pickup
        Debug.Log("Pickable interacted with");
    }
}
