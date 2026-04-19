using System;
using System.Collections.Generic;
using UnityEngine.Rendering;

///<summary>
/// A single player game session
/// </summary>
public class LocalGameSession : IGameSession
{
    //IGameSession implementation
    public bool IsGameOver { get; private set; }
    public IReadOnlyList<PlayerResult> Players => _players;

    public event Action OnGameStarted;
    public event Action OnGameEnded;
    //internal systems
    public ScoreCounter ScoreCounter { get; private set; }
    public GameTimer GameTimer { get; private set; }
    public PersonalBestTracker PersonalBestTracker { get; private set; }

    private readonly List<PlayerResult> _players = new List<PlayerResult>();
    private readonly string _playerName;

    public LocalGameSession(string playerName, float duration)
    {
        _playerName = playerName;

        ScoreCounter = new ScoreCounter();
        GameTimer = new GameTimer(duration);
        PersonalBestTracker = new PersonalBestTracker();

        GameTimer.OnExpired += OnGameExpired;
    }

    public void OnTap()
    {
        if (IsGameOver) return;

        if (!GameTimer.IsRunning)
        {
            GameTimer.Start();
            OnGameStarted?.Invoke();
        }
        ScoreCounter.Increment();
    }

    public void Tick(float deltaTime)
    {
        GameTimer.Tick(deltaTime);
    }

    private void OnGameExpired()
    {
        IsGameOver = true;
        PersonalBestTracker.Check(ScoreCounter.Score);

        _players.Clear();
        _players.Add(new PlayerResult(_playerName, ScoreCounter.Score));

        OnGameEnded?.Invoke();
    }

    public void Dispose()
    {
        GameTimer.OnExpired -= OnGameExpired;
    }
}