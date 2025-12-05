using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    public int chosenLevel = 1;
    private int[] levelSequence = new int[]
    {
        6, 1, 3
    };

    public List<int> inputSequence = new List<int>();

    [Header("Level Ending")]
    public SetOnOff keypad;
    private Fade eyeFadeEffect;
    public AudioSource elevatorRumbleSFX;

    private bool correct;

    private void Awake()
    {
        instance = this;
        eyeFadeEffect = FindFirstObjectByType<Fade>();
        keypad = FindFirstObjectByType<SetOnOff>();
    }

    private void Update()
    {
        CheckSequence();
    }

    public void AddInput(int num)
    {
        inputSequence.Add(num);
    }

    
     void CheckSequence()
    {
        // Index to track progress through the correct sequence
        int correctSequenceIndex = 0;

        // Loop through the player's sequence
        for (int i = 0; i < inputSequence.Count; i++)
        {
            // If the current player input matches the expected input in the correct sequence
            if (inputSequence[i] == levelSequence[correctSequenceIndex])
            {
                // Move to the next expected input in the correct sequence
                correctSequenceIndex++; 

                // If the entire correct sequence has been matched
                if (correctSequenceIndex == levelSequence.Length)
                {
                    Debug.Log("Correct sequence! Exit unlocked.");
                   
                    NarratorSystem.Instance.TriggerEvent(NarratorState.CorrectSequence);
                    inputSequence.Clear();
                    correct = true;
                    levelCutEnding();
                    StartCoroutine(loadFloor("GFloor"));
                    return;
                }
            }
        }
    }

    public void changeScene()
    {
        //sequence of exit level 1: turn off keypad, elevator rumbled, eyes closed.
        NarratorSystem.Instance.TriggerEvent(NarratorState.OtherFloor);
        levelCutEnding();
        int index = 0;
        switch (chosenLevel)
        {
            case 1: index = 1; break;
            case 2: index = 2; break;
            case 3: index = 3; break;
            case 4: index = 4; break;
            case 5: index = 5; break;
            case 6: index = 6; break;
            case 7: index = 7; break;
            case 8: index = 8; break;
            case 9: index = 9; break;
        }
        Debug.Log("go to floor " + index);
        
        FloorManager.Instance.currentFloorDialogue = chosenLevel;

        if (chosenLevel == 1)
        {
            StartCoroutine(loadFloor("Level" + chosenLevel));
        }
        else if (chosenLevel == 2 || chosenLevel == 4 || chosenLevel == 6 || chosenLevel == 8)
        {
            StartCoroutine(loadFloor("Level" + 2));
        }
        else if( chosenLevel == 3 || chosenLevel == 5 || chosenLevel == 7 || chosenLevel == 9)
        {
            StartCoroutine(loadFloor("Level" + 3));
        }
        else
        {
            StartCoroutine(loadFloor("Level" + 1));
        }
    }

    public void levelCutEnding()
    {
        //set keypad inactive
        keypad.ToggleOff();

        //play elevator moving sound
        StartCoroutine(playSoundEffect());


        //eye closed 
        StartCoroutine(eyeFadeEffect.FadeSequence());
    }

    private IEnumerator playSoundEffect()
    {
        //play sound
        elevatorRumbleSFX.Play();
        yield return new WaitForSeconds(2.5f);
    }

    IEnumerator loadFloor(string level)
    {
        
        yield return new WaitForSeconds(12.5f);
        //if(correct -> load G floor)
        if (correct)
        {
            chosenLevel = 0;
            FloorManager.Instance.currentFloorDialogue = chosenLevel;
            SceneManager.LoadScene(level);
        }
        else // load other floor
        {
            SceneManager.LoadScene(level);
        }
    }

}
