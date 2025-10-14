using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorOpenClose : MonoBehaviour
{
    //public bool open;
    [SerializeField] Animator doorLeft;
    [SerializeField] Animator doorRight;
    private void Update()
    {
        //if (open)
        //{
            
        //}
        //else
        //{
           
        //}
    }
    public void OpenDoor()
    {
        //open = true;
        doorLeft.SetTrigger("open");
        doorRight.SetTrigger("open");
        StartCoroutine(autoClose(4));
    }

    IEnumerator autoClose(int time)
    {
        yield return new WaitForSeconds(time);
       // open = false;
        doorLeft.SetTrigger("close");
        doorRight.SetTrigger("close");
    }
}
