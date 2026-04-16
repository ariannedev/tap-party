using System;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine.Rendering;

/// <summary>
/// Counts down from a starting duration and fires events on tick and/or expiry
/// </summary>
public class GameTimer
{
    public float TimeRemaining { get; private set; }
    public bool IsRunning { get; private set; }

    public event Action<float> OnTick;
    public event Action OnExpired;

    public GameTimer(float duration)
    {
        TimeRemaining = duration;
        IsRunning = false;
    }

    public void Start()
    {
        IsRunning = true;
    }

    public void Tick(float deltaTime)
    {
        if (!IsRunning) return;

        TimeRemaining -= deltaTime;

        if (TimeRemaining <= 0f)
        {
            TimeRemaining = 0f;
            IsRunning = false;
            OnExpired?.Invoke();
            return;
        }
        OnTick?.Invoke(TimeRemaining);
    }
}