using System.Collections;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    [Header("Light and Sound")]
    public Light lightOB;
    public AudioSource lightSound;

    [Header("Random Flicker Settings")]
    public bool useRandomFlicker = true;
    public float minTime = 0.1f;
    public float maxTime = 0.5f;

    [Header("Morse Flicker Settings")]
    public string morsePattern = "...--";
    public float dotDuration = 0.2f;      // Short flash
    public float dashDuration = 0.6f;     // Long flash
    public float intraSymbolPause = 0.2f; // Pause between dots/dashes
    public float interPatternPause = 1.5f;// Pause after full sequence

    private float timer;
    private Coroutine morseRoutine;

    void Start()
    {
        if (useRandomFlicker)
        {
            timer = Random.Range(minTime, maxTime);
        }
        else
        {
            if (lightOB == null)
            {
                Debug.LogError("LightFlicker: No Light assigned!");
                return;
            }

            morseRoutine = StartCoroutine(MorseFlicker());
        }
    }

    void Update()
    {
        if (useRandomFlicker)
        {
            RandomFlicker();
        }
    }

    void RandomFlicker()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }

        if (timer <= 0)
        {
            lightOB.enabled = !lightOB.enabled;
            if (lightSound != null) lightSound.Play();

            timer = Random.Range(minTime, maxTime);
        }
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
                    continue; // skip spaces or invalid chars

                // Turn light on
                lightOB.enabled = true;
                if (lightSound != null) lightSound.Play();

                yield return new WaitForSeconds(duration);

                // Turn light off
                lightOB.enabled = false;

                yield return new WaitForSeconds(intraSymbolPause);
            }

            // Pause between full Morse cycles
            yield return new WaitForSeconds(interPatternPause);
        }
    }
}
