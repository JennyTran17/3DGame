using System.Collections.Generic;
using UnityEngine;

public class FloorManager : MonoBehaviour
{
    public static FloorManager Instance;
    private LevelManager levelManager;

    public List<NarrationProfile> profiles;
    public int currentFloorDialogue = 0; //ground floor

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public NarrationProfile GetCurrentProfile()
    {

        // Clamp to max profile index
        if (currentFloorDialogue >= profiles.Count)
            currentFloorDialogue = profiles.Count - 1;

        Debug.Log($"FloorManager returning profile {profiles[currentFloorDialogue].name}");
        return profiles[currentFloorDialogue];
    }
}
