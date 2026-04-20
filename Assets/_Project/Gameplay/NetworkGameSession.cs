using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor.Build.Reporting;
using UnityEngine.Rendering;
using UnityEngine.SocialPlatforms.Impl;

/// <summary>
/// A networked multiplayer game session
///  Implements IGameSession over an INetworkTransport.
/// </summary>
public class NetworkGameSession : IGameSession
{
    //IGameSession implementation
    public bool IsGameOver { get; set; }
    public IReadOnlyList<PlayerResult> Players => _players;

    public event Action OnGameStarted;
    public event Action OnGameEnded;

    // internal systems
    public ScoreCounter ScoreCounter { get; private set; }
    public GameTimer GameTimer { get; private set; }

    private readonly List<PlayerResult> _players = new List<PlayerResult>();
    private readonly INetworkTransport _transport;
    private readonly string _playerName;
    private readonly float _duration;
    private bool _gameStarted;

    public NetworkGameSession(INetworkTransport transport, string playerName, float duration)
    {
        _transport = transport;
        _playerName = playerName;
        _duration = duration;

        ScoreCounter = new ScoreCounter();
        GameTimer = new GameTimer(duration);

        GameTimer.OnExpired += OnGameExpired;
        _transport.OnMessageReceived += OnMessageReceived;
    }

    public void OnTap()
    {
        if (IsGameOver) return;
        if (!_gameStarted) return;
        ScoreCounter.Increment();
    }

    public void Tick(float deltaTime)
    {
        GameTimer.Tick(deltaTime);
    }

    private void OnMessageReceived(INetworkMessage message)
    {
        switch (message)
        {
            case GameStartMessage startMessage:
                HandleGameStart(startMessage);
                break;
            case GameResultsMessage resultsMessage:
                HandleGameResults(resultsMessage);
                break;
        }
    }

    private void HandleGameStart(GameStartMessage message)
    {
        _gameStarted = true;
        GameTimer.Start();
        OnGameStarted?.Invoke();
    }

    private void HandleGameResults(GameResultsMessage message)
    {
        _players.Clear();
        _players.AddRange(message.Results);
        IsGameOver = true;
        OnGameEnded?.Invoke();
    }

    private async void OnGameExpired()
    {
        IsGameOver = true;

        await _transport.SendToHostAsync(new FinalScoreMessage
        {
            PlayerId = _transport.LocalPlayerId,
            Score = ScoreCounter.Score
        });
    }

    public async Task StartGameAsync()
    {
        if (!_transport.IsHost) return;

        await _transport.SendToAllAsync(new GameStartMessage
        {
            StartTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
    }

    public void Dispose()
    {
        GameTimer.OnExpired -= OnGameExpired;
        _transport.OnMessageReceived -= OnMessageReceived;
    }
}