using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetOnOff : MonoBehaviour
{
    [SerializeField] GameObject gObject;
    GameObject player;

    private void Start()
    {
        gObject.SetActive(false);
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleOff();
        }
    }
    public void ToggleOn()
    {
        if (gObject != null)
        { 
            gObject.SetActive(true);
            player.GetComponent<FirstPersonController>().enabled = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void ToggleOff()
    {
        if (gObject != null)
        {
            gObject.SetActive(false);
            player.GetComponent<FirstPersonController>().enabled = true;
            Cursor.visible = false;
        }
    }
}
