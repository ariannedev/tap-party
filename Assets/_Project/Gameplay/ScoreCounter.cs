// ScoreCounter.cs
using System;

/// <summary>
/// Tracks the player's score for a single game session.
/// </summary>
public class ScoreCounter
{
    public int Score { get; private set; }
    public event Action<int> OnScoreChanged;

    public void Increment()
    {
        Score++;
        OnScoreChanged?.Invoke(Score);
    }
}