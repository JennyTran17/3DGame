using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum NarratorState
{
    Casual,
    Flashlight,
    WallText,
    FirstAnomaly,
    FirstDynamicSound,
    SecondDynamicSound,
    Businessman,
    Hint,
    Ending
}



public class NarratorManager : MonoBehaviour
{
    public static NarratorManager Instance;

    [Header("UI Reference")]
    public TextMeshProUGUI narratorText;

    [Header("Narration Settings")]
    [TextArea] public List<string> introLines;
    [TextArea] public List<string> casualLines;
    public float casualDelay = 4f;
    public float eventLineDelay = 3f;
    public float fadeDuration = 0.5f;

    [Header("Event Narration")]
    public List<NarratorEvent> events;

    [Header("Hallway Tracking")]
    public int endingHallway = 10;

    private int casualIndex = 0;
    private int casualCharIndex = 0;
    private bool isEventPlaying = false;
    private Queue<NarratorEvent> eventQueue = new Queue<NarratorEvent>();
    private Coroutine casualCoroutine;
    private bool snapNextFade = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (narratorText == null)
        {
            return;
        }

        // Start introduction first
        StartCoroutine(PlayIntro());
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
                int currentIndex = casualCharIndex;

                for (casualCharIndex = currentIndex; casualCharIndex <= line.Length; casualCharIndex++)
                {
                    narratorText.text = line.Substring(0, casualCharIndex);
                    yield return new WaitForSeconds(casualDelay / line.Length);

                    // Pause if event triggers
                    if (isEventPlaying)
                        yield return new WaitUntil(() => !isEventPlaying);
                }

                casualCharIndex = 0;
                casualIndex = (casualIndex + 1) % casualLines.Count;
                yield return new WaitForSeconds(casualDelay);
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

        // If casual was active, snap to next event
        if (!isEventPlaying) snapNextFade = true;

        eventQueue.Enqueue(nEvent);

        if (!isEventPlaying)
        {
            StartCoroutine(ProcessEventQueue());
        }
    }

    private IEnumerator ProcessEventQueue()
    {
        while (eventQueue.Count > 0)
        {
            NarratorEvent nEvent = eventQueue.Dequeue();
            isEventPlaying = true;

            foreach (string line in nEvent.lines)
            {
                // Snap only if flagged (Casual -> Event)
                bool snap = snapNextFade;
                snapNextFade = false;

                yield return StartCoroutine(FadeText(line, fadeDuration, snap));
                yield return new WaitForSeconds(eventLineDelay);
            }

            isEventPlaying = false;
        }
    }

    private IEnumerator FadeText(string line, float duration, bool snap = false)
    {
        if (snap)
        {
            narratorText.text = line;
            narratorText.color = new Color(narratorText.color.r, narratorText.color.g, narratorText.color.b, 1f);
            yield break;
        }

        // Fade out current text
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

        // Fade in new text
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

    public void CheckHallwayProgress(int hallway)
    {
        if (hallway == endingHallway)
        {
            TriggerEvent(NarratorState.Ending);
        }
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