using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameState GameState { get; private set; } = new GameState();
    [field: SerializeField] public BubbleWaves WaveContainer { get; private set; }
    public static GameManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    public void NewGame()
    {
        GameState.Time = 0;
        GameState.Lives = 3;
        GameState.Difficulty = 0;
        GameState.Score = 0;
        GameState.Wave = 0;
    }

    public void StartGame()
    {
        NextWave();
    }

    private void NextWave()
    {
        
    }

    public void SubmitBubble(BubbleKind kind)
    {
        Debug.Log($"Submitted: {kind.Name}");
    }
}