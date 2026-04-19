using System;
using System.Collections.Generic;

/// <summary>
/// Defines the contract for the game session (local and networked)
/// </summary>
public interface IGameSession
{
    // State
    bool IsGameOver { get; }
    IReadOnlyList<PlayerResult> Players { get; }

    // Events
    event Action OnGameStarted;
    event Action OnGameEnded;

    // Input
    void OnTap();
}