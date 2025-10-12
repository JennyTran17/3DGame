using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public string message = "Press LMB to interact";
    public UnityEvent onInteraction;

    public virtual void Interact()
    {
        onInteraction.Invoke();
        Debug.Log("Interacted with " + gameObject.name);
    }

}
