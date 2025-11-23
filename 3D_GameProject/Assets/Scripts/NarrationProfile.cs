using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Narration/NarrationProfile")]
public class NarrationProfile : ScriptableObject
{
    [Header("Intro")]
    [TextArea] public List<string> introLines;

    [Header("Casual Loop")]
    [TextArea] public List<string> casualLines;

    [Header("Event Narration Presets")]
    public List<NarratorEvent> eventPresets;
}
