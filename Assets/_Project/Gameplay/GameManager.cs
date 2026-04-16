// GameManager.cs
using UnityEngine;

/// <summary>
/// Holds and provides access to game logic objects, acting as the composition root for the scene.
/// </summary>
public class GameManager : MonoBehaviour
{
    public ScoreCounter ScoreCounter { get; private set; }
    public GameTimer GameTimer { get; private set; }

    public bool IsGameOver { get; private set; }

    void Awake()
    {
        ScoreCounter = new ScoreCounter();
        GameTimer = new GameTimer(10f);

        Debug.Assert(ScoreCounter != null, "GameManager: Failed to create ScoreCounter.");
        Debug.Assert(GameTimer != null, "GameManager: Failed to create GameTimer.");

        if (ScoreCounter == null) return;
        if (GameTimer == null) return;

        GameTimer.OnExpired += OnGameExpired;
    }

    void Update()
    {
        GameTimer.Tick(Time.deltaTime);
    }

    //when the button is tapped incremement the score
    //if the timer hasn't started (single player) start it
    public void OnTap()
    {
        if (IsGameOver) return;

        if (!GameTimer.IsRunning)
        {
            GameTimer.Start();
        }
        ScoreCounter.Increment();
    }

    void OnGameExpired()
    {
        IsGameOver = true;
    }

    void OnDestroy()
    {
        GameTimer.OnExpired -= OnGameExpired;
    }
}