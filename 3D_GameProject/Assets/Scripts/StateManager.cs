using System;
using UnityEngine;

public enum GameState
{
    Normal,
    HintFound,
    ReverseMode,
    ElevatorGone,
    FinalHallway
}

public class StateManager : MonoBehaviour
{
    public static StateManager Instance;

    public GameState CurrentState { get; private set; } = GameState.Normal;

    public event Action<GameState> OnStateChanged;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void SetState(GameState newState)
    {
        if (newState == CurrentState)
            return;

        CurrentState = newState;
        Debug.Log($"[StateManager] State changed to {newState}");
        OnStateChanged?.Invoke(newState);
    }
}
