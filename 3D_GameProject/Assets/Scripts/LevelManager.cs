using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    public int totalLevels = 5;
    public int currentLevel = 1;
    private int[] levelSequence = new int[]
    {
        5, 3, 1, 4, 9
    };

    public List<int> inputSequence = new List<int>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        DontDestroyOnLoad(instance);
    }

    private void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        //get inputs from buttons or ray cast from button objects in elevator

        //sample code
        //if (Keyboard.current.aKey.wasPressedThisFrame || Keyboard.current.leftArrowKey.wasPressedThisFrame) // Left
        //{
        //    playerSequence.Add("L");
        //    CheckSequence();
        //}
        //else if (Keyboard.current.dKey.wasPressedThisFrame || Keyboard.current.rightArrowKey.wasPressedThisFrame) // Right
        //{
        //    playerSequence.Add("R");
        //    CheckSequence();
        //}
        //else if (Keyboard.current.upArrowKey.wasPressedThisFrame) // Up
        //{
        //    playerSequence.Add("U");
        //    CheckSequence();
        //}
    }
    /*
     void CheckSequence()
    {
        // Index to track progress through the correct sequence
        int correctSequenceIndex = 0;

        // Loop through the player's sequence
        for (int i = 0; i < playerSequence.Count; i++)
        {
            // If the current player input matches the expected input in the correct sequence
            if (playerSequence[i] == correctSequence[correctSequenceIndex])
            {
                correctSequenceIndex++; // Move to the next expected input in the correct sequence

                // If the entire correct sequence has been matched
                if (correctSequenceIndex == correctSequence.Length)
                {
                    Debug.Log("Correct sequence! Exit unlocked.");
                   
                   
                    playerSequence.Clear(); // Optionally clear the sequence after success
                    return;
                }
            }
        }
    }
     
     */
}
