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
        "You think you’re escaping?",
        "It can see you",
        "Something’s wrong behind you.",
        "Stop going forward",
        "You never learn, do you",
        "Someone was killed here",
        "WATCH OUT",
        "Listen to me or you'll die"
    };

    private string currentDisplayedText;
    private int lastHallwayCount = -1;

    private void Start()
    {
        if (dynamicText == null)
        {
            return;
        }

        currentDisplayedText = startText;
        dynamicText.text = currentDisplayedText;

        StateManager.Instance.OnStateChanged += OnGameStateChanged;
        StartCoroutine(MonitorHallwayProgress());
    }

    private void OnDestroy()
    {
        if (StateManager.Instance != null)
            StateManager.Instance.OnStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState newState)
    {
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

    private IEnumerator MonitorHallwayProgress()
    {
        lastHallwayCount = LoopManager.Instance.hallwayCount;

        while (true)
        {
            int currentHallway = LoopManager.Instance.hallwayCount;

            if (currentHallway != lastHallwayCount)
            {
                UpdateTextBasedOnHallway(currentHallway);
                lastHallwayCount = currentHallway;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    private void UpdateTextBasedOnHallway(int hallway)
    {
        if (hallway == 0)
        {
            ChangeWallText(startText);
        }
        else if (hallway == 1)
        {
            ChangeWallText(elevatorGoneText);
        }
        else if (hallway >= 10)
        {
            ChangeWallText(elevatorAppearText);
        }
        else
        {
            
            int index = Mathf.Clamp(hallway - 2, 0, hallwayTexts.Count - 1);
            ChangeWallText(hallwayTexts[index]);
        }
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
