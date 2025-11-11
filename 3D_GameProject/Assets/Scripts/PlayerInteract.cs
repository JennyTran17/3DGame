using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [Header("Settings")]
    public float playerReach = 10f;
    public Transform pickUpParent;  // Where held items go (e.g. under camera)
    [SerializeField] GameObject flashlight;

    [Header("Runtime")]
    private Interactable currentInteractable;
    private GameObject inHandItem;
    private Rigidbody inHandRb;

    void Update()
    {
        CheckInteraction();

        if (Input.GetMouseButtonDown(0) && currentInteractable != null)
        {
            HandleInteraction(currentInteractable);
        }

        // Drop item
        if (Input.GetKeyDown(KeyCode.Q) && inHandItem != null)
        {
            DropItem();
        }

        if (Input.GetKeyDown(KeyCode.F) && GameManager.Instance.flashlightActivated)

        {

            if (flashlight.activeInHierarchy)
            {
                flashlight.SetActive(false);

            }
            else
            {
                flashlight.SetActive(true);

            }
        }
    }

    void CheckInteraction()
    {
        int layerMask = ~LayerMask.GetMask("Player");
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;

        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * playerReach, Color.red);

        if (Physics.Raycast(ray, out hit, playerReach, layerMask))
        {
            Interactable newInteractable = hit.collider.GetComponent<Interactable>();

            if (newInteractable != null)
            {
                if (currentInteractable != newInteractable)
                {
                    currentInteractable = newInteractable;
                    HUDController.instance.EnableInteractionText(currentInteractable.message);
                    
                }
                return;
            }
        }

        // Nothing interactable in reach
        DisableCurrentInteractable();
    }

    void HandleInteraction(Interactable interactable)
    {
        if (interactable.gameObject.GetComponent<Attachable>())
        {
            Attachable attachable = interactable.gameObject.GetComponent<Attachable>();
            
            AttachItem(attachable);
        }
        else if (interactable.gameObject.GetComponent<Pickable>())
        {
            Pickable pickable = interactable.GetComponent<Pickable>();
            PickUpItem(pickable);
        }
        else
        {
            interactable.Interact(); // Normal UnityEvent-based interactions (doors, buttons, etc.)
        }
    }

    void AttachItem(Attachable attachable)
    {
        if (inHandItem != null) return; // already holding something

        inHandItem = attachable.gameObject;
        inHandItem.transform.SetParent(pickUpParent, false);
        inHandItem.transform.localPosition = Vector3.zero;
        inHandItem.transform.localRotation = Quaternion.identity;

        inHandRb = inHandItem.GetComponent<Rigidbody>();
        if (inHandRb != null)
        {
            inHandRb.isKinematic = true;
        }

        Debug.Log($"Attached item: {inHandItem.name}");
    }

    void PickUpItem(Pickable pickable)
    {
        if (inHandItem != null) return; // already holding something

        inHandItem = pickable.gameObject;
        inHandItem.transform.SetParent(pickUpParent, false);
        inHandItem.transform.localPosition = Vector3.zero;
        inHandItem.transform.localRotation = Quaternion.identity;

        inHandRb = inHandItem.GetComponent<Rigidbody>();
        if (inHandRb != null)
        {
            inHandRb.isKinematic = true;
        }

        Debug.Log($"Picked up item: {inHandItem.name}");
    }

    void DropItem()
    {
        if (inHandItem == null) return;

        Debug.Log($"Dropped item: {inHandItem.name}");

        inHandItem.transform.SetParent(null);

        if (inHandRb != null)
        {
            inHandRb.isKinematic = false;
            inHandRb.AddForce(Camera.main.transform.forward * 2f, ForceMode.Impulse);
        }

        inHandItem = null;
        inHandRb = null;
    }

    void DisableCurrentInteractable()
    {
        if (currentInteractable != null)
        {
            HUDController.instance.DisableInteractionText();
            currentInteractable = null;
        }
    }
}
