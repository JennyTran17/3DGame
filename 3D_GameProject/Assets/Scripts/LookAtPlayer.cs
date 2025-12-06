using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    public Transform eyeDest;

    private void Start()
    {
        eyeDest = GameObject.FindGameObjectWithTag("MainCamera").transform;
    }
    private void Update()
    {
        transform.LookAt(eyeDest);
    }
}
