using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SceneDistortionManager : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioSource ambientAudio; // Main ambient source
    public Vector2 eventIntervalRange = new Vector2(60f, 200f); // Time between events
    public Vector2 muteDurationRange = new Vector2(1.5f, 3f);
    public float fadeSpeed = 2f;

    [Header("Visual Settings")]
    public Image blackoutCanvas; // Black screen overlay
    public bool useBlackout = true;
    public Vector2 blackoutDurationRange = new Vector2(1f, 2f);

    [Header("Scare Objects")]
    public List<GameObject> scareObjects = new List<GameObject>();
    public bool useScareObjects = true;
    public Vector2 scareVisibleTimeRange = new Vector2(0.5f, 2f);

    
    private bool isEventRunning = false;

    void Start()
    {
        if (ambientAudio == null)
            ambientAudio = FindObjectOfType<AudioSource>();

        if (blackoutCanvas != null)
            blackoutCanvas.color = new Color(0f, 0f, 0f, 0f);

        StartCoroutine(EventLoop());
    }

    IEnumerator EventLoop()
    {
        while (true)
        {
            float waitTime = Random.Range(eventIntervalRange.x, eventIntervalRange.y);
            yield return new WaitForSeconds(waitTime);

            if (!isEventRunning)
                StartCoroutine(TriggerDistortionEvent());
        }
    }

    IEnumerator TriggerDistortionEvent()
    {
        isEventRunning = true;

        float muteDuration = Random.Range(muteDurationRange.x, muteDurationRange.y);
        float blackoutDuration = Random.Range(blackoutDurationRange.x, blackoutDurationRange.y);

        // 1. Fade out audio
        float originalVolume = ambientAudio.volume;
        while (ambientAudio.volume > 0)
        {
            ambientAudio.volume -= Time.deltaTime * fadeSpeed;
            yield return null;
        }
        ambientAudio.volume = 0;

        // 2. Fade to blackout
        if (useBlackout && blackoutCanvas != null)
        {
            Blackout(1f);
        }

        // 3. Delay to simulate "silence before the scare"
        yield return new WaitForSeconds(muteDuration * 0.3f);

        // 4. Activate random scare object
        GameObject chosen = null;
        if (useScareObjects && scareObjects.Count > 0)
        {
            chosen = scareObjects[Random.Range(0, scareObjects.Count)];

            // Reference to player
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                // Decide how far in front of player to spawn
                float spawnDistance = 4f;

                // Spawn position in front of player
                Vector3 spawnPos = player.transform.position + player.transform.forward * spawnDistance;

                // Match Y height to player
                spawnPos.y = player.transform.position.y;

                if (!chosen.activeSelf)
                {
               
                    chosen = Instantiate(chosen, spawnPos, Quaternion.identity);
                }
                else
                {
                    // Move existing scene object
                    chosen.transform.position = spawnPos;
                }

                // Make it face the player
                chosen.transform.LookAt(player.transform);
            }

            chosen.SetActive(true);
           
        }

        // 5. Hold blackout for a while
        yield return new WaitForSeconds(blackoutDuration);

        // 6. Fade back from blackout
        if (useBlackout && blackoutCanvas != null)
        {
            Blackout(0f);
            chosen.GetComponent<AudioSource>().enabled = true;
            yield return new WaitForSeconds(1.5f);
            Blackout(1f);
            
        }
        
        // 7. Wait briefly then deactivate scare object
        if (chosen != null)
        {
            yield return new WaitForSeconds(Random.Range(scareVisibleTimeRange.x, scareVisibleTimeRange.y));
            Destroy(chosen);
        }

        if (useBlackout && blackoutCanvas != null)
        {
            yield return new WaitForSeconds(0.5f);
            Blackout(0f);
        }

        // 8. Fade audio back in
        while (ambientAudio.volume < originalVolume)
        {
            ambientAudio.volume += Time.deltaTime * fadeSpeed;
            yield return null;
        }
        ambientAudio.volume = originalVolume;

        isEventRunning = false;
    }

    void Blackout(float targetAlpha)
    {
        if (blackoutCanvas == null)
           return;

        blackoutCanvas.color = new Color(0f, 0f, 0f, targetAlpha);
        Debug.Log("Fade completed, alpha = " + targetAlpha);
    }
}
