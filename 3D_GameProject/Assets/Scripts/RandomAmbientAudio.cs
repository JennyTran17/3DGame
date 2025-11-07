using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RandomAmbientAudio : MonoBehaviour
{
    [Header("Audio Clips")]
    [Tooltip("List of creepy ambient clips like whispers, creaks, knocks, etc.")]
    public List<AudioClip> ambientClips = new List<AudioClip>();

    [Header("Timing Settings")]
    [Tooltip("Time range between each random sound (in seconds).")]
    public Vector2 timeBetweenSounds = new Vector2(10f, 30f);

    [Header("Audio Effects")]
    [Tooltip("Randomize pitch slightly to make each sound less repetitive.")]
    public Vector2 pitchRange = new Vector2(0.9f, 1.1f);

    [Tooltip("Random volume range for variation.")]
    public Vector2 volumeRange = new Vector2(0.7f, 1.0f);

    [Tooltip("Random Doppler level (how quickly the pitch changes when moving).")]
    public Vector2 dopplerRange = new Vector2(0.0f, 2.0f);

    [Tooltip("How 3D the sound is. 0 = 2D, 1 = fully 3D.")]
    [Range(0f, 1f)] public float spatialBlend = 1f;

    [Tooltip("Where the sound is. -1 = left, 1 = right.")]
    [Range(-1f, 1f)] public float stereoPan = 0f;

    [Tooltip("Max distance where sound can be heard.")]
    public float maxDistance = 30f;

    private AudioSource audioSource;
    private Coroutine soundRoutine;
    private int currentClipIndex = 0;
    GameObject player;
    int num = 0;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.loop = false;
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = spatialBlend;
        audioSource.panStereo = stereoPan;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.maxDistance = maxDistance;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Start()
    {
        if (ambientClips.Count > 0)
        {
            soundRoutine = StartCoroutine(PlayRandomSoundsRoutine());
        }
        else
        {
            Debug.LogWarning("RandomAmbientAudio: No audio clips assigned!");
        }
    }

    private IEnumerator PlayRandomSoundsRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(timeBetweenSounds.x, timeBetweenSounds.y);
            yield return new WaitForSeconds(waitTime);

            if (ambientClips.Count == 0)
                continue;

            // Pick next clip
            currentClipIndex = (currentClipIndex + 1) % ambientClips.Count;
            AudioClip selectedClip = ambientClips[currentClipIndex];

            // Randomize audio properties
            audioSource.pitch = Random.Range(pitchRange.x, pitchRange.y);
            audioSource.volume = Random.Range(volumeRange.x, volumeRange.y);
            audioSource.dopplerLevel = Random.Range(dopplerRange.x, dopplerRange.y);
            audioSource.panStereo = Random.Range(-1f, 1f);
            transform.position = player.transform.position + Random.insideUnitSphere * Random.Range(2f, 8f);

            // Play the sound
            audioSource.clip = selectedClip;
            audioSource.Play();

            switch (num)
            {
                case 0: NarratorSystem.Instance.TriggerEvent(NarratorState.FirstDynamicSound); break;
                case 1: NarratorSystem.Instance.TriggerEvent(NarratorState.SecondDynamicSound); break;
            }
            


            yield return new WaitForSeconds(15);
            audioSource.Stop();
            num = 1;
        }
    }


}
