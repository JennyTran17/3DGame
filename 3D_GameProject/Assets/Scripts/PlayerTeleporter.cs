using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTeleporter : MonoBehaviour
{
    public Transform TeleportZoneObject;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Vector3 localOffset = transform.InverseTransformPoint(other.transform.position);
            Quaternion relativeRotation = TeleportZoneObject.transform.rotation * Quaternion.Inverse(transform.rotation);

            CharacterController cc = other.GetComponent<CharacterController>();

            if (cc != null)
            {
                cc.enabled = false;
                other.transform.position = TeleportZoneObject.TransformPoint(localOffset);
                other.transform.rotation = relativeRotation * other.transform.rotation;

                cc.enabled = true;
            }
        }
    }
}
