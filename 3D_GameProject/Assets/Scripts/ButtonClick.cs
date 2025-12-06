using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ButtonClick : MonoBehaviour
{
    [SerializeField] public int number;

    public void buttonPressed()
    {
        if (number != 0)
        {
            LevelManager.instance.AddInput(number);
            LevelManager.instance.chosenLevel = number;
        }
    }
}
