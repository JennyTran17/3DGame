using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum NarratorState
{
    Casual,
    Flashlight,
    WallText,
    FirstDynamicSound,
    SecondDynamicSound,
    Businessman,
    Ending,
    OtherFloor,
    CorrectSequence
}

public class NarratorSystem : MonoBehaviour
{
    public static NarratorSystem Instance;

    [Header("UI Reference")]
    public TextMeshProUGUI narratorText;

    [Header("Narration Settings")]
    [TextArea] public List<string> introLines;
    [TextArea] public List<string> casualLines;
    public float casualDelay = 4f;
    public float eventLineDelay = 3f;
    public float fadeDuration = 0.5f;

    [Header("Event Transition")]
    public float interruptDelay = 3f; // time to wait before new event overrides

    [Header("Event Narration")]
    public List<NarratorEvent> events;

    private int casualIndex = 0;
    private bool isEventPlaying = false;
    private Queue<NarratorEvent> eventQueue = new Queue<NarratorEvent>();
    private Coroutine casualCoroutine;
    private Coroutine eventCoroutine;
    private float interruptTimer = 0f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (narratorText == null) return;

        //StartCoroutine(PlayIntro());
        casualCoroutine = StartCoroutine(CasualLoop());
    }

    IEnumerator PlayIntro()
    {
        foreach (string line in introLines)
        {
            yield return StartCoroutine(FadeText(line, fadeDuration));
            yield return new WaitForSeconds(eventLineDelay);
        }
    }

    IEnumerator CasualLoop()
    {
        while (true)
        {
            if (!isEventPlaying && casualLines.Count > 0)
            {
                string line = casualLines[casualIndex];
                yield return StartCoroutine(FadeText(line, fadeDuration));
                yield return new WaitForSeconds(casualDelay);

                casualIndex = (casualIndex + 1) % casualLines.Count;
            }
            else
            {
                yield return null;
            }
        }
    }

    public void TriggerEvent(NarratorState state)
    {
        NarratorEvent nEvent = events.Find(e => e.state == state);
        if (nEvent == null) return;
        if (nEvent.triggerOnce && nEvent.triggered) return;

        nEvent.triggered = true;
        eventQueue.Enqueue(nEvent);

        if (isEventPlaying)
        {
            // Reset the timer when new event arrives
            interruptTimer = interruptDelay;
        }
        else
        {
            eventCoroutine = StartCoroutine(ProcessEventQueue());
        }
    }

    private IEnumerator ProcessEventQueue()
    {
        isEventPlaying = true;

        NarratorEvent currentEvent = null;
        bool interruptRequested = false;

        while (eventQueue.Count > 0)
        {
            currentEvent = eventQueue.Dequeue();

            // Countdown interrupt delay before next event can override
            interruptRequested = false;
            interruptTimer = 0f;

            foreach (string line in currentEvent.lines)
            {
                yield return StartCoroutine(FadeText(line, fadeDuration));
                float timer = 0f;

                // Wait for eventLineDelay or until interrupted
                while (timer < eventLineDelay)
                {
                    if (eventQueue.Count > 0)
                    {
                        interruptTimer += Time.deltaTime;
                        if (interruptTimer >= interruptDelay)
                        {
                            interruptRequested = true;
                            break;
                        }
                    }

                    timer += Time.deltaTime;
                    yield return null;
                }

                if (interruptRequested) break;
            }

            if (interruptRequested)
            {
                // Immediately fade out before switching to next event
                yield return StartCoroutine(FadeText("", fadeDuration));
                continue; // process next event right away
            }
        }

        isEventPlaying = false;
    }

    private IEnumerator FadeText(string line, float duration, bool snap = false)
    {
        if (snap)
        {
            narratorText.text = line;
            narratorText.color = new Color(narratorText.color.r, narratorText.color.g, narratorText.color.b, 1f);
            yield break;
        }

        // Fade out
        if (!string.IsNullOrEmpty(narratorText.text))
        {
            float t = 0f;
            Color c = narratorText.color;
            while (t < duration)
            {
                t += Time.deltaTime;
                narratorText.color = new Color(c.r, c.g, c.b, Mathf.Lerp(1f, 0f, t / duration));
                yield return null;
            }
        }

        narratorText.text = line;

        // Fade in
        float tIn = 0f;
        Color cIn = narratorText.color;
        while (tIn < duration)
        {
            tIn += Time.deltaTime;
            narratorText.color = new Color(cIn.r, cIn.g, cIn.b, Mathf.Lerp(0f, 1f, tIn / duration));
            yield return null;
        }
        narratorText.color = new Color(cIn.r, cIn.g, cIn.b, 1f);
    }

    
}

[System.Serializable]
public class NarratorEvent
{
    public NarratorState state;
    [TextArea] public List<string> lines;
    public bool triggerOnce = true;
    [HideInInspector] public bool triggered = false;
}
