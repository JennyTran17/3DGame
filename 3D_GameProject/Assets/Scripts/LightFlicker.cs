using System.Collections;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    [Header("Light and Sound")]
    public Light lightOB;
    public AudioSource lightSound;

    [Header("Morse Flicker Settings")]
    public string morsePattern = "...--";  // Morse code for number 3
    public float dotDuration = 0.3f;       // Short flash
    public float dashDuration = 1.5f;      // Long flash
    public float intraSymbolPause = 0.5f;  // Pause between dots/dashes
    public float interPatternPause = 3f; // Pause after full sequence

    private Coroutine morseRoutine;

    //Not using update because only need to start/stop the flicker when enabled/disabled
    //improves performance slightly by avoiding unnecessary checks every frame
    //coroutine handles timing internally, so no need for per-frame updates
    private void OnEnable()
    {
        if (lightOB == null)
        {
            Debug.LogError("LightFlicker: No Light assigned!");
            return;
        }

        // Restart the morse flicker 
        morseRoutine = StartCoroutine(MorseFlicker());
    }

    private void OnDisable()
    {
        if (morseRoutine != null)
            StopCoroutine(morseRoutine);
    }

    IEnumerator MorseFlicker()
    {
        while (true)
        {
            foreach (char symbol in morsePattern)
            {
                float duration = 0f;

                if (symbol == '.')
                    duration = dotDuration;
                else if (symbol == '-')
                    duration = dashDuration;
                else
                    continue; // Skip invalid chars or spaces

                // Turn light ON
                lightOB.enabled = true;
                if (lightSound != null) lightSound.Play();

                yield return new WaitForSeconds(duration);

                // Turn light OFF
                lightOB.enabled = false;
                yield return new WaitForSeconds(intraSymbolPause);
            }

            // Pause before repeating the full Morse pattern
            yield return new WaitForSeconds(interPatternPause);
        }
    }
}
