using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WallTextManager : MonoBehaviour
{
    public TextMeshPro dynamicText;

    [Header("Static State Messages")]
    [TextArea] public string startText = "You shouldn't be here.";
    [TextArea] public string elevatorGoneText = "Where’s your way out now?";
    [TextArea] public string elevatorAppearText = "GO BACK";

    [Header("Dynamic Hallway Messages")]
    [TextArea]
    public List<string> hallwayTexts = new List<string>
    {
        "I told you not to go that way.",
        "You think you’re escaping?",
        "Why do you keep coming back?",
        "Keep walking. See where that gets you.",
        "It feels familiar, doesn’t it?",
        "It can see you",
        "Something’s wrong behind you.",
        "You never learn, do you?",
        "Someone was killed here",
        "All you can do is walk back and forth",
        "WATCH OUT",
        "Listen to me or you'll die"
    };

    private string currentDisplayedText;
    private int hallwayTextIndex = 0;
    private int lastHallwayCount = -1;

    private void Start()
    {
        if (dynamicText == null)
        {
            Debug.LogError("WallTextManager: Missing TextMeshPro reference!");
            return;
        }

        currentDisplayedText = startText;
        dynamicText.text = currentDisplayedText;

        // Subscribe to state and hallway events
        StateManager.Instance.OnStateChanged += OnGameStateChanged;
        StartCoroutine(MonitorHallwayProgress());
    }

    private void OnDestroy()
    {
        if (StateManager.Instance != null)
            StateManager.Instance.OnStateChanged -= OnGameStateChanged;
    }

    // --- Called when GameManager updates state ---
    private void OnGameStateChanged(GameState newState)
    {
        // Only react to specific states
        if (newState == GameState.Normal)
        {
            ChangeWallText(startText);
        }
        else if (newState == GameState.ElevatorGone)
        {
            ChangeWallText(elevatorGoneText);
        }
        else if (newState == GameState.FinalHallway)
        {
            ChangeWallText(elevatorAppearText);
        }
    }

    // --- Called every time hallway count changes ---
    private IEnumerator MonitorHallwayProgress()
    {
        lastHallwayCount = LoopManager.Instance.hallwayCount;

        while (true)
        {
            int currentHallway = LoopManager.Instance.hallwayCount;

            if (currentHallway != lastHallwayCount)
            {
                OnHallwayUpdated();
                lastHallwayCount = currentHallway;
            }

            yield return new WaitForSeconds(0.25f);
        }
    }

    private void OnHallwayUpdated()
    {
        if (hallwayTexts.Count == 0) return;

        string newText = hallwayTexts[hallwayTextIndex];
        hallwayTextIndex = (hallwayTextIndex + 1) % hallwayTexts.Count;

        ChangeWallText(newText);
    }

    private void ChangeWallText(string newText)
    {
        if (newText != currentDisplayedText)
            StartCoroutine(FlickerTextChange(newText));
    }

    private IEnumerator FlickerTextChange(string newText)
    {
        for (int i = 0; i < 4; i++)
        {
            dynamicText.enabled = !dynamicText.enabled;
            yield return new WaitForSeconds(0.1f);
        }

        currentDisplayedText = newText;
        dynamicText.text = newText;
        dynamicText.enabled = true;
    }
}
