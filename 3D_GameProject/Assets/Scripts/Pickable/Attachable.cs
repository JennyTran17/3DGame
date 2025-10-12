using UnityEngine;

public class Attachable : Interactable
{
    public override void Interact()
    {
        // Let PlayerInteract handle the attachment
        Debug.Log("Attachable interacted with");
    }
}
