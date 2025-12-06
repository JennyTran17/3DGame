using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DoorScript
{
	[RequireComponent(typeof(AudioSource))]


public class Door : MonoBehaviour {
		public bool open;
		public float smooth = 1.0f;
		float DoorOpenAngle = -90.0f;
		float DoorCloseAngle = 0.0f;
		public AudioSource asource;
		public AudioClip openDoor,closeDoor;

        [Header("Detection Settings")]
        public float detectionDistance = 3f;
 

        // Use this for initialization
        void Start () {
		asource = GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (open)
		{
            var target = Quaternion.Euler (0, DoorOpenAngle, 0);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, target, Time.deltaTime * 5 * smooth);
	
		}
		else
		{
            var target1= Quaternion.Euler (0, DoorCloseAngle, 0);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, target1, Time.deltaTime * 5 * smooth);
	
		}  
		DetectCharacter();
	}

	public void OpenDoor(){
		open = true;
		asource.clip = open?openDoor:closeDoor;
		asource.Play ();

		StartCoroutine(autoClose(4));
	}

	IEnumerator autoClose(int time)
	{
			yield return new WaitForSeconds(time);
			open = false;
	}

        //Add detection raycast forward and backward. if ray hit an object with tag "Man", open door automatically
        private void DetectCharacter()
        {
            // Raycast forward and backward
            Ray forwardRay = new Ray(transform.position, transform.forward);
            Ray backwardRay = new Ray(transform.position, -transform.forward);

            RaycastHit hitF;
            RaycastHit hitB;

            bool hitForward = Physics.Raycast(forwardRay, out hitF, detectionDistance);
            bool hitBackward = Physics.Raycast(backwardRay, out hitB, detectionDistance);

            if (hitForward || hitBackward)
            {
                if ((hitForward && hitF.collider.CompareTag("Man")) ||
                    (hitBackward && hitB.collider.CompareTag("Man")))
                {
                    if (!open)
                        OpenDoor();
                }
            }
        }

    }
}